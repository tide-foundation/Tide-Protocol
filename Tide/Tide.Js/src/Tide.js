import DAuthFlow from "./dauth/DAuthFlow";
import IdGenerator from "./IdGenerator";
import request from "superagent";
import { encodeBase64Url } from "./Helpers";
import { AESKey, C25519Key } from "cryptide";
import DAuthV2Flow from "../src/dauth/DAuthV2Flow";
import KeyStore from "./keyStore";
import Cipher from "./Cipher";
import Num64 from "./Num64";
import Rule from "./rule";
import RuleClient from "./dauth/RuleClient";
import RuleClientSet from "./dauth/RuleClientSet";
/**
 * A client-side library to interface with the Tide scosystem.
 * @class
 * @classdesc The client side library to interface with the Tide ecosystem.
 */
export default class {
  /**
   * Create Tide.
   *
   * @param {String} vendorId - Your designated VendorId in which you will operate
   * @param {String} serverUrl - The endpoint of your backend Tide server
   * @param {Array} homeOrks - The suggested initial point of contacts. At least 1 is required.
   * @param {String} publicKey - The vendor backend public key.
   *
   */

  constructor(vendorId, serverUrl, homeOrks, publicKey = "", mandatoryTags = []) {
    this.vendorId = vendorId;
    this.serverUrl = serverUrl;
    this.homeOrks = homeOrks;
    this.vendorStore = new KeyStore(C25519Key.from(publicKey));
    this.mandatoryTags = mandatoryTags;
  }

  /**
   * Initialize Tide.
   */
  async initialize() {
    // var discoveryOrk = await selectDiscoveryOrk(this.homeOrks);
    // this.orks = await get(`${discoveryOrk}/getorks/10`);
  }

  /**
   * Create a new Tide account.
   *
   * This will generate a new Tide user using the provided username and providing a keypaid to manage the account (user-secret).
   *
   * @param {String} username - Plain text username of the new user
   * @param {String} password - Plain text password of the new user
   * @param {String} email - The recovery email to be used by the user.
   * @param {Array} orkIds - The desired ork nodes to be used for registration. An account can only be activated when all ork nodes have confirmed they have stored the shard.
   *
   * @fires progress
   *
   * @returns {AESKey} - The users keys to be used on the data
   * @example
   * var registerResult = await tide.register("myUsername", "pa$$w0rD", "john@wick.com",["ork-1","ork-2","ork-3"]);
   */
  register(username, password, email, orkIds) {
    return new Promise(async (resolve, reject) => {
      try {
        // Some local validation, which is all we can really do.
        if (username.length < 3 || password.length < 6) return reject("Invalid credentials");

        var flow = new DAuthFlow(generateOrkUrls(orkIds), username);
        var userId = encodeBase64Url(IdGenerator.seed(username).buffer);
        // Ask the vendor to create the user as a liability.

        await post(`${this.serverUrl}/CreateUser/${userId}`, orkIds);
        event("progress", { progress: 20, action: "Initialized user" });

        // Send all shards to selected orks
        var key = await flow.signUp(password, email, 2);
        event("progress", { progress: 80, action: "Fragments stored" });

        // Finally, ask the vendor to confirm the user
        await get(`${this.serverUrl}/ConfirmUser/${userId}/`);
        event("progress", { progress: 100, action: "Finalized creation" });

        this.key = key;
        resolve({ key: key });
      } catch (error) {
        reject(error);
        // await get(`${this.serverUrl}/RollbackUser/${userId}/`);
      }
    });
  }

  /**
   * Login to a previously created Tide account. The account must be fully enabled by the vendor before use.
   *
   * This will generate a new Tide user using the provided username and providing a keypaid to manage the account (user-secret).
   *
   * @param {String} username - Plain text username of the user
   * @param {String} password - Plain text password of the user
   *
   * @returns {AESKey} - The users keys to be used on the data
   */
  login(username, password) {
    return new Promise(async (resolve, reject) => {
      try {
        var userId = encodeBase64Url(IdGenerator.seed(username).buffer);
        var userNodes = JSON.parse(await get(`${this.serverUrl}/GetUserNodes/${userId}`));

        var flow = new DAuthFlow(generateOrkUrls(userNodes.map((un) => un.ork)), username);
        var keyTag = await flow.logIn(password);
        return resolve({ key: keyTag });
      } catch (error) {
        return reject(error);
      }
    });
  }

  registerV2(username, password, email, orks) {
    return new Promise(async (resolve, reject) => {
      try {
        var flow = new DAuthV2Flow(username);
        flow.cmkUrls = orks;
        flow.cvkUrls = orks;
        flow.vendorUrl = this.serverUrl;

        var { vuid, cvk } = await flow.signUp(password, email, orks.length);
        this.vuid = vuid;
        this.cvk = cvk;
        this.cvkUrls = orks;

        if (this.mandatoryTags.length > 0) await this.allowTags(this.mandatoryTags);

        resolve({ vuid: vuid, publicKey: this.cvk.public().toString() });
      } catch (error) {
        reject(error);
        // await get(`${this.serverUrl}/RollbackUser/${userId}/`);
      }
    });
  }

  loginV2(username, password, orks) {
    return new Promise(async (resolve, reject) => {
      try {
        var flow = new DAuthV2Flow(username);
        flow.vendorUrl = this.serverUrl;
        flow.cmkUrls = orks;
        flow.cvkUrls = orks;
        flow.vendorUrl = this.serverUrl;

        var { vuid, cvk } = await flow.logIn(password);
        this.cvkUrls = orks;
        this.vuid = vuid;
        this.cvk = cvk;

        return resolve({ vuid: vuid, publicKey: this.cvk.public().toString() });
      } catch (error) {
        return reject(error);
      }
    });
  }

  /** @param tagArray {string[]} */
  async allowTags(tagArray) {
    const ruleCln = new RuleClientSet(this.cvkUrls, this.vuid);
    await Promise.all(tagArray.map((tag) => Rule.allow(this.vuid, Num64.seed(tag.name), this.vendorStore, tag.condition)).map(async (rule) => await ruleCln.setOrUpdate(rule)));
  }

  /** @param {Rule} rule */
  async updateRule(rule) {
    const ruleCln = new RuleClientSet(this.cvkUrls, this.vuid);
    await ruleCln.setOrUpdate(rule);
  }

  /** @param {Rule} rule */
  async updateCondition(ruleId, newCondition) {
    const ruleCln = new RuleClient(this.cvkUrls[0], this.vuid);

    var rule = await ruleCln.getById(ruleId);
    rule.condition = newCondition;

    await ruleCln.setOrUpdate(rule);
  }

  async getRules() {
    const ruleCln = new RuleClient(this.cvkUrls[0], this.vuid);
    console.log(ruleCln);
    return await ruleCln.getSet();
  }

  // /** @param tagArray {any[]} */
  // async allowTags(tagArray) {
  //   const ruleCln = new RuleClientSet(this.cvkUrls, this.vuid);
  //   await Promise.all(tagArray.map((tag) => Rule.allow(this.vuid, Num64.seed(tag.name), this.vendorStore, JSON.stringify(tag.condition))).map(async (rule) => await ruleCln.setOrUpdate(rule)));
  // }

  /**
   * Strips all local user data from the browser.
   */
  logout() {
    this.key = null;
  }

  /**
   * Encrypt a string with the logged in user keys.
   *
   * This action requires a logged in user.
   *
   * @param {String} msg - The string you wish to encrypt using the user keys
   * @param {String} tag - The string you wish to encrypt using the user keys
   *
   * @returns {String} - The encrypted payload
   */
  encrypt(msg, tag) {
    if (this.cvk == null) throw "You must be logged in to encrypt";
    return Cipher.encrypt(msg, Num64.seed(tag), this.cvk).toString("base64");
  }

  /**
   * Decrypt an encrypted string with the logged in user keys.
   *
   * This action requires a logged in user.
   *
   * @param {String} cipher - The encrypted string you wish to decrypt using the user keys
   *
   * @returns {String} - The plain text message
   */
  decrypt(cipher) {
    if (this.cvk == null) throw "You must be logged in to decrypt";

    var buffer = Buffer.from(cipher, "base64");
    return Cipher.decrypt(buffer, this.cvk);
  }

  /**
   * Send a request to the ORK nodes used by the user to email them recovery shards. This is step 1 in a 2 step process to recover the user keys.
   *
   * @param {String} username - The username of the user who wishes to recover
   */
  async recover(username) {
    var userId = encodeBase64Url(IdGenerator.seed(username).buffer);
    var userNodes = JSON.parse(await get(`${this.serverUrl}/GetUserNodes/${userId}`));

    var flow = new DAuthFlow(generateOrkUrls(userNodes.map((un) => un.ork)), username);
    flow.Recover(username);
  }

  async recoverV2(username, orks) {
    var flow = new DAuthFlow(orks, username);
    flow.Recover(username);
  }

  /**
   * Login to a previously created Tide account. The account must be fully enabled by the vendor before use.
   *
   * This will generate a new Tide user using the provided username and providing a keypaid to manage the account (user-secret).
   *
   * @param {String} username - Plain text username of the user
   * @param {Array} shares - An array of shares sent to the users email(s)
   * @param {String} newPass - The new password of the user
   */
  reconstruct(username, shares, newPass) {
    return new Promise(async (resolve, reject) => {
      try {
        var userId = encodeBase64Url(IdGenerator.seed(username).buffer);
        var userNodes = JSON.parse(await get(`${this.serverUrl}/GetUserNodes/${userId}`));
        var urls = generateOrkUrls(userNodes.map((un) => un.ork));
        var flow = new DAuthFlow(urls, username);

        return resolve(await flow.Reconstruct(shares, newPass, urls.length));
      } catch (error) {
        return reject(error);
      }
    });
  }

  reconstructV2(username, shares, newPass, orks) {
    return new Promise(async (resolve, reject) => {
      try {
        var flow = new DAuthFlow(orks, username);

        return resolve(await flow.Reconstruct(shares, newPass, orks.length));
      } catch (error) {
        return reject(error);
      }
    });
  }
}

async function selectDiscoveryOrk(homeOrks) {
  return new Promise(async (resolve, reject) => {
    var count = 0;
    homeOrks.forEach(async (ork) => {
      try {
        (await request.get(`${ork}/discover`)).body;

        return resolve(ork);
      } catch (error) {
        count++;
        if (count == homeOrks.length) return reject();
      }
    });
  });
}

function post(url, data) {
  return new Promise(async (resolve, reject) => {
    try {
      return extractTideResponse(await request.post(url).send(data), resolve, reject);
    } catch (error) {
      throw error;
    }
  });
}

function get(url) {
  return new Promise(async (resolve, reject) => {
    try {
      return extractTideResponse(await request.get(url), resolve, reject);
    } catch (error) {
      throw error;
    }
  });
}

function extractTideResponse(result, resolve, reject) {
  // Temporary function until we normalize the way the orks and vendor backend respond.
  // Currently the Tide Vendor SDK is returning text instead of correctly returning body.
  if (result.body != null && Object.keys(result.body).length != 0) {
    var r = result.body;
    return r.success ? resolve(r.content) : reject(r.error);
  } else {
    var r = JSON.parse(result.text);
    return r.Success ? resolve(r.Content) : reject(r.Error);
  }
}

function generateOrkUrls(ids) {
  return ids.map((id) => `https://${id}.azurewebsites.net`);
}

function event(name, payload) {
  // const event = new CustomEvent(name, payload);
  // document.dispatchEvent(event);
}

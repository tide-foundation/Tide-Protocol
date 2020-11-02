// @ts-ignore
import DAuthV2Flow from "../dauth/DAuthV2Flow";
import DAuthJwtFlow from "../dauth/DAuthJwtFlow";
import { CP256Key, C25519Key, EcKeyFormat } from "cryptide";
import Account from "./models/Account";
import TideConfiguration from "./models/TideConfiguration";
import { encode } from "../jwtToken";
import TemporaryDns from "./TemporaryDns";

export default class TideAuthentication {
  /**
   * Tide Authentication Module
   *
   * @param {string} vendorId - Your designated VendorId in which you will operate
   * @param {string} serverUrl - The endpoint of your backend Tide server
   * @param {string[]} homeOrks - The suggested initial point of contacts. At least 1 is required.
   *
   */
  constructor(vendorId, serverUrl, homeOrks, vendorPublic) {
    this.config = new TideConfiguration(vendorId, serverUrl, homeOrks, vendorPublic);
  }

  /**
   * Create a new Tide account.
   *
   * This will generate a new Tide user using the provided username and providing a keypaid to manage the account (user-secret).
   *
   * @param {string}  username - Plain text username of the new user
   * @param {string} password - Plain text password of the new user
   * @param  {string} email - The recovery email to be used by the user.
   * @param  {string[]} orks - The desired ork nodes to be used for registration. An account can only be activated when all ork nodes have confirmed they have stored the shard.
   *
   * @returns  - The Tide user account
   * @example
   * var registerResult = await tide.register("myUsername", "pa$$w0rD", "john@wick.com",["ork-1","ork-2","ork-3"]);
   */
  async register(username, password, email, orks) {
    return new Promise(async (resolve, reject) => {
      try {
        const flow = generateFlow(username, orks, this.config.serverUrl);

        var { vuid, auth } = await flow.signUp(password, email, orks.length);
        this.account = new Account(username, vuid, auth);
        return resolve(this.account);
      } catch (error) {
        reject(error);
      }
    });
  }

  /**
   * Create a new Tide account.
   *
   * This will generate a new Tide user using the provided username and providing a keypaid to manage the account (user-secret).
   *
   * @param {string}  username - Plain text username of the new user
   * @param {string} password - Plain text password of the new user
   * @param  {string} email - The recovery email to be used by the user.
   * @param  {string[]} orks - The desired ork nodes to be used for registration. An account can only be activated when all ork nodes have confirmed they have stored the shard.
   *
   * @returns  - The Tide user account
   * @example
   * var registerResult = await tide.register("myUsername", "pa$$w0rD", "john@wick.com",["ork-1","ork-2","ork-3"]);
   */
  async registerJwt(username, password, email, orks, serverTime) {
    return new Promise(async (resolve, reject) => {
      try {
        if (await TemporaryDns.doesUserExist(username)) throw new Error("That username exists");

        const flow = generateJwtFlow(username, orks, this.config.serverUrl, this.config.vendorPublic);
        flow.vendorPub = CP256Key.from(this.config.vendorPublic);

        var { vuid, cvk } = await flow.signUp(password, email, orks.length);

        const token = encode({ vuid: vuid.toString(), exp: serverTime }, cvk);

        var cvkPublic = EcKeyFormat.PemPublic(cvk);

        await TemporaryDns.setUserOrks(username, orks);

        this.account = new Account(username, vuid, token, cvkPublic, cvk);
        return resolve(this.account);
      } catch (error) {
        reject(error);
      }
    });
  }

  /**
   * Login to a previously created Tide account. The account must be fully enabled by the vendor before use.
   *
   * This will generate a new Tide user using the provided username and providing a keypaid to manage the account (user-secret).
   *
   * @param {string} username - Plain text username of the user
   * @param {string} password - Plain text password of the user
   *
   * @returns {Account} - The Tide user account
   */
  async loginJwt(username, password, serverTime) {
    // Orks should be replaced in the future with a DNS call
    return new Promise(async (resolve, reject) => {
      try {
        const orks = await TemporaryDns.getUserOrks(username);
        if (orks == null) throw new Error("Cannot locate DNS record");
        const flow = generateJwtFlow(username, orks, this.config.serverUrl, this.config.vendorPublic);
        flow.vendorPub = CP256Key.from(this.config.vendorPublic);

        var { vuid, cvk } = await flow.logIn(password);
        const token = encode({ vuid: vuid.toString(), exp: serverTime }, cvk);

        var cvkPublic = EcKeyFormat.PemPublic(cvk);

        this.account = new Account(username, vuid, token, cvkPublic);
        return resolve(this.account);
      } catch (error) {
        return reject(error);
      }
    });
  }

  /**
   * Send a request to the ORK nodes used by the user to email them recovery shards. This is step 1 in a 2 step process to recover the user keys.
   *
   * @param {String} username - The username of the user who wishes to recover
   * @param {string[]} orks - The orks used to register the account
   */
  async recover(username, orks) {
    const flow = generateJwtFlow(username, orks, this.config.serverUrl, this.config.vendorPublic);

    await flow.Recover(username);
  }

  /**
   * Login to a previously created Tide account. The account must be fully enabled by the vendor before use.
   *
   * This will generate a new Tide user using the provided username and providing a keypaid to manage the account (user-secret).
   *
   * @param {String} username - Plain text username of the user
   * @param {Array} shares - An array of shares sent to the users email(s)
   * @param {String} newPass - The new password of the user
   * @param {string[]} orks - The orks used to register the account
   */
  reconstruct(username, shares, newPass, orks) {
    return new Promise(async (resolve, reject) => {
      try {
        var flow = generateJwtFlow(username, orks, this.config.serverUrl, this.config.vendorPublic);

        return resolve(await flow.Reconstruct(shares, newPass, orks.length));
      } catch (error) {
        return reject(error);
      }
    });
  }

  /**
   * Logs out and clears all local Tide user data
   */
  logout() {
    this.account = null;
  }

  /**
   * Change the password of the currently logged in Tide user
   *
   * @param {string} pass - The current Tide user password
   * @param {string} newPass - The new password the user wishes to change to
   *
   */
  async changePassword(pass, newPass, orks) {
    // Threshold should be replaced in the future with a DNS call
    return new Promise(async (resolve, reject) => {
      try {
        const flow = generateJwtFlow(this.account.username, orks, this.config.serverUrl, this.config.vendorPublic);
        await flow.changePass(pass, newPass, getUserOrks(this.config).length);
        return resolve();
      } catch (error) {
        return reject(error);
      }
    });
  }

  validateReturnUrl(returnUrl, hashedReturnUrl) {
    try {
      var pubkey = C25519Key.from(this.config.vendorPublic);
      return pubkey.verify(returnUrl, Buffer.from(hashedReturnUrl, "base64"));
    } catch (error) {
      return false;
    }
  }
}

function generateFlow(username, orks, serverUrl) {
  var flow = new DAuthV2Flow(username);
  flow.cmkUrls = orks;
  flow.cvkUrls = orks;
  flow.vendorUrl = serverUrl;
  return flow;
}

function generateJwtFlow(username, orks, serverUrl, vendorPublic) {
  var flow = new DAuthJwtFlow(username);
  flow.cmkUrls = orks;
  flow.cvkUrls = orks;
  flow.vendorUrl = serverUrl;
  flow.vendorPub = vendorPublic;
  return flow;
}

function getUserOrks(config) {
  // Return a DNS call
  return config.homeOrks;
}

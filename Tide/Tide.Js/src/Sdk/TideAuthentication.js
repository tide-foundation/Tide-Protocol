// @ts-ignore
import DAuthV2Flow from "../dauth/DAuthV2Flow";
import Account from "./models/Account";
import TideConfiguration from "./models/TideConfiguration";

export default class TideAuthentication {
  /**
   * Tide Authentication Module
   *
   * @param {string} vendorId - Your designated VendorId in which you will operate
   * @param {string} serverUrl - The endpoint of your backend Tide server
   * @param {string[]} homeOrks - The suggested initial point of contacts. At least 1 is required.
   *
   */
  constructor(vendorId, serverUrl, homeOrks) {
    this.config = new TideConfiguration(vendorId, serverUrl, homeOrks);
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
   * Login to a previously created Tide account. The account must be fully enabled by the vendor before use.
   *
   * This will generate a new Tide user using the provided username and providing a keypaid to manage the account (user-secret).
   *
   * @param {string} username - Plain text username of the user
   * @param {string} password - Plain text password of the user
   *
   * @returns {Account} - The Tide user account
   */
  async login(username, password) {
    // Orks should be replaced in the future with a DNS call
    return new Promise(async (resolve, reject) => {
      try {
        const flow = generateFlow(username, getUserOrks(this.config), this.config.serverUrl);
        var { vuid, auth } = await flow.logIn(password);
        this.account = new Account(username, vuid, auth);
        return resolve(this.account);
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
  async changePassword(pass, newPass) {
    // Threshold should be replaced in the future with a DNS call
    return new Promise(async (resolve, reject) => {
      try {
        const flow = generateFlow(this.account.username, getUserOrks(this.config), this.config.serverUrl);
        await flow.changePass(pass, newPass, getUserOrks(this.config).length);
        return resolve();
      } catch (error) {
        return reject(error);
      }
    });
  }

  decryptToken(cipher) {
    return this.account.vendorKey.decryptStr(cipher);
  }
}

function generateFlow(username, orks, serverUrl) {
  var flow = new DAuthV2Flow(username);
  flow.cmkUrls = orks;
  flow.cvkUrls = orks;
  flow.vendorUrl = serverUrl;
  return flow;
}

function getUserOrks(config) {
  // Return a DNS call
  return config.homeOrks;
}

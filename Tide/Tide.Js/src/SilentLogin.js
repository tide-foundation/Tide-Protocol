import Account from "./export/models/Account";
import { encode } from "./jwtToken";

const tokenKey = "silent_login_token";

function parseJwt(token) {
  var base64Payload = token.split(".")[1];
  var payload = Buffer.from(base64Payload, "base64");
  return JSON.parse(payload.toString());
}

export default class SilentLogin {
  /**
   * @returns {Account}>}
   */
  static GetAccount() {
    var token = localStorage.getItem(tokenKey);

    if (token == null) return null;

    var decoded = parseJwt(token);

    if (decoded.exp < new Date().getTime()) {
      localStorage.removeItem(tokenKey);
      return null;
    }
    delete decoded.exp;
    return decoded;
  }

  /**
   * @param {Account} account - The user account to save
   */
  static SetAccount(account) {
    var exp = Date.now() + 1000 * 60 * 60; // One hour for now
    var token = encode({ ...account, exp: exp }, account.encryptionKey);

    localStorage.setItem(tokenKey, token);
  }
}

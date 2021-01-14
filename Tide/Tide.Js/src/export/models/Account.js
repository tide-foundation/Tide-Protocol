import { AESKey } from "cryptide";
import Guid from "../../guid";

export default class Account {
  /**
   * A Tide user account
   *
   * @param {String} username - The plain text username of the Tide user
   * @param {Guid} vuid - The vendor user ID of the user/vendor pair
   *  @param {String} tideToken - The jwt used to authenticate the user with the vendor
   * @param {String} cvkPublic - The public key of the users account
   * @param {C25519Key} encryptionKey - The key used for encryption
   *
   */
  constructor(username, vuid, jwt, cvkPublic, encryptionKey) {
    this.username = username;
    this.vuid = vuid;
    this.tideToken = jwt;
    this.cvkPublic = cvkPublic;
    this.encryptionKey = encryptionKey;
  }
}

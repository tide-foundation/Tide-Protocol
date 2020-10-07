import { AESKey } from "cryptide";
import Guid from "../../guid";

export default class Account {
  /**
   * A Tide user account
   *
   * @param {String} username - The plain text username of the Tide user
   * @param {Guid} vuid - The vendor user ID of the user/vendor pair
   * @param {AESKey} vendorKey - The key used for encryption/decryption of the vendor fields
   *
   */
  constructor(username, vuid, vendorKey) {
    this.username = username;
    this.vuid = vuid;
    this.vendorKey = vendorKey;
  }
}

export default class TideConfiguration {
  /**
   * Configuration
   *
   * @param {String} vendorId - Your designated VendorId in which you will operate
   * @param {String} serverUrl - The endpoint of your backend Tide server
   * @param {Array} homeOrks - The suggested initial point of contacts. At least 1 is required.
   *  @param {String} vendorPublic - The public of the vendor. Used to generate JWT
   *
   */

  constructor(vendorId, serverUrl, homeOrks, vendorPublic) {
    this.vendorId = vendorId;
    this.serverUrl = serverUrl;
    this.homeOrks = homeOrks;
    this.vendorPublic = vendorPublic;
  }
}

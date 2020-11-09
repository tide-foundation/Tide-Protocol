/**
 * Babel Starter Kit (https:
 *
 * Copyright © 2015-2016 Kriasoft, LLC. All rights reserved.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE.txt file in the root directory of this source tree.
 */

import Guid from "./guid";
import { C25519Key, Hash } from "cryptide";

export default class DnsEntry {
  constructor() {
    this.uid = new Guid();
    this.modifided = this.utcUnixSeconds();
    this.signature = "";
    /** @type {C25519Key} */
    this.public = null;
    /** @type { string[] } */
    this.signatures = [];
    /** @type { string[] } */
    this.orks = [];
  }

  /** @param {C25519Key} key */
  sign(key) {
    if (!this.public)
      throw new Error("The public key must be provided");
    
    this.signature = Buffer.from(key.sign(this.messageToSign())).toString('base64');
    return this.signature;
  }

  toString() {
    return JSON.stringify({ 
      uid: this.uid.toString(),
      orks: this.orks,
      public: this.public.toString(),
      modifided: this.modifided,
      signature: this.signature,
      signatures: this.signatures
    });
  }

  static from(data) {
    if (!data)
      throw Error("Null is not allowed to be parsed as a DnsEntry");
    
    data = typeof data === "string" ? JSON.parse(data) : data;
    var entry = new DnsEntry();

    entry.uid = Guid.from(data["uid"] || data["uId"]);
    entry.modifided = data.modifided;
    entry.signature = data.signature;
    entry.public = C25519Key.from(data.public);
    entry.signatures = data.signatures;
    entry.orks = data.orks;

    return entry;
  }

  /** @private */
  messageToSign() {
    var message = { uid: this.uid.toString(), orks: this.orks, 
      public: this.public.toString(), modifided: this.modifided };
    
    return Hash.shaBuffer(JSON.stringify(message));
  }

  /** @private */
  utcUnixSeconds() {
    const now = new Date()
    return Math.round((now.getTime() + (now.getTimezoneOffset() * 60 * 1000)) / 1000)  
  }
}

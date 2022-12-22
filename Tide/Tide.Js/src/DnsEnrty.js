// Tide Protocol - Infrastructure for the Personal Data economy
// Copyright (C) 2019 Tide Foundation Ltd
//
// This program is free software and is subject to the terms of
// the Tide Community Open Source License as published by the
// Tide Foundation Limited. You may modify it and redistribute
// it in accordance with and subject to the terms of that License.
// This program is distributed WITHOUT WARRANTY of any kind,
// including without any implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE.
// See the Tide Community Open Source License for more details.
// You should have received a copy of the Tide Community Open
// Source License along with this program.
// If not, see https://tide.org/licenses_tcosl-1-0-en

import Guid from "./guid";
import { C25519Key, Hash , ed25519Key, ed25519Point} from "cryptide";

/**
 * @typedef {Object} JsonDnsEntry - creates a new type named 'SpecialType'
 * @prop {string} id
 * @prop {number} modified
 * @prop {string} signature
 * @prop {string} Public
 * @prop {string[]} signatures
 * @prop {string[]} orks
 * @prop {string[]} urls
 * @prop {string[]} publics
 * @prop {string} gr
 * @prop {string []} vIdORK
 */

 export default class DnsEntry {
  constructor() {
    this.id = new Guid();
    this.modified = this.utcUnixSeconds();
    /** @type {ed25519Key} */
    this.Public = null;
    /** @type { string[] } */
    this.vIdORK =[];
    this.timestamp = "";
    this.s = "";
    /** @type {ed25519Point} */
    this.gR = null
}

  /** @param {ed25519Key} key */
  sign(key) {
    if (!this.Public)
      throw new Error("The public key must be provided");
    
    this.signature = Buffer.from(key.sign(this.messageToSign())).toString('base64'); // ecdsa needs hash here   // hardcoded edDSA here
    return this.signature;
  }

  toString() {
    return JSON.stringify({ 
      id: this.id.toString(),
      Public: this.Public.toString(),
      modified: this.modified,
      s: this.s,
      timestamp: this.timestamp.toString(),
      gR : Buffer.from(this.gR.toArray()).toString('base64'),
      vIdORK : this.vIdORK
    });
  }

  /** @param {string|JsonDnsEntry} data */
  static from(data) {
    if (!data)
      throw Error("Null is not allowed to be parsed as a DnsEntry");
    
    /** @type {JsonDnsEntry} */
    var json = typeof data === "string" ? JSON.parse(data) : data;
    var entry = new DnsEntry();

    entry.id = Guid.from(json.id);
    entry.modified = json.modified;
    entry.Public = json.Public==null ? ed25519Key.from(json.public) : ed25519Key.from(json.Public);
    entry.s = json.s;
    entry.vIdORK =json.vIdORK;
    entry.timestamp =json.timestamp;
    entry.gR = ed25519Point.from(Buffer.from(json.gr, 'base64'));

    return entry;
  }

  /** @private */
  messageToSign() {
    var message = { id: this.id.toString(), orks: this.orks, 
      Public: this.Public.toString() , modified: this.modified };
    
    return JSON.stringify(message);
  }

  /** @private */
  utcUnixSeconds() {
    const now = new Date()
    return Math.round((now.getTime() + (now.getTimezoneOffset() * 60 * 1000)) / 1000)  
  }
}

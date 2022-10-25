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
import { C25519Key, Hash , ed25519Key} from "cryptide";

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
 */

 export default class DnsEntry {
  constructor() {
    this.id = new Guid();
    this.modified = this.utcUnixSeconds();
    this.signature = "";
    /** @type {ed25519Key} */
    this.Public = null;
    /** @type { string[] } */
    this.signatures = [];
    /** @type { string[] } */
    this.orks = [];
    /** @type { string[] } */
    this.urls = [];
    /** @type { string[] } */
    this.Publics = [];
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
      orks: this.orks,
      Public: this.Public.toString(),
      modified: this.modified,
      signature: this.signature,
      signatures: this.signatures
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
    entry.signature = json.signature;
    entry.Public = ed25519Key.from(json.Public);
    entry.signatures = json.signatures;
    entry.orks = json.orks;
    entry.urls = json.urls || entry.urls;
    entry.publics = json.publics || entry.publics;

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

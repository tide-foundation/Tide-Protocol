/**
 * Babel Starter Kit (https:
 *
 * Copyright © 2015-2016 Kriasoft, LLC. All rights reserved.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE.txt file in the root directory of this source tree.
 */

import Cipher from "./Cipher";
import {verify} from "./MetaValidation";
import Num64 from "./Num64";

/** @typedef {'bool'|'date'|'datetime'|'string'|'number'} MetaType */

export default class MetaField {
  get value() { return this._value; } 
  set value(val) { 
    if (this._isEncrypted)
      throw new Error('Value cannot be modified if it is encrypted')
    
    this._value = val;
  } 

  /**
   * @private
   * @param {string} field
   * @param {string} value
   **/
  constructor(field, value, isEncrypted = false) {
    this.field = field;
    
    /**@type {MetaType}*/
    this.type = 'string';

    /**@type {string[]}*/
    this.valRules = [];

    /**@type {string[]}*/
    this.classRules = [];

    /**@type {string[]}*/
    this.classifications = [];
    
    /**@private*/
    this._value = value;

    /**@private*/
    this._isEncrypted = isEncrypted;

    /**@private*/
    this._previous = new Uint8Array();
  }

  isValid() {
    if (this._isEncrypted)
      true;

    return verify(this._value, this.valRules);
  }

  classify() {
    throw new Error('Method not implemented');
  }

  /**
   * @param {import("cryptide").C25519Key} key
   * @param {Num64} tag
   */
  encrypt(key, tag = null) {
    if (this._isEncrypted)
      throw new Error(`Data is already encrypted`);

    const tagCipher = tag ||
      (this._previous.length && Cipher.tag(this._previous)) || new Num64(0);
    
    this._value = Cipher.encrypt(this._value, tagCipher, key).toString('base64');
    this._isEncrypted = true;
  }

  /** @param {import("cryptide").C25519Key} key */
  decrypt(key) {
    if (!this._isEncrypted)
      throw new Error(`Data is already decrypted`);

    this._previous = Buffer.from(this._value, 'base64');

    this._value = Buffer.from(Cipher.decrypt(this._previous, key)).toString('utf-8');
    this._isEncrypted = false;
  }

  /**
  * @param {string} field
  * @param {string} value
  * @returns {MetaField}
  **/
  static fromCipher(field, value) {
    if (!value) return null;

    return new MetaField(field, value, true);
  }

  /**
  * @param {string} field
  * @param {string} value
  * @returns {MetaField}
  **/
  static fromPlain(field, value) {
    if (!value) return null;

    return new MetaField(field, value, false);
  }

  /**
  * @param {object} obj
  * @returns {MetaField[]}
  **/
  static fromCipherObject(obj) {
    if (!obj) return [];

    return Object.keys(obj).map(key => MetaField.fromCipher(key, String(obj[key])));
  }

  /**
  * @param {object} obj
  * @returns {MetaField[]}
  **/
  static fromPlainObject(obj) {
    if (!obj) return [];

    return Object.keys(obj).map(key => MetaField.fromPlain(key, String(obj[key])));
  }
}

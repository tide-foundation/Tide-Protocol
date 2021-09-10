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

import Cipher from "../../../../../Tide.Js/src/Cipher";
import Num64 from "../../../../../Tide.Js/src/Num64";
import Validator from "validatorjs";
import classificator, { EmptyClassification } from "./classification";

/** @typedef {'bool'|'date'|'datetime'|'string'|'number'} MetaType */
/** @typedef {{value: string; text: string;}} MetaOption */

export default class MetaField {
  get value() { return this._value; }
  set value(val) { 
    if (this._isEncrypted)
      throw new Error('Value cannot be modified if it is encrypted')
    
    this._value = val;
  } 

  get isEncrypted() { return this._isEncrypted; }

  get isValid() {
    if (this._isEncrypted || !this.valRules) return true;

    return new Validator({val: this._value} , {val: this.valRules}).passes();
  }

  /**@type {MetaOption[]}*/
  get options() { return this._class.options(); }
  get isInput() { return this._class.fieldType === 'input'; }
  get isSelect() { return this._class.fieldType === 'select'; }
  //
  /**
   * @private
   * @param {string} field
   * @param {string} value
   **/
  constructor(field, value, isEncrypted = false) {
    this.field = field;

    /**@type {MetaType}*/
    this.type = 'string';

    this.tag = new Num64(0);

    /**@type {string}*/
    this.valRules = null;

    /**@private*/
    this._value = value;

    /**@private*/
    this._isEncrypted = isEncrypted;

    /**@private*/
    this._previous = new Uint8Array();

    /**@private*/
    this._class = new EmptyClassification();
  }

  classify() {
    return this._class.classify();
  }

  /** @param {import("cryptide").C25519Key} key */
  encrypt(key) {
    if (this._isEncrypted)
      throw new Error(`Data is already encrypted`);

    const tagCipher = (!this.tag.isZero && this.tag)
      || (this._previous.length && Cipher.tag(this._previous))
      || new Num64(0);
    
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
   * @param {boolean} isEncrypted
   * @returns {MetaField}
   */
  static fromText(field, value, isEncrypted) {
    return new MetaField(field, value || '', isEncrypted);
  }

  /**
  * @param {object} data
  * @param {boolean} encrypted
  * @param {object} [validation]
  * @param {object} [classification]
  * @param {object} [tags]
  * @returns {MetaField[]}
  */
  static fromModel(data, encrypted, validation, classification, tags) {
    if (!data) return [];
    
    return Object.keys(data).map(field => {
      var val = typeof(data[field]) !== "undefined" && data[field] !== null ? data[field].toString() : '';
      var fld = MetaField.fromText(field, val, encrypted);
      
      if (validation && validation[field]) fld.valRules = validation[field];
      if (classification && classification[field]) fld._class = classificator(fld, classification[field]);
      if (tags && tags[field]) fld.tag = Num64.seed(tags[field]);
      
      return fld;
    });
  }
  /**
   * @param {MetaField[]} fields
   * @returns {object}
   */
  static buildModel(fields) {
    if (!fields || !fields.length) throw new Error('It cannot build a model with empty fields');
    if (fields.some(field => !field._isEncrypted)) throw new Error('All fields must be encrypted');

    const model = {};
    for (const field of fields) {
      model[field.field] = field.value;
    }
    return model;
  }

  /**
   * @param {MetaField[]} fields
   * @returns {object}
   */
  static buildClassification(fields) {
    if (!fields || !fields.length || fields.some(field => field._isEncrypted)) return null;

    let count = 0;
    const model = {};
    for (const field of fields) {
      const classification = field.classify();
      if (classification !== null) {
        model[field.field] = classification;
        count++;
      }
    }
    
    return count > 0 ? model : null;
  }
}

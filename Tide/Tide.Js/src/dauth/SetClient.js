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
// @ts-check

import { Dictionary, DictionaryPromise } from "../Tools";

/** @template T */
export default class SetClient {
  /** @param {T[]} clients */
  constructor(clients) {
    /** @private */
    this.clients = clients;
  }

  get length() { return this.clients.length; }

  get keys() { return Array.from(this.clients.keys()).map(String); }
  
  get values() { return this.clients; }

  /** 
   * @param {string|number} key
   * @returns {T} 
   */
  get(key) { return this.clients[key]; }

  /** @param {T} value */
  push(value) { this.clients.push(value); }

  /**
  * @template Y
  * @template U
  * @param {Dictionary<Y>} dic 
  * @param {(cli: T, value: Y, key: string, dic: Dictionary<Y>) => Promise<U>} fun
  * @returns {DictionaryPromise<U>}
  */
  map(dic, fun) {
    return DictionaryPromise.buildFrom(dic.keys, key => fun(this.clients[key], dic.get(key), key, dic));
  }

  /**
  * @template U
  * @param {string[]} keys
  * @param {(cli: T, key: string, keys: string[]) => Promise<U>} fun
  * @returns {DictionaryPromise<U>}
  */
   filter(keys, fun) {
    return DictionaryPromise.buildFrom(keys, key => fun(this.clients[key], key, keys));
  }

  /**
  * @template Y
  * @param {(cli: T, key: string) => Promise<Y>} fun
  * @returns {DictionaryPromise<Y>}
  */
  all(fun) {
    return DictionaryPromise.buildFrom(this.keys, key => fun(this.clients[key], key));
  }
}

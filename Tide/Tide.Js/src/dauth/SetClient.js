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

import TideConfig from "../TideConfig";
import { processError } from "./ClientBase";
import { Errors } from "./Errors";

/** @template T */
export default class SetClient {
  /** @param {T[]} clients */
  constructor(clients) {
    this.clients = clients;
  }

  /**
  * @template Y
  * @param {Array<string>} keys 
  * @param {(cli: T, key: string) => Promise<Y>} fun
  * @returns {Promise<{[x: string]: Y}|Errors>}
  */
  async call(fun, keys = null) {
    keys = keys ? keys : Array.from(this.clients.keys()).map(String);
    var allData = await Promise.all(keys.map(key => processError(fun(this.clients[key], key))));

    /** @type {{[x: string]: Y}} */
    const data = {};
    
    /** @type {Error[] } */
    const errors = [];
    for (let [i, elem] = [0, allData[0]]; i < allData.length; (i++, elem = allData[i])) {
      if (elem instanceof Error) errors.push(elem);
      else data[i] = elem;
    }

    return Object.keys(data).length >= TideConfig.threshold ?  data : new Errors(errors);
  }
}

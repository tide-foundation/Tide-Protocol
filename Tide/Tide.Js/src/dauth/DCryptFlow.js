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

import DCryptClient from "./DCryptClient";
import { C25519Point, AESKey, C25519Key, C25519Cipher } from "cryptide";
import KeyStore from "../keyStore";
import Cipher from "../Cipher";

export default class DCryptFlow {
  //TODO: cvkAuth should not be included here to generate user id
  //      due to vendor workflow that does not have access to it.
  /**
   * @param {string[]} urls
   * @param {string} user
   * @param {AESKey} cvkAuth
   */
  constructor(urls, user, cvkAuth) {
    this.clients = urls.map((url) => new DCryptClient(url, user, cvkAuth));
    this.user = user;
    this.cvkAuth = cvkAuth;
  }

  /**
   * @param {C25519Key} vendor
   * @param {number} threshold
   */
  async signUp(vendor, threshold) {
    try {
      const cvk = C25519Key.generate();
      const ids = this.clients.map((c) => c.clientId);
      
      const shrs = cvk.share(threshold, ids, true);
      const auths = this.clients.map((c) => this.cvkAuth.derive(c.clientBuffer));
      await Promise.all(this.clients.map((cli, i) => 
        cli.register(vendor, shrs[i].x, auths[i])));

      return cvk;
    } catch (err) {
      throw err;
    }
  }

  /** 
   * @param {Uint8Array} cipher
   * @param {C25519Key} prv
   */
  async decrypt(cipher, prv) {
    try {
      const keyId = new KeyStore(prv.public()).keyId;
      const challenges = await Promise.all(this.clients.map(cli => cli.challenge(keyId)));

      const sessionKeys = challenges.map(ch => prv.decryptKey(ch.challenge));
      const signs = sessionKeys.map(key => key.hash(cipher));

      const ciphers = await Promise.all(this.clients.map((cli, i) =>
        cli.decrypt(cipher, keyId, challenges[i].token, signs[i])));

      const ciph = Cipher.cipherFromAsymmetric(cipher);
      const partials = ciphers.map((cph, i) => C25519Point.from(sessionKeys[i].decrypt(cph)))
        .map(pnt => new C25519Cipher(pnt, ciph.c2));

      const ids = this.clients.map((c) => c.clientId);
      return C25519Cipher.decryptShares(partials, ids);
    } catch (err) {
      return Promise.reject(err);
    }
  }
}

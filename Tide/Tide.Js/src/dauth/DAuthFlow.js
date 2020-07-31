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

import BigInt from "big-integer";
import DAuthClient from "./DAuthClient";
import DAuthShare from "./DAuthShare";
import { SecretShare, Utils, C25519Point, AESKey } from "cryptide";
import Num64 from "../Num64";

export default class DAuthFlow {
  /**
   * @param {string[]} urls
   * @param {string} user
   */
  constructor(urls, user) {
    this.clients = urls.map((url) => new DAuthClient(url, user));
    this.user = user;
  }

  /**
   * @param {string} password
   * @param {string} email
   * @param {number} threshold
   */
  async signUp(password, email, threshold) {
    try {
      var key = random();
      var auth = random();
      var g = C25519Point.fromString(password);

      var mAuth = AESKey.seed(g.times(auth).toArray());
      var cmkAuth = AESKey.seed(Buffer.from(key.toArray(256).value));

      var ids = this.clients.map((c) => c.clientId);
      var sAuths = this.clients.map((c) => mAuth.derive(c.clientBuffer));
      var cmkAuths = this.clients.map((c) => cmkAuth.derive(c.clientBuffer));
      var [, kis] = SecretShare.shareFromIds(
        key,
        ids,
        threshold,
        C25519Point.n
      );
      var [, ais] = SecretShare.shareFromIds(
        auth,
        ids,
        threshold,
        C25519Point.n
      );

      await Promise.all(
        this.clients.map((cli, i) =>
          cli.signUp(ais[i], kis[i], sAuths[i], cmkAuths[i], email)
        )
      );
      return cmkAuth;
    } catch (err) {
      return Promise.reject(err);
    }
  }

  /** @param {string} password */
  async logIn(password) {
    try {
      var mAuth = await this.getAuthKey(password);
      var sAuths = this.clients.map((c) => mAuth.derive(c.clientBuffer));

      var ticks = getTicks();
      var signs = this.clients.map((c, i) =>
        sAuths[i].hash(Buffer.concat([c.userBuffer, Buffer.from(ticks.toArray())]))
      );

      var ciphers = await Promise.all(
        this.clients.map((cli, i) => cli.signIn(ticks, signs[i]))
      );
      var shares = sAuths
        .map((auth, i) => auth.decrypt(ciphers[i]))
        .map((shr) => BigInt.fromArray(Array.from(shr), 256, false));

      var ids = this.clients.map((c) => c.clientId);
      var key = SecretShare.interpolate(ids, shares, C25519Point.n);

      return AESKey.seed(Buffer.from(key.toArray(256).value));
    } catch (err) {
      return Promise.reject(err);
    }
  }

  /** @param {string} pass */
  async getAuthKey(pass) {
    try {
      var r = random();
      var n = C25519Point.n;
      var g = C25519Point.fromString(pass);
      var gR = g.multiply(r);

      var ids = this.clients.map((c) => c.clientId);
      var lis = ids.map((id) => SecretShare.getLi(id, ids, n));

      var gRKis = await Promise.all(
        this.clients.map((cli) => cli.GetShare(gR))
      );
      var gRK = gRKis
        .map((ki, i) => ki.times(lis[i]))
        .reduce((rki, sum) => rki.add(sum));
      var gK = gRK.times(r.modInv(n));

      return AESKey.seed(gK.toArray());
    } catch (err) {
      return Promise.reject(err);
    }
  }

    Recover() {
        return Promise.all(this.clients.map(cli => cli.Recover()));
    }

    /**
     * @param {string} textShares
     * @param {string} newPass
     * @param {number} threshold
     */
    async Reconstruct(textShares, newPass = null, threshold = null) {
        var shares = textShares.replace(/( +?)|\[|\]/g, '')
            .split(/\r?\n/).map(key => DAuthShare.from(key));
       
        var ids = shares.map(c => c.id);
        var values = shares.map(c => c.share);

    var ids = shares.map((c) => c.id);
    var values = shares.map((c) => c.share);

    var secretValue = SecretShare.interpolate(ids, values, C25519Point.n);
    var key = AESKey.seed(Buffer.from(secretValue.toArray(256).value));

    if (newPass !== null && threshold !== null) {
      await this.changePassWithKey(key, newPass, threshold);
    }

    return key;
  }

  /**
   * @param {string} pass
   * @param {string} newPass
   * @param {number} threshold
   */
  async changePass(pass, newPass, threshold) {
    try {
      var mAuth = await this.getAuthKey(pass);
      await this._changePass(mAuth, newPass, threshold);
    } catch (err) {
      return Promise.reject(err);
    }
  }

  /**
   * @param {AESKey} key
   * @param {string} pass
   * @param {number} threshold
   */
  changePassWithKey(key, pass, threshold) {
    return this._changePass(key, pass, threshold, true);
  }

  /**
   * @param {AESKey} key
   * @param {string} pass
   * @param {number} threshold
   */
  async _changePass(key, pass, threshold, withCmk = false) {
    try {
      var auth = random();
      var g = C25519Point.fromString(pass);
      var mAuth = AESKey.seed(g.times(auth).toArray());
      var sAuths = this.clients.map((c) => mAuth.derive(c.clientBuffer));
      var derivedKeys = this.clients.map((c) => key.derive(c.clientBuffer));

      var ticks = getTicks();
      var ids = this.clients.map((c) => c.clientId);
      var [, ais] = SecretShare.shareFromIds(
        auth,
        ids,
        threshold,
        C25519Point.n
      );
      var signs = this.clients.map((c, i) =>
        derivedKeys[i].hash(
          Buffer.concat([
            c.userBuffer,
            Buffer.from(ais[i].toArray(256).value),
            Buffer.from(sAuths[i].toArray()),
            Buffer.from(ticks.toArray()),
          ])
        )
      );

      await Promise.all(
        this.clients.map((cli, i) =>
          cli.changePass(ais[i], sAuths[i], ticks, signs[i], withCmk)
        )
      );
    } catch (err) {
      return Promise.reject(err);
    }
  }
}

function random() {
  return Utils.random(BigInt.one, C25519Point.n.subtract(BigInt.one));
}

function getTicks() {
  return new Num64(new Date().getTime()).mul(new Num64(10000))
    .add(Num64.from("621355968000000000"));
}

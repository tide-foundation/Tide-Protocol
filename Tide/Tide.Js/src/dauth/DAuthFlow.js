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

import BigInt from 'big-integer';
import DAuthClient from './DAuthClient';
import { SecretShare, Utils, C25519Point, AESKey } from 'cryptide';

export default class DAuthFlow {
    /**
     * @param {string[]} urls
     * @param {string} user
     */
    constructor(urls, user) {
        this.clients = urls.map(url => new DAuthClient(url, user));
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

            var ids = this.clients.map(c => c.clientId);
            var sAuths = this.clients.map(c => mAuth.derive(c.clientBuffer));
            var [, kis] = SecretShare.shareFromIds(key, ids, threshold, C25519Point.n);
            var [, ais] = SecretShare.shareFromIds(auth, ids, threshold, C25519Point.n);

            await Promise.all(this.clients.map((cli, i) => cli.signUp(ais[i], kis[i], sAuths[i], email)));
            return AESKey.seed(Buffer.from(key.toArray(256).value));
        } catch (err) {
            return Promise.reject(err);
        }
    }

    /** @param {string} password */
    async logIn(password) {
        try {
            var r = random();
            var n = C25519Point.n;
            var g = C25519Point.fromString(password);
            var gR = g.multiply(r);

            var gRKis = await Promise.all(this.clients.map(cli => cli.GetShare(gR)));
            var ids = this.clients.map(c => c.clientId);
            var lis = ids.map(id => SecretShare.getLi(id, ids, n));

            var gRK = gRKis.map((ki, i) => ki.times(lis[i])).reduce((rki, sum) => rki.add(sum));
            var gK = gRK.times(r.modInv(n));

            var mAuth = AESKey.seed(gK.toArray());
            var sAuths = this.clients.map(c => mAuth.derive(c.clientBuffer));
            var ticks = getTicks();
            var signs = this.clients.map((c, i) => sAuths[i].hash(Buffer.concat([c.userBuffer, ticks])))

            var ciphers = await Promise.all(this.clients.map((cli, i) => cli.signIn(ticks, signs[i])));
            var shares = sAuths.map((auth, i) => auth.decrypt(ciphers[i]))
                .map(shr => BigInt.fromArray(Array.from(shr), 256, false));

            var key = SecretShare.interpolate(ids, shares, C25519Point.n);
            return AESKey.seed(Buffer.from(key.toArray(256).value));
        } catch (err) {
            return Promise.reject(err);
        }
    }

    /**
     * @param {string} pass
     * @param {string} newPass
     * @param {number} threshold
     */
    async changePass(pass, newPass, threshold) {
        try {
            var r = random();
            var n = C25519Point.n;
            var auth = random();
            var g = C25519Point.fromString(pass);
            var gNew = C25519Point.fromString(newPass);
            var mAuthNew = AESKey.seed(gNew.times(auth).toArray());
            var gR = g.multiply(r);

            var ids = this.clients.map(c => c.clientId);
            var lis = ids.map(id => SecretShare.getLi(id, ids, n));
            
            var gRKis = await Promise.all(this.clients.map(cli => cli.GetShare(gR)));
            var gRK = gRKis.map((ki, i) => ki.times(lis[i])).reduce((rki, sum) => rki.add(sum));
            var gK = gRK.times(r.modInv(n));

            var ticks = getTicks();
            var mAuth = AESKey.seed(gK.toArray());
            var sAuths = this.clients.map(c => mAuth.derive(c.clientBuffer));
            
            var sAuthNews = this.clients.map(c => mAuthNew.derive(c.clientBuffer));
            var [, ais] = SecretShare.shareFromIds(auth, ids, threshold, C25519Point.n);
            var signs = this.clients.map((c, i) => sAuths[i].hash(Buffer.concat([c.userBuffer, Buffer.from(ais[i].toArray(256).value), sAuthNews[i].toArray(), ticks])))
            
            await Promise.all(this.clients.map((cli, i) => cli.changePass(ais[i], sAuthNews[i], ticks, signs[i])));
        } catch (err) {
            return Promise.reject(err);
        }
    }

}

function random() {
    return Utils.random(BigInt.one, C25519Point.n.subtract(BigInt.one));
}

function getTicks() {
    var time = BigInt(new Date().getTime());
    return Buffer.from(time.times(10000).add(621355968000000000).toArray(256).value);
}

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

import { AESKey, ed25519Key ,ed25519Point} from "cryptide";
import ClientBase, { urlEncode, fromBase64 } from "./ClientBase";
import Guid from "../guid";
import TranToken from "../TranToken";
import CVKRandomResponse from "./CVKRandomResponse";

export default class DCryptClient extends ClientBase {
  /**
   * @param {string|URL} url
   * @param {string|Guid} user
   */
  constructor(url, user, memory = false) {
    super(url, user, memory);
  }

  /**
   * @param {import("cryptide").ed25519Key} cvkPub
   * @param {import("big-integer").BigInteger} cvki
   * @param {AESKey} cvkAuthi
   * @param {Guid} signedKeyId
   * @param {Uint8Array} signature
   * @returns {Promise<{orkid: string, sign: string}>}
   */
  async register(cvkPub, cvki, cvkAuthi, signedKeyId, signature) {
    var body = [ urlEncode(cvkPub.toArray()),
      urlEncode(cvki),
      urlEncode(cvkAuthi.toArray()),
      urlEncode(signature) ];
    
    var resp = await this._put(`/cvk/${this.userGuid}/${signedKeyId}`).send(body);
    if (!resp.ok || !resp.body && !resp.body.success) {
      const error = !resp.ok || !resp.body ? resp.text : resp.body.error;
      return  Promise.reject(new Error(error));
    }
    
    return resp.body.content;
  }
  /**
   * @param {import("../guid").default[]} ids
   * @returns {Promise<CVKRandomResponse>}
   */
   async random(ids) {
    if (!ids || ids.length <= 0) throw Error('ids are not defined');
    const args = ids.map(id => `ids=${id}`).join('&');

    const resp = await this._get(`/cvk/random/${this.userGuid}?${args}`)
      .ok(res => res.status < 500);

    if (!resp.ok) return Promise.reject(new Error(resp.text));

    return CVKRandomResponse.from(resp.body);
  }
 /**
     * @param {import("./CVKRandRegistrationReq").default} body
     * * @param {import("../guid").default[]} ids
     * @param {ed25519Point} partialCvkPub
     * @param {ed25519Point} partialCvk2Pub
     * @returns {Promise<[OrkSign, string,ed25519Point,ed25519Point,bigint]>}
     */
  async randomSignUp(body,partialCvkPub,partialCvk2Pub,li) {
    if (!body) throw Error("The arguments cannot be null");

    var resp = await this._put(`/cvk/random/${this.userGuid}/${urlEncode(partialCvkPub.toArray())}/${urlEncode(partialCvk2Pub.toArray())}?li=${li.toString(10)}`).set("Content-Type", "application/json").send(JSON.stringify(body))
      .ok(res => res.status < 500);

    if (!resp.ok) return  Promise.reject(new Error(resp.text));
    
   // return resp.body;
   return [resp.body.signature, resp.body.encryptedToken, ed25519Point.from(Buffer.from(resp.body.cvkPub, 'base64')),ed25519Point.from(Buffer.from(resp.body.cvk2Pub, 'base64')),BigInt(resp.body.s)];
  }

    /**
   * @param {import("../guid").default } tranid
   * @param {TranToken} token
   * @param {DnsEntry} entry
   * @param {ed25519Point} partialPub
   *  @param {ed25519Point} partialCvk2Pub
   * @param {bigInt.BigInteger} li
   **/
     async signEntry(token, tranid, entry, partialPub, partialCvk2Pub, li) {
      const tkn = urlEncode(token.toArray());

      const resp = await this._get(`/cvk/sign/${this.userGuid}/${tkn}/${urlEncode(partialPub.toArray())}/${urlEncode(partialCvk2Pub.toArray())}?tranid=${tranid.toString()}&li=${li.toString(10)}`).set("Content-Type", "application/json").send(entry.toString())
        .ok(res => res.status < 500);
  
      if (!resp.ok) return  Promise.reject(new Error(resp.text));
      return BigInt(resp.text);
    }

  /** 
   * @param {string[]} vIdORKij
   * @param {number} numKeys
   * @param {string} signi
   * @param {ed25519Point} gVoucherPoint
   * @returns {Promise<[ed25519Point, string,string]>}
   */
  async genShard(vIdORKij , numKeys, signi , gVoucherPoint) {
    const gVoucher = urlEncode(gVoucherPoint.toArray());
    const orkIds = vIdORKij.map(id => `ids=${id}`).join('&');
        
    const resp = await this._get(`/cvk/genshard/${this.userGuid}?numKeys=${numKeys.toString()}&signi=${signi}&gVoucher=${gVoucher}&${orkIds}`)
      .ok(res => res.status < 500);
        
    if (!resp.ok) return Promise.reject(new Error(resp.text));
    return [ed25519Point.from(Buffer.from(resp.body.gCVKi, 'base64')), resp.body.yijCipher, resp.body.cVKtimestampi]
  }

  /** 
   * @param {string} yijCipher
   * @param {number} cVKtimestamp
   * @param {ed25519Point} gCMKAuth
   * @returns {Promise<[ed25519Point, ed25519Point, ed25519Point]>}
   */
  async setCVK(yijCipher,cVKtimestamp ,gCMKAuth) {
    const GCMKAuth = urlEncode(gCMKAuth.toArray());

    const resp = await this._get(`/cvk/set/${this.userGuid}?YijCipher=${yijCipher}&CVKtimestamp=${cVKtimestamp.toString()}&GCMKAuth=${GCMKAuth}`)
  
    if (!resp.ok) return  Promise.reject(new Error(resp.text));

    const obj = JSON.parse(resp.body.toString());
    return [ed25519Point.from(Buffer.from(obj.gCVKtesti, 'base64')),  ed25519Point.from(Buffer.from(obj.gCVK2testi, 'base64')), ed25519Point.from(Buffer.from(obj.gCVKRi, 'base64'))];

  }

  /**
   * @param { Guid } tranid
   * @param {AESKey} key
   * @param {bigInt.BigInteger} li
   **/
  async getCvk(tranid, key, li = null) {
    let token = new TranToken().sign(key, this.userBuffer);
    
    for (let i = 0; i < 2; i++) {
      try {
        const tkn = urlEncode(token.toArray());
        let url = `/cvk/${this.userGuid}/${tkn}?tranid=${tranid.toString()}`;
        if (li) {
          url += `&li=${li.toString(10)}`
        }

        const res = await this._get(url);

        return fromBase64(res.text);
      } catch (error) {
        if (error.status !== 408 || i > 0)
          throw error;

        token = TranToken.from(error.response.text).sign(key, this.userBuffer)
      }
    }
  }

  /** @param {Guid} keyId
   *  @return {Promise<{ token: string; challenge: string}>} */
  async challenge(keyId = null) {
    const pathId = keyId ? "/" + keyId.toString() : "";
    return (await this._get(`/cvk/challenge/${this.userGuid}${pathId}`)).body;
  }

  /**
   * @param {Uint8Array} data
   * @param {Guid} keyId
   * @param {string} token
   * @param {Uint8Array} sign
   */
  async decrypt(data, keyId, token, sign) {
    var cipher = Buffer.from(data).toString("base64");
    var tkn = urlEncode(token);
    var sgn = urlEncode(sign);

    var res = await this._post(`/cvk/plaintext/${this.userGuid}/${keyId}/${tkn}/${sgn}`)
      .set("Content-Type", "application/json").send(JSON.stringify(cipher));
    return fromBase64(res.text);
  }

  /**
   * @param {Uint8Array[]} data
   * @param {Guid} keyId
   * @param {string} token
   * @param {Uint8Array} sign
   */
  async decryptBulk(data, keyId, token, sign) {
    var cipher = data.map(dta => Buffer.from(dta).toString("base64")).join('\n');
    var tkn = urlEncode(token);
    var sgn = urlEncode(sign);

    var res = await this._post(`/cvk/plaintext/${this.userGuid}/${keyId}/${tkn}/${sgn}`)
      .set("Content-Type", "application/json").send(JSON.stringify(cipher));
    
    return res.text.split(/\r?\n/).map(txt => fromBase64(txt));
  }

  async confirm() {
    await this._post(`/cvk/${this.userGuid}`);
  }
}

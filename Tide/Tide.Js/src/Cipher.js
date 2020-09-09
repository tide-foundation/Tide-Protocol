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

import { randomBytes } from "crypto";
import {
  Hash,
  C25519Key,
  AesSherableKey,
  C25519Cipher,
  BnInput,
  Utils,
} from "cryptide";
import Num64 from "./Num64";

export default class Cipher {
  /**
   * @param {string|Uint8Array} data
   * @param {Num64} tag
   * @param {C25519Key} key
   */
  static encrypt(data, tag, key) {
    const buffer = typeof data === 'string' ? Buffer.from(data, "utf-8") : Buffer.from(data);
    
    /**@type Uint8Array */
    let toAsymmetricEncrypt = C25519Cipher.pad(buffer);
    let bufferSymmetric = Buffer.alloc(0);
    
    if (buffer.length > 32) {
      const secret = new AesSherableKey();
      bufferSymmetric = secret.encrypt(buffer);
      toAsymmetricEncrypt = secret.toArray();
    }
    
    const bufferAsymmetric = key.encrypt(toAsymmetricEncrypt).toArray();
    const tagBuffer = tag.toArray();
    const signature = Buffer.from(Utils.padLeft(key.sign(Buffer.concat([bufferAsymmetric, tagBuffer])), 32 * 3));
    
    const size =
      bufferAsymmetric.length +
      tagBuffer.length +
      signature.length +
      bufferSymmetric.length;
    const dimension = dimensionBuffer(size);

    const all = Buffer.alloc(1 + dimension.length + size);

    all[0] = 1; // version #

    dimension.copy(all, 1);
    let step = dimension.length + 1;

    bufferAsymmetric.copy(all, step);
    step += bufferAsymmetric.length;

    tagBuffer.copy(all, step);
    step += tagBuffer.length;

    signature.copy(all, step);
    step += signature.length;

    bufferSymmetric.copy(all, step);
    return all;
  }

  /**
   * @param {Uint8Array} data
   * @param {C25519Key} key
   */
  static decrypt(data, key) {
    let size = data[1] & 127;
    let sizeLength = 0;
    const hasFieldSize = (data[1] & 128) !== 0;
    if (hasFieldSize) {
      sizeLength = size;
      size = BnInput.getBig(data.slice(2, 2 + sizeLength)).valueOf();
    }
    let step = 2 + sizeLength;

    const asymmetricCipher = data.slice(step, step + 32 * 3);
    var asymmetricPlain = key.decrypt(asymmetricCipher);
    step += asymmetricSize();

    if (size === 200)
      return C25519Cipher.unpad(asymmetricPlain);

    const symmetricKey = AesSherableKey.from(asymmetricPlain);
    return symmetricKey.decrypt(data.slice(step));;
  }

  /** @param {Uint8Array} data */
  static asymmetric(data) {
    const step = headEnd(data);
    return data.slice(step, step + asymmetricSize());
  }

  /** @param {Uint8Array} data */
  static symmetric(data) {
    let step = headEnd(data);
    step += asymmetricSize()
    
    return data.slice(step);
  }

  /** @param {Uint8Array} data */
  static cipherFromAsymmetric(data) {
    return C25519Cipher.from(data.slice(0, 32 * 3));
  }
}

/** @param {Uint8Array} data */
function headEnd(data) {
  const sizeLength = (data[1] & 128) !== 0 ? data[1] & 127 : 0;
  return 2 + sizeLength;
}

/**
 * @param {number} size
 * @returns {Uint8Array}
 */
function dimensionBuffer(size) {
  const buffer = BnInput.getArray(BnInput.getBig(size));
  return buffer.length === 1 && buffer[0] < 128
    ? buffer
    : Buffer.concat([Buffer.from([(1 << 7) | buffer.length]), buffer]);
}

/*cipher + signature + tag*/
function asymmetricSize() {
  return 32 * 3 * 2 + 8;
}

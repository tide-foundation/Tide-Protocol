import { AESKey } from 'cryptide';
import IdGenerator from './IdGenerator';

export default class IdMacGenerator extends IdGenerator  {
    /** @param {string} text
     *  @param {AESKey} key */
    constructor(text, key) {
        super(text);
        this.key = key;
    }

    get buffer() { return getBufferId(this.text, this.key); }
}

/** @param {string|Uint8Array} data
 *  @param {AESKey} key */
export function getBufferId(data, key) {
    return key.hash(data).slice(0, 16);
}

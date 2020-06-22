import BigInt from 'big-integer';
import { Hash } from 'cryptide';

export default class IdGenerator {
    /** @param {string} text */
    constructor(text) {
        this.text = text;
    }

    get id() { return getId(this.buffer); }

    get buffer() { return getBufferId(this.text); }
}

/** @param {Uint8Array} buffer */
export function getId(buffer) {
    return BigInt.fromArray(Array.from(buffer), 256, false);
}

/** @param {string|Uint8Array} data */
export function getBufferId(data) {
    return Hash.shaBuffer(data).slice(0, 16);
}

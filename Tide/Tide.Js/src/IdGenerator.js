import { BnInput, AESKey } from 'cryptide';
import Guid from './guid';

export default class IdGenerator {
    /** @param {Guid} guid */
    constructor(guid) {
        this.guid = guid;
    }

    get id() { return BnInput.getBig(this.buffer); }

    get buffer() { return this.guid.toArray(); }

    /** @param {string | Uint8Array} data
     * @param {AESKey} key */
    static seed(data, key = null) {
        if (key == null)
            return new IdGenerator(Guid.seed(data));
        
        return new IdGenerator(Guid.from(key.hash(data).slice(0, 16)));
    }
}

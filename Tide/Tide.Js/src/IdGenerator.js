import { BnInput, AESKey } from "cryptide";
import Guid from "./guid";

export default class IdGenerator {
  /** @param {Guid} guid */
  constructor(guid) {
    this.guid = guid;
  }

  get id() {
    return BnInput.getBig(this.buffer);
  }

  get buffer() {
    return this.guid.toArray();
  }

  /** @param {string | URL | Uint8Array} data
   * @param {AESKey} key */
  static seed(data, key = null) {
    const buffer = typeof data === "string" ? Buffer.from(data, "utf8") : data instanceof URL ? Buffer.from(data.host, "utf8") : data;

    if (key == null) return new IdGenerator(Guid.seed(buffer));

    return new IdGenerator(Guid.from(key.hash(buffer).slice(0, 16)));
  }
}

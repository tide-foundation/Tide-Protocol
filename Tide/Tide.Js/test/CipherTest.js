import { randomBytes } from 'crypto';
import assert from 'assert';
import { C25519Key } from "cryptide";
import Cipher from "../src/Cipher";

describe('C25519Key', function () {
  it('should encrypt and decrypt using elgamal only', function () {
    console.log()
    var msg = randomBytes(24).toString("base64");
    var tag = randomBytes(10).toString("base64");
    var key = C25519Key.generate();

    for (let i = 0; i < 15; i++) {
      var cipher = Cipher.encrypt(msg, tag, key);
      var plain = Cipher.decrypt(cipher, key);
      var msgTag = Buffer.from(plain).toString("utf-8");

      assert.equal(msgTag, msg);
      assert.equal(cipher.length, 203);
    }
  });

  it('should encrypt and decrypt using elgamal and aes', function () {
    console.log()
    var msg = randomBytes(27).toString("base64");
    var tag = randomBytes(10).toString("base64");
    var key = C25519Key.generate();

    for (let i = 0; i < 15; i++) {
      var cipher = Cipher.encrypt(msg, tag, key);
      var plain = Cipher.decrypt(cipher, key);
      var msgTag = Buffer.from(plain).toString("utf-8");

      assert.equal(msgTag, msg);
      assert.equal(cipher.length > 203, true);
    }
  });
});

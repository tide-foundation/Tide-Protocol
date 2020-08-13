import { C25519Key } from "cryptide";
import Cipher from "../../src/Cipher";
import Num64 from "../../src/Num64";

var msg = "👋🏿 this is you!!!  😰🥰";
var tag = Num64.seed("note");
var key = C25519Key.generate();
var cipher = Cipher.encrypt(msg, tag, key);
var plain = Cipher.decrypt(cipher, key);
var msgTag = Buffer.from(plain).toString("utf-8");

console.log(msgTag);
console.log(msg);

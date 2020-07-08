import { DCryptFlow } from "tide";
import { C25519Key, AesSherableKey, AESKey, C25519Cipher } from "cryptide";

const dev = true;
const nodes = dev ? 3 : 10;
const dauthUrl = "http://127.0.0.1:500";

main();

function main() {
  document.getElementById("register").addEventListener("click", register);
  document.getElementById("register-genauth").addEventListener("click", genAuth);
  document.getElementById("registred-gentoencrypt").addEventListener("click", genToEncrypt);
  document.getElementById("registred-encrypt").addEventListener("click", encrypt);
  document.getElementById("vendor-genvendorkey").addEventListener("click", genVendroKey);
  document.getElementById("vendor-decrypt").addEventListener("click", decrypt);
}

function dcryptFlow(user, nodes) {
  var urls = dev ? [...Array(nodes)].map((_, i) => dauthUrl + (i + 1)) : [...Array(nodes)].map((_, i) => `https://ork-${i}.azurewebsites.net`);
  var cvkAuth = document.getElementById("register-cvkAuth").getAttribute("value")
  return new DCryptFlow(urls, user, AESKey.from(cvkAuth));
}

async function register() {
  var user = document.getElementById("register-username").value;
  var cvk = document.getElementById("registred-cvk");
  var vendorUser = document.getElementById("vendor-username");

  var vendorPub = document.getElementById("register-vendorPub").value;
  var vendorKey = C25519Key.fromString(vendorPub);

  var flow = dcryptFlow(user, nodes);
  var key = await flow.signUp(vendorKey, nodes);

  cvk.setAttribute("value", key.toString())
  vendorUser.setAttribute("value", flow.clients[0].userUrl);
  
  console.log(`CVK key generated and shared for user ${user}:`, key.toString());
}

function genToEncrypt() {
  var secret = new AesSherableKey();
  document.getElementById("registred-input")
    .setAttribute("value", secret.toString());
}

function genVendroKey() {
  var key = C25519Key.generate();
  var userVendorPub = document.getElementById("register-vendorPub");
  var vendorKey = document.getElementById("vendor-key");
  var vendorPub = document.getElementById("vendor-pub");

  vendorKey.setAttribute("value", key.toString());
  vendorPub.setAttribute("value", key.public().toString());
  userVendorPub.setAttribute("value", key.public().toString());
}

function genAuth() {
  var cvkAuth = new AESKey();
  document.getElementById("register-cvkAuth")
    .setAttribute("value", cvkAuth.toString());
}

function encrypt() {
  var cvk = document.getElementById("registred-cvk").value;
  var input = document.getElementById("registred-input").value;
  var encrypted = document.getElementById("registred-encrypted");
  var vendor = document.getElementById("vendor-encrypted");

  var cvkKey = C25519Key.fromString(cvk);
  var cipher = cvkKey.encryptKey(AesSherableKey.from(input)).toString();

  encrypted.textContent = cipher;
  vendor.textContent = cipher;
}

async function decrypt() {
  var user = document.getElementById("register-username").value;
  var cipherText = document.getElementById("vendor-encrypted").value;
  var keyValue = document.getElementById("vendor-key").value;
  var todecrypt = document.getElementById("vendor-todecrypt");

  var cipher = C25519Cipher.fromString(cipherText);
  var vendorKey = C25519Key.fromString(keyValue);

  var flow = dcryptFlow(user, nodes);

  var plain = await flow.decrypt(cipher, vendorKey);
  todecrypt.setAttribute("value", Buffer.from(plain).toString("base64"));
}

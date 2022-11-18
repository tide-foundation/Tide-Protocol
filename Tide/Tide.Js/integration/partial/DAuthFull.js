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

import DAuthJwtFlow from "../../src/dauth/DAuthJwtFlow";
import { CP256Key } from "cryptide";
import Guid from "../../src/guid";

var threshold = 3;
var pass = "123456";
var newPass = "1234567";
var email = "tmp@tide.org";
//var user = new Guid(); //A random user id key every time
//var user = Guid.seed("admin"); //An user if from text
var user = Guid.from('41581384-b772-3852-ba9e-25529721a1be');
  
//var vendorPub = CP256Key.generate(); //A random vendor key every time
var vendorPub = CP256Key.from('35S3ysH0quRR4cHV05VP87YlNiZ2KDX9Cxe7JNIsC34oHXTwbTW0Of2AtbvjYeXhPgOBpboWf+20I8EiFtMyBKbNkrKd9RdqFN6v7PoLDS09MTAE91POUJhqv5MeKzqH');
var orkUrls = [...Array(threshold)].map((_, i) => `http://ork${i+1}.local`); //the url of the orks

(async () => {
  try {
    console.log(`Trying a dAuth flow for... \nuserid: ${user} \nvvk: ${vendorPub.toString()}`);

    //await signUp();
    //await changePass(user, pass, newPass);
    await signIn(user, newPass);
    //await resetPass(user);
    //await reconstructPass(user);

    console.log(`all went well!!!`);
  } catch (err) {
    console.log('there was an error')
    console.log(err);
  }
})();

async function signUp() {
  var flowCreate = new DAuthJwtFlow(user);
  flowCreate.cmkUrls = orkUrls;
  flowCreate.cvkUrls = orkUrls;
  flowCreate.vendorPub = vendorPub;

  var account =  await flowCreate.signUp(pass, email, threshold);
  console.log(`[signUp] vuid: ${account.vuid} cvk: ${account.cvk.toString()}`);
}

async function signIn(user, pass) {
  var flowLogin = new DAuthJwtFlow(user);
  flowLogin.homeUrl = orkUrls[0];
  flowLogin.vendorPub = vendorPub;
  
  var account =  await flowLogin.logIn2(pass);
  console.log(`[signIn] vuid: ${account.vuid} cvk: ${account.cvk.toString()}`);
}

async function changePass(user, pass, newPass) {
  var flowLogin = new DAuthJwtFlow(user);
  flowLogin.homeUrl = orkUrls[0];
  flowLogin.vendorPub = vendorPub;

  await flowLogin.changePass(pass, newPass, threshold);
}

async function resetPass(user) {
  var flowLogin = new DAuthJwtFlow(user);
  flowLogin.homeUrl = orkUrls[0];
  flowLogin.vendorPub = vendorPub;

  await flowLogin.Recover();
}

async function reconstructPass(user) {
  var flowLogin = new DAuthJwtFlow(user);
  flowLogin.homeUrl = orkUrls[0];
  flowLogin.vendorPub = vendorPub;

  var shares = `[Rz2GgigbgKuXUrkwSYOF5wi2O9GzKdnktNTIKHVLTncyXv/FZZsQjaw2oHwACwi0]
[9UMaFR74kxhyxmRKjkC3Yg0sBI+3erqkgE1wGobBk35BQq2RR0Kv6nokoMru0YI9]
[PTupV0ZOEyw8p5rMK+PX0QN/TOjAmVahSKUSNj0+DUR9onrdbb4goGO/zgNsS2QZ]`;
  
  await flowLogin.Reconstruct(shares, newPass, threshold);
}

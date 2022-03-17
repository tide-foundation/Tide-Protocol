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

import assert from "assert";
import DAuthCmkJwtFlow from "../../src/dauth/DAuthCmkJwtFlow";
import DAuthJwtFlow from "../../src/dauth/DAuthJwtFlow";
import { CP256Key } from "cryptide";
import Guid from "../../src/guid";
import BigInt from "big-integer";
import { SecretShare, Utils, C25519Point, AESKey, C25519Key } from "cryptide";

var nodes = 3;
var threshold = 3;
var user = new Guid();
var pass = "123456";
var newPass = "1234567";
var email = "tmp@tide.org";

var orkUrls = [...Array(nodes)].map((_, i) => `http://ork${i+1}.local:500${i+1}`);
var vendorPub = CP256Key.generate();

// var orkUrls = [...Array(3)].map((_, i) => `https://ork-${i}.azurewebsites.net`);
// var vendorUrl = "https://tidevendor.azurewebsites.net/";
 
(async () => { 
  //tmp(); 
  await signUp();
})();

function tmp() {
  const cmkis = ['0f5431a151ef8931872c6644a8e40b762', '3d3ba957464e132c3ca79acc2be3d7d1', '473d8682281b80ab9752b930498385e7'].map(n => new BigInt(n, 16));
  const cmk = cmkis.reduce((sum, n) => sum.add(n).mod(C25519Point.n));
  const g = C25519Point.fromString(pass);
  const gPrism = C25519Point.fromString(pass).mul(cmk);

  const prismAuth = AESKey.seed(gPrism.toArray());
  console.log('prismX:', prismAuth.toString());

  const pointsV2 = cmkis.map(cmki => g.mul(cmki));
  console.log('point fromjs', pointsV2.map(itm => Buffer.from(itm.toArray()).toString('base64')));

  const prismAuthV2 = AESKey.seed(pointsV2.reduce((sum, itm) => sum.add(itm)).toArray());
  console.log('prismX2:', prismAuthV2.toString());

  const points = ['HYlL3oKc9uKM8m+vMdMgcw89T/kgeSSI00REwr3gUQd297q926VP3nuL6TbEntNFnurlaiQdcAsvWJFuZu4V3Q==',
    'HlnzVhil4FSz1Qdn+bO7dUK7RJGevGdYFL9A6YI6HhhLId160w17dMj435OzdaOOXhh+5ZemP4hLKVUq6oUH4w==',
    'DH7sTW2s7DWwoJ5YH/pYykoOuhg4C+63XKZJMVwVTQwCKMzXNGTkK5r+xFMLr/RcaV9r7uJ13url4H1xN+AGlg==']
    .map(pnt => Buffer.from(pnt, 'base64'))
    .map(pnt => C25519Point.from(pnt));

  const finalPnt = points.reduce((sum, pnt) => sum.add(pnt));
  const prismAuthTag = AESKey.seed(finalPnt.toArray());
  console.log('prismY:', prismAuthTag.toString());
}

async function signUp() {
  try {
    var flowCreate = new DAuthJwtFlow(user);
    flowCreate.cmkUrls = orkUrls;
    flowCreate.cvkUrls = orkUrls;
    flowCreate.vendorPub = vendorPub;

    var { auth: auth0 } = await flowCreate.signUp(pass, email, threshold);
    console.log('all perfect!!!');
  
    /*
    var flowLogin = new DAuthCmkJwtFlow(user);
    flowLogin.homeUrl = orkUrls[0];
    flowLogin.vendorPub = vendorPub;
    flowLogin.cmk = flowCreate.cmk;

    var { auth: auth1 } = await flowLogin.logIn();
    assert.equal(auth0.toString(), auth1.toString());

    await flowCreate.changePass(pass, newPass, threshold);
    var { auth: auth2 } = await flowCreate.logIn(newPass);
    assert.equal(auth0.toString(), auth2.toString());

    console.log(`all good for vuid ${flowCreate.vuid}`);
    */
  } catch (error) {
    console.log(error);
  }
}
    
       
           
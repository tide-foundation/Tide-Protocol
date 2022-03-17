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
// @ts-check

import assert from "assert";
import Guid from "../../src/guid";
import DAuthClient from "../../src/dauth/DAuthClient";
import { AESKey, C25519Point, C25519Key } from "cryptide";
import RandRegistrationReq from "../../src/dauth/RandRegistrationReq";


const threshold = 3;
const user = new Guid();
const pass = "123456";
const newPass = "1234567";
const email = "tmp@tide.org";

const urls = [...Array(threshold)].map((_, i) => `http://ork${i+1}.local:500${i+1}/`);
const clients = urls.map(url => new DAuthClient(url, user));

(async () => {
  await test();
  //await signUp();
})();

async function test () {
  try {
    const cli = clients[0];

  const jsonV2 = '{"prismAuth":"AhCJ09s7D75S4orOdrauyQhOIHQ6lpoD2kn3QaePhUzq+ziv0G7ipSelD6hnHrraPKbI","email":"tmp@tide.org","shares":[{"id":"151a43f5-f81e-1893-72c6-644a8e40b762","cmk":"9f7uPlVfmP8Pg8vo1qwxNaQ8sS6ZJ5oyiJ9/4e7QjA==","prism":"BvqMmNacK21hgW7JST6fesj/0ypJPjqG1tgNTZbjLbo="},{"id":"151a43f5-f81e-1893-72c6-644a8e40b762","cmk":"Bft574k7dj1Z/Yfw60oQlvbUJNXHF27ll/1KTqy28s0=","prism":"DO10/NeczuwH7bZPl5YQeAdOLqtcwwJWI39JdGPRk7I="},{"id":"151a43f5-f81e-1893-72c6-644a8e40b762","cmk":"CzDP3SbYvrN8NYF+lredIMGkQldXQH5VuXmrrBQmyz0=","prism":"DqmsupWEtmkeQfbqr0WZ5ESNmDS0wm5I2keu3FPUPck="}]}';
  const tmp = RandRegistrationReq.from(jsonV2);

  await cli.randomSignUp(tmp);
 
      
  console.log(tmp);
  }
  catch (error) {
    console.log(error);
  }
}

async function signUp() {
  try {
    //const ids = (await Promise.all(clients.map(cli => cli.getClientBuffer()))).map(buff => new Guid(buff));
    //console.log(ids);
    
    const cli = clients[0];
    let ids = [...Array(threshold)].map(() => new Guid());

    const gPass = C25519Point.g;
    const key = new AESKey();
    
    const random = await cli.random(gPass, ids);

    const req = new RandRegistrationReq(key, email, random);
    const req_ = RandRegistrationReq.from(req.toString());

    await cli.randomSignUp(req_);
 

    console.log("wujuuuu!!!!", req_);
  } catch (error) {
    console.log(error);
  }
}    
                 
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

import VendorClient from "../../src/VendorClient";
import Guid from "../../src/guid";
import { AESKey } from "cryptide";
import DCryptClient from "../../src/dauth/DCryptClient";

var vendorUrl = "http://127.0.0.1:6001";
var orkUrl = "http://127.0.0.1:5001";

(async () => {
  await signUp();
})();

async function signUp() {
  try {
    const vuid = new Guid();
    var auth = new AESKey();

    const vendor = new VendorClient(vendorUrl);
    const ork = new DCryptClient(orkUrl, vuid);

    await vendor.signup(vuid, auth, [ork.clientGuid]);
    await vendor.signin(vuid, auth);
    console.log("Wahoooo");
  } catch (error) {
    console.log(error);
  }
}

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

import C25519Key from "cryptide/src/c25519Key";
import ed25519Key from "cryptide/src/ed25519Key";
import MetaField from "./MetaField";
import Validator from "validatorjs";

testEncryption();
testValidation();

function testValidation() {
  const model = {
    message: "this is my secret message 🥵",
    age: 10,
    date: "2021/12/30",
  };

  const validation = {
    message: "required",
    age: "required|numeric|digits:2",
    date: "required|date",
  };

  const fields = MetaField.fromModel(model, false, validation);

  console.log(fields.every((itm) => itm.isValid));

  fields[0].value = "";
  fields[1].value = "";
  fields[2].value = "";

  console.log(fields.every((itm) => !itm.isValid));
}

function testEncryption() {
  var key = ed25519Key.generate();

  var field = MetaField.fromText("message", "this is my secret message 🥵", false);
  field.encrypt(key);

  const fieldTag = MetaField.fromText(field.field, field.value, true);

  field.decrypt(key);
  fieldTag.decrypt(key);

  console.log(field.value);
  console.log(fieldTag.value);
}

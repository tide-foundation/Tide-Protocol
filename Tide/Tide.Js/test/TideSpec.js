/**
 * Babel Starter Kit (https://www.kriasoft.com/babel-starter-kit)
 *
 * Copyright © 2015-2016 Kriasoft, LLC. All rights reserved.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE.txt file in the root directory of this source tree.
 */

import Tide from "../src/Tide";
import chai from "chai";
var expect = chai.expect;
var chaiAsPromised = require("chai-as-promised");

chai.use(chaiAsPromised);

const nodeCount = 3;
const nodes = [...Array(nodeCount)].map(
  (_, i) => `https://ork-${i}.azurewebsites.net`
);

var uniqueUsername = guid();
console.log('ran')
describe("Tide", () => {
  describe("tide.register()", () => {
    it("should return false when given invalid username", function () {
      const tide = new Tide(nodes);
      return expect(tide.register("abc", "123")).to.be.rejected;
    });

    it("should return keys when given unique credentials", function () {
      const tide = new Tide(nodes);
      return expect(
        tide.register(uniqueUsername, "123456789")
      ).to.eventually.have.property("key");
    });
  });

  describe("tide.login()", () => {
    it("should return false when given invalid username", function () {
      const tide = new Tide(nodes);
      return expect(tide.register(guid(), "123456789")).to.be.rejected;
    });

    it("should return keys when given correct credentials", function () {
      const tide = new Tide(nodes);
      return expect(
        tide.register(uniqueUsername, "123456789")
      ).to.eventually.have.property("key");
    });
  });
});

function guid() {
  return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function (c) {
    var r = (Math.random() * 16) | 0,
      v = c == "x" ? r : (r & 0x3) | 0x8;
    return v.toString(16);
  });
}

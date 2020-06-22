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

describe("Tide", () => {
  describe("tide.countNodes()", () => {
    it("should return the node count for default constructor (0)", () => {
      const tide = new Tide();
      const count = tide.countNodes();
      expect(count).to.be.equal(0);
    });

    it(`should return the node count for array of ${nodeCount}`, () => {
      const tide = new Tide(nodes);
      const count = tide.countNodes();
      expect(count).to.be.equal(nodeCount);
    });
  });

  describe("tide.register()", () => {
    it("should return false when given invalid username", function () {
      const tide = new Tide(nodes);
      return expect(tide.register("abc", "123")).to.be.rejected;
    });

    it("should return keys when given unique credentials", function () {
      const tide = new Tide(nodes);
      return expect(
        tide.register(guid(), "123456789")
      ).to.eventually.have.property("pub");
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
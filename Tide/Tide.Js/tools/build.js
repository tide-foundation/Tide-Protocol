const pkg = require("../package.json");
const fs = require("fs");
let promise = Promise.resolve();

promise = promise
  .then(() => {
    delete pkg.private;
    delete pkg.devDependencies;
    delete pkg.scripts;
    delete pkg.babel;
    fs.writeFileSync("dist/package.json", JSON.stringify(pkg, null, "  "), "utf-8");
    fs.copyFileSync("dist/tide.js", "test/integration/web/tide.js");
    fs.copyFileSync("dist/tide.js", "test/integration/es6/tide.js");
  })
  .catch((err) => console.error(err.stack));

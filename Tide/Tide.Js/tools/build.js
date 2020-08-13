const pkg = require("../package.json");
const fs = require("fs");
let promise = Promise.resolve();

promise = promise.then(() => {
  delete pkg.private;
  delete pkg.devDependencies;
  delete pkg.scripts;
  delete pkg.babel;
  fs.writeFileSync("dist/package.json", JSON.stringify(pkg, null, "  "), "utf-8");
});

promise.catch((err) => console.error(err.stack));

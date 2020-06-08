const path = require("path");

module.exports = {
  mode: "development",
  entry: {
    app: "./src/js/index.js",
  },
  output: {
    filename: "main.js",
    path: path.resolve(__dirname, "wwwroot", "js"),
  },
};

const path = require("path");

const profile = {
  mode: "production",
  entry: ["./src/index.js"],
  output: {
    path: path.resolve(__dirname, "dist/"),
    filename: "tide-button.js",
  },
  module: {
    rules: [
      {
        test: /\.css$/i,
        use: ["style-loader", "css-loader"],
      },
      {
        test: /\.(js|jsx)$/,
        exclude: /(node_modules|bower_components)/,
        loader: "babel-loader",
      },
    ],
  },
  resolve: { extensions: ["*", ".js", ".jsx"] },
};

// We need to do this 2 times. One to build the css and then another to inject it.
// There is certainly a better way but my webpack-foo is not strong
module.exports = [profile];

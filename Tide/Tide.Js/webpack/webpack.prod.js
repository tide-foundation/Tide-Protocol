const { merge } = require("webpack-merge");
const path = require("path");
const common = require("./webpack.common.js");
const { CleanWebpackPlugin } = require("clean-webpack-plugin");
const HtmlWebpackPlugin = require("html-webpack-plugin");

const browser = merge(common, {
  mode: "production",
  devtool: "source-map",
  plugins: [
    new CleanWebpackPlugin(),
    new HtmlWebpackPlugin({
      title: "Tide Integration Test",
    }),
  ],
  output: {
    path: path.resolve(__dirname + "/..", "dist"),
    filename: "tide.js",
    libraryTarget: "umd",
    globalObject: "this",
    library: "Tide",
  },
});

module.exports = [browser];

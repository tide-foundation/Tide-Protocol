const path = require("path");
const FileManagerPlugin = require("filemanager-webpack-plugin");

module.exports = {
  mode: "production",
  entry: ["./src/index.ts"],
  output: {
    path: path.resolve(__dirname, "dist/"),
    filename: "tide-button.js",
    globalObject: "this",
    library: "Tide",
  },
  module: {
    rules: [
      {
        test: /\.tsx?$/,
        use: "ts-loader",
        exclude: /node_modules/,
      },
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
  resolve: { extensions: ["*", ".js", ".jsx", ".tsx", ".ts"] },
  plugins: [
    new FileManagerPlugin({
      onEnd: {
        copy: [
          {
            source: "./dist/tide-button.js",
            destination: "../Tide.Vendor/Client/public/tide-button.js",
          },
        ],
      },
    }),
  ],
};

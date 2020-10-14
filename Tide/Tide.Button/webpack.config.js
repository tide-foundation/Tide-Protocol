const path = require("path");

module.exports = {
  mode: "production",
  entry: ["./src/index.ts"],
  output: {
    path: path.resolve(__dirname, "dist/"),
    filename: "tide-button.js",
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
};

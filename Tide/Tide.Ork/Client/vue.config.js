module.exports = {
  productionSourceMap: false,
  css: {
    extract: false,
  },
  configureWebpack: {
    optimization: {
      splitChunks: false,
    },
  },
};

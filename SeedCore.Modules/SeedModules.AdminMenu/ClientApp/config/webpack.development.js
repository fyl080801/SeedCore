process.env.PUBLIC_URL = process.env.SEED_MODULE;

const merge = require('webpack-merge');
const common = require('./webpack.common.js');
const paths = require('./paths');

const DEFAULT_PORT = parseInt(process.env.PORT, 10) || 3000;

module.exports = merge(common, {
  mode: 'development',
  devtool: 'inline-source-map',
  devServer: {
    open: false,
    contentBase: paths.appBuild,
    port: DEFAULT_PORT
  }
});

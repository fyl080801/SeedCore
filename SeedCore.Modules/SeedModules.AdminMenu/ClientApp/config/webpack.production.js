process.env.PUBLIC_URL = '/' + process.env.SEED_MODULE;

const merge = require('webpack-merge');
const UglifyJSPlugin = require('uglifyjs-webpack-plugin');
const common = require('./webpack.common.js');

module.exports = merge(common, {
  mode: 'production',
  devtool: false,
  plugins: [new UglifyJSPlugin()]
});

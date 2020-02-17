const merge = require('webpack-merge');
const common = require('./webpack.common.js');

module.exports = merge(common, {
  mode: 'development',
  devServer: {
    inline: true,
    contentBase: './dist',
    publicPath: `/${process.env['SEED_MODULE']}/`,
    port: process.env.PORT,
    compress: true
  },
  devtool: 'inline-source-map'
});

const path = require('path');

module.exports = {
  entry: './src/index.js',
  mode: 'development',
  output: {
    path: path.resolve(__dirname, 'dist'),
    filename: 'bundle.js'
  },
  devServer: {
    inline: true,
    publicPath: '/SeedModules.Setup/',
    filename: 'bundle.js',
    port: 9000,
    compress: true
  },
  resolve: {
    extensions: ['.js', '.jsx'] //后缀名自动补全
  },
  module: {
    rules: [
      { test: /\.(js|jsx)$/, exclude: /node_modules/, loader: 'babel-loader' }
    ]
  }
};

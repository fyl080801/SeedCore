const { CleanWebpackPlugin } = require('clean-webpack-plugin');

const paths = require('./paths');

module.exports = {
  entry: {
    navi: './src/index.js'
  },
  resolve: {
    extensions: ['.js', '.jsx']
  },
  module: {
    rules: [
      {
        test: /\.jsx?$/,
        use: [{ loader: 'babel-loader' }],
        exclude: /node_modules/
      }
    ]
  },
  plugins: [new CleanWebpackPlugin()],
  externals: {
    // react: 'react',
    // 'react-dom': 'react-dom'
  },
  output: {
    filename: 'navi.js',
    path: paths.appBuild,
    publicPath: paths.publicUrlOrPath
  }
};

const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin;

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
      },
      {
        test: /\.css$/,
        loader: 'style-loader!css-loader?modules'
      },
      {
        test: /\.less$/,
        exclude: /node_modules/,
        loader: 'style-loader!css-loader?modules!less-loader'
      },
      // 如果使用 moudles 并且 less 里 import 了 antd
      // less 类名会被转义
      {
        test: /\.less$/,
        include: /node_modules|antd\.less/,
        loader: 'style-loader!css-loader!less-loader'
      }
    ]
  },
  plugins: [
    new CleanWebpackPlugin(),
    new BundleAnalyzerPlugin()
    // new MiniCssExtractPlugin({ filename: 'navi.css', esModule: true })
  ],
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

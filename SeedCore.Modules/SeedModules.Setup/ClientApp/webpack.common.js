const path = require('path');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const { CleanWebpackPlugin } = require('clean-webpack-plugin');

module.exports = {
    entry: './src/index.js',
    output: {
        path: path.resolve(__dirname, 'dist'),
        filename: 'js/bundle.js'
    },
    plugins: [
        new CleanWebpackPlugin(),
        new HtmlWebpackPlugin({
            template: './public/index.html',
            templateParameters: (compilation, assets) => {
                assets.js = assets.js.map(item => `/${process.env['SEED_MODULE']}/` + item);
                assets.css = assets.css.map(item => `/${process.env['SEED_MODULE']}/` + item);
                assets.favicon = assets.favicon ? `/${process.env['SEED_MODULE']}/` + assets.favicon : assets.favicon;
                return assets;
            }
        })
    ],
    resolve: {
        extensions: ['.js', '.jsx', '.css'] //后缀名自动补全
    },
    module: {
        rules: [
            { test: /\.(js|jsx)$/, exclude: /node_modules/, loader: 'babel-loader' }
        ]
    }
};

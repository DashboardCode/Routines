const path = require('path');
const webpack = require('webpack');
const babelPresetEnv = require('babel-preset-env');
const ExtractTextPlugin = require('extract-text-webpack-plugin');
const CleanWebpackPlugin = require('clean-webpack-plugin'); // remove your build folder(s) before building
const CheckerPlugin = require('awesome-typescript-loader').CheckerPlugin;
const merge = require('webpack-merge');


//const bundleFolder = "wwwroot/build/";

module.exports = {
    module: {
        rules: [
            {
                test: /\.es8\.js$/,
                exclude: /node_modules/,
                use: {
                    loader: "babel-loader",
                    options: {
                        presets: ["env"] // https://babeljs.io/docs/plugins/preset-env , alternative https://github.com/christophehurpeau/babel-preset-modern-browsers
                        //babelrc: false,
                        //plugins: [require('@babel/plugin-proposal-object-rest-spread')]
                        //plugins: ['@babel/plugin-transform-runtime']
                    }
                }
            }
        ]
    }
};

//const path = require('path');
//const webpack = require('webpack');
////const ExtractTextPlugin = require('extract-text-webpack-plugin');
////const extractCSS = new ExtractTextPlugin('allstyles.css');

//module.exports = {
////    entry: { 'main': './wwwroot/source/app.js' },
//    output: {
//        path: path.resolve(__dirname, 'wwwroot/dist'),
//        filename: 'bundle.js',
//        publicPath: 'dist/'
//    },
////    plugins: [
////        extractCSS,
////        new webpack.ProvidePlugin({
////            $: 'jquery',
////            jQuery: 'jquery',
////            'window.jQuery': 'jquery',
////            Popper: ['popper.js', 'default']
////        }),
////        new webpack.optimize.UglifyJsPlugin()
////    ],
////    module: {
////        rules: [
////            { test: /\.css$/, use: extractCSS.extract(['css-loader?minimize']) },
////            { test: /\.js?$/, use: { loader: 'babel-loader', options: { presets: ['@babel/preset-react', '@babel/preset-env'] } } },
////        ]
////    }
//};
const path = require('path');
const autoprefixer = require('autoprefixer');
const webpack = require('webpack');
const babelPresetEnv = require('babel-preset-env');
const ExtractTextPlugin = require('extract-text-webpack-plugin');
const ManifestPlugin = require('webpack-manifest-plugin');
const CleanWebpackPlugin = require('clean-webpack-plugin'); // remove your build folder(s) before building
const CheckerPlugin = require('awesome-typescript-loader').CheckerPlugin;
const merge = require('webpack-merge');

const publicPath = process.env.PUBLIC_PATH || '/';
console.log(publicPath);
//const bundleFolder = "wwwroot/build/";

// How to cope with “broken modules” in webpack
// https://medium.com/webpack/how-to-cope-with-broken-modules-in-webpack-4c0427fb23a
// https://medium.com/@stefanledin/webpack-2-jquery-plugins-and-imports-loader-e0d984650058

// devserver
// https://medium.com/@estherfalayi/setting-up-webpack-for-bootstrap-4-and-font-awesome-eb276e04aaeb

// chunks, CleanWebpackPlugin, HashedModuleIdsPlugin, CommonsChunkPlugin, LoaderOptionsPlugin, UglifyJSPlugin, ExtractTextPlugin,ManifestPlugin
// https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/webpack.prod.js

// HtmlWebPackPlugin
// https://github.com/valentinogagliardi/webpack-4-quickstart/blob/master/webpack.config.js

// Typescript
// https://habrahabr.ru/post/328638/

// awesome-typescript-loader
// https://dotnetcore.gaprogman.com/2017/01/05/bundling-in-net-core-mvc-applications-with-webpack/

// ts loader
// http://leruplund.dk/2017/04/15/setting-up-asp-net-core-in-visual-studio-2017-with-npm-webpack-and-typescript-part-ii/

module.exports = {
    //externals: {
    //    react: 'React',
    //    'react-dom': 'ReactDOM'
    //},
    entry: [
        './src/images2'
    ],
    // MANAGE DEPENDENCY. METHOD 1. Manage dependencies at build-time.
    // replaces a symbol in another source through the respective import
     plugins: [
         new webpack.ProvidePlugin({
             '$': 'jquery',
             jQuery: 'jquery',
             'window.jQuery': 'jquery',
             'window.$': 'jquery'
         })
     ],
    module: {
        rules: [
            // MANAGE DEPENDENCY. METHOD 1. Manage dependencies at run-time. Adds modules, require('whatever') calls, to concreate modules
            // https://github.com/webpack-contrib/imports-loader
            // add the require('whatever') calls, to those modules (not global) 
                //{
                //    test: require.resolve('jquery'), // /legacy\.js$/,
                //    use: "imports-loader?this=>window"
                //    //use: "imports-loader?$=jquery"
                //    use: imports-loader?define=>false // disable AMD if you see that webpack include the same module two times: commonJS and AMD
                //},
            //{
            //    test: /vendor\/.+\.(jsx|js)$/,
            //    loader: 'imports?jQuery=jquery,$=jquery,this=>window'
            //}


            //{ 
            //     MANAGE DEPENDENCY. METHOD 2. Manage dependencies at run-time. Adds modules to the global object
            //     https://github.com/webpack-contrib/expose-loader
            //     exposes $, jQuery, window.$, window.jQuery on global level;
            //    test: require.resolve('jquery'),
            //    use: [{
            //        loader: 'expose-loader',
            //        options: 'jQuery'
            //    }, {
            //        loader: 'expose-loader',
            //        options: '$'
            //    }]
            //},
            {
                test: /\.(css)$/,
                use: [{ loader: 'style-loader' }, { loader: 'css-loader' }, { loader: 'resolve-url-loader' }]
            },
            {
                "test": /\.ts$/,
                "loader": 'awesome-typescript-loader'
            },
            {
                test: /\.(scss)$/,
                use: [{
                    loader: 'style-loader', // inject CSS to page
                }, {
                    loader: 'css-loader', // translates CSS into CommonJS modules
                }, {
                    loader: 'postcss-loader', // Run post css actions
                        options: {
                            sourceMap: true,
                            plugins: function () { // post css plugins, can be exported to postcss.config.js
                                return [
                                    require('precss'),
                                    require('autoprefixer')
                                ];
                            }
                        }
                }, {
                        loader: 'sass-loader', options: {
                            sourceMap: true
                        } // compiles Sass to CSS
                }]
            },
            //{
            //    test: /\.woff2?(\?v=[0-9]\.[0-9]\.[0-9])?$/,
            //    use: 'url-loader?limit=10000',
            //},
            {
                test: /\.(woff2|woff|ttf|svg)$/,
                use: 'url-loader',
            },
            {
                test: /\.(es8)\.(js)$/,
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
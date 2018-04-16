// webpack.config interpretated by node and node by default do not support ES6 (that means import etc.)

const CleanPlugin = require('clean-webpack-plugin');
const ManifestPlugin = require('webpack-manifest-plugin');
const PathModule = require('path');
const ExtractTextPlugin = require("extract-text-webpack-plugin");

const outputFolderPath = PathModule.resolve(__dirname, 'wwwroot/dist');

console.log('node interpretate webpack config:');
console.log('process.env.npm_config_shell: ' + process.env.npm_config_shell);
console.log('process.env.npm_config_version: ' + process.env.npm_config_version);
console.log('process.env.npm_config_script_shell: ' + process.env.npm_config_script_shell);
console.log('process.env.npm_package_version: ' + process.env.npm_package_version);

// How to master "legacy js" in webpack
// https://medium.com/webpack/how-to-cope-with-broken-modules-in-webpack-4c0427fb23a
// https://medium.com/@stefanledin/webpack-2-jquery-plugins-and-imports-loader-e0d984650058

// TODO: devserver
// https://medium.com/@estherfalayi/setting-up-webpack-for-bootstrap-4-and-font-awesome-eb276e04aaeb

// TODO: HashedModuleIdsPlugin, CommonsChunkPlugin, LoaderOptionsPlugin, UglifyJSPlugin, ExtractTextPlugin,ManifestPlugin
// https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/webpack.prod.js

// HtmlWebPackPlugin
// https://github.com/valentinogagliardi/webpack-4-quickstart/blob/master/webpack.config.js

// Typescript
// https://habrahabr.ru/post/328638/

// awesome-typescript-loader
// https://dotnetcore.gaprogman.com/2017/01/05/bundling-in-net-core-mvc-applications-with-webpack/

// ts loader
// http://leruplund.dk/2017/04/15/setting-up-asp-net-core-in-visual-studio-2017-with-npm-webpack-and-typescript-part-ii/

const extractCSS = new ExtractTextPlugin('main.css');
module.exports = {
    // TODO: ref "Vendor" files from CDN (externals, vendor options)
    // TODO: entry should be empty (npm run webuild used to define entry point and this should be enough)
    // TODO: HMR 'hot module replacement' as it is desribed in https://codeburst.io/how-to-use-webpack-in-asp-net-core-projects-a-basic-react-template-sample-25a3681a5fc2
    // one of HMR option is https://github.com/frankwallis/WebpackAspnetMiddleware 
    // TODO: build integration how is described with https://codeburst.io/how-to-use-webpack-in-asp-net-core-projects-a-basic-react-template-sample-25a3681a5fc2
    // IMPORTANT: only one js should be an entry point
    entry: './src/index.es8.js',
    output: {
        path: outputFolderPath,
        filename: '[name].js'
        //filename: '[name].[chunkhash].js',
    },
    plugins: [
        new ManifestPlugin(),
        new CleanPlugin(outputFolderPath, { verbose: false }),
        extractCSS
        // MANAGE DEPENDENCY. METHOD 1. Manage dependencies at build-time.
        // replaces a symbol in another source through the respective import
        //new webpack.ProvidePlugin({
        //    '$': 'jquery',
        //    jQuery: 'jquery',
        //    'window.jQuery': 'jquery',
        //    'window.$': 'jquery'
        // })
    ],
    module: {
        rules: [
            // MANAGE DEPENDENCY. METHOD 1. Manage dependencies at run-time. Adds modules, require('whatever') calls, to concreate modules
            // https://github.com/webpack-contrib/imports-loader
            // add the require('whatever') calls, to those modules (not global) 
            // {
            //     test: require.resolve('jquery'), // /legacy\.js$/,
            //     use: "imports-loader?this=>window"
            //     //use: "imports-loader?$=jquery"
            //     use: imports-loader?define=>false // disable AMD if you see that webpack include the same module two times: commonJS and AMD
            // },
            {
                // MANAGE DEPENDENCY. METHOD 2. Manage dependencies at run-time. Adds modules to the global object
                // https://github.com/webpack-contrib/expose-loader
                // exposes $, jQuery, window.$, window.jQuery on global level;
                test: require.resolve('jquery'),
                use: [{
                    loader: 'expose-loader',
                    options: 'jQuery'
                }, {
                    loader: 'expose-loader',
                    options: '$'
                }]
            },

            {
                test: /\.(scss)$/,
                use:
                    extractCSS.extract({
                        fallback: 'style-loader',
                        use:
                            [
                                {
                                    loader: 'css-loader'   // translates CSS into CommonJS JavaScript modules
                                }, {
                                    loader: 'postcss-loader', // run post css actions (here autoprefixer)
                                    options: {
                                        sourceMap: true,
                                        plugins: function () {
                                            return [
                                                require('precss'),
                                                require('autoprefixer') // adds "vendor's" prefixes e.g. -webkit-input-placeholder , -ms-input-placeholder etc.
                                            ];
                                        }
                                    }
                                }, {
                                    loader: 'sass-loader', options: {
                                        sourceMap: true
                                    } // compiles Sass to CSS
                                }]
                    })

            },

            {
                test: /\.ts$/,
                "loader": 'awesome-typescript-loader'
            },

            {
                test: /\.(woff2|woff|ttf|svg)$/,
                use: 'url-loader'
            },

            {
                test: /\.(es8)\.(js)$/,
                include: /src/,
                exclude: /node_modules/,
                use: {
                    loader: "babel-loader",
                    options: {
                        babelrc: false,
                        presets: [
                            ["@babel/env",
                                {
                                    "targets": {
                                        "browsers": ["last 2 chrome versions", "ie 11", "safari 11", "edge 15", "firefox 59"]
                                    },
                                    "debug": true
                                }
                            ]
                        ]
                        // presets: ["env"] // https://babeljs.io/docs/plugins/preset-env , alternative https://github.com/christophehurpeau/babel-preset-modern-browsers
                        // babelrc: false,
                        // plugins: [require('@babel/plugin-proposal-object-rest-spread')]
                        // plugins: ['@babel/plugin-transform-runtime']
                    }
                }
            }
            ,

            {
                include: /src/,
                exclude: /node_modules/,
                test: /global\.js$/,
                use: {
                    loader: "script-loader"
                }
            }
        ]
    }
};
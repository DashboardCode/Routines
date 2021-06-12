// webpack.config interpretated by node and node by default do not support ES6 (that means import etc.)
const webpack = require('webpack');
const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const { WebpackManifestPlugin } = require('webpack-manifest-plugin');
const PathModule = require('path');

// TODO uglify
//const UglifyJsPlugin = require("uglifyjs-webpack-plugin");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

// TODO replace with css-minimizer-webpack-plugin
//const OptimizeCSSAssetsPlugin = require("optimize-css-assets-webpack-plugin");

//const PolyfillInjectorPlugin = require('webpack-polyfill-injector');
const BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin;

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

var config = {
    // TODO: ref "Vendor" files from CDN (externals, vendor options)
    // TODO: entry should be empty (npm run webuild used to define entry point and this should be enough)
    // TODO: HMR 'hot module replacement' as it is desribed in https://codeburst.io/how-to-use-webpack-in-asp-net-core-projects-a-basic-react-template-sample-25a3681a5fc2
    // one of HMR option is https://github.com/frankwallis/WebpackAspnetMiddleware 
    // TODO: build integration how is described with https://codeburst.io/how-to-use-webpack-in-asp-net-core-projects-a-basic-react-template-sample-25a3681a5fc2
    // IMPORTANT: only one js should be an entry point

    // TRY: https://github.com/alexpalombaro/modernizr-webpack-plugin

    //entry: './src/index.js',

    //entry: './src/loaders.js',
    //entry: {
    //    app: `webpack-polyfill-injector?${JSON.stringify({
    //        modules: ['./src/index.es8.js'] 
    //    })}!` // don't forget the trailing exclamation mark!
    //},
    //entry: {
    //    app: [
            
    //        `webpack-polyfill-injector?${JSON.stringify({ modules: ['./src/index.es8.js'] })}!`,
    //        'jquery'
    //    ]
    //},
    output: {
        path: outputFolderPath,
        //mode: 'development',
        filename: '[name].js',  // filename: '[name].[contenthash].js' produce main.bca50319635bfdec741b.js - also add HashedModuleIdsPlugin if you want to use "constant" hashs
        publicPath: '/dist/'
    },
    resolve: {
        alias: {
           'datatables.net-buttons/buttons.colVis': 'datatables.net-buttons/js/buttons.colVis.js'
           // 'handlebars': 'handlebars/dist/handlebars.js',
           // 'corejs-typeahead': 'corejs-typeahead/dist/typeahead.jquery.js'
        }
    },
    optimization: {
        runtimeChunk: 'single',
        splitChunks: {
            chunks: 'all',
            maxInitialRequests: Infinity,
            minSize: 0,
            cacheGroups: {
                polyfill_io: {
                    name: 'polyfill_io',
                    test: /polyfill_io.js$/
                },
                styles: {
                    name: 'styles',
                    test: /customBootstrap.css$/,
                    chunks: 'all',
                    enforce: true
                },
                vendor: {
                    test: /[\\/]node_modules[\\/]/,
                    name: 'vendor'
                    // file for package
                    //name(module) {
                    //    // RP: node_modules/packageName/not/this/part.js or node_modules/packageName
                    //    const packageName = module.context.match(/[\\/]node_modules[\\/](.*?)([\\/]|$)/)[1];
                    //    return `npm.${packageName.replace('@', '')}`; //  RP: actual for .NET
                    //},
                }

            }
        }
    },
    plugins: [
         // TODO
        //new HtmlWebpackPlugin({
        //    template: 'src/index.html',
        //    filename: '../index.html',
        //    minify: false
        //}),

        new webpack.IgnorePlugin({
            resourceRegExp: /^\.\/locale$/,
            contextRegExp: /moment$/,
        }), // remove webpack locale files (safe 400KB space) https://github.com/jmblog/how-to-optimize-momentjs-with-webpack

        new MiniCssExtractPlugin({
            filename: "[name].css"
        }),
        new BundleAnalyzerPlugin({ analyzerMode: "static", openAnalyzer:false }),
        //new PolyfillInjectorPlugin({
        //    singleFile: true,
        //    polyfills: [
        //        'Element.prototype.matches',
        //        'Element.prototype.closest',
        //        'Element.prototype.classList'
        //    ]
        //}),
        new WebpackManifestPlugin(),
        new CleanWebpackPlugin() // defualt verbose:false

        // MANAGE DEPENDENCY. METHOD 1. Manage dependencies at build-time.
        // replaces a symbol in another source through the respective import
        //new webpack.ProvidePlugin({
        //    '$': 'jquery',
        //    jQuery: 'jquery',
        //    'window.jQuery': 'jquery',
        //    'window.$': 'jquery'
        // })
    ],
    devtool: "source-map",
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
                // MANAGE DEPENDENCY. METHOD 3. Manage dependencies at run-time. Adds modules to the global object
                // https://github.com/webpack-contrib/expose-loader
                // exposes $, jQuery, window.$, window.jQuery on global level;
                test: require.resolve('jquery'),
                loader: "expose-loader",
                options: {
                    exposes: ["$", "jQuery"]
                }
            },
            {
                test: require.resolve('moment'),
                loader: "expose-loader",
                options: {
                    exposes: ["moment"]
                }
            },
            {
                test: /\.(scss|css)$/,
                use: [
                    MiniCssExtractPlugin.loader,
                    {
                        loader: "css-loader",
                        options: {
                            sourceMap: true,
                            url: false // not locate the file during the bundling
                        }
                    },
                    {
                        loader: "postcss-loader",
                        options: {
                            postcssOptions: {
                                sourceMap: true,
                                plugins: [
                                    [
                                        "autoprefixer", // adds "vendor's" prefixes e.g. -webkit-input-placeholder , -ms-input-placeholder etc.
                                        {
                                            overrideBrowserslist: ["last 2 versions", ">1%"]
                                        }
                                    ],
                                ]
                            }
                        },
                    },
                    {
                        loader: "sass-loader",
                        options: { sourceMap: true }
                    }
                ]
            },
            {
                test: /\.(woff2|woff|eot|ttf|otf|svg|png|gif|jpg)$/,
                type: 'asset/inline'
            },
            {
                test: /\.(tsx?)|(js)$/,
                include: /src/,
                exclude: /node_modules/,
                use: {
                    loader: "babel-loader",
                    options: {
                        babelrc: false,
                        plugins: [
                            "@babel/plugin-proposal-class-properties",
                            "@babel/plugin-proposal-object-rest-spread"],
                        presets: [
                            "@babel/preset-typescript", // this preset contains plugin '@babel/plugin-transform-typescript'

                            // @babel/preset-env uses "@babel/polyfill" as facade for corejs polyfills:
                            // https://github.com/zloirock/core-js/blob/master/docs/2019-03-19-core-js-3-babel-and-a-look-into-the-future.md
                            // alternative to "@babel/polyfill" is "@babel/runtime":
                            // https://codersmind.com/babel-polyfill-babel-runtime-explained/
                            // https://babeljs.io/docs/en/babel-runtime-corejs2
                            ["@babel/preset-env",
                                {
                                    "useBuiltIns": "usage",
                                    "modules": false, // required for typescript?
                                    "corejs": 3,
                                    "targets": {
                                        "browsers": [
                                            "last 2 chrome versions", "ie 11", "safari 11", "edge 15", "firefox 59"
                                            //  bootsrap set:
                                            //  "chrome  >= 45",
                                            //  "Firefox >= 38",
                                            //  "Explorer >= 10",
                                            //  "edge >= 12",
                                            //  "iOS >= 9",
                                            //  "Safari >= 9",
                                            //  "Android >= 4.4",
                                            //  "Opera >= 30"
                                        ]
                                    },
                                    "debug": true
                                }
                            ]
                        ]
                    }
                }
            }
        ]
    }
};

module.exports = (env, argv) => {
    if (argv.mode === 'development') {
        console.log('!!! devServer started');

        // main app port is 63557
        config.devServer = {
            port: 63558,
            writeToDisk: true 
        };
    }
    return config;
};
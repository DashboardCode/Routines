// webpack.config interpretated by node and node by default do not support ES6 (that means import etc.)

const CleanPlugin = require('clean-webpack-plugin');
const ManifestPlugin = require('webpack-manifest-plugin');
const PathModule = require('path');

const UglifyJsPlugin = require("uglifyjs-webpack-plugin");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const OptimizeCSSAssetsPlugin = require("optimize-css-assets-webpack-plugin");


const PolyfillInjectorPlugin = require('webpack-polyfill-injector');
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

module.exports = {
    // TODO: ref "Vendor" files from CDN (externals, vendor options)
    // TODO: entry should be empty (npm run webuild used to define entry point and this should be enough)
    // TODO: HMR 'hot module replacement' as it is desribed in https://codeburst.io/how-to-use-webpack-in-asp-net-core-projects-a-basic-react-template-sample-25a3681a5fc2
    // one of HMR option is https://github.com/frankwallis/WebpackAspnetMiddleware 
    // TODO: build integration how is described with https://codeburst.io/how-to-use-webpack-in-asp-net-core-projects-a-basic-react-template-sample-25a3681a5fc2
    // IMPORTANT: only one js should be an entry point

    // TRY: https://github.com/alexpalombaro/modernizr-webpack-plugin

    entry: './src/index.js',

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
    optimization: {
        splitChunks: {
            cacheGroups: {
                styles: {
                    name: 'styles',
                    test: /\.css$/,
                    chunks: 'all',
                    enforce: true
                }
            }
        }
    },
    output: {
        path: outputFolderPath,
        publicPath: '/dist/',
        filename: '[name].js'
    },
    plugins: [
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
        new ManifestPlugin(),
        new CleanPlugin(outputFolderPath, { verbose: false })

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
                test: /\.(scss|css)$/,
                use: [
                    MiniCssExtractPlugin.loader,
                    {
                        loader: "css-loader",
                        options: {
                            sourceMap: true,
                            minimize: {
                                safe: true
                            }
                        }
                    },
                    {
                        loader: "postcss-loader",
                        options: {
                            sourceMap: true,
                            autoprefixer: {
                                browsers: ["last 2 versions"]
                            },
                            plugins: () => [
                                require('precss'),
                                require('autoprefixer') // adds "vendor's" prefixes e.g. -webkit-input-placeholder , -ms-input-placeholder etc.

                            ]
                        },
                    },
                    {
                        loader: "sass-loader",
                        options: { sourceMap: true }
                    }
                ]
            },
            {
                test: /\.(woff2|woff|ttf|svg|png)$/,
                use: 'url-loader'
            },

            {
                test: /\.(tsx?)|(js)$/, 
                include: /src/,
                exclude: /node_modules/,
                use: {
                    loader: "babel-loader",
                    options: {
                        babelrc: false,
                        plugins: ['babel-plugin-transform-class-properties'],
                        presets: [
                            "@babel/typescript", // this or plugin  @babel/plugin-transform-typescript
                            ["@babel/env",
                                {
                                    "modules": false, // required for typescript?
                                    "targets": {
                                        "browsers": ["last 2 chrome versions", "ie 11", "safari 11", "edge 15", "firefox 59"]
                                    },
                                    "debug": true
                                }
                            ]
                        ],
                        // plugins: [require('@babel/plugin-proposal-object-rest-spread')]
                        // plugins: ['@babel/plugin-transform-runtime']
                    }
                }
            }
        ]
    }
};
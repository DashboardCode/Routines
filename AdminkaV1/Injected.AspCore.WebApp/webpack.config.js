// webpack.config.js is interpretated by node;
// but only ECMAScript modules (mjs or package.json contains {"type": "module"}) can be imported using ES6 'import' statemnet.
// So using `import` requres additional investigation, therefore I stay with 'require'.
const webpack = require('webpack');
const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const { WebpackManifestPlugin } = require('webpack-manifest-plugin');
const PathModule = require('path');

// CONSIDER OPTION: uglify
//const UglifyJsPlugin = require("uglifyjs-webpack-plugin");

const MiniCssExtractPlugin = require("mini-css-extract-plugin");

const BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin;

const outputFolderPath = PathModule.resolve(__dirname, 'wwwroot/dist');

console.log('Node interpretates webpack config with such tools:');
// CONSIDER OPTION: to see all environment variables
// Object.keys(process.env).forEach(key => {console.log(`${key}: ${process.env[key]}`);});
(['NODE', 'npm_config_npm_version', 'npm_config_global_prefix', 'npm_config_user_agent', 'npm_package_name', 'npm_package_version'])
    .forEach(key => { console.log(`${key}: ${process.env[key]}`); });

const StatoscopeWebpackPlugin = require('@statoscope/webpack-plugin').default;

// TODO: HashedModuleIdsPlugin

// TODO: Typescript ???
// https://habrahabr.ru/post/328638/
// https://dotnetcore.gaprogman.com/2017/01/05/bundling-in-net-core-mvc-applications-with-webpack/
// http://leruplund.dk/2017/04/15/setting-up-asp-net-core-in-visual-studio-2017-with-npm-webpack-and-typescript-part-ii/

// TODO: How to link "Vendor" files from CDN

// TODO: HMR in ASP/webpack https://webpack.js.org/concepts/hot-module-replacement/
//                          https://learn.microsoft.com/en-us/aspnet/core/client-side/spa-services?view=aspnetcore-3.0#hot-module-replacement
//       Note: HMR replace modules while an application is running, without a full reload. This can significantly speed up development mode.
//       for using HMR withs ASP, ASP should work only as proxy to webpack development server (webpack-dev-server)
//       for configuring proxy this NUGET package https://www.nuget.org/packages/Microsoft.AspNetCore.SpaServices.Extensions should be used 
//       more information https://youtu.be/DH2yUVQDB0I?si=MipC50BzF00TTvrn&t=2092

var config = {
    output: {
        path: outputFolderPath,
        //mode: 'development',
        filename: '[name].js',  // CONSIDER OPTION - filename: '[name].[contenthash].js' produce main.bca50319635bfdec741b.js - add HashedModuleIdsPlugin if you need
        publicPath: '/dist/'
    },
    resolve: {
        alias: {
            'datatables.net-buttons#buttons.colVis': 'datatables.net-buttons/js/buttons.colVis.js',
            '@popperjs/core': '@popperjs/core/dist/umd/popper.js',
            'bootstrap#umd': 'bootstrap/dist/js/bootstrap.js',
            '@dashboardcode/bsmultiselect#umd':'@dashboardcode/bsmultiselect/dist/js/BsMultiSelect.js'
            
             // 'handlebars': 'handlebars/dist/handlebars.js',
             // 'corejs-typeahead': 'corejs-typeahead/dist/typeahead.jquery.js'
        }
    },
    optimization: { 
        runtimeChunk: 'single', // webpack runtime code in a separate file (runtime.js)
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
                    // CONSDIER OPTION: file for package for better control
                    // name(module) {
                    //    const packageName = module.context.match(/[\\/]node_modules[\\/](.*?)([\\/]|$)/)[1]; // this regexp should filter node_modules/packageName or smth like that
                    //    return `npm.${packageName.replace('@', '')}`; //  RP: actual for .NET
                    // },
                }

            }
        }
    },
    // Plugins that modify the build process itself are included in the "plugins" array. In contrast, some other plugins that
    // uses standard webpack process not need to be declared in the "plugins" array.
    plugins: [
        // IgnorePlugin prevents the generation of modules and ignore some `import` or `require` statements (works both css and js)
        new webpack.IgnorePlugin({
            resourceRegExp: /^\.\/locale$/,
            contextRegExp: /moment$/,
        }), // here configured to ignore moment.js locale files (save 400KB space) https://github.com/jmblog/how-to-optimize-momentjs-with-webpack

        // This plugin extracts CSS into separate files. It creates a CSS file per JS file which contains CSS. 
        // It supports On-Demand-Loading of CSS and SourceMaps.
        new MiniCssExtractPlugin({
            filename: "[name].css"
        }),
        new BundleAnalyzerPlugin({ analyzerMode: "static", openAnalyzer: false, reportFilename:"../reports/BundleAnalyzerReport.html" }),

        // WebpackManifestPlugin creates a manifest.json file in the output directory with a mapping of all source file names to their corresponding output files
        // CONSIDER OPTION: if there is a need to link the specific input file then manifest.json can be used to get the output file name
        // code like this should be used with Razor pages:
        // @{
        //    var manifest = System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "dist", "manifest.json"));
        //    var assets = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>> (manifest);
        // }
        // <!DOCTYPE html>
        // <html><head><link rel="stylesheet" href="/dist/@assets["main.css"]"></head>
        // <body>...
        // more info: https://webpack.js.org/concepts/manifest/ , https://webpack.js.org/guides/output-management/#the-manifest
        new WebpackManifestPlugin(),

        new CleanWebpackPlugin(), // defualt verbose:false

        new StatoscopeWebpackPlugin({
            saveReportTo: 'wwwroot/reports/statoscope-report.html', // Saves a detailed HTML report
            saveStatsTo: 'wwwroot/reports/statoscope-stats.json',   // Saves bundle stats JSON
            open: false,  // Change to `true` to open report automatically
        })
    ],
    devtool: "source-map",
    module: {
        rules: [
            {
                // expose-loader exposes/adds modules to the global object (window for browser, otherwise self and global)
                // https://github.com/webpack-contrib/expose-loader
                // exposes $, jQuery, window.$, window.jQuery
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
                // in Webpack, loaders are processed in reverse order from how they are listed in the use array, so the firs is sass-loader
                use: [
                    // MiniCssExtractPlugin is used together with css-loader when you want to extract CSS into separate files instead of including it in the JavaScript bundle. 
                    MiniCssExtractPlugin.loader,
                    {
                        // The css-loader interprets @import and url() like import/require() and will resolve them.The css-loader interprets @import and url() like import/require() and will resolve them.
                        loader: "css-loader",
                        options: {
                            sourceMap: true,
                            url: false // not locate the file during the bundling
                        }
                    },
                    {
                        // We need to use postcss since autoprefixer is postcss plugin
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
                        loader: "sass-loader", // the default order that sass-loader will resolve the implementation: sass-embedded, sass, node-sass
                        options: {
                            // https://sass-lang.com/documentation/js-api/interfaces/options/
                            sassOptions: {
                                verbose: true,
                                silenceDeprecations: ['mixed-decls','color-functions','import','global-builtin']
                            },
                            sourceMap: true
                        }
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
                        babelrc: false, // do not use babel config file, use only this config; babel config file left for "not bundle" compilation
                        presets: [
                            // preset options https://babeljs.io/docs/presets#preset-options ; next array is the one preset: name and second argument is options object
                            [
                                "@babel/preset-env", // maps javascript syntax to browsers features; uses two strategies: Babel transform plugins and core-js polyfills
                                                     // plugin's list: https://github.com/babel/babel/blob/main/packages/babel-preset-env/package.json
                                                     // core js is enabled through @babel/preset-env "babel-plugin-polyfill-corejs3" : facade for core-js polyfills:
                                                     // corej js: https://github.com/zloirock/core-js/blob/master/docs/2023-02-14-so-whats-next.md
                               {
                                    "loose": true,
                                    "modules": false,
                                    "useBuiltIns": "usage",
                                    "corejs": 3, 
                                    "include": [],
                                    "debug": true // list of used plugins to webpack output
                                    // exclude: ['@babel/plugin-transform-regenerator'], // use it to prevent transform into async/await into ES5 
                               }
                            ],
                            "@babel/react", // plugin's list: https://github.com/babel/babel/blob/main/packages/babel-preset-react/package.json
                            "@babel/preset-typescript" // plugin's list: https://github.com/babel/babel/blob/main/packages/babel-preset-typescript/package.json
                        ]
                    }
                }
            }
        ]
    }
};

module.exports = (env, argv) => {
    if (argv.mode === 'development') {
        console.log('webpack::module.exports: devServer started !!!');
        // CONSIDER OPTION: use HMR
        // config.plugins.push(new webpack.HotModuleReplacementPlugin());
        // main app port is 63557
        config.devServer = {
            port: 63558,
            writeToDisk: true 
        };
    }
    return config;
};
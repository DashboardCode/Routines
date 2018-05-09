import('jquery'); // will trigger expose-loader
import('webpack-polyfill-injector?{modules:["./src/index.es8.js"]}'); // will load polyfills (if necessary) and then start your normal entry module
//import '@babel/polyfill';
import Application from './Application';
import { createPopper } from '@popperjs/core';

// FYI: we use webpack' preset-env 'usage' option and that means no need of direct ES polyfill loading
// import '@babel/polyfill'; 

// TODO: investigate why this was not handled by usage? 
import "core-js/modules/es.array.includes";

// FYI: web polyfills from https://cdn.polyfill.io/v3/url-builder/
// https://polyfill.io/v3/polyfill.js?flags=always%7Cgated&features=Element.prototype.closest%2CElement.prototype.classList%2CElement.prototype.matches
// 'Element.prototype.matches', 'Element.prototype.closest', 'Element.prototype.classList'
// options: minify=NO, real user monitoring=no, feature detect=YES, always load=YES

import './polyfill_io';

// TODO: wait for polyfill-injectors synchronized loading of run-time scripts (async are not compatable with inline code)
// This is alternative to webpack configuration:
// import main from 'webpack-polyfill-injector?{modules:["./src/index.es8.js"]}!';  // will load polyfills (if necessary) and then start your normal entry module

// TODO migrate those popper v1 to v2 settings
//Popper.Defaults.modifiers.computeStyle.gpuAcceleration = !(window.devicePixelRatio < 1.5 && /Win/.test(navigator.platform));
//Popper.Defaults.modifiers.preventOverflow = { enabled: false };
//Popper.Defaults.modifiers.flip = { enabled: false };

// default bootstrap popover can't be setuped with dedicated element, this do the trick
$(document).ready(function () {

    const a = 2 ?? 3;
    $('[data-toggle="popover-content"]').popover({
        html: true,
        content: function () {
            return $('#popover-content').html();
        }
    });
});

window.AdminkaApp = new Application(window);
window.AdminkaApp.Es8TranspilerTest();
window.createPopper = createPopper;
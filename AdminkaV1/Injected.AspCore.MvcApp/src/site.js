import '@babel/polyfill';
import Es8Test from './Es8Test';
import WorkflowManager from './WorkflowManager.ts';
import Popper from 'popper.js';


// TODO: polyfil injectorss
//import jquery from 'jquery'; // will trigger expose-loader
//import main from 'webpack-polyfill-injector?{modules:["./src/index.es8.js"]}!';  // will load polyfills (if necessary) and then start your normal entry module


Popper.Defaults.modifiers.computeStyle.gpuAcceleration = !(window.devicePixelRatio < 1.5 && /Win/.test(navigator.platform));

let es8test = new Es8Test();
es8test.run();


let vm = new WorkflowManager("testop");


// global level functions, means browser interpretated.
// do not use code that need transpilers there
function ShowExceptionModal() {
    $("#exceptionModal").modal('toggle');
}

//// polyfill Element.closest 

if (!Element.prototype.matches)
    Element.prototype.matches = Element.prototype.msMatchesSelector ||
        Element.prototype.webkitMatchesSelector;

if (!Element.prototype.closest)
    Element.prototype.closest = function (s) {
        var el = this;
        if (!document.documentElement.contains(el)) return null;
        do {
            if (el.matches(s)) return el;
            el = el.parentElement || el.parentNode;
        } while (el !== null && el.nodeType === 1);
        return null;
    };
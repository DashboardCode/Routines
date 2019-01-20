(function (global, factory) {
  if (typeof define === "function" && define.amd) {
    define(["./Es8Test", "./WorkflowManager.ts", "popper.js"], factory);
  } else if (typeof exports !== "undefined") {
    factory(require("./Es8Test"), require("./WorkflowManager.ts"), require("popper.js"));
  } else {
    var mod = {
      exports: {}
    };
    factory(global.Es8Test, global.WorkflowManager, global.popper);
    global.site = mod.exports;
  }
})(this, function (_Es8Test, _WorkflowManager, _popper) {
  "use strict";

  _Es8Test = _interopRequireDefault(_Es8Test);
  _WorkflowManager = _interopRequireDefault(_WorkflowManager);
  _popper = _interopRequireDefault(_popper);

  function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

  //import '@babel/polyfill';
  //import $ from 'JQuery' 
  // TODO: polyfil injectorss
  //import jquery from 'jquery'; // will trigger expose-loader
  //import main from 'webpack-polyfill-injector?{modules:["./src/index.es8.js"]}!';  // will load polyfills (if necessary) and then start your normal entry module
  _popper.default.Defaults.modifiers.computeStyle.gpuAcceleration = !(window.devicePixelRatio < 1.5 && /Win/.test(navigator.platform));
  _popper.default.Defaults.modifiers.preventOverflow = {
    enabled: false
  };
  _popper.default.Defaults.modifiers.flip = {
    enabled: false
  };
  var es8test = new _Es8Test.default();
  es8test.run();
  var vm = new _WorkflowManager.default("testop"); // global level functions, means browser interpretated.
  // do not use code that need transpilers there

  function ShowExceptionModal() {
    $("#exceptionModal").modal('toggle');
  } //// polyfill Element.closest 
  //if (!Element.prototype.matches)
  //    Element.prototype.matches = Element.prototype.msMatchesSelector ||
  //        Element.prototype.webkitMatchesSelector;
  //if (!Element.prototype.closest)
  //    Element.prototype.closest = function (s) {
  //        var el = this;
  //        if (!document.documentElement.contains(el)) return null;
  //        do {
  //            if (el.matches(s)) return el;
  //            el = el.parentElement || el.parentNode;
  //        } while (el !== null && el.nodeType === 1);
  //        return null;
  //    };

});
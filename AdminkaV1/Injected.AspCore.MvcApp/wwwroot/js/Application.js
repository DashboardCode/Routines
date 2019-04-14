import "core-js/modules/web.dom.iterable";
import "core-js/modules/es6.array.iterator";
import "core-js/modules/es7.object.values";
import "core-js/modules/es6.promise";
import "core-js/modules/es6.object.to-string";
import "regenerator-runtime/runtime";

function asyncGeneratorStep(gen, resolve, reject, _next, _throw, key, arg) { try { var info = gen[key](arg); var value = info.value; } catch (error) { reject(error); return; } if (info.done) { resolve(value); } else { Promise.resolve(value).then(_next, _throw); } }

function _asyncToGenerator(fn) { return function () { var self = this, args = arguments; return new Promise(function (resolve, reject) { var gen = fn.apply(self, args); function _next(value) { asyncGeneratorStep(gen, resolve, reject, _next, _throw, "next", value); } function _throw(err) { asyncGeneratorStep(gen, resolve, reject, _next, _throw, "throw", err); } _next(undefined); }); }; }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function _createClass(Constructor, protoProps, staticProps) { if (protoProps) _defineProperties(Constructor.prototype, protoProps); if (staticProps) _defineProperties(Constructor, staticProps); return Constructor; }

import WorkflowManager from './WorkflowManager.ts';

var Application =
/*#__PURE__*/
function () {
  function Application(window) {
    _classCallCheck(this, Application);

    this.window = window;
    this.document = window.document;
    this.$ = this.window.$;
    this.console = window.console;
    this.alert = window.alert;
    this.WorkflowManager = WorkflowManager;
  }

  _createClass(Application, [{
    key: "Es8TranspilerTest",
    value: function () {
      var _Es8TranspilerTest = _asyncToGenerator(
      /*#__PURE__*/
      regeneratorRuntime.mark(function _callee() {
        var GetMessage, message;
        return regeneratorRuntime.wrap(function _callee$(_context) {
          while (1) {
            switch (_context.prev = _context.next) {
              case 0:
                GetMessage = function _ref() {
                  return new Promise(function (resolve) {
                    setTimeout(function () {
                      var object = {
                        x: 'es8',
                        y: 1
                      };
                      var values = Object.values(object);
                      resolve(values[0]);
                    }, 100);
                  });
                };

                _context.next = 3;
                return GetMessage();

              case 3:
                message = _context.sent;
                this.console.log("This is ".concat(message, " transpiler test: OK"));

              case 5:
              case "end":
                return _context.stop();
            }
          }
        }, _callee, this);
      }));

      function Es8TranspilerTest() {
        return _Es8TranspilerTest.apply(this, arguments);
      }

      return Es8TranspilerTest;
    }()
  }]);

  return Application;
}();

export { Application as default };
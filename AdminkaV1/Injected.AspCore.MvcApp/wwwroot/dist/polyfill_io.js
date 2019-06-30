(window["webpackJsonp"] = window["webpackJsonp"] || []).push([[3],{

/***/ 123:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* WEBPACK VAR INJECTION */(function(global) {/* harmony import */ var core_js_modules_es_symbol__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(99);
/* harmony import */ var core_js_modules_es_symbol__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(core_js_modules_es_symbol__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var core_js_modules_es_symbol_description__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(109);
/* harmony import */ var core_js_modules_es_symbol_description__WEBPACK_IMPORTED_MODULE_1___default = /*#__PURE__*/__webpack_require__.n(core_js_modules_es_symbol_description__WEBPACK_IMPORTED_MODULE_1__);
/* harmony import */ var core_js_modules_es_symbol_iterator__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(124);
/* harmony import */ var core_js_modules_es_symbol_iterator__WEBPACK_IMPORTED_MODULE_2___default = /*#__PURE__*/__webpack_require__.n(core_js_modules_es_symbol_iterator__WEBPACK_IMPORTED_MODULE_2__);
/* harmony import */ var core_js_modules_es_array_concat__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(125);
/* harmony import */ var core_js_modules_es_array_concat__WEBPACK_IMPORTED_MODULE_3___default = /*#__PURE__*/__webpack_require__.n(core_js_modules_es_array_concat__WEBPACK_IMPORTED_MODULE_3__);
/* harmony import */ var core_js_modules_es_array_iterator__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(128);
/* harmony import */ var core_js_modules_es_array_iterator__WEBPACK_IMPORTED_MODULE_4___default = /*#__PURE__*/__webpack_require__.n(core_js_modules_es_array_iterator__WEBPACK_IMPORTED_MODULE_4__);
/* harmony import */ var core_js_modules_es_array_join__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(112);
/* harmony import */ var core_js_modules_es_array_join__WEBPACK_IMPORTED_MODULE_5___default = /*#__PURE__*/__webpack_require__.n(core_js_modules_es_array_join__WEBPACK_IMPORTED_MODULE_5__);
/* harmony import */ var core_js_modules_es_array_slice__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(136);
/* harmony import */ var core_js_modules_es_array_slice__WEBPACK_IMPORTED_MODULE_6___default = /*#__PURE__*/__webpack_require__.n(core_js_modules_es_array_slice__WEBPACK_IMPORTED_MODULE_6__);
/* harmony import */ var core_js_modules_es_function_name__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(137);
/* harmony import */ var core_js_modules_es_function_name__WEBPACK_IMPORTED_MODULE_7___default = /*#__PURE__*/__webpack_require__.n(core_js_modules_es_function_name__WEBPACK_IMPORTED_MODULE_7__);
/* harmony import */ var core_js_modules_es_object_define_getter__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(138);
/* harmony import */ var core_js_modules_es_object_define_getter__WEBPACK_IMPORTED_MODULE_8___default = /*#__PURE__*/__webpack_require__.n(core_js_modules_es_object_define_getter__WEBPACK_IMPORTED_MODULE_8__);
/* harmony import */ var core_js_modules_es_object_define_setter__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(140);
/* harmony import */ var core_js_modules_es_object_define_setter__WEBPACK_IMPORTED_MODULE_9___default = /*#__PURE__*/__webpack_require__.n(core_js_modules_es_object_define_setter__WEBPACK_IMPORTED_MODULE_9__);
/* harmony import */ var core_js_modules_es_object_to_string__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(25);
/* harmony import */ var core_js_modules_es_object_to_string__WEBPACK_IMPORTED_MODULE_10___default = /*#__PURE__*/__webpack_require__.n(core_js_modules_es_object_to_string__WEBPACK_IMPORTED_MODULE_10__);
/* harmony import */ var core_js_modules_es_regexp_to_string__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(114);
/* harmony import */ var core_js_modules_es_regexp_to_string__WEBPACK_IMPORTED_MODULE_11___default = /*#__PURE__*/__webpack_require__.n(core_js_modules_es_regexp_to_string__WEBPACK_IMPORTED_MODULE_11__);
/* harmony import */ var core_js_modules_es_string_iterator__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(141);
/* harmony import */ var core_js_modules_es_string_iterator__WEBPACK_IMPORTED_MODULE_12___default = /*#__PURE__*/__webpack_require__.n(core_js_modules_es_string_iterator__WEBPACK_IMPORTED_MODULE_12__);
/* harmony import */ var core_js_modules_es_string_replace__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(116);
/* harmony import */ var core_js_modules_es_string_replace__WEBPACK_IMPORTED_MODULE_13___default = /*#__PURE__*/__webpack_require__.n(core_js_modules_es_string_replace__WEBPACK_IMPORTED_MODULE_13__);
/* harmony import */ var core_js_modules_es_string_split__WEBPACK_IMPORTED_MODULE_14__ = __webpack_require__(142);
/* harmony import */ var core_js_modules_es_string_split__WEBPACK_IMPORTED_MODULE_14___default = /*#__PURE__*/__webpack_require__.n(core_js_modules_es_string_split__WEBPACK_IMPORTED_MODULE_14__);
/* harmony import */ var core_js_modules_web_dom_collections_iterator__WEBPACK_IMPORTED_MODULE_15__ = __webpack_require__(144);
/* harmony import */ var core_js_modules_web_dom_collections_iterator__WEBPACK_IMPORTED_MODULE_15___default = /*#__PURE__*/__webpack_require__.n(core_js_modules_web_dom_collections_iterator__WEBPACK_IMPORTED_MODULE_15__);

















function _typeof(obj) { if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") { _typeof = function _typeof(obj) { return typeof obj; }; } else { _typeof = function _typeof(obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; } return _typeof(obj); }

/* Polyfill service v3.31.1
 * For detailed credits and licence information see https://github.com/financial-times/polyfill-service.
 * 
 * Features requested: Element.prototype.classList,Element.prototype.closest,Element.prototype.matches
 * 
 * - document, License: CC0 (required by "Element", "Element.prototype.classList", "Element.prototype.matches", "Element.prototype.closest", "document.querySelector")
 * - Element, License: CC0 (required by "Element.prototype.classList", "Element.prototype.matches", "Element.prototype.closest", "document.querySelector")
 * - document.querySelector, License: CC0 (required by "Element.prototype.matches", "Element.prototype.closest")
 * - Element.prototype.matches, License: CC0 (required by "Element.prototype.closest")
 * - Element.prototype.closest, License: CC0
 * - Object.defineProperty, License: CC0 (required by "Element.prototype.classList", "_DOMTokenList", "DOMTokenList")
 * - _DOMTokenList, License: ISC (required by "DOMTokenList", "Element.prototype.classList")
 * - DOMTokenList, License: CC0 (required by "Element.prototype.classList")
 * - Element.prototype.classList, License: ISC */
(function (undefined) {
  if (!("document" in this)) {
    // document
    if (typeof WorkerGlobalScope === "undefined" && typeof importScripts !== "function") {
      if (this.HTMLDocument) {
        // IE8
        // HTMLDocument is an extension of Document.  If the browser has HTMLDocument but not Document, the former will suffice as an alias for the latter.
        this.Document = this.HTMLDocument;
      } else {
        // Create an empty function to act as the missing constructor for the document object, attach the document object as its prototype.  The function needs to be anonymous else it is hoisted and causes the feature detect to prematurely pass, preventing the assignments below being made.
        this.Document = this.HTMLDocument = document.constructor = new Function('return function Document() {}')();
        this.Document.prototype = document;
      }
    }
  }

  if (!("Element" in this && "HTMLElement" in this)) {
    // Element
    (function () {
      // IE8
      if (window.Element && !window.HTMLElement) {
        window.HTMLElement = window.Element;
        return;
      } // create Element constructor


      window.Element = window.HTMLElement = new Function('return function Element() {}')(); // generate sandboxed iframe

      var vbody = document.appendChild(document.createElement('body'));
      var frame = vbody.appendChild(document.createElement('iframe')); // use sandboxed iframe to replicate Element functionality

      var frameDocument = frame.contentWindow.document;
      var prototype = Element.prototype = frameDocument.appendChild(frameDocument.createElement('*'));
      var cache = {}; // polyfill Element.prototype on an element

      var shiv = function shiv(element, deep) {
        var childNodes = element.childNodes || [],
            index = -1,
            key,
            value,
            childNode;

        if (element.nodeType === 1 && element.constructor !== Element) {
          element.constructor = Element;

          for (key in cache) {
            value = cache[key];
            element[key] = value;
          }
        }

        while (childNode = deep && childNodes[++index]) {
          shiv(childNode, deep);
        }

        return element;
      };

      var elements = document.getElementsByTagName('*');
      var nativeCreateElement = document.createElement;
      var interval;
      var loopLimit = 100;
      prototype.attachEvent('onpropertychange', function (event) {
        var propertyName = event.propertyName,
            nonValue = !cache.hasOwnProperty(propertyName),
            newValue = prototype[propertyName],
            oldValue = cache[propertyName],
            index = -1,
            element;

        while (element = elements[++index]) {
          if (element.nodeType === 1) {
            if (nonValue || element[propertyName] === oldValue) {
              element[propertyName] = newValue;
            }
          }
        }

        cache[propertyName] = newValue;
      });
      prototype.constructor = Element;

      if (!prototype.hasAttribute) {
        // <Element>.hasAttribute
        prototype.hasAttribute = function hasAttribute(name) {
          return this.getAttribute(name) !== null;
        };
      } // Apply Element prototype to the pre-existing DOM as soon as the body element appears.


      function bodyCheck() {
        if (!loopLimit--) clearTimeout(interval);

        if (document.body && !document.body.prototype && /(complete|interactive)/.test(document.readyState)) {
          shiv(document, true);
          if (interval && document.body.prototype) clearTimeout(interval);
          return !!document.body.prototype;
        }

        return false;
      }

      if (!bodyCheck()) {
        document.onreadystatechange = bodyCheck;
        interval = setInterval(bodyCheck, 25);
      } // Apply to any new elements created after load


      document.createElement = function createElement(nodeName) {
        var element = nativeCreateElement(String(nodeName).toLowerCase());
        return shiv(element);
      }; // remove sandboxed iframe


      document.removeChild(vbody);
    })();
  }

  if (!("document" in this && "querySelector" in this.document)) {
    // document.querySelector
    (function () {
      var head = document.getElementsByTagName('head')[0];

      function getElementsByQuery(node, selector, one) {
        var generator = document.createElement('div'),
            id = 'qsa' + String(Math.random()).slice(3),
            style,
            elements;
        generator.innerHTML = 'x<style>' + selector + '{qsa:' + id + ';}';
        style = head.appendChild(generator.lastChild);
        elements = getElements(node, selector, one, id);
        head.removeChild(style);
        return one ? elements[0] : elements;
      }

      function getElements(node, selector, one, id) {
        var validNode = /1|9/.test(node.nodeType),
            childNodes = node.childNodes,
            elements = [],
            index = -1,
            childNode;

        if (validNode && node.currentStyle && node.currentStyle.qsa === id) {
          if (elements.push(node) && one) {
            return elements;
          }
        }

        while (childNode = childNodes[++index]) {
          elements = elements.concat(getElements(childNode, selector, one, id));

          if (one && elements.length) {
            return elements;
          }
        }

        return elements;
      }

      Document.prototype.querySelector = Element.prototype.querySelector = function querySelectorAll(selector) {
        return getElementsByQuery(this, selector, true);
      };

      Document.prototype.querySelectorAll = Element.prototype.querySelectorAll = function querySelectorAll(selector) {
        return getElementsByQuery(this, selector, false);
      };
    })();
  }

  if (!("document" in this && "matches" in document.documentElement)) {
    // Element.prototype.matches
    Element.prototype.matches = Element.prototype.webkitMatchesSelector || Element.prototype.oMatchesSelector || Element.prototype.msMatchesSelector || Element.prototype.mozMatchesSelector || function matches(selector) {
      var element = this;
      var elements = (element.document || element.ownerDocument).querySelectorAll(selector);
      var index = 0;

      while (elements[index] && elements[index] !== element) {
        ++index;
      }

      return !!elements[index];
    };
  }

  if (!("document" in this && "closest" in document.documentElement)) {
    // Element.prototype.closest
    Element.prototype.closest = function closest(selector) {
      var node = this;

      while (node) {
        if (node.matches(selector)) return node;else node = 'SVGElement' in window && node instanceof SVGElement ? node.parentNode : node.parentElement;
      }

      return null;
    };
  }

  if (!("defineProperty" in Object && function () {
    try {
      var e = {};
      return Object.defineProperty(e, "test", {
        value: 42
      }), !0;
    } catch (t) {
      return !1;
    }
  }())) {
    // Object.defineProperty
    (function (nativeDefineProperty) {
      var supportsAccessors = Object.prototype.hasOwnProperty('__defineGetter__');
      var ERR_ACCESSORS_NOT_SUPPORTED = 'Getters & setters cannot be defined on this javascript engine';
      var ERR_VALUE_ACCESSORS = 'A property cannot both have accessors and be writable or have a value'; // Polyfill.io - This does not use CreateMethodProperty because our CreateMethodProperty function uses Object.defineProperty.

      Object['defineProperty'] = function defineProperty(object, property, descriptor) {
        // Where native support exists, assume it
        if (nativeDefineProperty && (object === window || object === document || object === Element.prototype || object instanceof Element)) {
          return nativeDefineProperty(object, property, descriptor);
        }

        if (object === null || !(object instanceof Object || _typeof(object) === 'object')) {
          throw new TypeError('Object.defineProperty called on non-object');
        }

        if (!(descriptor instanceof Object)) {
          throw new TypeError('Property description must be an object');
        }

        var propertyString = String(property);
        var hasValueOrWritable = 'value' in descriptor || 'writable' in descriptor;

        var getterType = 'get' in descriptor && _typeof(descriptor.get);

        var setterType = 'set' in descriptor && _typeof(descriptor.set); // handle descriptor.get


        if (getterType) {
          if (getterType !== 'function') {
            throw new TypeError('Getter must be a function');
          }

          if (!supportsAccessors) {
            throw new TypeError(ERR_ACCESSORS_NOT_SUPPORTED);
          }

          if (hasValueOrWritable) {
            throw new TypeError(ERR_VALUE_ACCESSORS);
          }

          Object.__defineGetter__.call(object, propertyString, descriptor.get);
        } else {
          object[propertyString] = descriptor.value;
        } // handle descriptor.set


        if (setterType) {
          if (setterType !== 'function') {
            throw new TypeError('Setter must be a function');
          }

          if (!supportsAccessors) {
            throw new TypeError(ERR_ACCESSORS_NOT_SUPPORTED);
          }

          if (hasValueOrWritable) {
            throw new TypeError(ERR_VALUE_ACCESSORS);
          }

          Object.__defineSetter__.call(object, propertyString, descriptor.set);
        } // OK to define value unconditionally - if a getter has been specified as well, an error would be thrown above


        if ('value' in descriptor) {
          object[propertyString] = descriptor.value;
        }

        return object;
      };
    })(Object.defineProperty);
  } // _DOMTokenList

  /*
  Copyright (c) 2016, John Gardner
  
  Permission to use, copy, modify, and/or distribute this software for any purpose with or without fee is hereby granted, provided that the above copyright notice and this permission notice appear in all copies.
  
  THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
  */


  var _DOMTokenList = function () {
    // eslint-disable-line no-unused-vars
    var dpSupport = true;

    var defineGetter = function defineGetter(object, name, fn, configurable) {
      if (Object.defineProperty) Object.defineProperty(object, name, {
        configurable: false === dpSupport ? true : !!configurable,
        get: fn
      });else object.__defineGetter__(name, fn);
    };
    /** Ensure the browser allows Object.defineProperty to be used on native JavaScript objects. */


    try {
      defineGetter({}, "support");
    } catch (e) {
      dpSupport = false;
    }

    var _DOMTokenList = function _DOMTokenList(el, prop) {
      var that = this;
      var tokens = [];
      var tokenMap = {};
      var length = 0;
      var maxLength = 0;

      var addIndexGetter = function addIndexGetter(i) {
        defineGetter(that, i, function () {
          preop();
          return tokens[i];
        }, false);
      };

      var reindex = function reindex() {
        /** Define getter functions for array-like access to the tokenList's contents. */
        if (length >= maxLength) for (; maxLength < length; ++maxLength) {
          addIndexGetter(maxLength);
        }
      };
      /** Helper function called at the start of each class method. Internal use only. */


      var preop = function preop() {
        var error;
        var i;
        var args = arguments;
        var rSpace = /\s+/;
        /** Validate the token/s passed to an instance method, if any. */

        if (args.length) for (i = 0; i < args.length; ++i) {
          if (rSpace.test(args[i])) {
            error = new SyntaxError('String "' + args[i] + '" ' + "contains" + ' an invalid character');
            error.code = 5;
            error.name = "InvalidCharacterError";
            throw error;
          }
        }
        /** Split the new value apart by whitespace*/

        if (_typeof(el[prop]) === "object") {
          tokens = ("" + el[prop].baseVal).replace(/^\s+|\s+$/g, "").split(rSpace);
        } else {
          tokens = ("" + el[prop]).replace(/^\s+|\s+$/g, "").split(rSpace);
        }
        /** Avoid treating blank strings as single-item token lists */


        if ("" === tokens[0]) tokens = [];
        /** Repopulate the internal token lists */

        tokenMap = {};

        for (i = 0; i < tokens.length; ++i) {
          tokenMap[tokens[i]] = true;
        }

        length = tokens.length;
        reindex();
      };
      /** Populate our internal token list if the targeted attribute of the subject element isn't empty. */


      preop();
      /** Return the number of tokens in the underlying string. Read-only. */

      defineGetter(that, "length", function () {
        preop();
        return length;
      });
      /** Override the default toString/toLocaleString methods to return a space-delimited list of tokens when typecast. */

      that.toLocaleString = that.toString = function () {
        preop();
        return tokens.join(" ");
      };

      that.item = function (idx) {
        preop();
        return tokens[idx];
      };

      that.contains = function (token) {
        preop();
        return !!tokenMap[token];
      };

      that.add = function () {
        preop.apply(that, args = arguments);

        for (var args, token, i = 0, l = args.length; i < l; ++i) {
          token = args[i];

          if (!tokenMap[token]) {
            tokens.push(token);
            tokenMap[token] = true;
          }
        }
        /** Update the targeted attribute of the attached element if the token list's changed. */


        if (length !== tokens.length) {
          length = tokens.length >>> 0;

          if (_typeof(el[prop]) === "object") {
            el[prop].baseVal = tokens.join(" ");
          } else {
            el[prop] = tokens.join(" ");
          }

          reindex();
        }
      };

      that.remove = function () {
        preop.apply(that, args = arguments);
        /** Build a hash of token names to compare against when recollecting our token list. */

        for (var args, ignore = {}, i = 0, t = []; i < args.length; ++i) {
          ignore[args[i]] = true;
          delete tokenMap[args[i]];
        }
        /** Run through our tokens list and reassign only those that aren't defined in the hash declared above. */


        for (i = 0; i < tokens.length; ++i) {
          if (!ignore[tokens[i]]) t.push(tokens[i]);
        }

        tokens = t;
        length = t.length >>> 0;
        /** Update the targeted attribute of the attached element. */

        if (_typeof(el[prop]) === "object") {
          el[prop].baseVal = tokens.join(" ");
        } else {
          el[prop] = tokens.join(" ");
        }

        reindex();
      };

      that.toggle = function (token, force) {
        preop.apply(that, [token]);
        /** Token state's being forced. */

        if (undefined !== force) {
          if (force) {
            that.add(token);
            return true;
          } else {
            that.remove(token);
            return false;
          }
        }
        /** Token already exists in tokenList. Remove it, and return FALSE. */


        if (tokenMap[token]) {
          that.remove(token);
          return false;
        }
        /** Otherwise, add the token and return TRUE. */


        that.add(token);
        return true;
      };

      return that;
    };

    return _DOMTokenList;
  }();

  if (!("DOMTokenList" in this && function (s) {
    return !("classList" in s) || !s.classList.toggle("x", !1) && !s.className;
  }(document.createElement("x")))) {
    // DOMTokenList
    (function (global) {
      var nativeImpl = "DOMTokenList" in global && global.DOMTokenList;

      if (!nativeImpl || !!document.createElementNS && !!document.createElementNS('http://www.w3.org/2000/svg', 'svg') && !(document.createElementNS("http://www.w3.org/2000/svg", "svg").classList instanceof DOMTokenList)) {
        global.DOMTokenList = _DOMTokenList;
      } // Add second argument to native DOMTokenList.toggle() if necessary


      (function () {
        var e = document.createElement('span');
        if (!('classList' in e)) return;
        e.classList.toggle('x', false);
        if (!e.classList.contains('x')) return;

        e.classList.constructor.prototype.toggle = function toggle(token
        /*, force*/
        ) {
          var force = arguments[1];

          if (force === undefined) {
            var add = !this.contains(token);
            this[add ? 'add' : 'remove'](token);
            return add;
          }

          force = !!force;
          this[force ? 'add' : 'remove'](token);
          return force;
        };
      })(); // Add multiple arguments to native DOMTokenList.add() if necessary


      (function () {
        var e = document.createElement('span');
        if (!('classList' in e)) return;
        e.classList.add('a', 'b');
        if (e.classList.contains('b')) return;
        var native = e.classList.constructor.prototype.add;

        e.classList.constructor.prototype.add = function () {
          var args = arguments;
          var l = arguments.length;

          for (var i = 0; i < l; i++) {
            native.call(this, args[i]);
          }
        };
      })(); // Add multiple arguments to native DOMTokenList.remove() if necessary


      (function () {
        var e = document.createElement('span');
        if (!('classList' in e)) return;
        e.classList.add('a');
        e.classList.add('b');
        e.classList.remove('a', 'b');
        if (!e.classList.contains('b')) return;
        var native = e.classList.constructor.prototype.remove;

        e.classList.constructor.prototype.remove = function () {
          var args = arguments;
          var l = arguments.length;

          for (var i = 0; i < l; i++) {
            native.call(this, args[i]);
          }
        };
      })();
    })(this);
  }

  if (!("document" in this && "classList" in document.documentElement && "Element" in this && "classList" in Element.prototype && function () {
    var t = document.createElement("span");
    return t.classList.add("a", "b"), t.classList.contains("b");
  }())) {
    // Element.prototype.classList

    /*
    Copyright (c) 2016, John Gardner
    
    Permission to use, copy, modify, and/or distribute this software for any purpose with or without fee is hereby granted, provided that the above copyright notice and this permission notice appear in all copies.
    
    THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
    */
    (function (global) {
      var dpSupport = true;

      var defineGetter = function defineGetter(object, name, fn, configurable) {
        if (Object.defineProperty) Object.defineProperty(object, name, {
          configurable: false === dpSupport ? true : !!configurable,
          get: fn
        });else object.__defineGetter__(name, fn);
      };
      /** Ensure the browser allows Object.defineProperty to be used on native JavaScript objects. */


      try {
        defineGetter({}, "support");
      } catch (e) {
        dpSupport = false;
      }
      /** Polyfills a property with a DOMTokenList */


      var addProp = function addProp(o, name, attr) {
        defineGetter(o.prototype, name, function () {
          var tokenList;
          var THIS = this,

          /** Prevent this from firing twice for some reason. What the hell, IE. */
          gibberishProperty = "__defineGetter__" + "DEFINE_PROPERTY" + name;
          if (THIS[gibberishProperty]) return tokenList;
          THIS[gibberishProperty] = true;
          /**
           * IE8 can't define properties on native JavaScript objects, so we'll use a dumb hack instead.
           *
           * What this is doing is creating a dummy element ("reflection") inside a detached phantom node ("mirror")
           * that serves as the target of Object.defineProperty instead. While we could simply use the subject HTML
           * element instead, this would conflict with element types which use indexed properties (such as forms and
           * select lists).
           */

          if (false === dpSupport) {
            var visage;
            var mirror = addProp.mirror || document.createElement("div");
            var reflections = mirror.childNodes;
            var l = reflections.length;

            for (var i = 0; i < l; ++i) {
              if (reflections[i]._R === THIS) {
                visage = reflections[i];
                break;
              }
            }
            /** Couldn't find an element's reflection inside the mirror. Materialise one. */


            visage || (visage = mirror.appendChild(document.createElement("div")));
            tokenList = DOMTokenList.call(visage, THIS, attr);
          } else tokenList = new DOMTokenList(THIS, attr);

          defineGetter(THIS, name, function () {
            return tokenList;
          });
          delete THIS[gibberishProperty];
          return tokenList;
        }, true);
      };

      addProp(global.Element, "classList", "className");
      addProp(global.HTMLElement, "classList", "className");
      addProp(global.HTMLLinkElement, "relList", "rel");
      addProp(global.HTMLAnchorElement, "relList", "rel");
      addProp(global.HTMLAreaElement, "relList", "rel");
    })(this);
  }
}).call('object' === (typeof window === "undefined" ? "undefined" : _typeof(window)) && window || 'object' === (typeof self === "undefined" ? "undefined" : _typeof(self)) && self || 'object' === (typeof global === "undefined" ? "undefined" : _typeof(global)) && global || {});
/* WEBPACK VAR INJECTION */}.call(this, __webpack_require__(3)))

/***/ })

}]);
//# sourceMappingURL=polyfill_io.js.map
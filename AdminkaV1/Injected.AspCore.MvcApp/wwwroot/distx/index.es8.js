import TestClass from './TestClass.es8';
import './legacy';
import 'bootstrap';
import 'datatables.net-bs';
import './site.scss';
import './WorkflowManager.ts';
console.log("11");
var arr = [1, 2, 3];
var iAmJavascriptES6 = function () { return console.log.apply(console, arr); };
window.iAmJavascriptES6 = iAmJavascriptES6;
window.iAmJavascriptES6();
console.log("222");
var testClass = new TestClass();
testClass.sayHelloWorld();
//# sourceMappingURL=index.es8.js.map
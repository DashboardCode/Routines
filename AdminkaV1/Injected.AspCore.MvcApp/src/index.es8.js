import TestClass from './TestClass.es8';
import './legacy';
import 'bootstrap';
import 'datatables.net-bs';
import './site.scss';
import './WorkflowManager.ts';

console.log(`11`);

const arr = [1, 2, 3];
const iAmJavascriptES6 = () => console.log(...arr);
window.iAmJavascriptES6 = iAmJavascriptES6;
window.iAmJavascriptES6();

console.log(`222`);

let testClass = new TestClass();
testClass.sayHelloWorld();
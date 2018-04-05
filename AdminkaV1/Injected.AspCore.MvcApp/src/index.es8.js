import TestClass from "./TestClass.es8";
import "./lib/jquery/jquery"
import "./lib/bootstrap/js/bootstrap";

//function GetNull() {
//    return null;
//}

console.log(`11`);

const arr = [1, 2, 3];
const iAmJavascriptES6 = () => console.log(...arr);
window.iAmJavascriptES6 = iAmJavascriptES6;
window.iAmJavascriptES6();
//require('./lib');

console.log(`222`);

let testClass = new TestClass();
testClass.sayHelloWorld();

//document.getElementById("fillthis").innerHTML = "aaaaaaaaaa";

//// Write your Javascript code.

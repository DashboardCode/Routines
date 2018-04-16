import '@babel/polyfill';
import 'bootstrap';
import 'datatables.net-bs';
//import $ from 'jquery';

import Es8Test from './Es8Test.es8';
import /*ShowExceptionModal from */'./global';
import './site.scss';
import './WorkflowManager.ts';

//console.log('zzz');
//window.ShowExceptionModal = ShowExceptionModal;

let es8test = new Es8Test();
es8test.run();
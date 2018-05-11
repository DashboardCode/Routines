import '@babel/polyfill';
import 'bootstrap';
import 'datatables.net-bs4';
import './multiselect/DashboardCode.BsMultiSelect.es8.js';

import Popper from 'popper.js';
//import $ from 'jquery';

import Es8Test from './Es8Test.es8';
import /*ShowExceptionModal from */'./global';
import './site.scss';
import './WorkflowManager.ts';

Popper.Defaults.modifiers.computeStyle.gpuAcceleration = !(window.devicePixelRatio < 1.5 && /Win/.test(navigator.platform));

let es8test = new Es8Test();
es8test.run();

import 'jquery'; // should be manually loaded first in order since '@dashboardcode/bsmultiselect' do not require it as dependency and can be loaded first // TODO create @dashboardcode/bsmultiselect.jQuery
import '@popperjs/core'; // do not use aliases for jquery and '@popperjs/core' since exact names are required by bootstrap and bsmultiselect
import 'bootstrap#umd'; // requires jquery and popper
import '@dashboardcode/bsmultiselect#umd'; // requires popper

//import 'moment'; // loaded by expose-loader

import 'datatables.net-bs5'; // this will bring dataTabales.js
import 'datatables.net-select-bs5';
import 'datatables.net-buttons#buttons.colVis';
import 'datatables.net-buttons-bs5';

import 'daterangepicker';
import 'jquery-validation';
import 'jquery-validation-unobtrusive';

import 'jstree';

import './index.scss';
import './site';

console.log({ msg: 'webpack entry end', window: window, global: global, g_jQuery: global.jQuery, w_jQuery: window.jQuery, w_$: window.$, moment: window.moment, bootstrap: window.bootstrap, popper: window.createPopper,  })
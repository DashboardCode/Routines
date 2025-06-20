/*import { StrictMode } from 'react'*/

import React from 'react';
import whyDidYouRender from '@welldone-software/why-did-you-render';

import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.jsx'


//const whyDidYouRender = require('@welldone-software/why-did-you-render');
//whyDidYouRender(React, {
//    trackAllPureComponents: true,
//});

createRoot(document.getElementById('root')).render(
  /*<StrictMode>*/
    <App  />
  /*</StrictMode>*/,
)

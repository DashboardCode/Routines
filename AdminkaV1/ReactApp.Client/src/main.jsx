/*import { StrictMode } from 'react'*/
import './tools/wdyr'; 

import { createRoot } from 'react-dom/client'
import './index.css'

import App from './App.jsx'

createRoot(document.getElementById('root')).render(
  /*<StrictMode>*/
    <App  />
  /*</StrictMode>*/,
)

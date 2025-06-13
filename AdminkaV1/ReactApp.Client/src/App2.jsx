import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import Dashboard from './pages/Dashboard';
import DebugMenu from './pages/DebugMenu';
import RemeltDataManagement from './pages/RemeltDataManagement';
import RemeltListManagement from './pages/RemeltListManagement';
import Settings from './pages/Settings';

import { BsRibbon, BsRibbonGroup, BsRibbonGroupItem, BsRibbonButton} from './ribbon/BsRibbon';

import 'bootstrap/dist/css/bootstrap.min.css';
import './ribbon/BsRibbon.css';
import './App.css';

const App = () => {

    const rerender = React.useReducer(() => ({}), {})[1]

    return (
        <Router>
            <div>
                <DebugMenu actions=
                    {[{ name: "rerender", action: () => rerender() }]} />
                <BsRibbon breakpoint="lg" height="6rem">
                    <BsRibbonGroup title="">
                        <BsRibbonGroupItem >
                            <BsRibbonButton>
                                <div><Link to="/"  className="btn btn-primary" href="#" role="button">Home</Link></div>
                            </BsRibbonButton>
                            <BsRibbonButton>
                                <div><Link to="/list"  className="btn btn-primary" href="#" role="button">Remelt List</Link></div>
                            </BsRibbonButton>
                            <BsRibbonButton>
                                <div><Link to="/chart" className="btn btn-primary" href="#" role="button">Chart</Link></div>
                            </BsRibbonButton>
                        </BsRibbonGroupItem>
                    </BsRibbonGroup>

                    <BsRibbonGroup title="" className="ms-auto">
                        <BsRibbonGroupItem  >
                            <BsRibbonButton>
                                <div><Link to="/settings"  className="btn btn-primary" href="#" role="button">Settings</Link></div>
                            </BsRibbonButton>
                        </BsRibbonGroupItem>
                    </BsRibbonGroup>

                </BsRibbon>
            </div>
            <main>
                <Routes>
                    <Route path="/" element={<Dashboard />} />
                    <Route path="/list" element={<RemeltListManagement />} />
                    <Route path="/chart" element={<RemeltDataManagement />} />
                    <Route path="/settings/*" element={<Settings />} />
                </Routes>
            </main>
        </Router>
    );
};

export default App;
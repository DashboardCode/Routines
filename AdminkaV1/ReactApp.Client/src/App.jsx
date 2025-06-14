import 'material-symbols/outlined.css';// Or 'rounded.css' or 'sharp.css'
import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link, useNavigate, useLocation } from 'react-router-dom';
import Dashboard from './pages/Dashboard';
import DebugMenu from './tools/DebugMenu';
import TablesManagement from './pages/TablesManagement';
import ConnectionsManagement from './pages/ConnectionsManagement';
import Settings from './pages/Settings';
import PropTypes from "prop-types";

import { BsRibbon, BsRibbonGroup, BsRibbonGroupItem, BsRibbonButton} from './ribbon/BsRibbon';

import 'bootstrap/dist/css/bootstrap.min.css';
import './ribbon/BsRibbon.css';
import './App.css';

import LoginDialog from './LoginDialog';
import setupJwtTokenMsal from './setupJwtTokenMsal';

import { isTokenExpired } from '@/fetchTokenized';

// this is an react hook-based app
const App = () => {
    // runs the function only once when the component mounts
    //const [isLoggedIn, setIsLoggedIn] = React.useState(false);
    const [isExpiredToken, setIsExpiredToken] = React.useState(false); // loading state
    const [isLoggedIn, setIsLoggedIn] = React.useState(() => {
        var token = localStorage.getItem("access_token");
        if (token != null) {
            if (isTokenExpired(token)) {
                localStorage.removeItem("access_token");
                setIsExpiredToken(true);
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    });
    const [isLoading, setIsLoading] = React.useState(true); // loading state

    const [isDialogOpen, setIsDialogOpen] = React.useState(false); // login status

    //React.useEffect(() => {
    //    login(setIsLoggedIn, setIsLoading);
    //}, []); // means run once on mount (empty dependicies)

    const rerender = React.useReducer(() => ({}), {})[1]
    
    return (
        <Router>
            <div>
                <DebugMenu actions=
                    {[{ name: "rerender", action: () => rerender() }]} />
                <BsRibbon breakpoint="lg" height="6rem">
                    <BsRibbonGroup title="" disabled={isLoading}>
                        <BsRibbonGroupItem className="d-inline-flex align-items-center gap-1">
                            <NavBar isLoggedIn={isLoggedIn} />
                            {/*<BsRibbonButton disabled={!isLoggedIn}>*/}
                            {/*    <Link to="/" className={`btn d-inline-flex align-items-center gap-1 ${location.pathname === '/' ? "btn-warning fs-1 fw-bold" : "btn-primary"}`} href="#" role="button">*/}{/*<img className="icon-centered" src="home.svg" />*/}{/*DshbX</Link>*/}
                            {/*</BsRibbonButton>*/}
                            {/*<BsRibbonButton disabled={!isLoggedIn}>*/}
                            {/*    <Link to="/connections" className={`btn d-inline-flex align-items-center gap-1 ${location.pathname === '/connections' ? "btn-warning fs-1 fw-bold" : "btn-primary"}`} href="#" role="button"><img className="icon-centered" src="table.svg" />Connections</Link>*/}
                            {/*</BsRibbonButton>*/}
                            {/*<BsRibbonButton disabled={!isLoggedIn}>*/}
                            {/*    <Link to="/tables" className="btn btn-primary d-inline-flex align-items-center gap-1" href="#" role="button"><img className="icon-centered" src="table.svg" />Tables</Link>*/}
                            {/*</BsRibbonButton>*/}
                            {/*<BsRibbonButton disabled={!isLoggedIn}>*/}
                            {/*    <Link to="/settings" className="btn btn-primary d-inline-flex align-items-center gap-1" href="#" role="button"><img className="icon-centered" src="settings.svg" />Settings</Link>*/}
                            {/*</BsRibbonButton>*/}
                        </BsRibbonGroupItem>
                    </BsRibbonGroup>

                    <BsRibbonGroup title="" className="ms-auto">
                        {!isLoggedIn ?
                            <BsRibbonGroupItem  >
                                <BsRibbonButton className="btn btn-warning mx-2" onClick={() => { setIsExpiredToken(false); loginMSAL(setIsLoggedIn, setIsLoading) }}>
                                    <img className="icon-centered" src="login.svg" />Log in (MSAL)
                                </BsRibbonButton>
                                <BsRibbonButton className="btn btn-warning mx-2" onClick={() => { setIsExpiredToken(false); setIsDialogOpen(true) } }>
                                    <img className="icon-centered" src="login.svg" />Log in (Middleware)
                                </BsRibbonButton>
                            </BsRibbonGroupItem>
                            :
                            <BsRibbonGroupItem  >
                                <LogoutButton setIsLoggedIn={setIsLoggedIn} />
                            </BsRibbonGroupItem>
                        }
                    </BsRibbonGroup>
                </BsRibbon>
                <LoginDialog
                    isDialogOpen={isDialogOpen}
                    setIsDialogOpen={setIsDialogOpen}
                    setIsLoggedIn={setIsLoggedIn}
                    setIsLoading={setIsLoading} />
            </div>
            <main>
                {isExpiredToken && <div className="alert alert-warning my-4" role="alert" >Session expired!</div>}
                <Routes>
                    <Route path="/" element={<Dashboard />} />
                    <Route path="/connections" element={<ConnectionsManagement />} />
                    <Route path="/tables" element={<TablesManagement />} />
                    <Route path="/settings/*" element={<Settings />} />
                </Routes>
            </main>
        </Router>
        
    );
};

const NavBar = ({ isLoggedIn }) => {
    const location = useLocation();

    const navButtons = [
        {
            path: "/", label: (<div><span style={{ fontSize: '150%', verticalAlign: 'middle', marginRight: '4px', marginBottom: '4px' }} className="material-symbols-outlined">home</span>DshbX</div>)
        },
        {
            path: "/connections", label: (<div><span style={{ fontSize: '150%', verticalAlign: 'middle', marginRight: '4px', marginBottom: '4px' }} className="material-symbols-outlined">table_view</span>Connections</div>)
        },
        {
            path: "/tables", label: (<div><span style={{ fontSize: '150%', verticalAlign: 'middle', marginRight: '4px', marginBottom: '4px' }} className="material-symbols-outlined">table_view</span>Tables</div>)
        },
        {
            path: "/settings", label: (<div><span style={{ fontSize: '150%', verticalAlign: 'middle', marginRight: '4px', marginBottom: '4px' }} className="material-symbols-outlined">settings</span>Settings</div>)
        }
    ];

    return (
        <div className="btn-group">
            {navButtons.map(({ path, label }) => {
                var isActive = false
                if (path == "/settings")
                    isActive = location.pathname.startsWith(path);
                else  
                    isActive = location.pathname==path;
                return (
                    <BsRibbonButton key={path} disabled={!isLoggedIn}>
                        <Link to={!isLoggedIn ? "#" : path} disabled={!isLoggedIn} className={`btn d-inline-flex align-items-center gap-1 ${!isLoggedIn ? "disabled" : ""} ${isActive ? "btn-warning fs-1 fw-bold" : "btn-primary"}`} href="#" role="button">{label}</Link>
                    </BsRibbonButton>
                );
            })}
        </div>
    );
};

NavBar.propTypes = {
    isLoggedIn: PropTypes.bool
};


// LogoutButton can't be inlinded, otherwise I get an error: useNavigate() may be used only in the context of a <Router> component.
function LogoutButton({ setIsLoggedIn }) {
    const navigate = useNavigate();

    const handleLogout = () => {
        setIsLoggedIn(false)
        localStorage.removeItem("access_token");
        navigate("/", { replace: true });
    };

    return <BsRibbonButton className="btn btn-dark mx-2" onClick={handleLogout}>
        <span style={{ fontSize: '150%', verticalAlign: 'middle', marginRight: '4px', marginBottom: '4px' }} className="material-symbols-outlined">logout</span>Logout
    </BsRibbonButton>;
}

LogoutButton.propTypes = {
    setIsLoggedIn: PropTypes.func
};

const loginMSAL = async (setIsLoggedIn, setIsLoading) => {
    try {
        await setupJwtTokenMsal(); // your async function
        setIsLoggedIn(true);   // or use actual check from response
    } catch (err) {
        console.error("JWT setup failed:", err);
        setIsLoggedIn(false);
    } finally {
        setIsLoading(false);   // re-enable buttons
    }
};

export default App;
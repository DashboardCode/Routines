import React, { useRef, useEffect } from 'react';
import PropTypes from "prop-types";

import 'bootstrap/dist/css/bootstrap.min.css';
import './ribbon/BsRibbon.css';
import './App.css';

import setupJwtTokenMiddleware from '@setupJwtTokenMiddleware';
import Dialog from "./tools/Dialog";
function LoginDialog(
    { isDialogOpen, setIsDialogOpen,
      setIsLoggedIn, setIsLoading
    }) {
    const inputRef = useRef(null);

    useEffect(() => {
        if (isDialogOpen && inputRef.current) {
            // delay focus until DOM and dialog transition are complete
            const timeout = setTimeout(() => {
                inputRef.current.focus();
            }, 0);
            return () => clearTimeout(timeout);
            //inputRef.current.focus();
        }
    }, [isDialogOpen]);


    const [password, setPassword] = React.useState('');
    const [login, setLogin] = React.useState('Anonymous');
    const [errorMessage, setErrorMessage] = React.useState('');

    const loginButton_onClick = async () => {
        try {
            setIsLoading(true);
            var result = await setupJwtTokenMiddleware(password); // your async function
            if (result == true) {
                setIsLoggedIn(true);   // or use actual check from response
                setIsDialogOpen(false);
                setErrorMessage(null);
            } else {
                setErrorMessage("Password not match!");
            }
        } catch (err) {
            var messageLink = "";// logDetails({ err });
            setIsLoggedIn(false);
            setErrorMessage((<div>{err.message}{messageLink}</div>));
        } finally {
            setIsLoading(false);   // re-enable buttons
        }
    };

    //var formAutoComplete = "off" and autoComplete="new-password" doesn't disable "save your password" in browser (at least in EDGE)
    return (
        <Dialog
            setIsDialogOpen={setIsDialogOpen}
            errorMessage={errorMessage} setErrorMessage={setErrorMessage} title="Login (Middleware)"
            okButtonTitle="Login" okButton_onClick={loginButton_onClick}
            //initialFocusRef={inputRef}
        >
            <form>
            <div className="px-2 py-2">
                    <label className="form-label">Password</label>
                    <input type="text" name="username" autoComplete="username" value={login} onChange={e => setLogin(e.target.value)} hidden />
                    <input
                        ref={inputRef}
                        autoComplete="current-password"
                    type="password"
                    className="form-control"
                    value={password}
                    onChange={e => setPassword(e.target.value)}
                />
                </div>
            </form>
        </Dialog>
    );
}

LoginDialog.propTypes = {
    isDialogOpen: PropTypes.bool,
    setIsDialogOpen: PropTypes.func,
    setIsLoggedIn: PropTypes.func,
    setIsLoading: PropTypes.func,
    setDialogMessage: PropTypes.func
};

export default LoginDialog;
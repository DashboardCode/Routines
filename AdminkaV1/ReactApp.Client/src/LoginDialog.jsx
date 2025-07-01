import React, { useRef, useEffect, useActionState, startTransition } from 'react';
import PropTypes from "prop-types";

import 'bootstrap/dist/css/bootstrap.min.css';
import './ribbon/BsRibbon.css';
import './App.css';

import setupJwtTokenMiddleware from '@setupJwtTokenMiddleware';
import Dialog from "./tools/Dialog";
function LoginDialog(
    { isDialogOpen, setIsDialogOpen, setIsLoggedIn
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
    //const [errorMessage, setErrorMessage] = React.useState('');

    const [errorMessage, loginButton_onClick, isPending] = useActionState(
        async () => {
            try {
                var result = await setupJwtTokenMiddleware(password); // your async function
                if (result == true) {
                    setIsLoggedIn(true);   // or use actual check from response
                    setIsDialogOpen(false);
                    return null; // handle success
                } else {
                    return "Password not match!"
                }
            } catch (err) {
                var messageLink = "";// logDetails({ err });
                setIsLoggedIn(false);
                return (<div>{err.message}{messageLink}</div>);
            }
        },
        null,
    );

    function okButton_onClick() {
        startTransition(() => {  // updating UI after data fetch is not urgent, prioritize user input (e.g. press close button)
            loginButton_onClick(); 
        });
    }
    return (
        <Dialog
            setIsDialogOpen={setIsDialogOpen}
            errorMessage={errorMessage} title="Login (Middleware)"
            okButtonTitle="Login" okButton_onClick={okButton_onClick}
            isLoading={isPending}
        >
            <form>
            <div className="px-2 py-2">
                    <label className="form-label">Password</label>
                    <input type="text" name="username" autoComplete="username" value={login} onChange={e => setLogin(e.target.value)} hidden />
                    <input
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
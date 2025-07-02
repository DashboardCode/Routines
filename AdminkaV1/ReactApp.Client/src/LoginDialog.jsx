import React, { /*useRef, useEffect,*/ useActionState, startTransition, useCallback } from 'react';
import PropTypes from "prop-types";

import 'bootstrap/dist/css/bootstrap.min.css';
import './ribbon/BsRibbon.css';
import './App.css';

import setupJwtTokenMiddleware from '@setupJwtTokenMiddleware';
import Dialog from "./tools/Dialog";
import { parseErrorException } from './parseErrorResponse';
function LoginDialog({ setIsDialogOpen , setIsLoggedIn }) {

    const [password, setPassword] = React.useState('');
    const [login, setLogin] = React.useState('Anonymous');

    const [errorMessage, loginButton_onClick, isPending] = useActionState(
        async () => {
            try {
                var result = await setupJwtTokenMiddleware(password); // your async function
                if (result.success == true) {
                    localStorage.setItem("access_token", result.data.token);
                    setIsLoggedIn(true);   // or use actual check from response
                    setIsDialogOpen(false);
                    return null; // handle success
                } else {
                    setIsLoggedIn(false);
                    var message = "";
                    if (result.errorContent.status == 401) {
                        message = "Password not match!"
                    }
                    else {
                        console.error(result.errorContent);
                        message = `Failed to authenticate: ${result.message}`
                    }
                    return message; 
                }
            } catch (err) {
                setIsLoggedIn(false);
                console.error(err);
                return (<div>{parseErrorException(err)}</div>);
            }
        },
        null,
    );

    const okButton_onClick = useCallback(() => {
        startTransition(() => { // updating UI after data fetch is not urgent, prioritize user input (e.g. press close button)
            loginButton_onClick();
        });
    }, [loginButton_onClick]); 

    return (
        <Dialog
            setIsDialogOpen={setIsDialogOpen}
            errorMessage={errorMessage}
            title="Login (Middleware)"
            okButtonTitle="Login"
            okButton_onClick={okButton_onClick}
            isPending={isPending}
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
    setIsDialogOpen: PropTypes.func,
    setIsLoggedIn: PropTypes.func
};

export default LoginDialog;
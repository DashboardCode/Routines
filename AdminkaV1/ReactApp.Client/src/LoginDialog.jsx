import React from 'react';
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

    const [password, setPassword] = React.useState('');
    const [errorMessage, setErrorMessage] = React.useState(null);

    //const clear = React.useCallback(() => {
    //    setErrorMessage(null);
    //}, []);

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

    return (
        <Dialog
            isDialogOpen={isDialogOpen} setIsDialogOpen={setIsDialogOpen}
            errorMessage={errorMessage} title="Login (Middleware)"
            okButtonTitle="Login" okButton_onClick={loginButton_onClick}
        >
            <div className="px-2 py-2">
                <label className="form-label">Password</label>
                <input
                    type="password"
                    className="form-control"
                    value={password}
                    onChange={e => setPassword(e.target.value)}
                />
            </div>
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
import { useState, useEffect } from 'react';

import * as RadixDialog from "@radix-ui/react-dialog";
import PropTypes from "prop-types";
function Dialog({ isDialogOpen, setIsDialogOpen, title, okButtonTitle, okButton_onClick, setDialogApi, children }) {
    const [errorMessage, setErrorMessage] = useState("");
    const [isLoading, setIsLoading] = useState("");
    const isError = !!errorMessage;    

    useEffect(() => {
        setDialogApi({
            setErrorMessage
        });
    }, [setDialogApi, setErrorMessage]);

    return (
            <RadixDialog.Root open={isDialogOpen} onOpenChange={setIsDialogOpen}>
                <RadixDialog.Portal>
                    <RadixDialog.Overlay className="modal-backdrop fade show" style={{ position: 'fixed', inset: 0, backgroundColor: 'rgba(0,0,0,0.5)' }} />
                    <RadixDialog.Content aria-describedby={undefined} className="modal d-block" style={{ position: 'fixed', top: '50%', left: '50%', transform: 'translate(-50%, -50%)' }}>
                    <div className="modal-dialog">
                        <div className="modal-content">
                            <div className="modal-header">
                                    <RadixDialog.Title asChild>
                                    <h5 className="modal-title">{title}</h5>
                                </RadixDialog.Title>
                                <button type="button" className="btn-close" onClick={() => setIsDialogOpen(false)}></button>
                            </div>
                            <div className="modal-body">
                                {children}
                                <div className={`alert alert-danger px-2 py-2 mx-2 ${isError == true ? '' : 'd-none'}`}>{errorMessage}</div>
                            </div>
                            <div className="modal-footer">
                                <button type="button" className="btn btn-sm btn-secondary" onClick={() => setIsDialogOpen(false)}>
                                    Cancel
                                </button>
                                <button type="button" className="btn btn-sm btn-danger" disabled={isLoading} onClick={() => okButton_onClick(setErrorMessage, setIsLoading)}>
                                    {okButtonTitle}
                                </button>
                            </div>
                        </div>
                    </div>
                </RadixDialog.Content>
            </RadixDialog.Portal>
        </RadixDialog.Root>
    );
}

Dialog.propTypes = {
    isDialogOpen: PropTypes.bool,
    setIsDialogOpen: PropTypes.func,
    title: PropTypes.oneOfType([PropTypes.string, PropTypes.node]),
    okButtonTitle: PropTypes.node,
    okButton_onClick: PropTypes.func,
    setDialogApi: PropTypes.func,
    children: PropTypes.element
};

export default Dialog;
import React, { useState, useRef } from 'react';

import * as RadixDialog from "@radix-ui/react-dialog";
import PropTypes from "prop-types";

const Dialog = React.memo(({ isDialogOpen, setIsDialogOpen, title, okButtonTitle, okButton_onClick, errorMessage, children, initialFocusRef }) => {
    const [isLoading, setIsLoading] = useState("");
    const isError = !!errorMessage;    
    const dialogRef = useRef(null);

    // trying to avoid warning in browser console "focusing input when the parent is hidden"
    // radix dialog put a focus on first element and here this is a X button, onOpenAutoFocus doens't help so only setTimeout work
    // to avoid still of first users "tab" there is prevention: we put focus only if there are no focus
    const onOpenAutoFocus = (e) => {
         e.preventDefault();
         setTimeout(() => {
             var input = initialFocusRef?.current ? initialFocusRef.current : dialogRef.current?.querySelector('input:not([hidden]):not([disabled])')
             console.log(input)
             if (input) {
                 const activeEl = document.activeElement;
                 if (!dialogRef.current?.contains(activeEl)) { // no focus in the form
                     input.focus();
                 }
             }
         }, 0);
    }
    console.log("Dialog render")
    return (
            <RadixDialog.Root open={isDialogOpen} onOpenChange={setIsDialogOpen}>
                <RadixDialog.Portal>
                    <RadixDialog.Overlay className="modal-backdrop fade show" style={{ position: 'fixed', inset: 0, backgroundColor: 'rgba(0,0,0,0.5)' }} />
                <RadixDialog.Content aria-describedby={undefined} className="modal d-block" style={{ position: 'fixed', top: '50%', left: '50%', transform: 'translate(-50%, -50%)' }}
                    onOpenAutoFocus={onOpenAutoFocus}
                >
                    <div className="modal-dialog">
                        <div className="modal-content">
                            <div className="modal-header">
                                    <RadixDialog.Title asChild>
                                    <h5 className="modal-title">{title}</h5>
                                </RadixDialog.Title>
                                <button type="button" className="btn-close" onClick={() => setIsDialogOpen(false)}></button>
                            </div>
                            <div className="modal-body" ref={dialogRef}>
                                {children}
                                <div className={`alert alert-danger px-2 py-2 mx-2 ${isError == true ? '' : 'd-none'}`}>{errorMessage}</div>
                            </div>
                            <div className="modal-footer">
                                <button type="button" className="btn btn-sm btn-secondary" onClick={() => setIsDialogOpen(false)}>
                                    Cancel
                                </button>
                                <button type="button" className="btn btn-sm btn-danger" disabled={isLoading} onClick={() => okButton_onClick(setIsLoading)}>
                                    {okButtonTitle}
                                </button>
                            </div>
                        </div>
                    </div>
                </RadixDialog.Content>
            </RadixDialog.Portal>
        </RadixDialog.Root>
    );
})

Dialog.displayName = "Dialog";

Dialog.propTypes = {
    isDialogOpen: PropTypes.bool,
    setIsDialogOpen: PropTypes.func,
    title: PropTypes.oneOfType([PropTypes.string, PropTypes.node]),
    okButtonTitle: PropTypes.node,
    okButton_onClick: PropTypes.func,
    setDialogApi: PropTypes.func,
    children: PropTypes.node,
    errorMessage: PropTypes.node,
    initialFocusRef: PropTypes.shape({
        current: PropTypes.instanceOf(Element)
    })
};

export default Dialog;
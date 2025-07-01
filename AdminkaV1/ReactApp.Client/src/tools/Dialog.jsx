import React from 'react';

import PropTypes from "prop-types";

const Dialog = React.memo(({ setIsDialogOpen, title, okButtonTitle, okButton_onClick, errorMessage, isPending, children }) => {
    //const [isLoading, setIsLoading] = useState("");
    const isError = !!errorMessage;    

    // WARN: simple is the best. attempt to using Radix dialog lead to annoying warning in browser console "focusing input when the parent is hidden"
    console.log("Dialog render")
    return (
        <div>
            <div className="modal-backdrop show"  />
            <div className="modal show" tabIndex="-1" style={{ display: "block"}} role="dialog" aria-modal="true">
                <div className="modal-dialog" >
                    <div className="modal-content"  >
                                <div className="modal-header">
                                    <h5 className="modal-title">{title}</h5>
                                    <button type="button" className="btn-close" onClick={() => setIsDialogOpen(false)}></button>
                                </div>
                                <div className="modal-body">
                                    {children}
                            <div style={{ overflowWrap: "break-word" }} className={`alert alert-danger px-2 py-2 mx-2 ${isError == true ? '' : 'd-none'}`}>{errorMessage}</div>
                                </div>
                                <div className="modal-footer">
                                    <button type="button" className="btn btn-sm btn-secondary" onClick={() => setIsDialogOpen(false)}>
                                        Cancel
                                    </button>
                            <button type="button" className="btn btn-sm btn-danger" disabled={isPending} onClick={() => okButton_onClick(/*setIsLoading*/)}>
                                        {okButtonTitle}
                                    </button>
                                </div>
                            </div>
                </div>
            </div>
        </div>
    );
})

Dialog.displayName = "Dialog";

Dialog.propTypes = {
    setIsDialogOpen: PropTypes.func,
    title: PropTypes.oneOfType([PropTypes.string, PropTypes.node]),
    okButtonTitle: PropTypes.node,
    okButton_onClick: PropTypes.func,
    errorMessage: PropTypes.node,
    isPending: PropTypes.bool,
    children: PropTypes.node
};

export default Dialog;
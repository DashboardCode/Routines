import React, { useMemo } from 'react';

import PropTypes from "prop-types";
import './CrudTable.css';
import Dialog from "./Dialog";

const EditDialog = React.memo((
    { isForNew, isDialogOpen, setIsDialogOpen, okButton_onClick, errorMessage, setErrorMessage, children }) => {
    const title = useMemo(() => isForNew ? (<div>Add</div>) : (<div>Edit</div>), [isForNew]) ;
    const okButtonTitle = isForNew ? "Create" : "Update"; // string literals could not be memoized since should have the same references

    console.log("EditDialog render")
    return (
        <Dialog
            isDialogOpen={isDialogOpen}
            setIsDialogOpen={setIsDialogOpen}
            title={title}
            okButtonTitle={okButtonTitle}
            okButton_onClick={okButton_onClick}
            errorMessage={errorMessage}
            setErrorMessage={setErrorMessage}
        >
            {children}
        </Dialog>
    )
})
EditDialog.displayName = "EditDialog";

const DeleteDialog = React.memo(({ isDeleteDialogOpen, setIsDeleteDialogOpen, okButton_onClick, errorMessage, setErrorMessage }) => {
    const title = "Are you sure you want to delete this item?";
    const okButtonTitle = "Delete";

    return (
        <Dialog
            isDialogOpen={isDeleteDialogOpen}
            setIsDialogOpen={setIsDeleteDialogOpen}
            title={title}
            okButtonTitle={okButtonTitle}
            okButton_onClick={okButton_onClick}
            errorMessage={errorMessage}
            setErrorMessage={setErrorMessage}
        />
    )
})

DeleteDialog.displayName = "DeleteDialog";

EditDialog.propTypes = {
    isDialogOpen: PropTypes.bool,
    setIsDialogOpen: PropTypes.func,
    isForNew: PropTypes.bool,
    okButton_onClick: PropTypes.func,
    errorMessage: PropTypes.node,
    setErrorMessage: PropTypes.func,
    children: PropTypes.node
};

DeleteDialog.propTypes = {
    isDeleteDialogOpen: PropTypes.bool,
    setIsDeleteDialogOpen: PropTypes.func,
    okButton_onClick: PropTypes.func,
    errorMessage: PropTypes.node,
    setErrorMessage: PropTypes.func
};

export { EditDialog, DeleteDialog };
import React, { useMemo } from 'react';

import PropTypes from "prop-types";
import './CrudTable.css';
import Dialog from "./Dialog";

const EditDialog = React.memo((
    { isForNew, setIsDialogOpen, okButton_onClick, errorMessage, isPending, children }) => {
    const title = useMemo(() => isForNew ? (<div>Add</div>) : (<div>Edit</div>), [isForNew]) ;
    const okButtonTitle = isForNew ? "Create" : "Update"; // string literals could not be memoized since should have the same references

    console.log("EditDialog render")
    return (
        <Dialog
            setIsDialogOpen={setIsDialogOpen}
            title={title}
            okButtonTitle={okButtonTitle}
            okButton_onClick={okButton_onClick}
            errorMessage={errorMessage}
            isPending={isPending}
        >
            {children}
        </Dialog>
    )
})
EditDialog.displayName = "EditDialog";

const DeleteDialog = React.memo(({ setIsDialogOpen, okButton_onClick, errorMessage, isPending }) => {
    const title = "Are you sure you want to delete this item?";
    const okButtonTitle = "Delete";

    return (
        <Dialog
            setIsDialogOpen={setIsDialogOpen}
            title={title}
            okButtonTitle={okButtonTitle}
            okButton_onClick={okButton_onClick}
            errorMessage={errorMessage}
            isPending={isPending}
        />
    )
})
DeleteDialog.displayName = "DeleteDialog";

EditDialog.propTypes = {
    isForNew: PropTypes.bool,
    isPending: PropTypes.bool, 
    setIsDialogOpen: PropTypes.func,
    okButton_onClick: PropTypes.func,
    errorMessage: PropTypes.node,
    children: PropTypes.node
};

DeleteDialog.propTypes = {
    setIsDialogOpen: PropTypes.func,
    okButton_onClick: PropTypes.func,
    errorMessage: PropTypes.node,
    isPending: PropTypes.bool
};

export { EditDialog, DeleteDialog };
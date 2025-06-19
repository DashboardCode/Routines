import PropTypes from "prop-types";
import './CrudTable.css';
import Dialog from "./Dialog";

function EditDialog(
    {isDialogOpen,
     setIsDialogOpen,
     isForNew,
     renderFormFields,
     errorMessage,
     isLoadingEdit,
     okButton_onClick}) {
    const title = isForNew ? (<div>Add</div>) : (<div>Edit</div>) ;
    const okButtonTitle = isForNew ? "Create" : "Update";

    return (
        <Dialog
            isDialogOpen={isDialogOpen}
            setIsDialogOpen={setIsDialogOpen}
            errorMessage={errorMessage}
            isLoading={isLoadingEdit}
            title={title}
            okButtonTitle={okButtonTitle}
            okButton_onClick={okButton_onClick}
        >
            {renderFormFields()}
        </Dialog>
    )
}

function DeleteDialog({ isDeleteDialogOpen, setIsDeleteDialogOpen, okButton_onClick, errorMessage, isLoadingDelete}) {
    const title = "Are you sure you want to delete this item?";
    const okButtonTitle = "Delete";

    return (
        <Dialog
            isDialogOpen={isDeleteDialogOpen}
            setIsDialogOpen={setIsDeleteDialogOpen}
            errorMessage={errorMessage}
            isLoading={isLoadingDelete}
            title={title}
            okButtonTitle={okButtonTitle}
            okButton_onClick= {okButton_onClick}
        />
    )
}

EditDialog.propTypes = {
    isDialogOpen: PropTypes.bool,
    setIsDialogOpen: PropTypes.func,
    isForNew: PropTypes.bool,
    renderFormFields: PropTypes.func,
    entity: PropTypes.object,
    setEntity: PropTypes.func,
    errorMessage: PropTypes.node,
    okButton_onClick: PropTypes.func,
    isLoadingEdit: PropTypes.bool
};

DeleteDialog.propTypes = {
    isDeleteDialogOpen: PropTypes.bool,
    setIsDeleteDialogOpen: PropTypes.func,
    okButton_onClick: PropTypes.func,
    errorMessage: PropTypes.node,
    isLoadingDelete: PropTypes.bool
};

export { EditDialog, DeleteDialog };
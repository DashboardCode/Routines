import PropTypes from "prop-types";
import './CrudTable.css';
import Dialog from "./Dialog";

function EditDialog(
    {isDialogOpen,
     setIsDialogOpen,
     isForNew,
     okButton_onClick,
     setDialogApi,
     children }) {
    const title = isForNew ? (<div>Add</div>) : (<div>Edit</div>) ;
    const okButtonTitle = isForNew ? "Create" : "Update";

    return (
        <Dialog
            isDialogOpen={isDialogOpen}
            setIsDialogOpen={setIsDialogOpen}
            title={title}
            okButtonTitle={okButtonTitle}
            okButton_onClick={okButton_onClick}
            setDialogApi={setDialogApi}
        >
            {children}
        </Dialog>
    )
}

function DeleteDialog({ isDeleteDialogOpen, setIsDeleteDialogOpen, okButton_onClick, setDialogApi }) {
    const title = "Are you sure you want to delete this item?";
    const okButtonTitle = "Delete";

    return (
        <Dialog
            isDialogOpen={isDeleteDialogOpen}
            setIsDialogOpen={setIsDeleteDialogOpen}
            title={title}
            okButtonTitle={okButtonTitle}
            okButton_onClick={okButton_onClick}
            setDialogApi={setDialogApi}
        />
    )
}

EditDialog.propTypes = {
    isDialogOpen: PropTypes.bool,
    setIsDialogOpen: PropTypes.func,
    isForNew: PropTypes.bool,
    okButton_onClick: PropTypes.func,
    setDialogApi: PropTypes.func,
    children: PropTypes.element
};

DeleteDialog.propTypes = {
    isDeleteDialogOpen: PropTypes.bool,
    setIsDeleteDialogOpen: PropTypes.func,
    okButton_onClick: PropTypes.func,
    setDialogApi: PropTypes.func
};

export { EditDialog, DeleteDialog };
import React, { useState, useMemo, useCallback } from 'react';

import PropTypes from "prop-types";
import CrudTable from './CrudTable';

import { DeleteDialog, EditDialog } from './CrudDialogs'
const CrudTableWithDialogs = React.memo(({
    baseColumns,
    list,
    reload,
    errorMessageList,
    isLoadingList,

    fetchDelete,

    //fetchBulkDelete,

    trigger,
    getValues,
    reset,
    
    cloneEntity,
    createDefaultEmpty,
    editFormHandlers,
    editForm,
    //multiSelectDialogs
    
}) => {

    const [errorMessageEdit, setErrorMessageEdit] = useState("");
    const [errorMessageDelete, setErrorMessageDelete] = useState("");

    const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
    const [isEditDialogOpen, setIsEditDialogOpen] = useState(false);

    const [isForNew, setIsForNew] = useState(null); // pseudo-selected entity "clicked on button on the row"


    const [entity, setEntity] = useState(null);

    const buttonHandlers = useMemo(()=>({
        handleCreateButtonClick: () => {
            setIsForNew(true);
            var copy = createDefaultEmpty();
            setEntity(copy);
            reset(copy, {
                    keepErrors: false,
                    keepDirty: false,
                    keepTouched: false,
                }); // Set form to the selected item
            setErrorMessageEdit(null);
            setIsEditDialogOpen(true);
        },
        handleUpdateButtonClick: (e) => {
            setIsForNew(false);
            var copy = cloneEntity(e);
            setEntity(copy); 
            reset(copy, {
                    keepErrors: false,
                    keepDirty: false,
                    keepTouched: false,
                }); // Set form to the selected item
            setErrorMessageEdit(null);
            setIsEditDialogOpen(true);
        },
        handleDeleteButtonClick: (e) => {
            var copy = cloneEntity(e);
            setEntity(copy); 
            setErrorMessageDelete(null);
            setIsDeleteDialogOpen(true);
        },
        handleDetailsButtonClick: null
        //        (entity) => {
        //        // TODO await? get id
        //        //var id = getId(entity);
        //        //fetch(`${ADMINKA_API_BASE_URL}/ui/connections/${id}`, { headers: { "Content-Type": "application/json" } });
        //    }
    }), [createDefaultEmpty, cloneEntity, setEntity, reset, setIsForNew, setErrorMessageEdit, setErrorMessageDelete, setIsEditDialogOpen]);

    const multiSelectActionsMemo = useMemo(() => {
        var multiSelectActions = null;
        var isMultiSelectEdit = true;
        var isMultiSelectDelete = true;
        if (isMultiSelectEdit) {
            if (multiSelectActions == null)
                multiSelectActions = [];
            multiSelectActions.push({ handleButtonClick: () => { buttonHandlers.handleCreateButtonClick() /*TEST*/ }, buttonTitle: "Edit" });
        }
        if (isMultiSelectDelete) {
            if (multiSelectActions == null)
                multiSelectActions = [];
            multiSelectActions.push({ handleButtonClick: () => { buttonHandlers.handleCreateButtonClick() /*TEST*/ }, buttonTitle: "Delete" });
        }
        return multiSelectActions;
    }, [ buttonHandlers]
    );

    const okButton_onClick_Edit = useCallback((setErrorMessage, setIsLoading) =>
        saveButton_onClick(isForNew, editFormHandlers, entity, setErrorMessage, setIsEditDialogOpen, setIsLoading, reload, trigger, getValues),
        [isForNew, editFormHandlers, entity, setIsEditDialogOpen, reload, trigger, getValues]
    );

    const okButton_onClick_Delete = useCallback((setErrorMessage, setIsLoading) =>
        okButton_onClick(fetchDelete, entity, setErrorMessage, setIsDeleteDialogOpen, setIsLoading, reload),
        [fetchDelete, entity, setIsDeleteDialogOpen, reload]
    );

    console.log("CrudTableWithDialogs render")
    return (
        <div>
             <CrudTable
             list={list}
             errorMessage={errorMessageList}
             isLoading={isLoadingList}
             baseColumns={baseColumns}
             multiSelectActions={multiSelectActionsMemo}
             buttonHandlers={buttonHandlers}
             />

            {editForm &&
                <EditDialog
                isDialogOpen={isEditDialogOpen}
                setIsDialogOpen={setIsEditDialogOpen}
                isForNew={isForNew}
                    okButton_onClick={okButton_onClick_Edit}
                errorMessage={errorMessageEdit}
                setErrorMessage={setErrorMessageEdit}>
                    {editForm} {/*was renderFormFields(entity). how to pass entity now?*/}
                </EditDialog>}

            {fetchDelete &&
                <DeleteDialog
                isDeleteDialogOpen={isDeleteDialogOpen}
                setIsDeleteDialogOpen={setIsDeleteDialogOpen}
                okButton_onClick={okButton_onClick_Delete}
                errorMessage={errorMessageDelete}
                setErrorMessage = { setErrorMessageDelete }
                />
            }
        </div>
    );
})

CrudTableWithDialogs.displayName = "CrudTableWithDialogs"; // for debugging purposes

async function okButton_onClick(fetchDelete, entity, setErrorMessage, setIsDeleteDialogOpen, setIsLoading, reload) {
    try {
        setIsLoading(true);
        var success = await fetchDelete(entity, setErrorMessage);
        if (success) { 
            setIsDeleteDialogOpen(false);
            reload();
        }
    } catch (err) {
        setErrorMessage(err.message);
        console.error(err);
    } finally {
        setIsLoading(false);
    }
}

async function saveButton_onClick(isForNew, editFormHandlers, entity, setErrorMessage, setIsEditDialogOpen, setIsLoading, reload, trigger, getValues) {
    try {
        setIsLoading(true);
        const isValid = await trigger(); // validate all fields
        if (isValid) {
            const hookFormState = getValues();
            var success;
            if (isForNew) {
                success = await editFormHandlers.fetchCreate(entity, hookFormState, setErrorMessage)
            } else {
                success = await editFormHandlers.fetchReplace(entity, hookFormState, setErrorMessage)
            }
            if (success) {
                setIsEditDialogOpen(false);
                reload();
            }
        }
    } catch (err) {
        var message = (typeof err === 'string') ? err : (typeof err.message === 'string' ? err.message : String(err));
        setErrorMessage(message);
        console.error(err);
    } finally {
        setIsLoading(false);
    }
};

CrudTableWithDialogs.propTypes = {

    baseColumns: PropTypes.array, 
    list: PropTypes.array,
    reload: PropTypes.func,
    errorMessageList: PropTypes.node,
    isLoadingList: PropTypes.bool,

    //createDefaultEmpty: PropTypes.func,
    //cloneEntity: PropTypes.func,
    //renderFormFields: PropTypes.func,

    //fetchCreate: PropTypes.func,
    //fetchReplace: PropTypes.func,
    fetchDelete: PropTypes.func,
    fetchBulkDelete: PropTypes.func,
    formManager: PropTypes.object,
    cloneEntity: PropTypes.func.isRequired,
    createDefaultEmpty: PropTypes.func.isRequired,
    trigger: PropTypes.func.isRequired,
    getValues: PropTypes.func.isRequired,
    reset: PropTypes.func.isRequired,
    editForm: PropTypes.element,
    editFormHandlers: PropTypes.object,
    multiSelectDialogs: PropTypes.array
};

CrudTableWithDialogs.whyDidYouRender = false;

export default CrudTableWithDialogs;
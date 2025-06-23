import { useState, useCallback } from 'react';

import PropTypes from "prop-types";
import CrudTable from './CrudTable';
import Selectable from './Selectable';

import { DeleteDialog, EditDialog } from './CrudDialogs'
function CrudTableWithDialogs({
    baseColumns,
    fetchList,
    fetchDelete,
    //fetchBulkDelete,
    editFormApi,
    cloneEntity,
    createDefaultEmpty,
    editForm,
    //multiSelectDialogs
    
}) {

    const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
    const [isEditDialogOpen, setIsEditDialogOpen] = useState(false);

    const [isForNew, setIsForNew] = useState(null); // pseudo-selected entity "clicked on button on the row"

    const [editDialogApi, setEditDialogApi] = useState(null); // for dialogs, set by EditDialog and DeleteDialog
    const handleSetEditDialogApi = useCallback((api) => { setEditDialogApi(api); }, []);
    const [deleteDialogApi, setDeleteDialogApi] = useState(null); // for dialogs, set by EditDialog and DeleteDialog
    const handleSetDeleteDialogApi = useCallback((api) => { setDeleteDialogApi(api); }, []);
    const [crudTableApi, setCrudTableApi] = useState(null);
    const handleSetCrudTableApi = useCallback((api) => { setCrudTableApi(api); }, []);
    const [selectableApi, setSelectableApi] = useState(null);
    const handleSetSelectableApi = useCallback((api) => { setSelectableApi(api); }, []);

    const buttonHandlers = {
        handleCreateButtonClick: () => {
            setIsForNew(true);
            //var copy = editForm.createDefaultEmpty();
            var copy = selectableApi.resetEntity();
            editFormApi.reset(copy, {
                keepErrors: false,
                keepDirty: false,
                keepTouched: false,
            }); // Set form to the selected item
            editDialogApi.setErrorMessage(null);
            setIsEditDialogOpen(true);
        },
        handleUpdateButtonClick: (e) => {
            setIsForNew(false);
            //var copy = editForm.cloneEntity(e);
            var copy = selectableApi.setEntity(e);
            editFormApi.reset(copy, {
                keepErrors: false,
                keepDirty: false,
                keepTouched: false,
            }); // Set form to the selected item
            editDialogApi.setErrorMessage(null);
            setIsEditDialogOpen(true);
        },
        handleDeleteButtonClick: (e) => {
            selectableApi.setEntity(e);
            deleteDialogApi.setErrorMessageEdit(null);
            setIsDeleteDialogOpen(true);
        },
        handleDetailsButtonClick: null
        //        (entity) => {
        //        // TODO await? get id
        //        //var id = getId(entity);
        //        //fetch(`${ADMINKA_API_BASE_URL}/ui/connections/${id}`, { headers: { "Content-Type": "application/json" } });
        //    }
    }

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

    //var { crudTableProps, reload } = useCrudTable({
    //        fetchList,
    //        baseColumns,
    //        options: {
    //            multiSelectActions,
    //            buttonHandlers
    //        }
    //    }
    //);
    console.log("CrudTableWithDialogs render")
    return (
        <div>
            <CrudTable fetchList={fetchList} baseColumns={baseColumns} multiSelectActions={multiSelectActions} buttonHandlers={buttonHandlers} setCrudTableApi={handleSetCrudTableApi} />
            <Selectable createDefaultEmpty={createDefaultEmpty} cloneEntity={cloneEntity} setSelectableApi={handleSetSelectableApi}>
                {   editForm ?? <EditDialog
                isDialogOpen={isEditDialogOpen}
                setIsDialogOpen={setIsEditDialogOpen}
                isForNew={isForNew}
                    okButton_onClick={(setErrorMessage, setIsLoading) => saveButton_onClick(isForNew, editForm, editFormApi, selectableApi, setErrorMessage, setIsEditDialogOpen, setIsLoading, crudTableApi)}
                setDialogApi={handleSetEditDialogApi}
                >
                        {editForm.form} {/*was renderFormFields(entity). how to pass entity now?*/}
                </EditDialog>}

                {fetchDelete && <DeleteDialog
                    isDeleteDialogOpen={isDeleteDialogOpen}
                    setIsDeleteDialogOpen={setIsDeleteDialogOpen}
                    okButton_onClick={(setErrorMessage, setIsLoading) => okButton_onClick(fetchDelete, selectableApi, setErrorMessage, setIsDeleteDialogOpen, setIsLoading, crudTableApi)}
                    setDialogApi={handleSetDeleteDialogApi}
                    />}
            </Selectable>
            
        </div>
    );
}

async function okButton_onClick(fetchDelete, selectableApi, setErrorMessage, setIsDeleteDialogOpen, setIsLoading, crudTableApi) {
    var entity = selectableApi.getEntity();
    try {
        setIsLoading(true);
        var success = await fetchDelete(entity, setErrorMessage);
        if (success) { 
            setIsDeleteDialogOpen(false);
            crudTableApi.reload();
        }
    } catch (err) {
        setErrorMessage(err.message);
        console.error(err);
    } finally {
        setIsLoading(false);
    }
}

async function saveButton_onClick(isForNew, editForm, editFormManager, selectableManager, setErrorMessage, setIsEditDialogOpen, setIsLoading, listManager) {
    try {
        var entity = selectableManager.getEntity();
        var setError = editFormManager.setError;
        var dirtyFields = editFormManager.dirtyFields;
        var trigger = editFormManager.trigger;
        var getValues = editFormManager.getValues
        setIsLoading(true);
        const isValid = await trigger(); // validate all fields
        if (isValid) {
            const hookFormState = getValues();
            var success;
            if (isForNew) {
                success = await editForm.fetchCreate(entity, hookFormState, setErrorMessage, setError)
            } else {
                success = await editForm.fetchReplace(entity, hookFormState, setErrorMessage, setError, dirtyFields)
            }
            if (success) {
                setIsEditDialogOpen(false);
                listManager.reload();
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
    fetchList: PropTypes.func,

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
    editFormApi: PropTypes.shape({
        reset: PropTypes.func.isRequired,
        setError: PropTypes.func.isRequired,
        dirtyFields: PropTypes.object.isRequired,
        trigger: PropTypes.func.isRequired,
        getValues: PropTypes.func.isRequired,
        setEntity: PropTypes.func.isRequired,
        getEntity: PropTypes.func.isRequired
    }),
    editForm: PropTypes.shape({
        fetchCreate: PropTypes.func,
        fetchReplace: PropTypes.func,
        form: PropTypes.element.isRequired
    }),

};

CrudTableWithDialogs.whyDidYouRender = false;

export default CrudTableWithDialogs;
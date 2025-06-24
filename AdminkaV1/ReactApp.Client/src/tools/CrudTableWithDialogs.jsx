import { useState, useCallback, useEffect } from 'react';

import DebugMenu from './DebugMenu';

import PropTypes from "prop-types";
import CrudTable from './CrudTable';

import { DeleteDialog, EditDialog } from './CrudDialogs'
function CrudTableWithDialogs({
    baseColumns,
    fetchList,
    fetchDelete,

    //fetchBulkDelete,

    trigger,
    getValues,
    reset,
    
    cloneEntity,
    createDefaultEmpty,
    editForm,
    //multiSelectDialogs
    
}) {

    const [errorMessageEdit, setErrorMessageEdit] = useState("");
    const [errorMessageDelete, setErrorMessageDelete] = useState("");

    const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
    const [isEditDialogOpen, setIsEditDialogOpen] = useState(false);

    const [isForNew, setIsForNew] = useState(null); // pseudo-selected entity "clicked on button on the row"


    const [entity, setEntity] = useState(null);

    const buttonHandlers = {
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

    const [list, setList] = useState();
    const [isLoading, setIsLoading] = useState(true);
    const [errorMessageList, setErrorMessageList] = useState(0);

    const [incTrigger, setIncTrigger] = useState(0); // Changing this triggers refetch
    const reload = useCallback(() => {
        setIncTrigger(v => v + 1);
    }, []);

    useEffect(() => {
        // isMountCancelled prevent updating state on an unmounted component, which can cause warnings or memory leaks.
        // It is a common pattern to use a flag to track whether the component is still mounted when the async operation completes.
        // Otherwise you will get a warning in the console like "Can't perform a React state update on an unmounted component".
        let isMountCancelled = false;

        setIsLoading(true);
        console.log("CrudTableWithDialogs render.fetch")
        fetchList(setList, setErrorMessageList) // is async but we call it without await inside useEffect since this is a promise,
            .then((result) => {
                if (result) {
                    setErrorMessageList(null);
                }
            })
            .catch((error) => {
                if (!isMountCancelled)
                    console.error('Fetch failed', error);
            })
            .finally(() => {
                if (!isMountCancelled)  // if component is still mounted, e.g. not navigated away or second time opened
                    setIsLoading(false);
            });

        return () => {
            isMountCancelled = true; // mark as cancelled
        };
    }, [incTrigger, fetchList]);
    // ----------------------------

    console.log("CrudTableWithDialogs render")
    return (
        <div>
            <DebugMenu actions={[
                { name: "refreshData", action: () => reload() },
                { name: "Remove First Row", action: () => setList((l) => l.slice(1)) },
            ]} />
            <CrudTable
                list={list}
                errorMessage={errorMessageList}
                isLoading={isLoading}
                baseColumns={baseColumns}
                multiSelectActions={multiSelectActions}
                buttonHandlers={buttonHandlers}
                />
            {editForm && <EditDialog
                isDialogOpen={isEditDialogOpen}
                setIsDialogOpen={setIsEditDialogOpen}
                isForNew={isForNew}
                okButton_onClick={(setErrorMessage, setIsLoading) => saveButton_onClick(isForNew, editForm, entity, setErrorMessage, setIsEditDialogOpen, setIsLoading, reload, trigger, getValues)}
                errorMessage={errorMessageEdit}
                setErrorMessage = { setErrorMessageEdit }
            >{editForm.form} {/*was renderFormFields(entity). how to pass entity now?*/}
            </EditDialog>}

            {fetchDelete &&
                <DeleteDialog
                isDeleteDialogOpen={isDeleteDialogOpen}
                setIsDeleteDialogOpen={setIsDeleteDialogOpen}
                okButton_onClick={(setErrorMessage, setIsLoading) => okButton_onClick(fetchDelete, entity, setErrorMessage, setIsDeleteDialogOpen, setIsLoading, reload)}
                errorMessage={errorMessageDelete}
                setErrorMessage = { setErrorMessageDelete }
            />
            }
        </div>
    );
}

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

async function saveButton_onClick(isForNew, editForm, entity, setErrorMessage, setIsEditDialogOpen, setIsLoading, reload, trigger, getValues) {
    try {
        setIsLoading(true);
        const isValid = await trigger(); // validate all fields
        if (isValid) {
            const hookFormState = getValues();
            var success;
            if (isForNew) {
                success = await editForm.fetchCreate(entity, hookFormState, setErrorMessage)
            } else {
                success = await editForm.fetchReplace(entity, hookFormState, setErrorMessage)
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
    trigger: PropTypes.func.isRequired,
    getValues: PropTypes.func.isRequired,
    reset: PropTypes.func.isRequired,
    editForm: PropTypes.shape({
        fetchCreate: PropTypes.func,
        fetchReplace: PropTypes.func,
        form: PropTypes.element.isRequired
    }),
    multiSelectDialogs: PropTypes.array
};

CrudTableWithDialogs.whyDidYouRender = false;

export default CrudTableWithDialogs;
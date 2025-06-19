import { useState } from 'react';

import PropTypes from "prop-types";
import CrudTable from './CrudTable';
import useCrudTable from './useCrudTable';
import { DeleteDialog, EditDialog } from './CrudDialogs'
function CrudTableWithDialogs({
    baseColumns,
    fetchList,

    createDefaultEmpty,
    cloneEntity,
    renderFormFields,
    
    fetchCreate,
    fetchReplace,
    fetchDelete,

    hookFormReset,
    hookFormTrigger,
    hookFormGetValues
}) {

    const [errorMessageEdit, setErrorMessageEdit] = useState("");
    const [errorMessageDelete, setErrorMessageDelete] = useState(""); 

    const [isLoadingEdit, setIsLoadingEdit] = useState(false); 
    const [isLoadingDelete, setIsLoadingDelete] = useState(false); 

    const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
    const [isEditDialogOpen, setIsEditDialogOpen] = useState(false);

    const [entity, setEntity] = useState(null); // create and update form filds
    const [isForNew, setIsForNew] = useState(null); // pseudo-selected entity "clicked on button on the row"

    const buttonHandlers = {
        handleCreateButtonClick: () => {
            setIsForNew(true);
            var copy = createDefaultEmpty();
            setEntity(copy);
            hookFormReset(copy, {
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
            hookFormReset(copy, {
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
            setErrorMessageEdit(null);
            setIsDeleteDialogOpen(true);
        },
        handleDetailsButtonClick: null
    //        (entity) => {
    //        // TODO await? get id
    //        //var id = getId(entity);
    //        //fetch(`${ADMINKA_API_BASE_URL}/ui/connections/${id}`, { headers: { "Content-Type": "application/json" } });
    //    }
    }

    var { crudTableProps, reload } = useCrudTable(
        fetchList,
        baseColumns,
        /*options*/{
            multiSelectActions: [],
            buttonHandlers
        }
    );


    return (
        <div>
            <CrudTable {...crudTableProps} />

            <EditDialog
                isDialogOpen={isEditDialogOpen}
                setIsDialogOpen={setIsEditDialogOpen}
                isForNew={isForNew}
                renderFormFields={() => renderFormFields(entity)}
                errorMessage={errorMessageEdit}
                isLoadingEdit={isLoadingEdit}
                okButton_onClick={() => saveButton_onClick(isForNew, entity, setIsEditDialogOpen, fetchCreate, fetchReplace, setErrorMessageEdit, hookFormTrigger, hookFormGetValues, setIsLoadingEdit, reload)}
            />

            <DeleteDialog
                isDeleteDialogOpen={isDeleteDialogOpen}
                setIsDeleteDialogOpen={setIsDeleteDialogOpen}
                errorMessage={errorMessageDelete}
                isLoadingDelete={isLoadingDelete }
                okButton_onClick={() => okButton_onClick(entity, setIsDeleteDialogOpen, fetchDelete, setErrorMessageDelete, setIsLoadingDelete, reload)}
            />
        </div>
    );
}

async function okButton_onClick(entity, setIsDeleteDialogOpen, fetchDelete, setErrorMessageDelete, setIsLoading, reload) {
    try {
        setIsLoading(true);
        var success = await fetchDelete(entity, setErrorMessageDelete);
        if (success) { 
            setIsDeleteDialogOpen(false);
            reload();
        }
    } catch (err) {
        setErrorMessageDelete(err.message);
        console.error(err);
    } finally {
        setIsLoading(false);
    }
}

async function saveButton_onClick(isForNew, entity, setIsEditDialogOpen, fetchCreate, fetchReplace, setErrorMessageEdit, hookFormTrigger, hookFormGetValues, setIsLoading, reload) {
    try {
        setIsLoading(true);
        const isValid = await hookFormTrigger(); // validate all fields
        if (isValid) {
            const hookFormState = hookFormGetValues();
            var success;
            if (isForNew) {
                success = await fetchCreate(entity, hookFormState, setErrorMessageEdit)
            } else {
                success = await fetchReplace(entity, hookFormState, setErrorMessageEdit)
            }
            if (success) {
                setIsEditDialogOpen(false);
                reload();
            }
        }
    } catch (err) {
        var message = (typeof err === 'string') ? err : (typeof err.message === 'string' ? err.message : String(err));
        setErrorMessageEdit(message);
        console.error(err);
    } finally {
        setIsLoading(false);
    }
};

CrudTableWithDialogs.propTypes = {
    baseColumns: PropTypes.array, 
    fetchList: PropTypes.func,

    createDefaultEmpty: PropTypes.func,
    cloneEntity: PropTypes.func,
    renderFormFields: PropTypes.func,

    fetchCreate: PropTypes.func,
    fetchReplace: PropTypes.func,
    fetchDelete: PropTypes.func,

    hookFormReset: PropTypes.func,
    hookFormTrigger:PropTypes.func,
    hookFormGetValues: PropTypes.func
};

export default CrudTableWithDialogs;
import { useState, useEffect } from 'react';

import PropTypes from "prop-types";
import './CrudTable.css';
import CrudTable from './CrudTable';
import { DeleteDialog, EditDialog } from './CrudDialogs'
function CrudTableWithDialogs({
    list, setList,
    createDefaultEmpty, cloneEntity,
    isLoading,
    baseColumns,

    setIsLoading,

    renderFormFields,

    fetchCreate,
    fetchReplace,
    fetchDelete,

    errorMessageEdit,
    setErrorMessageEdit,
    errorMessageDelete,
    setErrorMessageDelete,

    hookFormReset,
    hookFormTrigger,
    hookFormGetValues
}) {

    const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
    const [isEditDialogOpen, setIsEditDialogOpen] = useState(false);

    const [entity, setEntity] = useState(createDefaultEmpty());
    const [choosedEntity, setChoosedEntity] = useState(null); // pseudo-selected entity "clicked on button on the row"

    useEffect(() => {
        if (isEditDialogOpen && choosedEntity) {
            hookFormReset(choosedEntity,{
                keepErrors: false,
                keepDirty: false,
                keepTouched: false,
            }); // Set form to the selected item
            setErrorMessageEdit(null)
        } else if (isEditDialogOpen && choosedEntity==null) {
            hookFormReset(createDefaultEmpty(),{
                keepErrors: false,
                keepDirty: false,
                keepTouched: false,
            }); // Set form to the selected item
            setErrorMessageEdit(null)
        }
    }, [isEditDialogOpen, choosedEntity, hookFormReset, createDefaultEmpty]);


    const buttonHandlers = {
        handleCreateButtonClick: () => { setIsEditDialogOpen(true) },
        handleUpdateButtonClick: (entity) => {
            setEntity(cloneEntity(entity));
            setChoosedEntity(entity);
            setIsEditDialogOpen(true);
        },
        handleDeleteButtonClick: (entity) => {
            setEntity(cloneEntity(entity));
            setIsDeleteDialogOpen(true);
        },
        handleDetailsButtonClick: null
    //        (entity) => {
    //        // TODO await? get id
    //        //var id = getId(entity);
    //        //fetch(`${ADMINKA_API_BASE_URL}/ui/connections/${id}`, { headers: { "Content-Type": "application/json" } });
    //    }
    }

    return (
        <div>
            
            <CrudTable list={list}
                setList={setList}
                isLoading={isLoading}
                baseColumns={baseColumns}
                setChoosedEntity={setChoosedEntity}
                multiSelectActions={null}
                buttonHandlers={buttonHandlers}
            />

            <EditDialog
                isDialogOpen={isEditDialogOpen}
                setIsDialogOpen={setIsEditDialogOpen}
                isForNew={choosedEntity==null}
                renderFormFields={renderFormFields}
                entity={entity}
                setEntity={setEntity}
                errorMessage={errorMessageEdit}
                okButton_onClick={() => saveButton_onClick(choosedEntity, entity, createDefaultEmpty, setIsEditDialogOpen, setEntity, setChoosedEntity, fetchCreate, fetchReplace, setErrorMessageEdit, hookFormTrigger, hookFormGetValues)}
            />

            <DeleteDialog
                isDeleteDialogOpen={isDeleteDialogOpen}
                setIsDeleteDialogOpen={setIsDeleteDialogOpen}
                okButton_onClick={() => okButton_onClick(entity, createDefaultEmpty, setIsDeleteDialogOpen, setEntity, setChoosedEntity, fetchDelete, setErrorMessageDelete)}
                errorMessage={errorMessageDelete}
            />
        </div>
    );
}

async function okButton_onClick(entity, createDefaultEmpty, setIsDeleteDialogOpen, setEntity, setChoosedEntity, fetchDelete, setErrorMessageDelete) {
    try {
        fetchDelete(entity)
        setIsDeleteDialogOpen(false);
        setEntity(createDefaultEmpty());
        setChoosedEntity(null);
    } catch (err) {
        setErrorMessageDelete(err.message);
        console.error(err);
    }
}

async function saveButton_onClick(choosedEntity, entity, createDefaultEmpty, setIsEditDialogOpen, setEntity, setEditingEntity, fetchCreate, fetchReplace, setErrorMessageEdit, hookFormTrigger, hookFormGetValues) {
    try {
        const isValid = await hookFormTrigger(); // validate all fields
        if (isValid) {
            const hookFormState = hookFormGetValues();
            var success;
            if (choosedEntity) {
                success=await fetchReplace(entity, hookFormState)
            } else {
                success =await fetchCreate(entity, hookFormState)
            }
            if (success) {
                setIsEditDialogOpen(false);
                setEntity(createDefaultEmpty());
                setEditingEntity(null);
            }
        }
    } catch (err) {
        setErrorMessageEdit(err.message);
        console.error(err);
    }
};

CrudTableWithDialogs.propTypes = {
    list: PropTypes.array,
    setList: PropTypes.func,
    createDefaultEmpty: PropTypes.func,
    cloneEntity: PropTypes.func,
    isLoading: PropTypes.bool,
    baseColumns: PropTypes.array,

    setIsLoading: PropTypes.func,

    renderFormFields: PropTypes.func,

    fetchCreate: PropTypes.func,
    fetchReplace: PropTypes.func,
    fetchDelete: PropTypes.func,

    errorMessageEdit: PropTypes.node,
    setErrorMessageEdit: PropTypes.func,
    errorMessageDelete: PropTypes.node,
    setErrorMessageDelete:PropTypes.func,

    hookFormReset: PropTypes.func,
    hookFormTrigger:PropTypes.func,
    hookFormGetValues: PropTypes.func
};

export default CrudTableWithDialogs;
import{ useState, useCallback, useMemo } from 'react';

import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';

import { EditDialog } from '@/tools/CrudDialogs'
import { fetchTokenized } from '@/fetchTokenized';

function useEditDialog(useEditDialogOptions) {

    const { addForm, validationSchema, createDefaultEmpty, fetchCreate, fetchReplace, cloneEntity, reload } = useEditDialogOptions;

    const [entity, setEntity] = useState(null); // selectable list row "component"
    const [isEditDialogOpen, setIsEditDialogOpen] = useState(false);
    const [errorMessageEdit, setErrorMessageEdit] = useState("");
    const [isForNew, setIsForNew] = useState(null); // pseudo-selected entity "clicked on button on the row"
    
    //react-hook-form - state of the form with enabled zod validation
    const {
        register,
        trigger,
        getValues,
        reset, // reset form before reuse dialog for other entities
        /*setError, */ /*set error for fields, doesn't work well with schema validation - whe to show error as a customized generic error - not bad  */
        formState: { errors, /*isValid,*/ dirtyFields } }
        = useForm(
            {
                resolver: zodResolver(validationSchema),
                mode: 'all', /* alternatives onChange, onBlur, onTouched, onSubmit , all(onChange + onBlur + onSubmit) */
                defaultValues: createDefaultEmpty(),
            });

    const handleCreateButtonClick = useCallback(() => {
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
    }, [setEntity, reset, setIsForNew, setErrorMessageEdit, setIsEditDialogOpen, createDefaultEmpty]);

    const editAction = useMemo(
        () => {
            return {
                icon: 'edit_document',
                label: 'Edit',
                onClick: (e) => {
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
                }
            }
        }, [setEntity, reset, setIsForNew, setErrorMessageEdit, setIsEditDialogOpen, cloneEntity]);

    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    const okButton_onClick_Edit = useCallback((setIsLoading) =>
        saveButton_onClick(isForNew, fetchCreate, fetchReplace, entity, setErrorMessageEdit, setIsEditDialogOpen, setIsLoading, reload, trigger, getValues, dirtyFields),
        [isForNew, entity, setIsEditDialogOpen, reload, trigger, getValues, dirtyFields, fetchCreate, fetchReplace, setErrorMessageEdit]
    );

    var editDialog = null;
    if (isEditDialogOpen) {
        // depends on errors and isValid status  - thats mean changes "every time" — should not be memoized (avoiding unnecessary overhead)
        const editForm = addForm(isForNew, register, errors, dirtyFields);

        editDialog = (<EditDialog
                isDialogOpen={isEditDialogOpen}
                setIsDialogOpen={setIsEditDialogOpen}
                isForNew={isForNew}
                okButton_onClick={okButton_onClick_Edit}
            errorMessage={errorMessageEdit}
        >{editForm}</EditDialog>)
                
            
    }
    

    return {
        editDialog,
        editAction,
        handleCreateButtonClick
    };
}

async function saveButton_onClick(isForNew, fetchCreate, fetchReplace, entity, setErrorMessageEdit, setIsEditDialogOpen, setIsLoading, reload, trigger, getValues, dirtyFields) {
    try {
        setIsLoading(true);
        const isValid = await trigger(); // validate all fields
        if (isValid) {
            const hookFormState = getValues();
            var success;
            if (isForNew) {
                success = await fetchCreate(entity, hookFormState, setErrorMessageEdit)
            } else {
                success = await fetchReplace(entity, hookFormState, setErrorMessageEdit, dirtyFields)
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

function useDefaultFetchCreate(uri) {
    var fetch = useCallback((entity, formState, setErrorMessageEdit) => fetchCreateAsync(entity, formState, setErrorMessageEdit, uri), [uri])
    return fetch;
}

async function fetchCreateAsync(entity, formState, setErrorMessageEdit, uri) {
    var body = JSON.stringify(formState);
    console.log(body)
    var response = await fetchTokenized(uri, body, "POST");
    if (response.ok) {
        //await fetchList(setList, setIsLoading);
        return true
    } else {
        var result = await response.json();
        setErrorMessageEdit(result.message);
        //Object.entries(result.errors).forEach(([field, message]) => {
        //    setError(field, { type: 'server', message });
        //});
        return false
    }
}

function useDefaultFetchReplace(createUri) {
    var fetch = useCallback((entity, hookFormState, setErrorMessageEdit, dirtyFields) => fetchReplaceAync(entity, hookFormState, setErrorMessageEdit, dirtyFields, createUri(entity)), [createUri])
    return fetch;
}

async function fetchReplaceAync(entity, formState, setErrorMessageEdit, dirtyFields, uri) {

    var hookFormDelta = getDelta(formState, dirtyFields)
    var delta = JSON.stringify(hookFormDelta);
    var response = await fetchTokenized(uri, delta, "PATCH");
    if (response.ok) {
        //await fetchList(setList, setIsLoading);
        return true;
    }
    else {
        var result = await response.json();
        setErrorMessageEdit(result.message);
        //Object.entries(result.errors).forEach(([field, message]) => {
        //    setError(field, { type: 'server', message });
        //});
        return false
    }
}

function getDelta(allValues, dirtyFields) {
    const delta = {};
    Object.keys(dirtyFields).forEach((key) => {
        delta[key] = allValues[key];
    });
    return delta;
};

export { useEditDialog, useDefaultFetchCreate, useDefaultFetchReplace }
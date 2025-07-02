import{ useState, useCallback, useMemo } from 'react';

import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';

import { EditDialog } from '@/tools/CrudDialogs'
import { fetchTokenized } from '@/fetchTokenized';

function useEditDialog(useEditDialogOptions) {

    const { addForm, validationSchema, createDefaultEmpty, fetchCreate, fetchReplace, transformSelected, reload } = useEditDialogOptions;

    const [selected, setSelected] = useState(null); // selectable list row "component"
    const [isEditDialogOpen, setIsEditDialogOpen] = useState(false);
    const [errorMessageEdit, setErrorMessageEdit] = useState("");
    const [isForNew, setIsForNew] = useState(null); // pseudo-selected  "clicked on button on the row"
    const [isPending, setIsPending] = useState(false);
    
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

    const formState = useMemo(() => ({
        selected, register, errors, dirtyFields, isForNew, trigger, getValues
    }), [selected, register, errors, dirtyFields, isForNew, trigger, getValues]) 

    const handleCreateButtonClick = useCallback(() => {
        setIsForNew(true);
        var copy = createDefaultEmpty();
        setSelected(copy);
        reset(copy, {
            keepErrors: false,
            keepDirty: false,
            keepTouched: false,
        }); // Set form to the selected item
        setErrorMessageEdit(null);
        setIsEditDialogOpen(true);
    }, [setSelected, reset, setIsForNew, setErrorMessageEdit, setIsEditDialogOpen, createDefaultEmpty]);

    const action = useMemo(
        () => {
            return {
                icon: 'edit_document',
                label: 'Edit',
                onClick: (e) => {
                    setIsForNew(false);
                    var copy = transformSelected(e);
                    setSelected(copy);
                    reset(copy, {
                        keepErrors: false,
                        keepDirty: false,
                        keepTouched: false,
                    }); // Set form to the selected item
                    setErrorMessageEdit(null);
                    setIsEditDialogOpen(true);
                }
            }
        }, [setSelected, reset, setIsForNew, setErrorMessageEdit, setIsEditDialogOpen, transformSelected]);

    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    const okButton_onClick_Edit = useCallback(async () =>
        await saveButton_onClick(formState, fetchCreate, fetchReplace, setErrorMessageEdit, setIsEditDialogOpen, setIsPending, reload),
        [formState, fetchCreate, fetchReplace, setErrorMessageEdit, setIsEditDialogOpen, reload]
    );

    var dialog = null;
    if (isEditDialogOpen) {
        // depends on errors and isValid status  - thats mean changes "every time" — should not be memoized (avoiding unnecessary overhead)
        const editForm = addForm(formState);

        dialog = (<EditDialog
            setIsDialogOpen={setIsEditDialogOpen}
            isForNew={isForNew}
            okButton_onClick={okButton_onClick_Edit}
            errorMessage={errorMessageEdit}
            isPending={isPending}
        >{editForm}</EditDialog>)
    }

    return {
        dialog,
        action,
        handleCreateButtonClick
    };
}

async function saveButton_onClick(formState, fetchCreate, fetchReplace, setErrorMessageEdit, setIsEditDialogOpen, setIsPending, reload) {
    try {
        setIsPending(true);
        const { isForNew, trigger} = formState;
        const isValid = await trigger(); // validate all fields
        if (isValid) {
            var success;
            if (isForNew) {
                success = await fetchCreate(formState, setErrorMessageEdit)
            } else {
                success = await fetchReplace(formState, setErrorMessageEdit)
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
        setIsPending(false);
    }
};

function useDefaultFetchCreate(uri) {
    var fetch = useCallback(async (formState, setErrorMessageEdit) => await fetchCreateAsync(formState, setErrorMessageEdit, uri), [uri])
    return fetch;
}

async function fetchCreateAsync(formState, setErrorMessageEdit, uri) {
    const { getValues } = formState;
    var hookFormValues = getValues();
    var body = JSON.stringify(hookFormValues);
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
    const fetch = useCallback(async (formState, setErrorMessageEdit) => await fetchReplaceAync(formState, setErrorMessageEdit, createUri(formState)), [createUri])
    return fetch;
}

async function fetchReplaceAync(formState, setErrorMessageEdit, uri) {
    const { getValues, dirtyFields } = formState;
    const hookFormValues = getValues();
    const hookFormDelta = getDelta(hookFormValues, dirtyFields)
    const delta = JSON.stringify(hookFormDelta);
    const response = await fetchTokenized(uri, delta, "PATCH");
    if (response.ok) {
        //await fetchList(setList, setIsLoading);
        return true;
    }
    else {
        const result = await response.json();
        setErrorMessageEdit(result.message);
        //Object.entries(result.errors).forEach(([field, message]) => {
        //    setError(field, { type: 'server', message });
        //});
        return false;
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
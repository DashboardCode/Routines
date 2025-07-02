import { useState, useCallback, useMemo, useActionState, startTransition, useEffect } from 'react';

import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';

import { EditDialog } from '@/tools/CrudDialogs'
import { fetchTokenized } from '@/fetchTokenized';

import { parseErrorResponseAsync, parseErrorException } from '@/parseErrorResponse';
function useEditDialog(useEditDialogOptions) {
    console.log("useEditDialog")
    const { addForm, validationSchema, createDefaultEmpty, fetchCreate, fetchReplace, transformSelected, reload } = useEditDialogOptions;

    const [selected, setSelected] = useState(null); // selectable list row "component"
    const [isEditDialogOpen, setIsEditDialogOpen] = useState(false);
    
    const [isForNew, setIsForNew] = useState(null); // pseudo-selected  "clicked on button on the row"
    //const [isPending, setIsPending] = useState(false);
    
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

    const [errorMessage, setErrorMessage] = useState("");
    const handleCreateButtonClick = useCallback(() => {
        setIsForNew(true);
        var copy = createDefaultEmpty();
        setSelected(copy);
        reset(copy, {
            keepErrors: false,
            keepDirty: false,
            keepTouched: false,
        }); // Set form to the selected item
        setErrorMessage(null); // TODO !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! test it
        setIsEditDialogOpen(true);
    }, [setSelected, reset, setIsForNew, setErrorMessage, setIsEditDialogOpen, createDefaultEmpty]);

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
                    setErrorMessage(null);
                    setIsEditDialogOpen(true);
                }
            }
        }, [setSelected, reset, setIsForNew, setErrorMessage, setIsEditDialogOpen, transformSelected]);

    const [, okButton_onClick_Edit, isPending] = useActionState(
        async () => {
            try {
                
                const { isForNew, trigger } = formState;
                const isValid = await trigger(); // validate all fields
                if (isValid) {
                    var result;
                    if (isForNew) {
                        result = await fetchCreate(formState)
                    } else {
                        result = await fetchReplace(formState)
                    }
                    if (result.success) {
                        setIsEditDialogOpen(false);
                        reload();
                        setErrorMessage(null);
                        return null;
                    } else {
                        console.error(result.errorContent);
                        setErrorMessage(result.errorContent.message);
                        return result.errorContent.message;

                    }
                }
            } catch (err) {
                console.error(err);
                setErrorMessage(parseErrorException(err));
                return parseErrorException(err);
            } 
        },
        null,
    );

    const okButton_onClick = useCallback(() => {
        startTransition(() => {  // updating UI after data fetch is not urgent, prioritize user input (e.g. press close button)
            okButton_onClick_Edit();
        });
    }, [okButton_onClick_Edit]); 


    var dialog = null;
    if (isEditDialogOpen) {
        // depends on errors and isValid status  - thats mean changes "every time" — should not be memoized (avoiding unnecessary overhead)
        const editForm = addForm(formState);

        dialog = (<EditDialog
            setIsDialogOpen={setIsEditDialogOpen}
            isForNew={isForNew}
            okButton_onClick={okButton_onClick}
            errorMessage={errorMessage}
            isPending={isPending}
        >{editForm}</EditDialog>)
    }

    return {
        dialog,
        action,
        handleCreateButtonClick
    };
}

function useDefaultFetchCreate(uri) {
    var fetch = useCallback(async (formState) => await fetchCreateAsync(formState, uri), [uri])
    return fetch;
}

async function fetchCreateAsync(formState, uri) {
    const { getValues } = formState;
    var hookFormValues = getValues();
    var body = JSON.stringify(hookFormValues);
    var response = await fetchTokenized(uri, body, "POST");
    if (response.ok) {
        return { success:true }
    } else {
        var errorContent = await parseErrorResponseAsync(response);
        return { success: false, errorContent }
    }
}

function useDefaultFetchReplace(createUri) {
    const fetch = useCallback(async (formState) => await fetchReplaceAync(formState, createUri(formState)), [createUri])
    return fetch;
}

async function fetchReplaceAync(formState, uri) {
    const { getValues, dirtyFields } = formState;
    const hookFormValues = getValues();
    const hookFormDelta = getDelta(hookFormValues, dirtyFields)
    const delta = JSON.stringify(hookFormDelta);
    const response = await fetchTokenized(uri, delta, "PATCH");
    if (response.ok) {
        return { success: true }
    } else {
        var errorContent = await parseErrorResponseAsync(response);
        return { success: false, errorContent }
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
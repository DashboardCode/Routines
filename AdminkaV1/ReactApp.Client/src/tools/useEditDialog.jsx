import{ useState, useCallback, useMemo } from 'react';

import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';

import { EditDialog } from '@/tools/CrudDialogs'

function useEditDialog(useEditDialogOptions) {

    const { addForm, saveButton_onClick, connectionValidationSchema, createDefaultEmpty, fetchCreate, fetchReplace, setEntity, cloneEntity, entity,reload } = useEditDialogOptions;

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
                resolver: zodResolver(connectionValidationSchema),
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
        saveButton_onClick(isForNew, fetchCreate, fetchReplace, entity, errorMessageEdit, setIsEditDialogOpen, setIsLoading, reload, trigger, getValues, dirtyFields),
        [isForNew, entity, setIsEditDialogOpen, reload, trigger, getValues, dirtyFields, fetchCreate, fetchReplace, saveButton_onClick, errorMessageEdit]
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

export default useEditDialog;
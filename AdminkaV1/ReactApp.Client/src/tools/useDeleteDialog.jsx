import{ useState, useCallback, useMemo } from 'react';

import { DeleteDialog } from '@/tools/CrudDialogs'

function useDeleteDialog(useDeleteDialogOptions) {

    const { okButton_onClick, fetchDelete, setEntity, cloneEntity, entity, reload } = useDeleteDialogOptions;

    const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
    const [errorMessageDelete, setErrorMessageDelete] = useState("");

    const deleteAction = useMemo(() => {
        return {
            icon: 'delete_forever',
            label: 'Delete',
            onClick: (e) => {
                var copy = cloneEntity(e);
                setEntity(copy);
                setErrorMessageDelete(null);
                setIsDeleteDialogOpen(true);
            }
        }
    }, [setEntity, setErrorMessageDelete, setIsDeleteDialogOpen, cloneEntity]);


    const okButton_onClick_Delete = useCallback((setIsLoading) =>
        okButton_onClick(fetchDelete, entity, setErrorMessageDelete, setIsDeleteDialogOpen, setIsLoading, reload),
        [entity, setIsDeleteDialogOpen, reload, okButton_onClick, fetchDelete]
    );

    var deleteDialog = null;
    if (isDeleteDialogOpen) {
        deleteDialog = <DeleteDialog
            isDeleteDialogOpen={isDeleteDialogOpen}
            setIsDeleteDialogOpen={setIsDeleteDialogOpen}
            okButton_onClick={okButton_onClick_Delete}
            errorMessage={errorMessageDelete}
        />
    }
    

    return {
        deleteDialog,
        deleteAction
    };
}

export default useDeleteDialog;
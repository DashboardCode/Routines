import{ useState, useCallback, useMemo } from 'react';
import { fetchTokenized } from '@/fetchTokenized';
import { DeleteDialog } from '@/tools/CrudDialogs'

function useDeleteDialog(useDeleteDialogOptions) {

    const { fetchDelete, cloneEntity, reload } = useDeleteDialogOptions;

    const [entity, setEntity] = useState(null); // selectable list row "component"
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
        okButton_onClick_Async(fetchDelete, entity, setErrorMessageDelete, setIsDeleteDialogOpen, setIsLoading, reload),
        [entity, setIsDeleteDialogOpen, reload, fetchDelete]
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
        deleteAction,
    };
}

function useDefaultFetchDelete(createUri) {
    var fetch = useCallback((entity, setErrorMessage) => fetchDeleteAsync(setErrorMessage, createUri(entity)), [createUri])
    return fetch;
}

async function fetchDeleteAsync(setErrorMessage, uri) {
    var responce = await fetchTokenized(uri, null, "DELETE");
    if (responce.ok) {
        return true;
    }
    else {
        var result = await responce.json();
        setErrorMessage(result.message);
        return false
    }
};

async function okButton_onClick_Async(fetchDelete, entity, setErrorMessage, setIsDeleteDialogOpen, setIsLoading, reload) {
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
export { useDeleteDialog, useDefaultFetchDelete };
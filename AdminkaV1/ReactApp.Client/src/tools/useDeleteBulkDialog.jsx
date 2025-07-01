import{ useState, useCallback, useMemo } from 'react';
import { fetchTokenized } from '@/fetchTokenized';
import { DeleteDialog } from '@/tools/CrudDialogs'

function useDeleteBulkDialog(useDeleteBulkDialogOptions) {

    const { fetchDelete, adoptSelected, reload } = useDeleteBulkDialogOptions;

    const [selected, setSelected] = useState(null); // selectable list row "component"
    const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
    const [errorMessageDelete, setErrorMessageDelete] = useState("");

    const action = useMemo(() => {
        return {
            icon: 'delete_forever',
            label: 'Bulk Delete',
            onClick: (r) => {
                var s = adoptSelected(r);
                setSelected(s);
                setErrorMessageDelete(null);
                setIsDeleteDialogOpen(true);
            }
        }
    }, [setErrorMessageDelete, setIsDeleteDialogOpen, adoptSelected]);


    const okButton_onClick_Delete = useCallback((setIsLoading) =>
        okButton_onClick_Async(fetchDelete, selected, setErrorMessageDelete, setIsDeleteDialogOpen, setIsLoading, reload),
        [selected, setIsDeleteDialogOpen, reload, fetchDelete]
    );

    var dialog = null;
    if (isDeleteDialogOpen) {
        dialog = <DeleteDialog
            isDeleteDialogOpen={isDeleteDialogOpen}
            setIsDeleteDialogOpen={setIsDeleteDialogOpen}
            okButton_onClick={okButton_onClick_Delete}
            errorMessage={errorMessageDelete}
        />
    }

    return {
        dialog,
        action
    };
}

function useDefaultFetchDeleteBulk(createUri) {
    var fetch = useCallback((selected, setErrorMessage) => fetchDeleteBulkAsync(setErrorMessage, createUri(selected)), [createUri])
    return fetch;
}

async function fetchDeleteBulkAsync(setErrorMessage, uri) {
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

async function okButton_onClick_Async(fetchDelete, selected, setErrorMessage, setIsDeleteDialogOpen, setIsLoading, reload) {
    try {
        setIsLoading(true);
        var success = await fetchDelete(selected, setErrorMessage);
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
export { useDeleteBulkDialog, useDefaultFetchDeleteBulk };
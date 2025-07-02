import { useState, useCallback, useMemo, useActionState, startTransition } from 'react';
import { fetchTokenized } from '@/fetchTokenized';
import { parseErrorResponseAsync } from '@/parseErrorResponse';

import { DeleteDialog } from '@/tools/CrudDialogs'

function useDeleteDialog(useDeleteDialogOptions) {

    const { fetchDelete,  adoptSelected, reload } = useDeleteDialogOptions;

    const [selected, setSelected] = useState(null); // selectable list row "component"
    const [isDialogOpen, setIsDialogOpen] = useState(false);

    const action = useMemo(() => {
        return {
            icon: 'delete_forever',
            label: 'Delete',
            onClick: (e) => {
                var s = adoptSelected(e);
                setSelected(s);
                setIsDialogOpen(true);
            }
        }
    }, [setSelected, setIsDialogOpen, adoptSelected]);

    const [errorMessageDelete, deleteButton_onClick, isPending] = useActionState(
        async () => {
            try {
                var error = await fetchDelete(selected);
                if (error) {
                    return error; 
                } 
            } catch (err) {
                console.error(err);
                return err.message;
            } 
            // handle success
            setIsDialogOpen(false);
            reload();
            return null;
        },
        null,
    );

    const okButton_onClick = useCallback(() => {
        startTransition(() => {  // updating UI after data fetch is not urgent, prioritize user input (e.g. press close button)
            deleteButton_onClick();
        });
    }, [deleteButton_onClick]); 


    var dialog = null;
    if (isDialogOpen) {
        dialog = <DeleteDialog
            setIsDialogOpen={setIsDialogOpen}
            okButton_onClick={okButton_onClick}
            errorMessage={errorMessageDelete}
            isPending={isPending}
        />
    }

    return {
        dialog,
        action,
    };
}

function useDefaultFetchDelete(createUri) {
    var fetch = useCallback(async (selected) => await fetchDeleteAsync(createUri(selected)), [createUri])
    return fetch;
}

async function fetchDeleteAsync(uri) {
    var response = await fetchTokenized(uri, null, "DELETE");
    if (response.ok) {
        return null; // success
    }
    else {
        var errorContent = await parseErrorResponseAsync(response);
        console.error(errorContent);
        return errorContent.message;
    }
};



export { useDeleteDialog, useDefaultFetchDelete };
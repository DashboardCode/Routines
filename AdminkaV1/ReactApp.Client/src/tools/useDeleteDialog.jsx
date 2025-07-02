import { useState, useCallback, useMemo, useActionState, startTransition } from 'react';
import { fetchTokenized } from '@/fetchTokenized';
import { parseErrorResponseAsync, parseErrorException } from '@/parseErrorResponse';

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
                setErrorMessage(null)
            }
        }
    }, [setSelected, setIsDialogOpen, adoptSelected]);

    const [errorMessage, setErrorMessage] = useState(null);
    const [, deleteButton_onClick, isPending] = useActionState(
        async () => {
            try {
                
                var result = await fetchDelete(selected);
                if (!result.success) {
                    console.error(result.errorContent);
                    setErrorMessage(result.errorContent.message)
                    return result.errorContent.message; 
                } 
            } catch (err) {
                console.error(err);
                setErrorMessage(parseErrorException(err))
                return parseErrorException(err);
            } 
            // handle success
            setIsDialogOpen(false);
            reload();
            setErrorMessage(null)
            return null;
        },
        null
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
            errorMessage={errorMessage}
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
        return { success:true }; // success
    }
    else {
        var errorContent = await parseErrorResponseAsync(response);
        return { success: false, errorContent };
    }
};



export { useDeleteDialog, useDefaultFetchDelete };
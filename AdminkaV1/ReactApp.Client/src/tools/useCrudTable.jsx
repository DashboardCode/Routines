import { useState, useCallback, useEffect, useMemo, useRef, useActionState, startTransition } from 'react';
import CrudTable from './CrudTable';
import { fetchTokenized } from '@/fetchTokenized';
import { parseErrorResponseAsync } from '@/parseErrorResponse';


function useCrudTable(useCrudTableOptions) {

    const { fetchList, baseColumns } = useCrudTableOptions;

    const [list, setList] = useState([]);

    const [incTrigger, setIncTrigger] = useState(0); // used to trigger reload
    const reload = useCallback(() => {
        console.log("reload")
        setIncTrigger(v => v + 1);
    }, []);

    // isMountedRef prevent updating state on an unmounted component, which can cause warnings or memory leaks.
    // mount can be canceled because we allow to switch to another page from menu.
    // It is a common pattern to use a flag to track whether the component is still mounted when the async operation completes.
    // Otherwise you will get a warning in the console like "Can't perform a React state update on an unmounted component".
    const isMountedRef = useRef(true);
    const [errorMessage, loadList_onClickOrStart, isPending] = useActionState(
        async (/*prevState, formData, thirdArgument*/) => {
            try {
                var result = await fetchList();
                if (isMountedRef.current) {
                    if (result.success) {
                        setList(result.data);
                    } else {
                        console.error(result.errorContent);
                        return result.errorContent.message;
                    }
                }
                return null;   
            } catch (err) {
                return err.message; 
            }
        },
        null, // initial state for errorMessage, (no error)
        null // third arg to your action, not used
    );

    useEffect(() => {
        isMountedRef.current = true;
        startTransition(() => {
            loadList_onClickOrStart();
        });
        return () => {
            isMountedRef.current = false;
        };
    }, [incTrigger, loadList_onClickOrStart]);

    const crudTableProps = useMemo(() => ({
        list,
        errorMessage,
        isPending,
        baseColumns

    }), [
        list,
        errorMessage,
        isPending,
        baseColumns
    ]);

    const renderCrudTable = useCallback(
        (
            {   rowActions,
                handleDetailsButtonClick,
                handleCreateButtonClick,
                multiSelectActions }
        ) => (
            <CrudTable
                {...crudTableProps}
                multiSelectActions={multiSelectActions}
                handleCreateButtonClick={handleCreateButtonClick}
                handleDetailsButtonClick={handleDetailsButtonClick}
                rowActions={rowActions}
            />
        ),
        [crudTableProps]
    );
    return {
        crudTableProps,
        renderCrudTable,
        reload,
        errorMessage,
        isPending,
        list,
        setList
    };
}

function useDefaultFetchList(uri) {
    var fetch = useCallback( async () => await fetchListAsync(uri), [uri])
    return fetch;
}

async function fetchListAsync(uri) {
        const response = await fetchTokenized(uri);
    if (response.ok) {
        const odata = await response.json();
        return { success:true, data:odata.value };
    }
    else {
        var errorContent = await parseErrorResponseAsync(response);
        return { success: false, errorContent: errorContent };
    }
}
export { useCrudTable, useDefaultFetchList }
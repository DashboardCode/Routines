import { useState, useCallback, useEffect } from 'react';
import CrudTable from './CrudTable';
import { fetchTokenized } from '@/fetchTokenized';

function useCrudTable(useCrudTableOptions) {

    const { fetchList, baseColumns } = useCrudTableOptions;

    // Hook "do something after render" Data fetching, setting up a subscription, and manually changing the DOM, logging in React
    // components are all examples of side effects.
    // useEffect here since this is fetch: async with side effects
    //useEffect(() => {
    //    setIsLoading(true);
    //    fetchList(setList, setIsLoading); // setList - recalls the component render
    //                                      // fetchList is async but we call it without await inside useEffect since this is a promise,
    //                                      // istead useEffect fuctor can return a cleanup function
    //    setIsLoading(false);
    //}, [reload]);


    const [list, setList] = useState([]);
    const [isLoadingList, setIsLoadingList] = useState(true);
    const [errorMessageList, setErrorMessageList] = useState(0);

    const [incTrigger, setIncTrigger] = useState(0); // Changing this triggers refetch
    const reload = useCallback(() => {
        setIncTrigger(v => v + 1);
    }, []);

    useEffect(() => {
        // isMountCancelled prevent updating state on an unmounted component, which can cause warnings or memory leaks.
        // It is a common pattern to use a flag to track whether the component is still mounted when the async operation completes.
        // Otherwise you will get a warning in the console like "Can't perform a React state update on an unmounted component".
        let isMountCancelled = false;

        setIsLoadingList(true);
        console.log("ConnectionsManagement render.fetch")
        fetchList(setList, setErrorMessageList) // is async but we call it without await inside useEffect since this is a promise,
            .then((result) => {
                if (result) {
                    setErrorMessageList(null);
                }
            })
            .catch((error) => {
                if (!isMountCancelled)
                    console.error('Fetch failed', error);
            })
            .finally(() => {
                if (!isMountCancelled)  // if component is still mounted, e.g. not navigated away or second time opened
                    setIsLoadingList(false);
            });

        return () => {
            isMountCancelled = true; // mark as cancelled
        };
    }, [incTrigger, fetchList]);

    const renderEditDialog = (rowActions, handleDetailsButtonClick, handleCreateButtonClick, multiSelectActions )=> (<CrudTable
        list={list}
        errorMessage={errorMessageList}
        isLoading={isLoadingList}
        baseColumns={baseColumns}
        multiSelectActions={multiSelectActions}
        handleCreateButtonClick={handleCreateButtonClick}
        handleDetailsButtonClick={handleDetailsButtonClick}
        rowActions={rowActions}
    />)

    var crudTableProps = {
        reload, list, errorMessageList, isLoadingList, baseColumns
    };
    

    return {
        crudTableProps, reload, list, errorMessageList, isLoadingList, renderEditDialog
    };
}

function useDefaultFetchList(uri) {
    var fetch = useCallback((setList, setErrorMessage) => fetchListAsync(setList, setErrorMessage, uri), [uri])
    return fetch;
}

async function fetchListAsync(setList, setErrorMessageList, uri) {

    const response = await fetchTokenized(uri);
    if (response.ok) {
        const odata = await response.json();
        setList(odata.value);
        return true;
    }
    else {
        var result = await response.json();
        setErrorMessageList(result.message);
        return false
    }
}
export { useCrudTable, useDefaultFetchList }
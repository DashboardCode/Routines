import { useState, useEffect, useCallback} from 'react';

function useCrudTable(
    fetchList,
    baseColumns,
    options = { multiSelectActions:[], buttonHandlers: {} }        
) {
    const [list, setList] = useState();
    const [isLoading, setIsLoading] = useState(true);
    const [errorMessageList, setErrorMessageList] = useState(0);
    

    /*
    const isMountedRef = useRef(true);
    useEffect(() => {
        return () => {
            isMountedRef.current = false;
        };
    }, []); // never reset to true again (to avoid bugs in re-renders).

    const load = useCallback(async () => {
        setIsLoading(true);
        setErrorMessageList(null);
        try {
            const result = await fetchList(setList, setErrorMessageList, ()=>isMountedRef.current);
            if (!isMountedRef.current) {
                if (result) {
                    setErrorMessageList(null);
                }
            } 
        } catch (err) {
            if (!isMountedRef.current) 
                setErrorMessageList(err);
        } finally {
            if (!isMountedRef.current) 
                setIsLoading(false);
        }
    }, [fetchList]);

    useEffect(() => {
        load();
    }, [load]);

    const reload = () => {
        load();
    };*/

    const [incTrigger, setIncTrigger] = useState(0); // Changing this triggers refetch
    const reload = useCallback(() => {
        setIncTrigger(v => v + 1);
    }, []);

    useEffect(() => {
        // isMountCancelled prevent updating state on an unmounted component, which can cause warnings or memory leaks.
        // It is a common pattern to use a flag to track whether the component is still mounted when the async operation completes.
        // Otherwise you will get a warning in the console like "Can't perform a React state update on an unmounted component".
        let isMountCancelled = false;

        setIsLoading(true);
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
                    setIsLoading(false);
            });

        return () => {
            isMountCancelled = true; // mark as cancelled
        };
    }, [incTrigger, fetchList]);

    var crudTableProps = {
        list,
        setList,
        reload,
        isLoading,
        baseColumns,
        options
    };

    return {
        crudTableProps,
        reload,
        isLoading,
        errorMessageList
    };
}

export default useCrudTable;
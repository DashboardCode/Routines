import React, { useEffect, useState, useCallback, useMemo } from 'react';
import CrudTable from '@/tools/CrudTable';

import { fetchTokenized } from '@/fetchTokenized';
import DebugMenu from '@/tools/DebugMenu';
import ConnectionEditForm from './ConnectionEditForm';

import { z } from 'zod';

import useEditDialog from '@/tools/useEditDialog'
import useDeleteDialog from '@/tools/useDeleteDialog'

// ConnectionsManagement component - is not a pure component. it manages the state of all nested components.
// To define which nested components should be memoized
function ConnectionsManagement() {
    const [, setTick] = React.useState(0);
    const forceUpdate = () => setTick(tick => tick + 1);

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
    }, [incTrigger]);


    const [entity, setEntity] = useState(null); // selectable "component"

    const { editDialog, editAction, handleCreateButtonClick } = useEditDialog(
        {
            createDefaultEmpty: createDefaultEmpty,
            addForm: (isForNew, register, errors, dirtyFields) => <ConnectionEditForm
                    isForNew={isForNew}
                    register={register}
                    errors={errors}
                    dirtyFields={dirtyFields}
            />,
            saveButton_onClick: saveButton_onClick,
            connectionValidationSchema: connectionValidationSchema,
            fetchCreate,
            fetchReplace,
            setEntity, cloneEntity, entity, reload
        }
    )

    const { deleteDialog, deleteAction } = useDeleteDialog({
            okButton_onClick: okButton_onClick,
            fetchDelete,
            setEntity, cloneEntity, entity, reload
        }
    )

    const handleDetailsButtonClick = useCallback(() => {
        // TODO await? get id
        //var id = getId(entity);
        //fetch(`${ADMINKA_API_BASE_URL}/ui/connections/${id}`, { headers: { "Content-Type": "application/json" } });
    }, []);

    const rowActions = useMemo(() => ([
        editAction,
        deleteAction,
    ]), [editAction, deleteAction]);

    const multiSelectActionsMemo = useMemo(() => {
        var multiSelectActions = null;
        var isMultiSelectEdit = true;
        var isMultiSelectDelete = true;
        if (isMultiSelectEdit) {
            if (multiSelectActions == null)
                multiSelectActions = [];
            multiSelectActions.push({ handleButtonClick: () => { handleCreateButtonClick() /*TEST*/ }, buttonTitle: "Edit" });
        }
        if (isMultiSelectDelete) {
            if (multiSelectActions == null)
                multiSelectActions = [];
            multiSelectActions.push({ handleButtonClick: () => { handleCreateButtonClick() /*TEST*/ }, buttonTitle: "Delete" });
        }
        return multiSelectActions;
    }, [handleCreateButtonClick]
    );

    console.log(ConnectionsManagement.name + " render") // TODO: trace.render();
    return (
        <div>
            <DebugMenu actions=
                {[
                { name: "forceRerender", action: () => forceUpdate() },
                { name: "refreshData", action: () => reload() },
                { name: "Remove First Row", action: () => setList((l) => l.slice(1)) }
                ]} />
            <div className="my-4">
                <CrudTable
                    list={list}
                    errorMessage={errorMessageList}
                    isLoading={isLoadingList}
                    baseColumns={baseColumns}
                    multiSelectActions={multiSelectActionsMemo}
                    handleCreateButtonClick={handleCreateButtonClick}
                    handleDetailsButtonClick={handleDetailsButtonClick}
                    rowActions={rowActions}
                />
                {editDialog}
                {deleteDialog}
            </div>
            <br />
        </div>
    );
};

async function okButton_onClick(fetchDelete, entity, setErrorMessage, setIsDeleteDialogOpen, setIsLoading, reload) {
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

async function saveButton_onClick(isForNew, fetchCreate, fetchReplace, entity, setErrorMessage, setIsEditDialogOpen, setIsLoading, reload, trigger, getValues, dirtyFields) {
    try {
        setIsLoading(true);
        const isValid = await trigger(); // validate all fields
        if (isValid) {
            const hookFormState = getValues();
            var success;
            if (isForNew) {
                success = await fetchCreate(entity, hookFormState, setErrorMessage)
            } else {
                success = await fetchReplace(entity, hookFormState, setErrorMessage, dirtyFields)
            }
            if (success) {
                setIsEditDialogOpen(false);
                reload();
            }
        }
    } catch (err) {
        var message = (typeof err === 'string') ? err : (typeof err.message === 'string' ? err.message : String(err));
        setErrorMessage(message);
        console.error(err);
    } finally {
        setIsLoading(false);
    }
};

const connectionValidationSchema = z.object({
    excConnectionId: z.string()
        .max(8, "No more than 8 characters").regex(/^[0-9]+$/, {
            message: "Only letters and numbers are allowed",
        }).nonempty("Required"),
    excConnectionCode: z.string().min(4, "At least 4 characters")
        .max(8, "No more than 8 characters").regex(/^[a-zA-Z0-9]+$/, {
            message: "Only letters and numbers are allowed",
        }).nonempty("Required"),
    excConnectionName: z.string().min(4, "At least 4 characters"),
    excConnectionType: z.string().min(4, "At least 4 characters").regex(/^[a-zA-Z0-9]+$/, {
        message: "Only letters and numbers are allowed",
    }),
    excConnectionDescription: z.string().min(4, "At least 4 characters"),

    excConnectionXMeta: z.string().min(4, "At least 4 characters"),
    excConnectionString: z.string().min(4, "At least 4 characters").nonempty("Required")
});

function createDefaultEmpty() {
    return {
        excConnectionId: "",
        excConnectionCode: "",
        excConnectionName: "",
        excConnectionDescription: "",
        excConnectionXMeta: "",
        excConnectionType: "PARQUET",
        excConnectionString: "",
        excConnectionIsActive: false
    }
}

function cloneEntity(entity) {
    return {
        excConnectionId: entity.excConnectionId,
        excConnectionCode: entity.excConnectionCode, excConnectionName: entity.excConnectionName,
        excConnectionDescription: entity.excConnectionDescription, excConnectionXMeta: entity.excConnectionXMeta, excConnectionType: entity.excConnectionType,
        excConnectionString: entity.excConnectionString,
        excConnectionIsActive: entity.excConnectionIsActive
    }
}

const baseColumns =
    [
        { accessorKey: "excConnectionId", header: "excConnectionId", footer: "excConnectionId" },
        {
            // excConnectionCode, excConnectionName, excConnectionType
            accessorKey: "CodeNameType", header: (<div className="text-nowrap">Code | Name | Type</div>), footer: (<div className="text-nowrap">Code | Name | Type</div>),
            accessorFn: row => {
                return [
                    row.excConnectionCode,
                    row.excConnectionName,
                    row.excConnectionType,
                ].join(' ').toLowerCase(); // Join fields as one searchable string
            },
            cell: ({ row }) => {
                const rowData = row.original;
                const excConnectionCode = rowData.excConnectionCode; // boolean
                const excConnectionName = rowData.excConnectionName; // boolean
                const excConnectionType = rowData.excConnectionType; // boolean
                return (
                    <div>
                        <div ><span className="badge bg-dark me-2">code</span>{excConnectionCode}</div>
                        <div><span className="badge bg-dark me-2">name</span>{excConnectionName}</div>
                        <div><span className="badge bg-dark me-2">type</span>{excConnectionType}</div>
                    </div>
                );
            }
        },
        { accessorKey: "excConnectionDescription", header: "excConnectionDescription", footer: "excConnectionDescription" },
        { accessorKey: "excConnectionXMeta", header: "excConnectionXMeta", footer: "excConnectionXMeta" },
        { accessorKey: "excConnectionString", header: "excConnectionString", footer: "excConnectionString" },
        {
            accessorKey: "excConnectionIsActive", header: "Active", footer: "Active",
            cell: ({ getValue }) => {
                const value = getValue(); // boolean
                return (
                    <input
                        type="checkbox"
                        checked={value}
                        disabled
                    />
                );
            }
        }
    ];


async function fetchList(setList, setErrorMessageList) {
    
    const response = await fetchTokenized(`/ui/connections`);
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

async function fetchCreate(entity, formState, setErrorMessage) {
    var body = JSON.stringify(formState);
    console.log(body)
    var response = await fetchTokenized(`/ui/connections`, body, "POST");
    if (response.ok) {
        //await fetchList(setList, setIsLoading);
        return true
    } else {
        var result = await response.json();
        setErrorMessage(result.message);
        //Object.entries(result.errors).forEach(([field, message]) => {
        //    setError(field, { type: 'server', message });
        //});
        return false
    }  
}

async function fetchReplace(entity, formState, setErrorMessage, dirtyFields) {

    var hookFormDelta = getDelta(formState, dirtyFields)
    var delta = JSON.stringify(hookFormDelta);
    
    var response = await fetchTokenized(`/ui/connections/${entity.excConnectionId}`, delta, "PATCH");
    if (response.ok) {
        //await fetchList(setList, setIsLoading);
        return true;
    }
    else {
        var result = await response.json();
        setErrorMessage(result.message);
        //Object.entries(result.errors).forEach(([field, message]) => {
        //    setError(field, { type: 'server', message });
        //});
        return false
    }
}

async function fetchDelete(entity, setErrorMessage) {
    var responce = await fetchTokenized(`/ui/connections/${entity.excConnectionId}`, null, "DELETE");
    if (responce.ok) {
        return true; 
    } 
    else {
        var result = await responce.json();
        setErrorMessage(result.message);
        return false
    }
};

async function fetchBulkDelete(entities, setErrorMessage) {
    const ids = entities.map(e => e.excConnectionId);
    const jsonIds = JSON.stringify(ids);
    var responce = await fetchTokenized(`/ui/connections/bulkdelete`, jsonIds, "POST");
    if (responce.ok) {
        return true;
    }
    else {
        var result = await responce.json();
        setErrorMessage(result.message);
        return false
    }
};

function getDelta(allValues, dirtyFields){
    const delta = {};
    Object.keys(dirtyFields).forEach((key) => {
        delta[key] = allValues[key];
    });
    return delta;
};

ConnectionsManagement.whyDidYouRender = false;


export default ConnectionsManagement;
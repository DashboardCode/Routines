import React from 'react';
import CrudTableWithDialogs from '@/tools/CrudTableWithDialogs';

import { fetchTokenized } from '@/fetchTokenized';
import DebugMenu from '@/tools/DebugMenu';
import ConnectionEditForm from './ConnectionEditForm';

import { z } from 'zod';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';

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

function createDefaultEmpty(){
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

function cloneEntity(entity){
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


    

    //react-hook-form - state of the form with enabled zod validation
    const {
        register,
        trigger,
        getValues,
        reset, // reset form before reuse dialog for other entities
        setError /*set error for fields*/,
        formState: { errors, /*isValid,*/ dirtyFields } }
        = useForm(
            {
                resolver: zodResolver(connectionValidationSchema),
                mode: 'onTouched', /* alternatives onChange, onBlur, onTouched, onSubmit , all(onChange + onBlur + onSubmit) */
                defaultValues: createDefaultEmpty(),
            }); 

    var editForm = {
        fetchCreate: (entity, fState, setErrorMessageEdit) => fetchCreate(entity, fState, setErrorMessageEdit, setError),
        fetchReplace: (entity, fState, setErrorMessageEdit) => fetchReplace(entity, fState, setErrorMessageEdit, setError, dirtyFields),

        form: <ConnectionEditForm
            register={register}
            errors={errors}
        />,
    }


    //trigger = { trigger }
    //getValues = { getValues }
    //reset = { reset }
    //setError = { setError }
    //dirtyFields = { dirtyFields }

    var multiSelectDialogs = [
        //{
        //    key: 'user',
        //    label: 'Edit',
        //    form: <ConnectionEditForm ... />,
        //    onConfirm: handleUserSubmit,
        //}
    ];

    const contents = (<CrudTableWithDialogs
        baseColumns={baseColumns}
        fetchList={fetchList}
        fetchDelete={fetchDelete}
        fetchBulkDelete={fetchBulkDelete}
        trigger={trigger}
        getValues={getValues}
        reset={reset}
        createDefaultEmpty={createDefaultEmpty}
        cloneEntity={cloneEntity}
        editForm={editForm}
        multiSelectDialogs={multiSelectDialogs}
    />
    );

    console.log(ConnectionsManagement.name + " render") // TODO: trace.render();
    return (
        <div>
            <DebugMenu actions=
                {[
                    { name: "forceRerender", action: () => forceUpdate() }
                ]} />
            <div className="my-4">
                {contents}
            </div>
            <br />
        </div>
    );
};

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

async function fetchCreate(entity, formState, setErrorMessage /*, setError*/) {
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

async function fetchReplace(entity, formState, setErrorMessage, setError, dirtyFields) {

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

ConnectionsManagement.whyDidYouRender = true;


export default ConnectionsManagement;
import { useState } from 'react';
import CrudTableWithDialogs from '@/tools/CrudTableWithDialogs';
import { fetchTokenized } from '@/fetchTokenized';
import DebugMenu from '@/tools/DebugMenu';

import { useForm } from 'react-hook-form';
import { z } from 'zod';
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

    excConnectionXMeta:  z.string().min(4, "At least 4 characters"),
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

function ConnectionsManagement() {
    const [, setTick] = useState(0);
    const forceUpdate = () => setTick(tick => tick + 1);



    //const hookFormState = {
    //    excConnectionCode: register("excConnectionCode"),
    //    excConnectionName: register("excConnectionName"),
    //    excConnectionType: register("excConnectionType"),
    //    excConnectionDescription: register("excConnectionDescription"),
    //    excConnectionXMeta: register("excConnectionXMeta"),
    //    excConnectionString: register("excConnectionString")
    //};

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

    let baseColumns =
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

    // react-hook-form - state of the form with enabled zod validation
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
    
    // TODO: form validation (e.g. react-hook-form)
    var renderFormFields = (/*entity, setEntity*/) => {
        return (
            /*<div style={{ display: 'flex', flexWrap: 'wrap', gap: '2em' }}>*/
            <div>
                <div className="px-2 py-2">
                    <label className="form-label">ExcConnectionId</label>
                    <input
                        type="text"
                        className={`form-control ${errors.excConnectionId ? 'is-invalid' : ''}`}
                        //value={entity.excConnectionId}
                        //onChange={e => setEntity(prev => ({ ...prev, excConnectionId: e.target.value }))}
                        {...register('excConnectionId')}
                    />
                    {errors.excConnectionId && <div className="invalid-feedback">{errors.excConnectionId.message}</div>}
                </div>
                <div className="px-2 py-2">
                    <label className="form-label">ExcConnectionCode</label>
                    <input
                        type="text"
                        className={`form-control ${errors.excConnectionCode ? 'is-invalid' : ''}`}
                        //                    value={entity.excConnectionCode}
                        //                    onChange={e => setEntity(prev => ({ ...prev, excConnectionCode: e.target.value }))}
                        {...register('excConnectionCode')}
                    />
                    {errors.excConnectionCode && <div className="invalid-feedback">{errors.excConnectionCode.message}</div>}
                </div>
                <div className="px-2 py-2">
                    <label className="form-label">ExcConnectionName</label>
                    <input
                        type="text"
                        className={`form-control ${errors.excConnectionName ? 'is-invalid' : ''}`}
                        //value={entity.excConnectionName}
                        //onChange={e => setEntity(prev => ({ ...prev, excConnectionName: e.target.value }))}
                        {...register('excConnectionName')}
                    />
                    {errors.ExcConnectionName && <div className="invalid-feedback">{errors.excConnectionName.message}</div>}
                </div>

                <div className="px-2 py-2">
                    <label className="form-label">ExcConnectionDescription</label>
                    <input
                        type="text"
                        className={`form-control ${errors.excConnectionDescription ? 'is-invalid' : ''}`}
                        //value={entity.excConnectionDescription}
                        //onChange={e => setEntity(prev => ({ ...prev, excConnectionDescription: e.target.value }))}
                        {...register('excConnectionDescription')}
                    />
                    {errors.excConnectionDescription && <div className="invalid-feedback">{errors.excConnectionDescription.message}</div>}
                </div>
                <div className="px-2 py-2">
                    <label className="form-label">ExcConnectionXMeta</label>
                    <input
                        type="text"
                        className={`form-control ${errors.excConnectionXMeta ? 'is-invalid' : ''}`}
                        //value={entity.excConnectionXMeta}
                        //onChange={e => setEntity(prev => ({ ...prev, excConnectionXMeta: e.target.value }))}
                        {...register('excConnectionXMeta')}
                    />
                    {errors.excConnectionXMeta && <div className="invalid-feedback">{errors.excConnectionXMeta.message}</div>}
                </div>
                <div className="px-2 py-2">
                    <label className="form-label">ExcConnectionType</label>
                    <select
                        className={`form-control ${errors.excConnectionType ? 'is-invalid' : ''}`}
                        //value={entity.excConnectionType}
                        //onChange={e => setEntity(prev => ({ ...prev, excConnectionType: e.target.value }))}
                        {...register('excConnectionType')}
                    >
                        <option value="SqlServer">SqlServer</option>
                        <option value="PARQUET">PARQUET</option>
                    </select>
                    {errors.excConnectionType && <div className="invalid-feedback">{errors.excConnectionType.message}</div>}
                </div>

                <div className="px-2 py-2">
                    <label className="form-label">ExcConnectionString</label>
                    <input
                        type="text"
                        className={`form-control ${errors.excConnectionString ? 'is-invalid' : ''}`}
                        //value={entity.excConnectionString}
                        //onChange={e => setEntity(prev => ({ ...prev, excConnectionString: e.target.value }))}
                        {...register('excConnectionString')}
                    />
                    {errors.excConnectionString && <div className="invalid-feedback">{errors.excConnectionString.message}</div>}
                </div>
                <div className="px-2 py-2">
                    <div className="form-check">
                        <input
                            className={`form-check-input ${errors.excConnectionIsActive ? 'is-invalid' : ''}`}
                            type="checkbox"
                            //checked={entity.excConnectionIsActive}
                            id="flexCheckDefault"
                            //onChange={e => setEntity(prev => ({ ...prev, excConnectionIsActive: e.target.checked }))}
                            {...register('excConnectionIsActive')}
                        />
                        <label className="form-check-label" htmlFor="flexCheckDefault">
                            ExcConnectionIsActive
                        </label>
                    </div>
                </div>
            </div>
        )
    }

    const contents = <CrudTableWithDialogs
        baseColumns={baseColumns}
        fetchList={(setList, setErrorMessageList) => fetchList(setList, setErrorMessageList)}

        createDefaultEmpty={createDefaultEmpty}
        cloneEntity={cloneEntity}
        renderFormFields={renderFormFields}


        fetchCreate={(entity, data, setErrorMessageEdit) => fetchCreate(entity, setErrorMessageEdit, setError, data)}
        fetchReplace={(entity, data, setErrorMessageEdit) => fetchReplace(entity, setErrorMessageEdit, setError, data, dirtyFields)}
        fetchDelete={(entity, setErrorMessageDelete) => fetchDelete(entity, setErrorMessageDelete)}

        hookFormReset={reset}
        hookFormTrigger={trigger}
        hookFormGetValues={getValues}

        isMultiSelectEdit={true} 
        isMultiSelectDelete={true} 
        />;

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
}

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

async function fetchCreate(entity, setErrorMessageEdit, setError, hookFormData) {
    var body = JSON.stringify(hookFormData);
    console.log(body)
    var response = await fetchTokenized(`/ui/connections`, body, "POST");
    if (response.ok) {
        //await fetchList(setList, setIsLoading);
        return true
    } else {
        var result = await response.json();
        setErrorMessageEdit(result.message);
        //Object.entries(result.errors).forEach(([field, message]) => {
        //    setError(field, { type: 'server', message });
        //});
        return false
    }  
}

async function fetchReplace(entity, setErrorMessageEdit, setError, hookFormData, dirtyFields) {

    var hookFormDelta = getDelta(hookFormData, dirtyFields)
    var delta = JSON.stringify(hookFormDelta);
    
    var response = await fetchTokenized(`/ui/connections/${entity.excConnectionId}`, delta, "PATCH");
    if (response.ok) {
        //await fetchList(setList, setIsLoading);
        return true;
    }
    else {
        var result = await response.json();
        setErrorMessageEdit(result.message);
        //Object.entries(result.errors).forEach(([field, message]) => {
        //    setError(field, { type: 'server', message });
        //});
        return false
    }
}

async function fetchDelete(entity, setErrorMessageDelete) {
    var responce = await fetchTokenized(`/ui/connections/${entity.excConnectionId}`, null, "DELETE");
    if (responce.ok) {
  //      await fetchList(setList, setIsLoading);
        return true; 
    } 
    else {
        var result = await responce.json();
        setErrorMessageDelete(result.message);
        //Object.entries(result.errors).forEach(([field, message]) => {
        //    setError(field, { type: 'server', message });
        //});
        return false
    }
//    await fetchList(setList, setIsLoading);
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
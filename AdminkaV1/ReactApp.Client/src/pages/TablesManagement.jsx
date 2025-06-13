import { useEffect, useState } from 'react';
import CrudTableWithDialogs from '@/tools/CrudTableWithDialogs';
import { fetchTokenized } from '@/fetchTokenized';
import DebugMenu from '@/tools/DebugMenu';

import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';

const tablesValidationSchema = z.object({
    excTableId: z.string()
        .max(8, "No more than 8 characters").regex(/^[0-9]+$/, {
            message: "Only letters and numbers are allowed",
        }).nonempty("Required"),

    excTableDescription: z.string().min(4, "At least 4 characters"),
    excTableXMeta: z.string().min(4, "At least 4 characters"),
    excTablePath: z.string().min(4, "At least 4 characters"),
    excTableFields: z.string().min(4, "At least 4 characters"),

    excConnectionId: z.string().max(8, "No more than 8 characters").regex(/^[0-9]+$/, {
        message: "Only letters and numbers are allowed",
    }).nonempty("Required"), 
});

let createDefaultEmpty = () => {
    return {
        excTableId: "",
        excTableDescription: "",
        excTableXMeta: "",
        excTablePath: "",
        excTableFields: "",
        excConnectionId: "1"
    }
}

let cloneEntity = (entity) => {
    return {
        excTableId: entity.excTableId,
        excTableDescription: entity.excTableDescription,
        excTableXMeta: entity.excTableXMeta,
        excTablePath: entity.excTablePath,
        excTableFields: entity.excTableFields,
        excConnectionId: entity.excConnectionId
    }
}

function TablesManagement() {
    const [list, setList] = useState();
    const [isLoading, setIsLoading] = useState(true);
    const [reload, setReload] = useState(0); // Changing this triggers refetch

    //const [errors, setErrors] = useState({});

    const [errorMessageEdit, setErrorMessageEdit] = useState(0);
    const [errorMessageDelete, setErrorMessageDelete] = useState(0);


    // react-hook-form - state of the form with enabled zod validation
    const {
        register,
        trigger,
        getValues,
        reset, // reset form before reuse dialog for other entities 
        setError /*set error for fields*/,
        formState: { errors, isValid, dirtyFields } }
        = useForm(
            {
                resolver: zodResolver(tablesValidationSchema),
                mode: 'onTouched',
                defaultValues: createDefaultEmpty(),
            }); /* alternatives onChange, onBlur, onTouched, onSubmit , all(onChange + onBlur + onSubmit) */

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
    // UseState here, it is accesible in function's scope.
    useEffect(() => {
        fetchList(setList, setIsLoading); // setListData - recall the component render
    }, [reload]);

    useEffect(() => { }, [list]);

    let baseColumns =
        [
            { accessorKey: "excTableId", header: "excTableId", footer: "excTableId" },
            { accessorKey: "excTableDescription", header: "excTableDescription", footer: "excTableDescription" },
            { accessorKey: "excTableXMeta", header: "excTableXMeta", footer: "excTableXMeta" },
            { accessorKey: "excTablePath", header: "excTablePath", footer: "excTablePath" },
            { accessorKey: "excTableFields", header: "excTableFields", footer: "excTableFields" },
            { accessorKey: "excConnectionId", header: "excConnectionId", footer: "excConnectionId" }
        ];

    // TODO: form validation (e.g. react-hook-form)
    var renderFormFields = (entity, setEntity) => (
        /*<div style={{ display: 'flex', flexWrap: 'wrap', gap: '2em' }}>*/
        <div>
            <div className="px-2 py-2">
                <label className="form-label">ExcTableId</label>
                <input
                    type="text"
                    className="form-control"
                    //value={entity.excTableId}
                    //onChange={e => setEntity(prev => ({ ...prev, excTableId: e.target.value }))}
                    {...register('excTableId')}
                />
                {errors.excTableId && <div className="invalid-feedback">{errors.excTableId.message}</div>}
            </div>
            <div className="px-2 py-2">
                <label className="form-label">ExcTableDescription</label>
                <input
                    type="text"
                    className="form-control"
                    //value={entity.excTableDescription}
                    //onChange={e => setEntity(prev => ({ ...prev, excTableDescription: e.target.value }))}
                    {...register('excTableDescription')}
                />
                {errors.excTableDescription && <div className="invalid-feedback">{errors.excTableDescription.message}</div>}
            </div>
            <div className="px-2 py-2">
                <label className="form-label">ExcTableXMeta</label>
                <input
                    type="text"
                    className="form-control"
                    //value={entity.excTableXMeta}
                    //onChange={e => setEntity(prev => ({ ...prev, excTableXMeta: e.target.value }))}
                    {...register('excTableXMeta')}
                />
                {errors.excTableXMeta && <div className="invalid-feedback">{errors.excTableXMeta.message}</div>}
            </div>

            <div className="px-2 py-2">
                <label className="form-label">ExcTablePath</label>
                <input
                    type="text"
                    className="form-control"
                    //value={entity.excTablePath}
                    //onChange={e => setEntity(prev => ({ ...prev, excTablePath: e.target.value }))}
                    {...register('excTablePath')}
                />
                {errors.excTablePath && <div className="invalid-feedback">{errors.excTablePath.message}</div>}
            </div>
            <div className="px-2 py-2">
                <label className="form-label">ExcTableFields</label>
                <input
                    type="text"
                    className="form-control"
                    //value={entity.excTableFields}
                    //onChange={e => setEntity(prev => ({ ...prev, excTableFields: e.target.value }))}
                    {...register('excTableFields')}
                />
                {errors.excTableFields && <div className="invalid-feedback">{errors.excTableFields.message}</div>}
            </div>
            <div className="px-2 py-2">
                <label className="form-label">ExcConnectionId</label>
                <input
                    type="text"
                    className="form-control"
                    //value={entity.excConnectionId}
                    //onChange={e => setEntity(prev => ({ ...prev, excConnectionId: e.target.value }))}
                    {...register('excConnectionId')}
                />
                {errors.excConnectionId && <div className="invalid-feedback">{errors.excConnectionId.message}</div>}
            </div>
        </div>
    )

    
    const contents = isLoading
        ? <p><em>Loading... </em></p>
        : <CrudTableWithDialogs list={list}
            setList={setList}
            createDefaultEmpty={createDefaultEmpty}
            cloneEntity={cloneEntity}
            isLoading={isLoading}
            baseColumns={baseColumns}
            setIsLoading={setIsLoading}
            renderFormFields={renderFormFields}

            fetchCreate={(entity, data) => fetchCreate(entity, setErrorMessageEdit, setError, setList, setIsLoading, data)}
            fetchReplace={(entity, data) => fetchReplace(entity, setErrorMessageEdit, setError, setList, setIsLoading, data, dirtyFields)}
            fetchDelete={(entity) => fetchDelete(entity, setErrorMessageDelete, setList, setIsLoading)}
            //fetchDetails={null/*(id) => fetch(`${ADMINKA_API_BASE_URL}/ui/connections/${id}`, { headers: { "Content-Type": "application/json" } })*/}

            //multiSelectActions={null}
            errorMessageEdit={errorMessageEdit}
            setErrorMessageEdit={setErrorMessageEdit}
            errorMessageDelete={errorMessageDelete}
            setErrorMessageDelete={setErrorMessageDelete}
            hookFormReset={reset}
            hookFormTrigger={trigger}
            hookFormGetValues={getValues}
        />;

    return (
        <div>
            {
                //console.log("render " + ConnectionsManagement.name)
            }
            <DebugMenu actions=
                {[
                    { name: "refreshData", action: () => setReload((prev) => prev + 1) }
                ]} />
            <div className="my-4">
                {contents}
            </div>
            <br />
        </div>
    );
}

async function fetchList(setList, setIsLoading) {
    setIsLoading(true);
    const response = await fetchTokenized(`/ui/exctables`);
    if (response.ok) {
        const odata = await response.json();
        setList(odata.value);
    }
    setIsLoading(false);
}

async function fetchCreate(entity, setErrorMessageEdit, setError, setList, setIsLoading, hookFormData) {
    var body = JSON.stringify(hookFormData);
    console.log(body)
    var responce = await fetchTokenized(`/ui/exctables`, body, "POST");
    if (responce.ok) {
        await fetchList(setList, setIsLoading);
        return true
    } else {
        var result = await responce.json();
        setErrorMessageEdit(result.message);
        //Object.entries(result.errors).forEach(([field, message]) => {
        //    setError(field, { type: 'server', message });
        //});
        return false
    }
}

async function fetchReplace(entity, setErrorMessageEdit, setError, setList, setIsLoading, hookFormData, dirtyFields) {

    var hookFormDelta = getDelta(hookFormData, dirtyFields)
    var delta = JSON.stringify(hookFormDelta);

    var responce = await fetchTokenized(`/ui/exctables/${entity.excConnectionId}`, delta, "PATCH");
    if (responce.ok) {
        await fetchList(setList, setIsLoading);
        return true;
    }
    else {
        var result = await responce.json();
        setErrorMessageEdit(result.message);
        //Object.entries(result.errors).forEach(([field, message]) => {
        //    setError(field, { type: 'server', message });
        //});
        return false
    }
}

async function fetchDelete(entity, setErrorMessageDelete, setList, setIsLoading) {
    await fetchTokenized(`/ui/exctables/${entity.excConnectionId}`, null, "DELETE");
    await fetchList(setList, setIsLoading);
};

function getDelta(allValues, dirtyFields) {
    const delta = {};
    Object.keys(dirtyFields).forEach((key) => {
        delta[key] = allValues[key];
    });
    return delta;
};

export default TablesManagement;

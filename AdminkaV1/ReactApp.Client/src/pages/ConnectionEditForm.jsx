////import { useState } from 'react';
////import CrudTableWithDialogs from '@/tools/CrudTableWithDialogs';
////import { fetchTokenized } from '@/fetchTokenized';
////import DebugMenu from '@/tools/DebugMenu';

//import { useForm } from 'react-hook-form';
//import { z } from 'zod';
//import { zodResolver } from '@hookform/resolvers/zod';

//const connectionValidationSchema = z.object({
//    excConnectionId: z.string()
//        .max(8, "No more than 8 characters").regex(/^[0-9]+$/, {
//            message: "Only letters and numbers are allowed",
//        }).nonempty("Required"),
//    excConnectionCode: z.string().min(4, "At least 4 characters")
//        .max(8, "No more than 8 characters").regex(/^[a-zA-Z0-9]+$/, {
//            message: "Only letters and numbers are allowed",
//        }).nonempty("Required"),
//    excConnectionName: z.string().min(4, "At least 4 characters"),
//    excConnectionType: z.string().min(4, "At least 4 characters").regex(/^[a-zA-Z0-9]+$/, {
//        message: "Only letters and numbers are allowed",
//    }),
//    excConnectionDescription: z.string().min(4, "At least 4 characters"),

//    excConnectionXMeta: z.string().min(4, "At least 4 characters"),
//    excConnectionString: z.string().min(4, "At least 4 characters").nonempty("Required")
//});
//function ConnectionEditForm({
//    createDefaultEmpty
//}) {
//    // react-hook-form - state of the form with enabled zod validation
//    const {
//        register,
//        trigger,
//        getValues,
//        reset, // reset form before reuse dialog for other entities 
//        setError /*set error for fields*/,
//        formState: { errors, /*isValid,*/ dirtyFields } }
//        = useForm(
//            {
//                resolver: zodResolver(connectionValidationSchema),
//                mode: 'onTouched', /* alternatives onChange, onBlur, onTouched, onSubmit , all(onChange + onBlur + onSubmit) */
//                defaultValues: createDefaultEmpty(),
//            }); 

//    return (
//        /*<div style={{ display: 'flex', flexWrap: 'wrap', gap: '2em' }}>*/
//        <div>
//            <div className="px-2 py-2">
//                <label className="form-label">ExcConnectionId</label>
//                <input
//                    type="text"
//                    className={`form-control ${errors.excConnectionId ? 'is-invalid' : ''}`}
//                    //value={entity.excConnectionId}
//                    //onChange={e => setEntity(prev => ({ ...prev, excConnectionId: e.target.value }))}
//                    {...register('excConnectionId')}
//                />
//                {errors.excConnectionId && <div className="invalid-feedback">{errors.excConnectionId.message}</div>}
//            </div>
//            <div className="px-2 py-2">
//                <label className="form-label">ExcConnectionCode</label>
//                <input
//                    type="text"
//                    className={`form-control ${errors.excConnectionCode ? 'is-invalid' : ''}`}
//                    //                    value={entity.excConnectionCode}
//                    //                    onChange={e => setEntity(prev => ({ ...prev, excConnectionCode: e.target.value }))}
//                    {...register('excConnectionCode')}
//                />
//                {errors.excConnectionCode && <div className="invalid-feedback">{errors.excConnectionCode.message}</div>}
//            </div>
//            <div className="px-2 py-2">
//                <label className="form-label">ExcConnectionName</label>
//                <input
//                    type="text"
//                    className={`form-control ${errors.excConnectionName ? 'is-invalid' : ''}`}
//                    //value={entity.excConnectionName}
//                    //onChange={e => setEntity(prev => ({ ...prev, excConnectionName: e.target.value }))}
//                    {...register('excConnectionName')}
//                />
//                {errors.ExcConnectionName && <div className="invalid-feedback">{errors.excConnectionName.message}</div>}
//            </div>

//            <div className="px-2 py-2">
//                <label className="form-label">ExcConnectionDescription</label>
//                <input
//                    type="text"
//                    className={`form-control ${errors.excConnectionDescription ? 'is-invalid' : ''}`}
//                    //value={entity.excConnectionDescription}
//                    //onChange={e => setEntity(prev => ({ ...prev, excConnectionDescription: e.target.value }))}
//                    {...register('excConnectionDescription')}
//                />
//                {errors.excConnectionDescription && <div className="invalid-feedback">{errors.excConnectionDescription.message}</div>}
//            </div>
//            <div className="px-2 py-2">
//                <label className="form-label">ExcConnectionXMeta</label>
//                <input
//                    type="text"
//                    className={`form-control ${errors.excConnectionXMeta ? 'is-invalid' : ''}`}
//                    //value={entity.excConnectionXMeta}
//                    //onChange={e => setEntity(prev => ({ ...prev, excConnectionXMeta: e.target.value }))}
//                    {...register('excConnectionXMeta')}
//                />
//                {errors.excConnectionXMeta && <div className="invalid-feedback">{errors.excConnectionXMeta.message}</div>}
//            </div>
//            <div className="px-2 py-2">
//                <label className="form-label">ExcConnectionType</label>
//                <select
//                    className={`form-control ${errors.excConnectionType ? 'is-invalid' : ''}`}
//                    //value={entity.excConnectionType}
//                    //onChange={e => setEntity(prev => ({ ...prev, excConnectionType: e.target.value }))}
//                    {...register('excConnectionType')}
//                >
//                    <option value="SqlServer">SqlServer</option>
//                    <option value="PARQUET">PARQUET</option>
//                </select>
//                {errors.excConnectionType && <div className="invalid-feedback">{errors.excConnectionType.message}</div>}
//            </div>

//            <div className="px-2 py-2">
//                <label className="form-label">ExcConnectionString</label>
//                <input
//                    type="text"
//                    className={`form-control ${errors.excConnectionString ? 'is-invalid' : ''}`}
//                    //value={entity.excConnectionString}
//                    //onChange={e => setEntity(prev => ({ ...prev, excConnectionString: e.target.value }))}
//                    {...register('excConnectionString')}
//                />
//                {errors.excConnectionString && <div className="invalid-feedback">{errors.excConnectionString.message}</div>}
//            </div>
//            <div className="px-2 py-2">
//                <div className="form-check">
//                    <input
//                        className={`form-check-input ${errors.excConnectionIsActive ? 'is-invalid' : ''}`}
//                        type="checkbox"
//                        //checked={entity.excConnectionIsActive}
//                        id="flexCheckDefault"
//                        //onChange={e => setEntity(prev => ({ ...prev, excConnectionIsActive: e.target.checked }))}
//                        {...register('excConnectionIsActive')}
//                    />
//                    <label className="form-check-label" htmlFor="flexCheckDefault">
//                        ExcConnectionIsActive
//                    </label>
//                </div>
//            </div>
//        </div>
//    )
//}

//export default ConnectionEditForm;
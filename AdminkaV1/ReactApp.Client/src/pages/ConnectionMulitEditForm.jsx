import PropTypes from "prop-types";

import React, { useState, useMemo } from 'react';

import { Form, Row, Col } from 'react-bootstrap';

import { useForm } from 'react-hook-form';

import { zodResolver } from '@hookform/resolvers/zod';
import { z, ZodObject } from 'zod';



// Get shared values across all entities
function getCommonFields(entities) {
    const keys = Object.keys(entities[0]);
    const result = {};
    for (const key of keys) {
        const first = entities[0][key];
        if (entities.every((e) => e[key] === first)) {
            result[key] = first;
        } else {
            result[key] = '';
        }
    }
    return result;
}

// Dynamic schema factory
function schemaFactory(enabledFields) {
    const shape = {};

    if (enabledFields.name) {
        shape.name = z
            .string()
            .min(2, 'Name must be at least 2 characters');
    }

    if (enabledFields.email) {
        shape.email = z
            .string()
            .email('Invalid email address');
    }

    if (enabledFields.role) {
        shape.role = z
            .string()
            .min(1, 'Role is required')
            .refine((val) => ['admin', 'manager', 'user'].includes(val), {
                message: 'Invalid role',
            });
    }

    return z.object(shape);
}

const useConnectionMultiEditForm =()=>{
    const defaultValues = useMemo(() => getCommonFields(users), []);

    const [enabledFields, setEnabledFields] = useState({
        excConnectionId: false,
        excConnectionCode: false,
        excConnectionName: false,
    });

    // Rebuild schema based on enabled fields
    const schema = useMemo(() => schemaFactory(enabledFields), [enabledFields]);

    const {
        register,
        formState: { errors, dirtyFields },
        watch,
    } = useForm({
        defaultValues,
        resolver: zodResolver(schema),
        mode: 'onTouched',
    });

    const getDelta = (values) => {
        const delta = {};
        for (const key in enabledFields) {
            if (enabledFields[key] && dirtyFields[key]) {
                delta[key] = values[key];
            }
        }
        console.log('Changed fields (delta):', delta);
    };

    var connectionMultiEditFormProps = {
        register, errors, dirtyFields, enabledFields, setEnabledFields 
    };

    return {
        connectionMultiEditFormProps,
        getDelta,
        watch
    };
}

// this form bad for memoization. 'errors' changes on every render, so the useMemo just adds the overhead
const ConnectionMultiEditForm =/*React.memo(*/({ register, errors, dirtyFields, enabledFields, setEnabledFields }) => {
    console.log("ConnectionEditForm render")

    const fieldConfigs = [
        {
            name: 'excConnectionId',
            label: 'excConnectionId',
        },
        {
            name: 'excConnectionCode',
            label: 'excConnectionCode',
        },
        {
            name: 'excConnectionName',
            label: 'excConnectionName',
        },
    ];
    return (
        <Row>
            <Col md={ 3}>
            <h5 className="mb-3">Enable Fields</h5>
            {fieldConfigs.map(({ name, label }) => (
                <Form.Check
                    key={name}
                    type="checkbox"
                    label={`Edit ${label}`}
                    checked={enabledFields[name]}
                    onChange={() =>
                        setEnabledFields((prev) => ({
                            ...prev,
                            [name]: !prev[name],
                        }))
                    }
                    className="mb-2"
                />
            ))}
            </Col>
            <Col md={9}>
        {/*<div style={{ display: 'flex', flexWrap: 'wrap', gap: '2em' }}></div>*/}
        <div>
            <div className="px-2 py-2">
                <label className="form-label">ExcConnectionId {dirtyFields.excConnectionId == true ? <b>(changed)</b> : ""}</label>
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
                <label className="form-label">ExcConnectionCode {dirtyFields.excConnectionCode == true ? <b>(changed)</b> : ""}</label>
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
                <label className="form-label">ExcConnectionName {dirtyFields.excConnectionName == true ? <b>(changed)</b> : ""}</label>
                {console.log(errors.excConnectionName)}
                <input
                    type="text"
                    className={`form-control ${errors.excConnectionName ? 'is-invalid' : ''}`}
                    //value={entity.excConnectionName}
                    //onChange={e => setEntity(prev => ({ ...prev, excConnectionName: e.target.value }))}
                    {...register('excConnectionName')}
                />
                {errors.excConnectionName && <div className="invalid-feedback">{errors.excConnectionName.message}</div>}
            </div>

            <div className="px-2 py-2">
                <label className="form-label">ExcConnectionDescription {dirtyFields.excConnectionDescription == true ? <b>(changed)</b> : ""}</label>
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
                <label className="form-label">ExcConnectionXMeta {dirtyFields.excConnectionXMeta == true ? <b>(changed)</b> : ""}</label>
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
                <label className="form-label">ExcConnectionType {dirtyFields.excConnectionType == true ? <b>(changed)</b> : ""}</label>
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
                <label className="form-label">ExcConnectionString {dirtyFields.excConnectionString == true ? <b>(changed)</b> : ""}</label>
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
                        ExcConnectionIsActive {dirtyFields.excConnectionIsActive == true ? <b>(changed)</b> : ""}
                    </label>
                </div>
            </div>
                </div>
            </Col>
        </Row>
    )
}

ConnectionMultiEditForm.displayName = "ConnectionEditForm";

ConnectionMultiEditForm.propTypes = {
    register: PropTypes.func,
    errors: PropTypes.object,
    dirtyFields: PropTypes.object
};

export default ConnectionMultiEditForm;
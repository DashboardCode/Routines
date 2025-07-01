import PropTypes from "prop-types";

// this form bad for memoization. 'errors' changes on every render, so the useMemo just adds the overhead
const TableEditForm =/*React.memo(*/({ formState }) => {
    const { register, errors, dirtyFields } = formState;
    console.log("ConnectionEditForm render")
    return (
        /*<div style={{ display: 'flex', flexWrap: 'wrap', gap: '2em' }}>*/
        <div>
            <div className="px-2 py-2">
                <label className="form-label">ExcTableId {dirtyFields.excTableId == true ? <b>(changed)</b> : ""}</label>
                <input
                    type="text"
                    className={`form-control ${errors.excTableId ? 'is-invalid' : ''}`}
                    //value={entity.excTableId}
                    //onChange={e => setEntity(prev => ({ ...prev, excTableId: e.target.value }))}
                    {...register('excTableId')}
                />
                {errors.excTableId && <div className="invalid-feedback">{errors.excTableId.message}</div>}
            </div>
            <div className="px-2 py-2">
                <label className="form-label">ExcTableDescription {dirtyFields.excTableDescription == true ? <b>(changed)</b> : ""}</label>
                <input
                    type="text"
                    className={`form-control ${errors.excTableDescription ? 'is-invalid' : ''}`}
                    //value={entity.excTableDescription}
                    //onChange={e => setEntity(prev => ({ ...prev, excTableDescription: e.target.value }))}
                    {...register('excTableDescription')}
                />
                {errors.excTableDescription && <div className="invalid-feedback">{errors.excTableDescription.message}</div>}
            </div>
            <div className="px-2 py-2">
                <label className="form-label">ExcTableXMeta {dirtyFields.excTableXMeta == true ? <b>(changed)</b> : ""}</label>
                <input
                    type="text"
                    className={`form-control ${errors.excTableXMeta ? 'is-invalid' : ''}`}
                    //value={entity.excTableXMeta}
                    //onChange={e => setEntity(prev => ({ ...prev, excTableXMeta: e.target.value }))}
                    {...register('excTableXMeta')}
                />
                {errors.excTableXMeta && <div className="invalid-feedback">{errors.excTableXMeta.message}</div>}
            </div>

            <div className="px-2 py-2">
                <label className="form-label">ExcTablePath {dirtyFields.excTablePath == true ? <b>(changed)</b> : ""}</label>
                <input
                    type="text"
                    className={`form-control ${errors.excTablePath ? 'is-invalid' : ''}`}
                    //value={entity.excTablePath}
                    //onChange={e => setEntity(prev => ({ ...prev, excTablePath: e.target.value }))}
                    {...register('excTablePath')}
                />
                {errors.excTablePath && <div className="invalid-feedback">{errors.excTablePath.message}</div>}
            </div>
            <div className="px-2 py-2">
                <label className="form-label">ExcTableFields {dirtyFields.excTableFields == true ? <b>(changed)</b> : ""}</label>
                <input
                    type="text"
                    className={`form-control ${errors.excTableFields ? 'is-invalid' : ''}`}
                    //value={entity.excTableFields}
                    //onChange={e => setEntity(prev => ({ ...prev, excTableFields: e.target.value }))}
                    {...register('excTableFields')}
                />
                {errors.excTableFields && <div className="invalid-feedback">{errors.excTableFields.message}</div>}
            </div>
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
        </div>
    )
}/*)*/

TableEditForm.displayName = "TableEditForm";

TableEditForm.propTypes = {
    formState: PropTypes.shape({
        register: PropTypes.func,
        errors: PropTypes.object,
        dirtyFields: PropTypes.object
    })
};

export default TableEditForm;
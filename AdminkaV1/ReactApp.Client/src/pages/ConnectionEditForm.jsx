import PropTypes from "prop-types";

// this form bad for memoization. 'errors' changes on every render, so the useMemo just adds the overhead
const ConnectionEditForm =/*React.memo(*/({ formState }) => {
    const { register, errors, dirtyFields } = formState;
    console.log("ConnectionEditForm render")
    return (
        /*<div style={{ display: 'flex', flexWrap: 'wrap', gap: '2em' }}>*/
        <div>
            <div className="px-2 py-2">
                <label className="form-label">ExcConnectionId {dirtyFields.excConnectionId==true?<b>(changed)</b>:"" }</label>
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
    )
}/*)*/

ConnectionEditForm.displayName = "ConnectionEditForm";

ConnectionEditForm.propTypes = {
    formState: PropTypes.shape({
        register: PropTypes.func,
        errors: PropTypes.object,
        dirtyFields: PropTypes.object
    })
};

export default ConnectionEditForm;
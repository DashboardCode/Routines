import { useState } from 'react';
import DebugMenu from './DebugMenu';
import PropTypes from "prop-types";

function FormInputText({ inputValue, inputId, inputName, labelText, onChange, isRequred }) {
    //const [setInputValue] = useState(inputValue);
    
    const [setReload] = useState(0); // For Debug purpose

    //const handleChange = (e) => {
    //    const { name, value } = e.target;
    //    setInputValue((prev) => ({
    //        ...prev,
    //        [name]: value,
    //    }));
    //};

    return (
        <div className="form-group row">
            <DebugMenu actions=
                {[
                    { name: "refreshData", action: () => setReload((prev) => prev + 1) }
                ]} />
                <label className="form-control" htmlFor={inputId}>{labelText}</label>
                <div className="col-sm-10">
                <input
                    id={inputId}
                    type="text"
                    className="form-control"
                    name={inputName}
                    value={inputValue}
                    onChange={onChange}//{handleChange}
                    required={isRequred?true:false}
                    />
                    <span className="text-danger"></span>
                </div>
        </div>
    );
}

FormInputText.propTypes = {
    inputValue: PropTypes.string,
    inputId: PropTypes.string,
    inputName: PropTypes.string,
    labelText: PropTypes.string,
    onChange: PropTypes.func,
    isRequred: PropTypes.bool
};

export default FormInputText;
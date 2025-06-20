import { useEffect, useRef } from 'react';
import PropTypes from "prop-types";
import Form from 'react-bootstrap/Form';

function IndeterminateCheckbox({ checked, indeterminate, onChange, label }) {
    const ref = useRef(null);

    useEffect(() => {
        if (ref.current) {
            ref.current.indeterminate = indeterminate;
        }
    }, [indeterminate]);

    return (
        <Form.Check
            type="checkbox"
            ref={ref}
            checked={checked}
            onChange={onChange}
            label={<div className="badge bg-secondary">{label}</div>}
        />
    );
}

IndeterminateCheckbox.propTypes = {
    checked: PropTypes.bool,
    label: PropTypes.string,
    indeterminate: PropTypes.bool,
    onChange: PropTypes.func
};

export default IndeterminateCheckbox;
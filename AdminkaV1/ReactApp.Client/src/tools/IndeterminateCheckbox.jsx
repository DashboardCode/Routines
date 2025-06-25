import React, { useEffect, useRef, useMemo } from 'react';
import PropTypes from "prop-types";
import Form from 'react-bootstrap/Form';

// This component uses useRef and useEffect and still considered as a pure component and cabe safely memoized
// 1) Renders same output for same props
// 2) useEffect do not do any side effect
// 3) no global mutations
const IndeterminateCheckbox = React.memo(({ checked, indeterminate, onChange, label }) => {
    const ref = useRef(null);

    useEffect(() => {
        if (ref.current) {
            // indermediate is not standard HTML attribute, so we need to set it manually (and then bootstrap js changes it visually)
            ref.current.indeterminate = indeterminate; 
        }
    }, [indeterminate]);

    const wrappedLabel = useMemo(() => label ? <span className="ms-2">{label}</span> : null, [label]) 

    return (
        <Form.Check
            type="checkbox"
            ref={ref}
            checked={checked}
            onChange={onChange}
            label={wrappedLabel}
        />
    );
})

IndeterminateCheckbox.displayName = "IndeterminateCheckbox";

IndeterminateCheckbox.propTypes = {
    checked: PropTypes.bool,
    label: PropTypes.string,
    indeterminate: PropTypes.bool,
    onChange: PropTypes.func
};

export default IndeterminateCheckbox;
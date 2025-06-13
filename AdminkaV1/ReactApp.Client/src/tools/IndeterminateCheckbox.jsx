import { useEffect, useRef } from 'react';
import PropTypes from "prop-types";

function IndeterminateCheckbox({
    indeterminate,
    className = '',
    ...rest
}) {
    const ref = useRef(null);

    useEffect(() => {
        if (typeof indeterminate === 'boolean') {
            ref.current.indeterminate = !rest.checked && indeterminate
        }
    }, [ref, indeterminate, rest])

    return (
        <input
            type="checkbox"
            ref={ref}
            className={className + ' cursor-pointer'}
            {...rest}
        />
    )
}

IndeterminateCheckbox.propTypes = {
    indeterminate: PropTypes.bool,
    className: PropTypes.string,
    getRowSelectionLength: PropTypes.func,
    getRowTotalLength: PropTypes.func
};

export default IndeterminateCheckbox;
import { useState, useEffect } from 'react';
import PropTypes from "prop-types";

function Selectable({ createDefaultEmpty, cloneEntity, setSelectableApi, children }) {
    const [entity, setEntity] = useState("");

    useEffect(() => {
        setSelectableApi({
            setEntity: (e) => { var copy = cloneEntity(e); setEntity(copy); return copy; },
            resetEntity: () => { var e = createDefaultEmpty(); setEntity(e); return e; },
            getEntity: () => entity
        });
    }, [createDefaultEmpty, cloneEntity, setSelectableApi, entity]);

    return (
        {children}
    );
}

Selectable.propTypes = {
    createDefaultEmpty: PropTypes.func,
    cloneEntity: PropTypes.func,
    setSelectableApi: PropTypes.func,
    children: PropTypes.element
};

export default Selectable;
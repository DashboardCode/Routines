import PropTypes from "prop-types";

import BsRibbonGroup from './BsRibbonGroup';
import BsRibbonGroupItem  from './BsRibbonGroupItem';
import BsRibbonButton from './BsRibbonButton';

function BsRibbon(props){
    return (
        <div className="d-flex align-items-center border p-1 bg-light" style={{ height: props.height }}>
            {props.children}
        </div>
    );
};

BsRibbon.propTypes = {
    height: PropTypes.string,
    breakpoint: PropTypes.oneOf(["sm", "md", "lg", "xl", "xxl"]),
    children: PropTypes.node
};

export { BsRibbon, BsRibbonGroup, BsRibbonGroupItem, BsRibbonButton };
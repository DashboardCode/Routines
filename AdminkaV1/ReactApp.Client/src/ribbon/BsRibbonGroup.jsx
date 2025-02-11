import PropTypes from "prop-types";

const BsRibbonGroup = (props) => {
    return (
        <div className={props.className}>
            {props.children}
        </div>
    )
};

BsRibbonGroup.propTypes = {
    className: PropTypes.string,
    title: PropTypes.string,
    children: PropTypes.node
};

export default BsRibbonGroup;
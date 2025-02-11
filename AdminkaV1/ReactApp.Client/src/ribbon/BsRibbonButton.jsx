import PropTypes from "prop-types";

const BsRibbonButton = (props) => {
    return (
        <button type="button" className="btn btn-light btn-block text-nowrap" {...props}>
            {props.children}
        </button>
    );
};

BsRibbonButton.propTypes = {
    children: PropTypes.node
};

export default BsRibbonButton;
import PropTypes from "prop-types";

const BsRibbonGroupItem = (props) => {
    return (
        <div>
            {props.children}
        </div>
    );
};

BsRibbonGroupItem.propTypes = {
    colClass: PropTypes.string,
    children: PropTypes.node
};

export default BsRibbonGroupItem;
import { useState, useRef, useEffect } from "react";
import PropTypes from "prop-types";
import './DebugMenu.css';

const DebugMenu = (props) => {
    const [isOpen, setIsOpen] = useState(false);
    const menuRef = useRef(null);
    // Toggle menu open/close
    const toggleMenu = () => setIsOpen((prev) => !prev);

    // Close menu when clicking outside
    useEffect(() => {
        const handleClickOutside = (event) => {
            if (menuRef.current && !menuRef.current.contains(event.target)) {
                setIsOpen(false);
            }
        };
        document.addEventListener("mousedown", handleClickOutside);
        return () => document.removeEventListener("mousedown", handleClickOutside);
    }, []);

    return (
        <span className="debug-menu">
            {/* Small Button to Open Menu */}
            <button
                onClick={toggleMenu}
                className="debug-menu-button rounded"
            ></button>

            {/* Popup Menu */}
            {isOpen && props!=null && props.actions!=null && (
                <div
                    ref={menuRef}
                    className="debug-menu-popup absolute right-0 mt-2 w-40 bg-white border rounded-lg shadow-lg"
                >
                    {props.actions.map((a, index) =>
                        (<button key={index} className="debug-menu-popup-button" title={a.name} onClick={a.action} >{a.name}</button>)
                        )}
                </div>
            )}
        </span>
    );
};

DebugMenu.propTypes = {
    actions: PropTypes.arrayOf(PropTypes.shape({
        name: PropTypes.string.isRequired,
        acion: PropTypes.func
    })).isRequired
};

export default DebugMenu;


import { ISDEVDEBUG } from '@/config';
import { useState, useRef, useEffect } from "react";
import PropTypes from "prop-types";
import './DebugMenu.css';

const DebugMenu = ({actions}) => {
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

    return ( ISDEVDEBUG && 
        <div className="debug-menu">
            {/* Small Button to Open Menu */}
            <button
                onClick={toggleMenu}
                className="debug-menu-button rounded"
            ></button>

            {/* Popup Menu */}
            {isOpen && actions!=null && (
                <div
                    ref={menuRef}
                    className="debug-menu-popup "
                >
                    <div className="debug-menu-popup-panel">
                    {actions.map((a, index) =>
                        (<button key={index} className="debug-menu-popup-button bg-white border" title={a.name} onClick={a.action} >{a.name}</button>)
                        )}
                    </div>
                </div>
            )}
        </div>
    );
};

DebugMenu.propTypes = {
    actions: PropTypes.arrayOf(PropTypes.shape({
        name: PropTypes.string.isRequired,
        acion: PropTypes.func
    })).isRequired
};

export default DebugMenu;


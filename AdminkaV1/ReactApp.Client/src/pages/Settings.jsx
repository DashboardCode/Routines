//import React from 'react';
import { Link, Routes, Route, Navigate } from 'react-router-dom';
import SettingsPage1 from './SettingsPage1';
import SettingsPage2 from './SettingsPage2';

const Settings = () => (
    <div style={styles.container}>
        {
            console.log(Settings.name + " render.content")  // TODO: trace.renderContent();
        }
                <nav style={styles.sidebar}>
                    <h2 style={styles.logo}>Settings</h2>
                    <ul style={styles.menu}>
                <li><Link to="/settings/SettingsPage1" >SettingsPage1</Link></li>
                <li><Link to="/settings/SettingsPage2" >SettingsPage2</Link></li>
                    </ul>
                </nav>
                <main style={styles.content}>
                    <Routes>
                <Route path="/" element={<Navigate to="/settings/SettingsPage1" replace />} />
                        <Route path="SettingsPage1" element={<SettingsPage1 />} />
                        <Route path="SettingsPage2" element={<SettingsPage2 />} />
                        
                    </Routes>
                </main>
            </div>
);

export default Settings;

const styles = {
    container: {
        display: 'flex',
        height: '100vh',
    },
    sidebar: {
        width: '250px',
        backgroundColor: '#333',
        color: '#fff',
        padding: '20px',
    },
    logo: {
        marginBottom: '20px',
    },
    menu: {
        listStyleType: 'none',
        padding: 0,
    },
    content: {
        flex: 1,
        padding: '20px',
    },
};

// NOTE: prefix `VITE_` is required for Vite to expose the environment variable
export const ADMINKA_API_BASE_URL = import.meta.env.VITE_ADMINKA_API_BASE_URL;
export const ADMINKA_UI_BASE_URL = import.meta.env.VITE_ADMINKA_UI_BASE_URL;

export const AZURE_TENANTID = import.meta.env.VITE_AZURE_TENANTID;
export const AZURE_CLIENTID = import.meta.env.VITE_AZURE_CLIENTID;
export const AZURE_SCOPE = import.meta.env.VITE_AZURE_SCOPE;

// enables debugging popupmenu, ENV_ISDEVDEBUG managed by vite.config.js
export const ISDEVDEBUG = import.meta.env.ENV_ISDEVDEBUG;
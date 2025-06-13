import { AZURE_TENANTID, AZURE_CLIENTID, ADMINKA_UI_BASE_URL } from '@/config.js';

export const msalConfig = {
    auth: {
        clientId: AZURE_CLIENTID, // from Azure portal
        authority: "https://login.microsoftonline.com/" + AZURE_TENANTID,
        redirectUri: ADMINKA_UI_BASE_URL, // your deployment URI, must match Azure Portal configuration.
    },
    cache: {
        cacheLocation: "sessionStorage", // or "localStorage"
        storeAuthStateInCookie: false,
    },
};

export const loginRequest = {
    scopes: ["User.Read"], // or your API scopes
};
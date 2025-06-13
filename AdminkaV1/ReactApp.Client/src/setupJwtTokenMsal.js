//import React, { useEffect } from "react";
//import { useMsal } from "@azure/msal-react";
import { msalConfig, loginRequest } from "@/azureAuthConfig";
import { PublicClientApplication } from "@azure/msal-browser";

export const msalInstance = new PublicClientApplication(msalConfig);
//await this away causes publish errors
msalInstance.initialize();
// TODO: add logout
// TODO: add <p>Welcome, {accounts[0].username}</p>
async function setupJwtTokenMsal() {
    const accounts = msalInstance.getAllAccounts();
    var token;
    if (accounts.length === 0) {
        // TODO: loginPopup for click on button (it is impossible on startup: browsers open popup only on click on butto)
        const loginResponse = await msalInstance.loginPopup(loginRequest);
        //const loginResponse = await msalInstance.loginRedirect(loginRequest);
        token = acquireToken(loginResponse.account);
    } else {
        token = acquireToken(accounts[0]);
    }
    localStorage.setItem("access_token", token);

};

async function acquireToken(account) {
    try {
        const tokenResponse = await msalInstance.acquireTokenSilent({
            ...loginRequest,
            account: account,
        });
        return tokenResponse.accessToken;
    } catch (error) {
        console.error("Token acquisition failed:", error);
        throw error;
    }
}

export default setupJwtTokenMsal;
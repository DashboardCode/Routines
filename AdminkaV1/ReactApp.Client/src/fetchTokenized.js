import { ADMINKA_API_BASE_URL } from '@/config';

async function fetchTokenized(uri, body, method="GET"){

    const token = localStorage.getItem("access_token");
    if (!token || isTokenExpired(token)) {
        //localStorage.removeItem("access_token");
        window.location.href = '/';  // this initiates a relogin message
    }
    const response = await fetch(`${ADMINKA_API_BASE_URL}${uri}`, {
        method: method,
        body: body,
        headers: { Authorization: `Bearer ${token}`, "Content-Type": "application/json" } 
    });
    return response;
}

function isTokenExpired(token) {
    try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        const currentTime = Math.floor(Date.now() / 1000); // in seconds
        return payload.exp < currentTime;
    } catch (err) {
        console.error("Error parsing token:", err);
        return true; // invalid token format
    }
}
export { fetchTokenized, isTokenExpired };
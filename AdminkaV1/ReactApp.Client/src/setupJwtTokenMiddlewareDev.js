import { ADMINKA_API_BASE_URL } from '@/config';
import extractMessage from '@/extractMessage';

async function setupJwtTokenMiddleware(password) {
    const response = await fetch(`${ADMINKA_API_BASE_URL}/ui/authenticationdev`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ password })
    });
    if (response.ok === true) {
        const data = await response.json();
        localStorage.setItem("access_token", data.token);
        return true;
    } else {
        if (response.status == 401) {
            return false;
        }
        else {
            var message = await extractMessage(response);
            throw new Error(`Failed to authenticate: ${message}`);
        }
    }
}

export default setupJwtTokenMiddleware;
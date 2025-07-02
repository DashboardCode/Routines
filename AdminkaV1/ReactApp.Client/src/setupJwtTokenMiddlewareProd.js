import { ADMINKA_API_BASE_URL } from '@/config';
import { parseErrorResponseAsync } from '@/parseErrorResponse';

async function setupJwtTokenMiddleware(password) {

    const response = await fetch(`${ADMINKA_API_BASE_URL}/ui/authentication`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ password })
    });
    if (response.ok === true) {
        const data = await response.json();
        return { success: true, data };
    } else {
        var errorContent = await parseErrorResponseAsync(response);
        return { success: false, errorContent };
    }
}

export default setupJwtTokenMiddleware;
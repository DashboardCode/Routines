async function parseErrorResponseAsync(response) {
    const statusText = `(${response.status}) ` + getHttpStatusMessage(response.status);

    const contentType = response.headers.get("Content-Type");

    if (contentType && contentType.includes("application/json")) {
        const json = await response.json();
        return { message: json.error ?? statusText, json };
    } else if (contentType && contentType.includes("application/problem+json")) {
        const json = await response.json();
        return { message: json.title, json };
    } else {
        const text = await response.text();
        const firstLine = text.split('\n')[0];
        return { message: firstLine, text };
    }
}

function getHttpStatusMessage(status) {
    const messages = {
        100: "Continue",
        101: "Switching Protocols",
        102: "Processing",
        200: "OK",
        201: "Created",
        202: "Accepted",
        204: "No Content",
        301: "Moved Permanently",
        302: "Found",
        304: "Not Modified",
        400: "Bad Request",
        401: "Unauthorized",
        403: "Forbidden",
        404: "Not Found",
        405: "Method Not Allowed",
        409: "Conflict",
        410: "Gone",
        422: "Unprocessable Entity",
        429: "Too Many Requests",
        500: "Internal Server Error",
        502: "Bad Gateway",
        503: "Service Unavailable",
        504: "Gateway Timeout",
    };

    return messages[status] || `Unknown HTTP status: ${status}`;
}

export { parseErrorResponseAsync };
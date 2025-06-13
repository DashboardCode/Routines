async function extractMessage(httpResponse) {
    let message = `Error ${httpResponse.status}: ${httpResponse.statusText}`
    let type = httpResponse.type;
    try {
        const contentType = httpResponse.headers.get('content-type') || ''

        if (contentType.includes('application/json')) {
            const json = await httpResponse.json()
            if (json.message) {
                message = json.message
            } else if (typeof json === 'object') {
                message = JSON.stringify(json)
            }
        } else if (contentType.includes('application/problem+json')) {
            const problem = await httpResponse.json()
            message = JSON.stringify(problem)
        }
        else if (contentType.includes('text/')) {
            const text = await httpResponse.text()
            if (text) {
                message = text
            }
        }
        message = message + "; type: " + type
    } catch (err) {
        message += ` (Failed to parse error body: ${err.message})`
    }
    return message;
}

export default extractMessage;
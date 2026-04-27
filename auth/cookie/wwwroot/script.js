const BASE_URL = "http://localhost:5000/";
const HEALTH_PATH = "api/diagnostic/health";
const GENERATE_COOKIE_PATH = "api/security/cookies/generate";
const INTROSPECT_PATH = "api/security/cookies/introspect";

function buildUrl(endpointPath) {
    return `${BASE_URL}${endpointPath}`;
}

function refreshRequestUrls() {
    document.getElementById("step2_url").textContent = buildUrl(HEALTH_PATH);
    document.getElementById("step3_url").textContent = buildUrl(GENERATE_COOKIE_PATH);
    document.getElementById("step4_url").textContent = buildUrl(INTROSPECT_PATH);
}

function displayStep3() {
    const step3 = document.getElementById("step3");
    step3.style.display = 'block';
    refreshRequestUrls();
}

function displayStep4() {
    const step4 = document.getElementById("step4");
    step4.style.display = 'block';
    refreshRequestUrls();
}

async function healthCheck() {

    const response = await fetch(buildUrl(HEALTH_PATH), {
        method: 'GET'
    });

    const status = document.getElementById("step2_status");

    if (response.ok) {

        var health = await response.json()

        status.className = "text-success";
        status.innerHTML = health.Status;

        displayStep3();
    }
    else {
        status.className = "text-danger";
        status.innerHTML = "The API is <strong>not</strong> online.";
    }
}

async function generateCookie() {

    const token = document.getElementById("input_cookie_value").value;

    const response = await fetch(buildUrl(GENERATE_COOKIE_PATH), {
        method: 'POST',
        credentials: 'include', // ensure cookies are sent/received
        headers: {
            'Content-Type': 'text/plain'
        },
        body: token
    });

    // Wait for the cookie to fully generate.
    await new Promise(resolve => setTimeout(resolve, 500));

    const status = document.getElementById("step3_status");

    if (response.ok) {

        var cookie = await response.json()

        status.className = "text-success";
        status.innerHTML = "Cookie generation succeeded. Use the Developer Tools in your browser to confirm it now has a matching cookie."

        const display = document.getElementById("cookie_display");
        display.innerHTML = JSON.stringify(cookie);

        displayStep4();
    }
    else {
        status.className = "text-danger";
        status.innerHTML = "Cookie generation failed. An unexpected error occurred.";
    }
}

async function useCookie() {

    const status = document.getElementById("step4_status");

    const response = await fetch(buildUrl(INTROSPECT_PATH), {
        method: 'GET',
        credentials: 'include' // ensure cookies are sent/received
    });

    const display = document.getElementById("introspection_display");

    if (response.ok) {

        var result = await response.json()

        status.className = "text-success";
        status.innerHTML = "Cookie introspection succeeded. A detailed report is available below."

        display.innerHTML = JSON.stringify(result, null, 2);
    }
    else {

        status.className = "text-danger";
        status.innerHTML = `Cookie introspection failed with HTTP ${response.status}.`;

        if (response.status == 401) {
            var error = await response.json();
            display.innerHTML = JSON.stringify(error, null, 2);
        }
    }
}

document.addEventListener("DOMContentLoaded", refreshRequestUrls);

// healthCheck();
// generateCookie();
// useCookie();

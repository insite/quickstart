const FIELD_IDS = [
    "LearnerCode",
    "LearnerEmail",
    "LearnerNameFirst",
    "LearnerNameLast",
    "GroupName",
    "OrganizationIdentifier",
    "OrganizationSecret",
    "LaunchUrl"
];

async function loadDefaults() {
    try {
        const response = await fetch("/api/lti/defaults");
        if (!response.ok) return;
        const defaults = await response.json();
        for (const id of FIELD_IDS) {
            const input = document.getElementById(id);
            if (input && defaults[id] !== undefined) input.value = defaults[id];
        }
    } catch {
        // leave fields empty if defaults are unavailable
    }
}

function collectFormValues() {
    const data = {};
    for (const id of FIELD_IDS) {
        data[id] = document.getElementById(id).value;
    }
    return data;
}

function renderParameters(ticket) {
    const rows = document.getElementById("parameter-rows");
    const keys = document.getElementById("keys");
    rows.replaceChildren();
    keys.replaceChildren();

    for (const [key, value] of Object.entries(ticket.Parameters)) {
        const tr = document.createElement("tr");

        const tdKey = document.createElement("td");
        tdKey.textContent = key;
        tr.appendChild(tdKey);

        const tdValue = document.createElement("td");
        const input = document.createElement("input");
        input.type = "text";
        input.name = key;
        input.value = value;
        tdValue.appendChild(input);
        tr.appendChild(tdValue);

        rows.appendChild(tr);

        const span = document.createElement("span");
        span.textContent = key;
        keys.appendChild(span);
    }

    document.getElementById("launch-link").onclick = (event) => {
        event.preventDefault();
        submitLaunch(ticket.Url);
    };
}

function submitLaunch(url) {
    const form = document.createElement("form");
    form.method = "post";
    form.action = url;

    for (const input of document.querySelectorAll("#parameter-rows input")) {
        const hidden = document.createElement("input");
        hidden.type = "hidden";
        hidden.name = input.name;
        hidden.value = input.value;
        form.appendChild(hidden);
    }

    document.body.appendChild(form);
    form.submit();
}

async function validate(event) {
    event.preventDefault();

    const errorEl = document.getElementById("validate-error");
    errorEl.hidden = true;
    errorEl.textContent = "";

    try {
        const response = await fetch("/api/lti/ticket", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(collectFormValues())
        });

        if (!response.ok) {
            const body = await response.json().catch(() => ({}));
            errorEl.textContent = body.error || `Request failed (${response.status}).`;
            errorEl.hidden = false;
            return;
        }

        const ticket = await response.json();
        renderParameters(ticket);

        document.getElementById("step1").hidden = true;
        document.getElementById("step2").hidden = false;
    } catch (err) {
        errorEl.textContent = err.message;
        errorEl.hidden = false;
    }
}

document.getElementById("form-validate").addEventListener("submit", validate);
loadDefaults();

const apiBase = "https://localhost:7150/api";

let selectedOfficeId = null;

async function loadOffices() {
    const res = await fetch(`${apiBase}/doctorsOffice/all`);
    if (!res.ok) throw new Error(`Failed to load offices: ${res.status}`);
    
    const offices = await res.json();
    console.log('Loaded offices:', offices);

    const select = document.getElementById("officeSelect");
    select.innerHTML = `<option disabled selected>-- Επιλέξτε --</option>`;
    offices.forEach(office => {
        const opt = document.createElement("option");
        opt.value = office.id;
        opt.textContent = office.name;
        select.appendChild(opt);
    });

    select.addEventListener("change", () => {
        selectedOfficeId = select.value;
        refreshData();
    });
}

async function refreshData() {
    if (!selectedOfficeId) return;

    const waitingRes = await fetch(`${apiBase}/patients/waiting`);
    const waiting = await waitingRes.json();

    let current = null;
    const currentRes = await fetch(`${apiBase}/patients/current/${selectedOfficeId}`);
    if (currentRes.ok) {
    current = await currentRes.json();}

    renderWaitingList(waiting);
    renderActivePatient(current);
}

function renderWaitingList(patients) {
    const list = document.getElementById("waitingList");
    list.innerHTML = "";

    patients.forEach(p => {
        const item = document.createElement("li");
        item.className = "list-group-item d-flex justify-content-between align-items-center";

        item.innerHTML = `
      <span><strong>${p.patientNumber}</strong> - ${p.name}</span>
      <button class="btn btn-sm btn-primary">Call</button>
    `;

        item.querySelector("button").onclick = () => callPatient(p.id);
        list.appendChild(item);
    });
}

function renderActivePatient(patient) {
    const card = document.getElementById("activePatient");
    card.innerHTML = "";

    if (!patient || patient.status !== 1) {
        card.innerHTML = `<p>Κανένας ασθενής δεν έχει κληθεί ακόμα.</p>`;
        return;
    }
    const header = `<h4>${patient.patientNumber}</h4><p>${patient.name}</p>`;

    const releaseBtn = document.createElement("button");
    releaseBtn.textContent = "Release";
    releaseBtn.className = "btn btn-warning me-2";
    releaseBtn.onclick = () => releasePatient(patient.id);

    const exitBtn = document.createElement("button");
    exitBtn.textContent = "Exit";
    exitBtn.className = "btn btn-danger";
    exitBtn.onclick = () => exitPatient(patient.id);

    card.innerHTML = header;
    card.appendChild(releaseBtn);
    card.appendChild(exitBtn);
}

async function callPatient(patientId) {
  if (!selectedOfficeId) return;

  const res = await fetch(`${apiBase}/patients/${patientId}/call`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ doctorsOfficeId: selectedOfficeId })
    });

    if (!res.ok) {
        alert("This doctor already has a patient or something went wrong.");
        return;
    }

    await refreshData();
}

async function releasePatient(patientId) {
    await fetch(`${apiBase}/patients/${patientId}/release`, {
        method: "PUT"
    });
    await refreshData();
}

async function exitPatient(patientId) {
    await fetch(`${apiBase}/patients/${patientId}/complete`, {
        method: "PUT"
    });

    await refreshData();
}

// Initial Load
loadOffices();

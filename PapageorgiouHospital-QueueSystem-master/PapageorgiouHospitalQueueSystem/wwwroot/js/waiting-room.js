const apiBase = "https://localhost:7150/api";

async function loadWaitingRoom() {
    const grid = document.getElementById("officesGrid");
    grid.innerHTML = "";

    const res = await fetch(`${apiBase}/doctorsOffice/all`);
    const offices = await res.json();

    for (const office of offices) {
        const currentRes = await fetch(`${apiBase}/patients/current/${office.id}`);
        let currentPatient = null;
        if (currentRes.ok) {
            currentPatient = await currentRes.json();
        }

        const newNumber = currentPatient ? currentPatient.patientNumber : "-";
        const existing = document.getElementById(`patient-${office.id}`);
        let shouldBlink = false;

        if (existing && existing.textContent !== newNumber) {
            shouldBlink = true;
        }

        const col = document.createElement("div");
        col.className = "col-12 col-sm-6 col-md-4 col-lg-3";

        const tile = document.createElement("div");
        tile.className = "office-tile";

        const name = document.createElement("div");
        name.className = "office-name";
        name.textContent = office.name;

        const number = document.createElement("div");
        number.className = "patient-number";
        number.id = `patient-${office.id}`;
        number.textContent = currentPatient ? currentPatient.patientNumber : "-";

        if (shouldBlink) {
            number.classList.add("blink");
            setTimeout(() => number.classList.remove("blink"), 1500);
        }
        tile.appendChild(name);
        tile.appendChild(number);
        col.appendChild(tile);
        grid.appendChild(col);
    }
}

loadWaitingRoom();
setInterval(loadWaitingRoom, 5000);
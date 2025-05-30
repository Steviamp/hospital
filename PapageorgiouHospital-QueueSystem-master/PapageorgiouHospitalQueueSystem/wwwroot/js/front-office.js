document.getElementById('patientForm').addEventListener('submit', async function (e) {
    e.preventDefault();

    const name = document.getElementById('name').value.trim();
    const patientNumber = document.getElementById('patientNumber').value.trim();
    const comment = document.getElementById('comment').value.trim();
    const statusDiv = document.getElementById('statusMessage');

    if (!name || !patientNumber) {
        statusDiv.innerHTML = `<div class="alert alert-warning">Παρακαλώ εισάγετε όνομα και αριθμό ασθενούς.</div>`;
        return;
    }

    try {
        const response = await fetch('https://localhost:7150/api/patients', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ name, patientNumber, comment })
        });

        if (!response.ok) throw new Error("Σφάλμα στην καταχώρηση");

        const result = await response.json();
        statusDiv.innerHTML = `<div class="alert alert-success">Ο ασθενής καταχωρήθηκε με ID: ${result.id}</div>`;
        document.getElementById('patientForm').reset();
    } catch (error) {
        statusDiv.innerHTML = `<div class="alert alert-danger">Αποτυχία: ${error.message}</div>`;
    }
});

(function () {
    requireAuth();
    renderNavbar("dashboard");

    const alertBox = document.getElementById("dashboardAlert");

    function showError(message) {
        alertBox.textContent = message;
        alertBox.classList.remove("d-none");
    }

    async function loadSummary() {
        try {
            const summary = await apiRequest("/dashboard/summary", { method: "GET" });
            document.getElementById("totalEmployees").textContent = summary.totalEmployees;
            document.getElementById("activeEmployees").textContent = summary.activeEmployees;
            document.getElementById("inactiveEmployees").textContent = summary.inactiveEmployees;
        } catch (error) {
            showError(error.message);
        }
    }

    loadSummary();
})();

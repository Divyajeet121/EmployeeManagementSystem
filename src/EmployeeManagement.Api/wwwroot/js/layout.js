function renderNavbar(activePage) {
    const navHtml = `
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark mb-4">
        <div class="container-fluid">
            <a class="navbar-brand" href="dashboard.html">Employee Management System</a>
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarContent">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse" id="navbarContent">
                <ul class="navbar-nav me-auto">
                    <li class="nav-item">
                        <a class="nav-link ${activePage === "dashboard" ? "active fw-bold" : ""}" href="dashboard.html">Dashboard</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link ${activePage === "employees" ? "active fw-bold" : ""}" href="employees.html">Employees</a>
                    </li>
                </ul>
                <span class="navbar-text text-light me-3" id="currentUserLabel"></span>
                <button class="btn btn-outline-light btn-sm" id="logoutButton">Logout</button>
            </div>
        </div>
    </nav>`;

    document.getElementById("navbarPlaceholder").innerHTML = navHtml;
    document.getElementById("currentUserLabel").textContent = `${getUsername()} (${getRole()})`;

    document.getElementById("logoutButton").addEventListener("click", async function () {
        try {
            await apiRequest("/auth/logout", { method: "POST" });
        } catch (error) {
            clearSession();
        }
        clearSession();
        window.location.href = "/index.html";
    });
}

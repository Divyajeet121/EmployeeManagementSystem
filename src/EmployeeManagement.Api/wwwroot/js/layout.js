function renderNavbar(activePage) {
    const username = getUsername();
    const initial = username ? username.charAt(0).toUpperCase() : "?";

    const navHtml = `
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark mb-4">
        <div class="container-fluid">
            <a class="navbar-brand" href="/dashboard">Employee Management System</a>
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarContent">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse" id="navbarContent">
                <ul class="navbar-nav me-auto">
                    <li class="nav-item">
                        <a class="nav-link ${activePage === "dashboard" ? "active fw-bold" : ""}" href="/dashboard">Dashboard</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link ${activePage === "employees" ? "active fw-bold" : ""}" href="/employees">Employees</a>
                    </li>
                </ul>
                <div class="dropdown">
                    <button class="btn profile-toggle dropdown-toggle d-flex align-items-center" type="button" id="profileMenuButton" data-bs-toggle="dropdown" aria-expanded="false">
                        <span class="profile-avatar me-2">${initial}</span>
                        <span class="text-light">${username}</span>
                    </button>
                    <ul class="dropdown-menu dropdown-menu-end profile-menu" aria-labelledby="profileMenuButton">
                        <li>
                            <div class="px-3 py-2">
                                <div class="d-flex align-items-center mb-2">
                                    <span class="profile-avatar profile-avatar-lg me-2">${initial}</span>
                                    <div>
                                        <div class="fw-semibold">${username}</div>
                                        <span class="badge profile-role-badge">${getRole()}</span>
                                    </div>
                                </div>
                                <div class="text-muted small"><i class="bi bi-envelope me-1"></i>${getEmail()}</div>
                            </div>
                        </li>
                        <li><hr class="dropdown-divider" /></li>
                        <li><button class="dropdown-item text-danger" id="logoutButton"><i class="bi bi-box-arrow-right me-2"></i>Logout</button></li>
                    </ul>
                </div>
            </div>
        </div>
    </nav>`;

    document.getElementById("navbarPlaceholder").innerHTML = navHtml;

    document.getElementById("logoutButton").addEventListener("click", async function () {
        try {
            await apiRequest("/auth/logout", { method: "POST" });
        } catch (error) {
            clearSession();
        }
        clearSession();
        window.location.href = "/";
    });
}

(function () {
    requireAuth();
    renderNavbar("dashboard");

    const alertBox = document.getElementById("dashboardAlert");
    const departmentChart = document.getElementById("departmentChart");
    const departmentEmptyLabel = document.getElementById("departmentEmptyLabel");
    const recentEmployeesList = document.getElementById("recentEmployeesList");

    const DEPARTMENT_COLORS = ["#4e73df", "#1cc88a", "#f6c23e", "#e74a3b", "#36b9cc", "#858796", "#fd7e14"];

    document.getElementById("welcomeHeading").textContent = `Welcome back, ${getUsername()}`;
    document.getElementById("todayLabel").textContent = new Date().toLocaleDateString(undefined, {
        weekday: "long",
        year: "numeric",
        month: "long",
        day: "numeric"
    });

    function showError(message) {
        alertBox.textContent = message;
        alertBox.classList.remove("d-none");
    }

    async function loadSummary() {
        const summary = await apiRequest("/dashboard/summary", { method: "GET" });
        document.getElementById("totalEmployees").textContent = summary.totalEmployees;
        document.getElementById("activeEmployees").textContent = summary.activeEmployees;
        document.getElementById("inactiveEmployees").textContent = summary.inactiveEmployees;
    }

    function renderDepartmentChart(employees) {
        const counts = {};
        employees.forEach(function (employee) {
            const department = employee.department || "Unassigned";
            counts[department] = (counts[department] || 0) + 1;
        });

        const departments = Object.keys(counts).sort(function (a, b) {
            return counts[b] - counts[a];
        });

        if (departments.length === 0) {
            departmentEmptyLabel.classList.remove("d-none");
            return;
        }

        const maxCount = Math.max.apply(null, departments.map(function (department) { return counts[department]; }));

        departmentChart.innerHTML = departments.map(function (department, index) {
            const count = counts[department];
            const widthPercent = Math.round((count / maxCount) * 100);
            const color = DEPARTMENT_COLORS[index % DEPARTMENT_COLORS.length];

            return `<div class="dept-bar-row">
                <div class="dept-bar-label">${department}</div>
                <div class="dept-bar-track">
                    <div class="dept-bar-fill" style="width: ${widthPercent}%; background: ${color};"></div>
                </div>
                <div class="dept-bar-count">${count}</div>
            </div>`;
        }).join("");
    }

    function renderRecentEmployees(employees) {
        const recent = employees.slice().sort(function (a, b) {
            return new Date(b.createdOn) - new Date(a.createdOn);
        }).slice(0, 6);

        if (recent.length === 0) {
            recentEmployeesList.innerHTML = `<li class="list-group-item text-muted small">No employees yet.</li>`;
            return;
        }

        recentEmployeesList.innerHTML = recent.map(function (employee) {
            const statusBadge = employee.isActive
                ? `<span class="badge status-badge-active">Active</span>`
                : `<span class="badge status-badge-inactive">Inactive</span>`;

            return `<li class="list-group-item d-flex align-items-center justify-content-between">
                <div>
                    <div class="fw-semibold">${employee.firstName} ${employee.lastName}</div>
                    <div class="text-muted small">${employee.designation} &middot; ${employee.department}</div>
                </div>
                ${statusBadge}
            </li>`;
        }).join("");
    }

    async function loadEmployeeInsights() {
        const result = await apiRequest("/employees?pageNumber=1&pageSize=100", { method: "GET" });
        renderDepartmentChart(result.items);
        renderRecentEmployees(result.items);
    }

    async function loadDashboard() {
        try {
            await Promise.all([loadSummary(), loadEmployeeInsights()]);
        } catch (error) {
            showError(error.message);
        }
    }

    loadDashboard();
})();

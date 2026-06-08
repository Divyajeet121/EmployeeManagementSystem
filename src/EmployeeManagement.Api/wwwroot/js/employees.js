(function () {
    requireAuth();
    renderNavbar("employees");

    const PAGE_SIZE = 8;

    const state = {
        pageNumber: 1,
        searchTerm: "",
        department: "",
        isActive: ""
    };

    const isAdmin = getRole() === "Admin";

    const alertBox = document.getElementById("employeesAlert");
    const tableBody = document.getElementById("employeesTableBody");
    const pagination = document.getElementById("pagination");

    const employeeModalElement = document.getElementById("employeeModal");
    const employeeModal = new bootstrap.Modal(employeeModalElement);
    const employeeForm = document.getElementById("employeeForm");
    const employeeFormAlert = document.getElementById("employeeFormAlert");

    const deleteModalElement = document.getElementById("deleteConfirmModal");
    const deleteModal = new bootstrap.Modal(deleteModalElement);
    let pendingDeleteId = null;

    if (isAdmin) {
        document.getElementById("addEmployeeButton").classList.remove("d-none");
        document.getElementById("actionsHeader").classList.remove("d-none");
    }

    function showListError(message) {
        alertBox.textContent = message;
        alertBox.classList.remove("d-none");
    }

    function hideListError() {
        alertBox.classList.add("d-none");
    }

    function showFormError(message) {
        employeeFormAlert.textContent = message;
        employeeFormAlert.classList.remove("d-none");
    }

    function hideFormError() {
        employeeFormAlert.classList.add("d-none");
    }

    function formatDate(value) {
        if (!value) return "-";
        const date = new Date(value);
        return date.toLocaleDateString();
    }

    function formatDateTime(value) {
        if (!value) return "-";
        const date = new Date(value);
        return date.toLocaleString();
    }

    function buildQueryString() {
        const params = new URLSearchParams();
        params.set("pageNumber", state.pageNumber);
        params.set("pageSize", PAGE_SIZE);
        if (state.searchTerm) params.set("searchTerm", state.searchTerm);
        if (state.department) params.set("department", state.department);
        if (state.isActive !== "") params.set("isActive", state.isActive);
        return params.toString();
    }

    function renderRows(employees) {
        if (employees.length === 0) {
            tableBody.innerHTML = `<tr><td colspan="9" class="text-center text-muted">No employees found.</td></tr>`;
            return;
        }

        tableBody.innerHTML = employees.map(function (employee) {
            const statusBadge = employee.isActive
                ? `<span class="badge status-badge-active">Active</span>`
                : `<span class="badge status-badge-inactive">Inactive</span>`;

            const actionsCell = isAdmin
                ? `<td class="table-actions">
                       <button class="btn btn-sm btn-outline-primary" data-action="edit" data-id="${employee.employeeId}">Edit</button>
                       <button class="btn btn-sm btn-outline-danger" data-action="delete" data-id="${employee.employeeId}">Delete</button>
                   </td>`
                : "";

            const auditInfo = `Created by user #${employee.createdBy} on ${formatDateTime(employee.createdOn)}`
                + (employee.modifiedBy ? `<br/>Modified by user #${employee.modifiedBy} on ${formatDateTime(employee.modifiedOn)}` : "");

            return `<tr>
                <td>${employee.employeeCode}</td>
                <td>${employee.firstName} ${employee.lastName}</td>
                <td>${employee.email}</td>
                <td>${employee.department}</td>
                <td>${employee.designation}</td>
                <td>${formatDate(employee.joiningDate)}</td>
                <td>${statusBadge}</td>
                <td class="small text-muted">${auditInfo}</td>
                ${actionsCell}
            </tr>`;
        }).join("");

        if (isAdmin) {
            tableBody.querySelectorAll('[data-action="edit"]').forEach(function (button) {
                button.addEventListener("click", function () {
                    openEditModal(parseInt(button.getAttribute("data-id"), 10));
                });
            });
            tableBody.querySelectorAll('[data-action="delete"]').forEach(function (button) {
                button.addEventListener("click", function () {
                    pendingDeleteId = parseInt(button.getAttribute("data-id"), 10);
                    deleteModal.show();
                });
            });
        }
    }

    function renderPagination(totalCount) {
        const totalPages = Math.max(1, Math.ceil(totalCount / PAGE_SIZE));
        pagination.innerHTML = "";

        for (let page = 1; page <= totalPages; page++) {
            const isActive = page === state.pageNumber;
            const item = document.createElement("li");
            item.className = `page-item ${isActive ? "active" : ""}`;
            item.innerHTML = `<button class="page-link">${page}</button>`;
            item.querySelector("button").addEventListener("click", function () {
                state.pageNumber = page;
                loadEmployees();
            });
            pagination.appendChild(item);
        }
    }

    async function loadEmployees() {
        hideListError();
        tableBody.innerHTML = `<tr><td colspan="9" class="text-center text-muted">Loading...</td></tr>`;

        try {
            const result = await apiRequest(`/employees?${buildQueryString()}`, { method: "GET" });
            renderRows(result.items);
            renderPagination(result.totalCount);
        } catch (error) {
            tableBody.innerHTML = "";
            showListError(error.message);
        }
    }

    function resetEmployeeForm() {
        employeeForm.reset();
        employeeForm.classList.remove("was-validated");
        document.getElementById("employeeId").value = "";
        document.getElementById("isActive").checked = true;
        hideFormError();
    }

    function openAddModal() {
        resetEmployeeForm();
        document.getElementById("employeeModalTitle").textContent = "Add Employee";
        employeeModal.show();
    }

    async function openEditModal(employeeId) {
        resetEmployeeForm();
        document.getElementById("employeeModalTitle").textContent = "Edit Employee";

        try {
            const employee = await apiRequest(`/employees/${employeeId}`, { method: "GET" });
            document.getElementById("employeeId").value = employee.employeeId;
            document.getElementById("employeeCode").value = employee.employeeCode;
            document.getElementById("firstName").value = employee.firstName;
            document.getElementById("lastName").value = employee.lastName;
            document.getElementById("email").value = employee.email;
            document.getElementById("department").value = employee.department;
            document.getElementById("designation").value = employee.designation;
            document.getElementById("joiningDate").value = employee.joiningDate.substring(0, 10);
            document.getElementById("isActive").checked = employee.isActive;
            employeeModal.show();
        } catch (error) {
            showListError(error.message);
        }
    }

    async function saveEmployee() {
        hideFormError();

        if (!employeeForm.checkValidity()) {
            employeeForm.classList.add("was-validated");
            return;
        }
        employeeForm.classList.add("was-validated");

        const employeeId = document.getElementById("employeeId").value;
        const payload = {
            employeeCode: document.getElementById("employeeCode").value.trim(),
            firstName: document.getElementById("firstName").value.trim(),
            lastName: document.getElementById("lastName").value.trim(),
            email: document.getElementById("email").value.trim(),
            department: document.getElementById("department").value.trim(),
            designation: document.getElementById("designation").value.trim(),
            joiningDate: document.getElementById("joiningDate").value,
            isActive: document.getElementById("isActive").checked
        };

        const saveButton = document.getElementById("saveEmployeeButton");
        saveButton.disabled = true;
        saveButton.textContent = "Saving...";

        try {
            if (employeeId) {
                await apiRequest(`/employees/${employeeId}`, { method: "PUT", body: payload });
            } else {
                await apiRequest("/employees", { method: "POST", body: payload });
            }

            employeeModal.hide();
            await loadEmployees();
        } catch (error) {
            showFormError(error.message);
        } finally {
            saveButton.disabled = false;
            saveButton.textContent = "Save";
        }
    }

    async function confirmDelete() {
        if (pendingDeleteId === null) return;

        const confirmButton = document.getElementById("confirmDeleteButton");
        confirmButton.disabled = true;
        confirmButton.textContent = "Deleting...";

        try {
            await apiRequest(`/employees/${pendingDeleteId}`, { method: "DELETE" });
            deleteModal.hide();
            pendingDeleteId = null;
            await loadEmployees();
        } catch (error) {
            showListError(error.message);
            deleteModal.hide();
        } finally {
            confirmButton.disabled = false;
            confirmButton.textContent = "Delete";
        }
    }

    document.getElementById("addEmployeeButton").addEventListener("click", openAddModal);
    document.getElementById("saveEmployeeButton").addEventListener("click", saveEmployee);
    document.getElementById("confirmDeleteButton").addEventListener("click", confirmDelete);

    document.getElementById("searchButton").addEventListener("click", function () {
        state.pageNumber = 1;
        state.searchTerm = document.getElementById("searchInput").value.trim();
        state.department = document.getElementById("departmentInput").value.trim();
        state.isActive = document.getElementById("statusSelect").value;
        loadEmployees();
    });

    employeeModalElement.addEventListener("hidden.bs.modal", resetEmployeeForm);

    loadEmployees();
})();

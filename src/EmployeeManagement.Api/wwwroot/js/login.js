(function () {
    if (isAuthenticated()) {
        window.location.href = "pages/dashboard.html";
        return;
    }

    const form = document.getElementById("loginForm");
    const alertBox = document.getElementById("loginAlert");
    const loginButton = document.getElementById("loginButton");

    function showError(message) {
        alertBox.textContent = message;
        alertBox.classList.remove("d-none");
    }

    function hideError() {
        alertBox.classList.add("d-none");
    }

    form.addEventListener("submit", async function (event) {
        event.preventDefault();
        event.stopPropagation();
        hideError();

        if (!form.checkValidity()) {
            form.classList.add("was-validated");
            return;
        }
        form.classList.add("was-validated");

        const username = document.getElementById("username").value.trim();
        const password = document.getElementById("password").value;

        loginButton.disabled = true;
        loginButton.textContent = "Signing in...";

        try {
            const result = await apiRequest("/auth/login", {
                method: "POST",
                body: { username, password }
            });

            saveSession(result.token, result.username, result.role);
            window.location.href = "pages/dashboard.html";
        } catch (error) {
            showError(error.message);
        } finally {
            loginButton.disabled = false;
            loginButton.textContent = "Login";
        }
    });
})();

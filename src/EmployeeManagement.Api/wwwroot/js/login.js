(function () {
    if (isAuthenticated()) {
        window.location.href = "/dashboard";
        return;
    }

    const form = document.getElementById("loginForm");
    const alertBox = document.getElementById("loginAlert");
    const loginButton = document.getElementById("loginButton");
    const passwordInput = document.getElementById("password");
    const togglePasswordButton = document.getElementById("togglePasswordButton");
    const togglePasswordIcon = document.getElementById("togglePasswordIcon");

    togglePasswordButton.addEventListener("click", function () {
        const isVisible = passwordInput.type === "text";
        passwordInput.type = isVisible ? "password" : "text";
        togglePasswordIcon.classList.toggle("bi-eye", isVisible);
        togglePasswordIcon.classList.toggle("bi-eye-slash", !isVisible);
        togglePasswordButton.setAttribute("aria-label", isVisible ? "Show password" : "Hide password");
    });

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

            saveSession(result.token, result.username, result.email, result.role);
            window.location.href = "/dashboard";
        } catch (error) {
            showError(error.message);
        } finally {
            loginButton.disabled = false;
            loginButton.textContent = "Login";
        }
    });
})();

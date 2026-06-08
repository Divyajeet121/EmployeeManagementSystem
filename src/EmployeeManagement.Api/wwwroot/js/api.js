const API_BASE_URL = "/api";

const TOKEN_STORAGE_KEY = "ems_token";
const USERNAME_STORAGE_KEY = "ems_username";
const EMAIL_STORAGE_KEY = "ems_email";
const ROLE_STORAGE_KEY = "ems_role";

function saveSession(token, username, email, role) {
    sessionStorage.setItem(TOKEN_STORAGE_KEY, token);
    sessionStorage.setItem(USERNAME_STORAGE_KEY, username);
    sessionStorage.setItem(EMAIL_STORAGE_KEY, email);
    sessionStorage.setItem(ROLE_STORAGE_KEY, role);
}

function clearSession() {
    sessionStorage.removeItem(TOKEN_STORAGE_KEY);
    sessionStorage.removeItem(USERNAME_STORAGE_KEY);
    sessionStorage.removeItem(EMAIL_STORAGE_KEY);
    sessionStorage.removeItem(ROLE_STORAGE_KEY);
}

function getToken() {
    return sessionStorage.getItem(TOKEN_STORAGE_KEY);
}

function getUsername() {
    return sessionStorage.getItem(USERNAME_STORAGE_KEY);
}

function getEmail() {
    return sessionStorage.getItem(EMAIL_STORAGE_KEY);
}

function getRole() {
    return sessionStorage.getItem(ROLE_STORAGE_KEY);
}

function isAuthenticated() {
    return Boolean(getToken());
}

function requireAuth() {
    if (!isAuthenticated()) {
        window.location.href = "/";
    }
}

async function apiRequest(path, options = {}) {
    const headers = options.headers ? { ...options.headers } : {};
    headers["Accept"] = "application/json";

    if (options.body !== undefined && options.body !== null) {
        headers["Content-Type"] = "application/json";
    }

    const token = getToken();
    if (token) {
        headers["Authorization"] = `Bearer ${token}`;
    }

    const response = await fetch(`${API_BASE_URL}${path}`, {
        ...options,
        headers,
        body: options.body !== undefined && options.body !== null ? JSON.stringify(options.body) : undefined
    });

    if (response.status === 401 && token) {
        clearSession();
        window.location.href = "/";
        throw new Error("Session expired. Please log in again.");
    }

    if (response.status === 204) {
        return null;
    }

    const text = await response.text();
    const data = text ? JSON.parse(text) : null;

    if (!response.ok) {
        const message = data && data.message ? data.message : "Something went wrong. Please try again.";
        throw new Error(message);
    }

    return data;
}

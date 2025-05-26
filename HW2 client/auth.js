function getCurrentUser() {
    const json = sessionStorage.getItem("currentUser");
    return json ? JSON.parse(json) : null;
}

function requireLogin() {
    
    if (location.pathname.endsWith("index.html")) return;

    if (!getCurrentUser()) {                      
        if (!location.pathname.endsWith("login.html"))
            location.href = "login.html";
    }
}

function logout() {
    sessionStorage.clear();
    alert("Logged out successfully!");
    location.href = "index.html";
}


document.addEventListener("DOMContentLoaded", requireLogin);

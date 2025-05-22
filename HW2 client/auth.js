function getCurrentUser() {
    return sessionStorage.getItem("currentUser");   
}

function requireLogin() {
    
    if (location.pathname.endsWith("index.html")) return;

    if (!getCurrentUser()) {                      
        if (!location.pathname.endsWith("login.html"))
            location.href = "login.html";
    }
}

function logout() {
    sessionStorage.removeItem("isLoggedIn");
    sessionStorage.removeItem("currentUser");
    alert("Logged out successfully!");
    location.href = "index.html";
}


document.addEventListener("DOMContentLoaded", requireLogin);

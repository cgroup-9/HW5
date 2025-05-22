$(document).ready(() => {
    const port = 7110;
    const baseUrl = `https://localhost:${port}`;
    $("#btnLoadMoviesToSER").on("click", sendMoviesToServer);
    function sendMoviesToServer() {
        if (!movies || movies.length === 0) {
            alert("❌ אין סרטים לשלוח.");
            return;
        }

        const isDev = location.host.includes('localhost');
        const port = 7110;
        const baseUrl = isDev
            ? `https://localhost:${port}`
            : "https://proj.ruppin.ac.il/cgroup9/test2/tar1";

        const url = `${baseUrl}/api/Movies/bulk`;

        try {
            console.log("📤 Sending movies:", movies);
            ajaxCall("POST", url, JSON.stringify(movies), sendSuccess, sendFail);
        } catch (err) {
            console.error("❌ Error before POST:", err);
            alert("שליחת הסרטים נכשלה לפני ההגשה לשרת.");
        }
    }


    function sendSuccess(res) {
        alert(`✅ ${res.Inserted} סרטים נשלחו מתוך ${res.Total}`);
    }

    function sendFail(err) {
        alert("❌ שגיאה בשליחת הסרטים לשרת: " + err.statusText);
    }
}); 


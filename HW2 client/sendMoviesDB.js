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
            console.log("🎬 Example movie:", JSON.stringify(movies[0], null, 2));
            const cleanedMovies = sanitizeMovies(movies);
            console.log(`🔍 מתוך ${movies.length}, נשלחו ${cleanedMovies.length} סרטים לאחר סינון.`);
            ajaxCall("POST", url, JSON.stringify(cleanedMovies), sendSuccess, sendFail);
        } catch (err) {
            console.error("❌ Error before POST:", err);
            alert("שליחת הסרטים נכשלה לפני ההגשה לשרת.");
        }
    }

    function sanitizeMovies(movies) {
        const cleaned = [];

        for (let movie of movies) {
            try {
                const releaseDate = new Date(movie.releaseDate);
                const isValidDate = !isNaN(releaseDate.getTime());

                const sanitized = {
                    url: movie.url || "",
                    primaryTitle: typeof movie.primaryTitle === "string" ? movie.primaryTitle.trim() : "",
                    description: movie.description || "",
                    primaryImage: movie.primaryImage || "",
                    year: Number(movie.year) || new Date().getFullYear(), // אם חסר – השנה הנוכחית
                    releaseDate: isValidDate ? releaseDate.toISOString().split("T")[0] : null,
                    language: typeof movie.language === "string" ? movie.language.trim() : "unknown",
                    budget: isNaN(parseFloat(movie.budget)) ? 1000000 : parseFloat(movie.budget),
                    grossWorldwide: isNaN(parseFloat(movie.grossWorldwide)) ? 0 : parseFloat(movie.grossWorldwide),
                    genres: Array.isArray(movie.genres) ? movie.genres.join(", ") : (movie.genres || "Unknown"),
                    isAdult: movie.isAdult === true,
                    runtimeMinutes: Number(movie.runtimeMinutes) || 90,
                    averageRating: Number(movie.averageRating) || 0,
                    numVotes: Number(movie.numVotes) || 0,
                    priceToRent: Number(movie.priceToRent) || (Math.floor(Math.random() * 21) + 10)
                };

                // רק תנאים קריטיים לפסילה
                const valid =
                    sanitized.primaryTitle !== "" &&
                    sanitized.releaseDate !== null &&
                    !isNaN(sanitized.budget);

                if (valid) {
                    cleaned.push(sanitized);
                } else {
                    console.warn("⚠️ Skipped invalid movie:", movie);
                }

            } catch (e) {
                console.warn("❌ Skipped movie due to error:", e.message, movie);
            }
        }

        return cleaned;
    }









    function sendSuccess(res) {
        try {
            const response = typeof res === "string" ? JSON.parse(res) : res;
            alert(`✅ ${response.Inserted} סרטים נשלחו מתוך ${response.Total}`);
        } catch (err) {
            console.error("Failed to parse server response", err, res);
            alert("🎬 סרטים נשלחו, אך לא הצלחנו לקרוא את התשובה.");
        }
    }

    function sendFail(err) {
        let msg = "Unknown error";
        if (err.responseJSON) {
            msg = JSON.stringify(err.responseJSON);
        } else if (err.responseText) {
            msg = err.responseText;
        }
        alert("❌ שגיאה בשליחת הסרטים לשרת:\n" + msg);
    }

}); 


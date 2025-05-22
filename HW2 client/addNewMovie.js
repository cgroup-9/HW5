$(document).ready(() => {
    const divAddAMovie = $("#addAMovie");
    function isDevEnv() {
        return location.host.includes('localhost');
    }

    const port = 7110;
    const baseApiUrl = isDevEnv()
        ? `https://localhost:${port}`
        : "https://proj.ruppin.ac.il/cgroup9/test2/tar1"; 
    const url = `${baseApiUrl}/api/Movies`;

    document.getElementById("logoutBtn").addEventListener("click", logout);

    function addToCart(movie) {
        try {
            console.log("Sending movie:", JSON.stringify(movie));
            ajaxCall("POST", url, JSON.stringify(movie), addToCartSuc, addToCartFa);
        } catch (err) {
            console.error("❌ Error before POST:", err);
            alert("Failed to send movie information to the server. Please try again.");
        }
    }

    function addToCartSuc(res) {
        if (res === true) {
            alert("🎬 Movie added successfully!");
        } else {
            alert("⚠️ Movie already exists.");
        }
    }

    function addToCartFa(err) {
        alert("Failed to add movie: " + err.statusText);
    }

    divAddAMovie.empty();
    divAddAMovie.append('<h2 class="fullRowTitle">Add New Movie</h2>');

    let formAddMovieHtml = `
    <div class="newMovieDet">
        <form id="movieForm">
            URL: <input type="text" id="url" placeholder="https://example.com/movie"><div class="error-msg" id="err-url"></div><br>

            Primary Title: <input type="text" id="primaryTitle" required placeholder="Inception"><div class="error-msg" id="err-primaryTitle"></div><br>

            Description: <input type="text" id="description" placeholder="A mind-bending thriller"><div class="error-msg" id="err-description"></div><br>

            Primary Image: <input type="text" id="primaryImage" required placeholder="https://image.url.jpg"><div class="error-msg" id="err-primaryImage"></div><br>

            Year: <input type="number" id="year" required placeholder="2024"><div class="error-msg" id="err-year"></div><br>

            Release Date: <input type="date" id="releaseDate" required><div class="error-msg" id="err-releaseDate"></div><br>

            Language:
            <input list="languageList" id="language" required placeholder="English">
            <datalist id="languageList">
                <option value="English">
                <option value="Hebrew">
                <option value="French">
                <option value="Spanish">
                <option value="German">
            </datalist><div class="error-msg" id="err-language"></div><br>

            Budget: <input type="number" id="budget" min="100000" placeholder="e.g. 150000000"><div class="error-msg" id="err-budget"></div><br>

            Gross Worldwide: <input type="number" id="grossWorldwide" value="0" placeholder="e.g. 500000000"><div class="error-msg" id="err-grossWorldwide"></div><br>

            <label for="genreDatalist">Choose Genre (optional):</label>
            <input list="genreList" id="genreDatalist" placeholder="e.g. Action">
            <datalist id="genreList">
                <option value="Action">
                <option value="Drama">
                <option value="Comedy">
                <option value="Horror">
                <option value="Romance">
                <option value="Sci-Fi">
                <option value="Adventure">
                <option value="Fantasy">
            </datalist>
            <br><br>

            <label for="genreManual">Add Genres Manually (comma-separated):</label>
            <input type="text" id="genreManual" placeholder="e.g. Thriller, Mystery">
            <div class="error-msg" id="err-genres"></div><br>

            Adult (true/false): <input type="text" id="isAdult" value="false" placeholder="false"><div class="error-msg" id="err-isAdult"></div><br>

            Runtime Minutes: <input type="number" id="runtimeMinutes" required placeholder="e.g. 120"><div class="error-msg" id="err-runtimeMinutes"></div><br>

            Average Rating: <input type="number" step="0.1" id="averageRating" value="0" placeholder="e.g. 8.5"><div class="error-msg" id="err-averageRating"></div><br>

            Number Of Votes: <input type="number" id="numVotes" value="0" placeholder="e.g. 250000"><div class="error-msg" id="err-numVotes"></div><br>

            <button type="button" id="submitMovie">Send Movie</button>
        </form>
    </div>
    `;

    divAddAMovie.append(formAddMovieHtml);

    $("#submitMovie").click(() => {
        $(".error-msg").text(""); // Clear previous error messages

        const fields = [
            { id: "primaryTitle", required: true, regex: /^(?!\s*$).+/, msg: "Primary Title is required." },
            { id: "primaryImage", required: true, regex: /^(?!\s*$).+/, msg: "Primary Image is required." },
            { id: "year", required: true, regex: /^\d{4}$/, msg: "Year must be a 4-digit number." },
            { id: "releaseDate", required: true, regex: /^\d{4}-\d{2}-\d{2}$/, msg: "Release Date must be YYYY-MM-DD format." },
            { id: "language", required: true, regex: /^[a-zA-Z]+$/, msg: "Language must contain only letters." },
            { id: "budget", required: true, validate: val => /^\d+$/.test(val) && Number(val) >= 100000, msg: "Budget must be at least 100,000." },
            { id: "runtimeMinutes", required: true, regex: /^\d+$/, msg: "Runtime Minutes must be a number." },
            { id: "isAdult", required: false, regex: /^(true|false)$/i, msg: "IsAdult must be 'true' or 'false'." },
            { id: "averageRating", required: false, regex: /^\d+(\.\d+)?$/, msg: "Average Rating must be a valid number." },
            { id: "numVotes", required: false, regex: /^\d+$/, msg: "Number of Votes must be a valid number." }
        ];

        let hasError = false;

        fields.forEach(({ id, required, regex, validate, msg }) => {
            const input = $(`#${id}`);
            const value = input.val().trim();
            const errorDiv = $(`#err-${id}`);

            let isValid = true;

            if (required && !value) {
                isValid = false;
            } else if (value) {
                isValid = regex ? regex.test(value) : validate(value);
            }

            if (!isValid) {
                input.addClass("invalid").removeClass("valid");
                errorDiv.text(msg);
                hasError = true;
            } else {
                input.removeClass("invalid").addClass("valid");
                errorDiv.text("");
            }
        });

        // URL Validation
        const urlVal = $("#url").val().trim();
        if (urlVal && !/^https?:\/\/.+/.test(urlVal)) {
            $("#url").addClass("invalid").removeClass("valid");
            $("#err-url").text("URL must start with http:// or https://");
            hasError = true;
        } else {
            $("#url").removeClass("invalid").addClass("valid");
            $("#err-url").text("");
        }

        // Genres Validation
        const selectedFromDatalist = $("#genreDatalist").val().trim();
        const manualInput = $("#genreManual").val().trim();
        let genresCombined = [];

        if (manualInput) {
            genresCombined = manualInput.split(",").map(g => g.trim()).filter(g => g.length > 0);
        }

        if (selectedFromDatalist && !genresCombined.includes(selectedFromDatalist)) {
            genresCombined.unshift(selectedFromDatalist);
        }

        if (genresCombined.length > 0) {
            if (genresCombined.some(g => !/^[a-zA-Z\s\-]+$/.test(g))) {
                $("#genreManual").addClass("invalid").removeClass("valid");
                $("#err-genres").text("Genres must be letters, spaces, or hyphens.");
                hasError = true;
            } else {
                $("#genreManual").removeClass("invalid").addClass("valid");
                $("#err-genres").text("");
            }
        } else {
            $("#genreManual").removeClass("invalid").addClass("valid");
            $("#err-genres").text("");
        }
        
        const currentYear = new Date().getFullYear();

        // Year special check
        const yearInput = $("#year");
        const yearVal = yearInput.val().trim();
        if (!yearVal) {
            yearInput.addClass("invalid").removeClass("valid");
            $("#err-year").text("Year is required.");
            hasError = true;
        } else {
            const yearNumber = Number(yearVal);
            if (yearNumber > currentYear) {
                yearInput.addClass("invalid").removeClass("valid");
                $("#err-year").text(`Year cannot be greater than ${currentYear}.`);
                hasError = true;
            } else {
                yearInput.removeClass("invalid").addClass("valid");
                $("#err-year").text("");
            }
        }

        
        const today = new Date();
        today.setHours(0, 0, 0, 0); 

        // Release Date special check
        const releaseDateInput = $("#releaseDate");
        const releaseDateVal = releaseDateInput.val().trim();
        if (!releaseDateVal) {
            releaseDateInput.addClass("invalid").removeClass("valid");
            $("#err-releaseDate").text("Release Date is required.");
            hasError = true;
        } else {
            const releaseDateObj = new Date(releaseDateVal);
            if (releaseDateObj > today) {
                releaseDateInput.addClass("invalid").removeClass("valid");
                $("#err-releaseDate").text("Release date cannot be in the future.");
                hasError = true;
            } else {
                releaseDateInput.removeClass("invalid").addClass("valid");
                $("#err-releaseDate").text("");
            }
        }

        if (hasError) return;

        const movieToSend = {
            url: urlVal,
            primaryTitle: $("#primaryTitle").val().trim(),
            description: $("#description").val().trim(),
            primaryImage: $("#primaryImage").val().trim(),
            year: Number($("#year").val()),
            releaseDate: new Date($("#releaseDate").val()).toISOString(),
            language: $("#language").val().trim(),
            budget: Number($("#budget").val()),
            grossWorldwide: Number($("#grossWorldwide").val()) || 0,
            genres: genresCombined.join(", "),
            isAdult: $("#isAdult").val().toLowerCase() === "true",
            runtimeMinutes: Number($("#runtimeMinutes").val()),
            averageRating: Number($("#averageRating").val()) || 0,
            numVotes: Number($("#numVotes").val()) || 0
        };

        addToCart(movieToSend);
    });
});

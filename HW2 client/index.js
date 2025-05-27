$(document).ready(() => {
    const btnLoad = $("#loadMovies");
    const divCards = $("#movieCard");

    function isDevEnv() {
        return location.host.includes("localhost");
    }

    const port = 7110;
    const baseApiUrl = isDevEnv()
        ? `https://localhost:${port}`
        : "https://proj.ruppin.ac.il/cgroup9/test2/tar1";
    const url = `${baseApiUrl}/api/Movies`;

    let movies = [];

    btnLoad.click(function () {
        ajaxCall("GET", url, null,
            res => {
                movies = res;
                divCards.empty().append('<h2 class="fullRowTitle">All Movies</h2>');

                for (let i in movies) {
                    const m = movies[i];
                    let cardHtml = `
                        <div class="card">
                            <div class="topcard">
                                <button class="btnaddcart" data-index="${i}">Rent me</button>
                                <p class="rating">★${m.averageRating}/10</p>
                            </div>
                            <img class="movieimg" src="${m.primaryImage}" />
                            <h2>${m.primaryTitle}</h2>
                            <div class="shortinfo">
                                <p class="year">${m.startYear || new Date(m.releaseDate).getFullYear()}</p>
                                <p class="time">${m.runtimeMinutes} min</p>
                                <p class="isAdult">${m.isAdult ? "+18" : "All"}</p>
                            </div>
                            <p class="description">${m.description}</p>
                            <div class="geners">
                                ${(m.genres || "").split(',').map(g => `<p class="interests">${g.trim()}</p>`).join("")}
                            </div>
                            <div class="financial">
                                <div class="budget"><h2>Budget</h2><p>${m.budget}</p></div>
                                <div class="boxoffice"><h2>Box Office</h2><p>$${m.grossWorldwide}M</p></div>
                                <div class="votes"><h2>Votes</h2><p>${m.numVotes}</p></div>
                            </div>
                        </div>`;
                    divCards.append(cardHtml);
                }
            },
            err => alert("Failed to load movies: " + (err.responseText || err.statusText))
        );
    });

    let selectedMovie = null;
    $(document).on("click", ".btnaddcart", function () {
        const user = sessionStorage.getItem("currentUser");
        if (!user) {
            if (confirm("🔐 You must be logged in to rent movies.\nGo to login page?")) {
                location.href = "login.html";
            }
            return;
        }

        const index = $(this).data("index");
        selectedMovie = movies[index];
        console.log("🎬 Selected Movie:", selectedMovie);

        if (!selectedMovie || !selectedMovie.priceToRent || selectedMovie.priceToRent <= 0) {
            alert("❌ This movie has no rental price defined.");
            return;
        }

        const modalHtml = `
    <div class="modal-content">
        <span class="close" id="closeModal">&times;</span>
        <h2>🎮 Rent Movie</h2>
        <p id="rentMovieTitle">"${selectedMovie.primaryTitle}"</p>
        <form id="rentForm">
            <label>Start Date: <input type="date" id="rentStartDate" required></label><br><br>
            <label>End Date: <input type="date" id="rentEndDate" required></label><br><br>
            <p>Total Price: <span id="totalPriceDisplay">0</span> ₪</p>
            <button type="submit" id="confirmRentBtn">Confirm Rent</button>
            <button type="button" id="cancelRentBtn">Cancel</button>
        </form>
    </div>`;

        $("#rentModal").html(modalHtml).fadeIn();
    });



    // חישוב מחיר השכרה
    $(document).on("change", "#rentStartDate, #rentEndDate", function () {
        const start = new Date($("#rentStartDate").val());
        const end = new Date($("#rentEndDate").val());

        if (start && end && end > start) {
            const days = Math.ceil((end - start) / (1000 * 60 * 60 * 24));
            const total = days * selectedMovie.priceToRent;
            $("#totalPriceDisplay").text(total);
        } else {
            $("#totalPriceDisplay").text("0");
        }
    });

    $(window).on("click", function (event) {
        const modal = document.getElementById("rentModal");
        if (event.target === modal) {
            $("#rentModal").fadeOut();
        }
    });

    // שליחת הטופס
    $(document).on("submit", "#rentForm", function (e) {
        e.preventDefault();

        const user = JSON.parse(sessionStorage.getItem("currentUser"));
        if (!user || !selectedMovie) return;

        const rentStart = $("#rentStartDate").val();
        const rentEnd = $("#rentEndDate").val();
        const totalPrice = Number($("#totalPriceDisplay").text());

        if (!rentStart || !rentEnd || totalPrice === 0) {
            alert("⛔ Please select valid dates.");
            return;
        }

        const rentData = {
            userId: user.id,
            movieId: selectedMovie.id,
            rentStart,
            rentEnd,
            totalPrice
        };

        ajaxCall("POST", `${baseApiUrl}/api/Movies/rent`, JSON.stringify(rentData),
            () => {
                alert("✅ Rental successful!");
                $("#rentModal").fadeOut();
            },
            err => {
                alert("❌ Rental failed: " + (err.responseText || err.statusText));
            });
    });

    // סגירת מודאל
    $(document).on("click", "#cancelRentBtn, #closeModal", () => {
        $("#rentModal").fadeOut();
    });
});

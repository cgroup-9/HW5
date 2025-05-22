$(document).ready(() => {
    const btnLoad = $("#loadMovies");
    const divCards = $("#movieCard");
    const authBtn = $("#authBtn");

    function isDevEnv() {
        return location.host.includes('localhost');
    }

    const port = 7110;
    const baseApiUrl = isDevEnv()
        ? `https://localhost:${port}`
        : "https://proj.ruppin.ac.il/cgroup9/test2/tar1"; 
    const url = `${baseApiUrl}/api/Movies`;

    
    function updateAuthButton() {
        const user = sessionStorage.getItem("currentUser");
        const btn = $("#authBtn");

        if (user) {
            btn.text(`🚪 Logout (${user})`)
                .off("click").on("click", logout);
        } else {
            btn.text("🔐 Login")
                .off("click").on("click", () => location.href = "login.html");
        }
    }
    updateAuthButton();

    function addToCart(movie) {
        try {
            const movieToSend = {
                url: movie.url,
                primaryTitle: movie.primaryTitle,
                description: movie.description,
                primaryImage: movie.primaryImage,
                year: movie.startYear || new Date(movie.releaseDate).getFullYear(),
                releaseDate: new Date(movie.releaseDate).toISOString(),
                language: movie.language,
                budget: Number(movie.budget),
                grossWorldwide: Number(movie.grossWorldwide),
                genres: Array.isArray(movie.genres) ? movie.genres.join(", ") : movie.genres,
                isAdult: movie.isAdult === true || movie.isAdult === "true" || movie.isAdult === 1,
                runtimeMinutes: Number(movie.runtimeMinutes),
                averageRating: Number(movie.averageRating),
                numVotes: Number(movie.numVotes)
            };

            console.log("Sending movie:", JSON.stringify(movieToSend));
            ajaxCall("POST", url, JSON.stringify(movieToSend), addToCartSuc, addToCartFa);
        } catch (err) {
            console.error("❌ Error before POST:", err);
            alert("Failed sending movie information to the server. Please try again.");
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

    btnLoad.click(function () {
        divCards.empty();
        divCards.append('<h2 class="fullRowTitle">All Movies</h2>');

        for (let i in movies) {
            let cardHtml = `
                <div class="card">
                    <div class="topcard">
                        <button class="btnaddcart" data-index="${i}">Add to cart</button>
                        <p class="rating">★${movies[i].averageRating}/10</p>
                    </div>
                    <img class="movieimg" src="${movies[i].primaryImage}" />
                    <h2>${movies[i].primaryTitle}</h2>
                    <div class="shortinfo">
                        <p class="year">${movies[i].startYear}</p>
                        <p class="time">${movies[i].runtimeMinutes} min</p>
                        <p class="isAdult">${movies[i].contentRating}</p>
                    </div>
                    <p class="description">${movies[i].description}</p>
                    <div class="geners">
                        ${movies[i].interests.map(genre => `<p class="interests">${genre}</p>`).join("")}
                    </div>
                    <div class="financial">
                        <div class="budget">
                            <h2>Budget</h2>
                            <p>${movies[i].budget}</p>
                        </div>
                        <div class="boxoffice">
                            <h2>Box Office</h2>
                            <p>$${movies[i].grossWorldwide}M</p>
                        </div>
                        <div class="votes">
                            <h2>Votes</h2>
                            <p>${movies[i].numVotes}</p>
                        </div>
                    </div>
                </div>
            `;
            divCards.append(cardHtml);
        }

        $(".btnaddcart").click(function () {
            const user = sessionStorage.getItem("currentUser");
            if (!user) {
                if (confirm("🔐 You must be logged in to add movies to your cart.\nGo to login page?")) {
                    location.href = "login.html";
                }
                return;
            }

            const index = $(this).data("index");
            const movie = movies[index];
            addToCart(movie);
        });

    });
});

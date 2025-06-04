$(document).ready(() => {
    const divCards = $("#myMovieContainer");

    function isDevEnv() {
        return location.host.includes('localhost');
    }

    const port = 7110;
    const baseApiUrl = isDevEnv()
        ? `https://localhost:${port}`
        : "https://proj.ruppin.ac.il/cgroup9/test2/tar1";
    const baseUrl = `${baseApiUrl}/api/Movies`;

    function deleteMovie(userId, movieId) {
        const confirmDelete = confirm("Are you sure you want to delete this rented movie?");
        if (!confirmDelete) return;

        $.ajax({
            type: "DELETE",
            url: `${baseUrl}/delete-rented/${userId}/${movieId}`,
            success: function () {
                $(`button[data-id="${movieId}"]`).closest(".card").remove();
                alert("🎬 Rented movie deleted successfully.");
            },
            error: function (err) {
                alert("❌ Failed to delete rented movie: " + err.statusText);
            }
        });
    }

    function forwardMovie(userId, movieId) {
        const toUserName = prompt("Enter the username you want to forward the movie to:");
        if (!toUserName) return;

        $.ajax({
            type: "POST",
            url: `${baseUrl}/forward-rented`,
            contentType: "application/json",
            data: JSON.stringify({
                fromUserId: userId,
                movieId: movieId,
                toUserName: toUserName
            }),
            success: function (res) {
                alert(res.message || "✅ Movie forwarded successfully.");
                $(`button[data-id="${movieId}"]`).closest(".card").remove();
            },
            error: function (err) {
                alert("❌ Failed to forward movie: " + (err.responseJSON?.message || err.statusText));
            }
        });
    }

    function renderMovies(movies) {
        if (movies.length === 0) {
            alert("😕 No movies found in the database. You are being redirected to the main page.");
            window.location.href = "index.html";
            divCards.empty();
            return;
        }

        divCards.empty();
        divCards.append('<h2 class="fullRowTitle">My Movies</h2>');

        for (let i in movies) {
            let cardHtml = `
            <div class="card">
                <div class="topcard">
                    <p class="rating">★${movies[i].averageRating}/10</p>
                </div>
                <img class="movieimg" src="${movies[i].primaryImage}" />
                <h2>${movies[i].primaryTitle}</h2>
                <div class="shortinfo">
                    <p class="year">${movies[i].year}</p>
                    <p class="time">${movies[i].runtimeMinutes} min</p>
                    <p class="isAdult">${movies[i].isAdult}</p>
                </div>
                <p class="description">${movies[i].description}</p>
                <div class="geners">
                    ${movies[i].genres?.split(',').map(g => `<p class="interests">${g.trim()}</p>`).join("")}
                </div>
                <div class="financial">
                    <div class="budget">
                        <h2>Budget</h2>
                        <p>$${movies[i].budget}M</p>
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
                <div class="movie-actions">
                    <button class="deleteBtn" data-id="${movies[i].id}">🗑 Delete</button>
                    <button class="forwardBtn" data-id="${movies[i].id}">📤 Forward To</button>
                </div>
            </div>`;
            divCards.append(cardHtml);
        }

        $('.deleteBtn').click(function () {
            const movieId = $(this).data('id');
            const userId = currentUser.id;
            deleteMovie(userId, movieId);
        });

        $('.forwardBtn').click(function () {
            const movieId = $(this).data('id');
            const userId = currentUser.id;
            forwardMovie(userId, movieId);
        });
    }

    const currentUser = JSON.parse(sessionStorage.getItem("currentUser"));
    if (!currentUser) {
        alert("You must be logged in.");
        window.location.href = "login.html";
        return;
    }

    $.ajax({
        type: "GET",
        url: `${baseUrl}/rented/${currentUser.id}`,
        success: function (data) {
            renderMovies(data);
        },
        error: function (err) {
            alert("❌ Failed to load rented movies: " + err.statusText);
        }
    });
});

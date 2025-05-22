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

    document.getElementById("logoutBtn").addEventListener("click", logout);

  
    function deleteMovie(id) {
        $.ajax({
            type: "DELETE",
            url: `${baseUrl}/${id}`,
            success: function (updatedList) {
                renderMovies(updatedList);
            },
            error: function (err) {
                alert("❌ Failed to delete movie: " + err.statusText);
            }
        });
    }

    function renderMovies(movies, isSearch = false) {
        if (movies.length === 0) {
            if (isSearch) {
                alert("😕 No matching movies found.");
                return; 
            } else {
                alert("😕 No movies found in the database. You are being redirected to the main page.");
                window.location.href = "index.html";
                divCards.empty(); 
                return;
            }
        }

        divCards.empty();

        divCards.append('<h2 class="fullRowTitle">My Movies</h2>');

        let formMyMoviesHtml = `
        <div class="searchBy">
            <div class="searchBox">
                <input type="text" id="titleInput" class="searchTextBox" placeholder="Search by title">
                <button id="titleSearchBtn">🔍</button>
            </div>
            <div class="searchBox">
                <input type="date" id="startDateInput" class="searchTextBox">
                <input type="date" id="endDateInput" class="searchTextBox">
                <button id="dateSearchBtn">🔍</button>
            </div>
        </div>
        `;

        divCards.append(formMyMoviesHtml);

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
                <button class="deleteBtn" data-id="${movies[i].id}">🗑 Delete</button>
            </div>`;
            divCards.append(cardHtml);
        }

        // מחיקה
        $('.deleteBtn').click(function () {
            const id = $(this).data('id');
            deleteMovie(id);
        });

        // חיפוש לפי כותרת
        $('#titleSearchBtn').click(() => {
            const title = $('#titleInput').val().trim();
            if (title === "") return alert("📛 Please enter a title.");
            $.ajax({
                type: "GET",
                url: `${baseUrl}/search?title=${encodeURIComponent(title)}`,
                success: function (data) {
                    renderMovies(data, true); 
                },
                error: function (err) {
                    alert("❌ Title search failed: " + err.statusText);
                }
            });
        });

       
        $('#dateSearchBtn').click(() => {
            const start = $('#startDateInput').val();
            const end = $('#endDateInput').val();

            if (!start || !end) return alert("📅 Please provide both start and end dates.");
            $.ajax({
                type: "GET",
                url: `${baseUrl}/searchByPath/startDate(YYYY-MM-DD)/${start}/endDate/${end}`,
                success: function (data) {
                    renderMovies(data, true); 
                },
                error: function (err) {
                    alert("❌ Date search failed: " + err.statusText);
                }
            });
        });
    }

    
    $.ajax({
        type: "GET",
        url: baseUrl,
        success: function (data) {
            renderMovies(data);
        },
        error: function (err) {
            alert("❌ Failed to load movies: " + err.statusText);
        }
    });
});

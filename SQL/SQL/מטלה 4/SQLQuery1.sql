CREATE TABLE UsersTable (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NOT NULL,
    email NVARCHAR(100) NOT NULL,
    password NVARCHAR(100) NOT NULL,
    active BIT DEFAULT 1,
    deletedAt DATE DEFAULT NULL
);

CREATE TABLE MoviesTable (
    id INT IDENTITY(1,1) PRIMARY KEY,
    url NVARCHAR(MAX),
    primaryTitle NVARCHAR(255),
    description NVARCHAR(MAX),
    primaryImage NVARCHAR(MAX),
    year INT,
    releaseDate DATE,
    language NVARCHAR(50),
    budget FLOAT,
    grossWorldWide FLOAT,
    genres NVARCHAR(255),
    isAdult BIT,
    runtimeMinutes INT,
    averageRating FLOAT,
    numVotes INT,
    priceToRent INT DEFAULT ((ABS(CHECKSUM(NEWID())) % 21) + 10), 
    rentalCount INT DEFAULT 0,
    deletedAt DATE DEFAULT NULL
);

CREATE TABLE RentedMovie (
    userId INT NOT NULL,
    movieId INT NOT NULL,
    rentStart DATE NOT NULL,
    rentEnd DATE NOT NULL,
    totalPrice FLOAT NOT NULL,
    deletedAt DATE DEFAULT NULL,
    PRIMARY KEY (userId, movieId, rentStart), -- composite PK
    FOREIGN KEY (userId) REFERENCES UsersTable(id) ON DELETE CASCADE,
    FOREIGN KEY (movieId) REFERENCES MoviesTable(id) ON DELETE CASCADE
);

DELETE FROM UsersTable;

DBCC CHECKIDENT ('UsersTable', RESEED, 0);


ALTER TABLE UsersTable
ADD CONSTRAINT UQ_UsersTable_Name UNIQUE (name);

ALTER TABLE UsersTable
ADD CONSTRAINT UQ_UsersTable_Email UNIQUE (email);
-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Eden>
-- Create date: <04-05-25>
-- Description:	<Update movie in the database>
-- =============================================
ALTER PROCEDURE SP_UpdateMovie
    @movieID INT, 
    @url NVARCHAR(255),
    @primaryTitle NVARCHAR(255),
    @description NVARCHAR(255),
    @primaryImage NVARCHAR(255),
    @year INT,
    @releaseDate DATE,
    @language NVARCHAR(50),
    @budget FLOAT,
    @grossWorldwide FLOAT,
    @genres NVARCHAR(200),
    @isAdult BIT,
    @runtimeMinutes INT,
    @averageRating FLOAT,
    @numVotes INT,
    @priceToRent INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

	IF NOT EXISTS (
	        SELECT 1
			FROM MoviesTable 
			WHERE id = @movieID)
	BEGIN 
	    RAISERROR('Movie not found', 16, 1);
        RETURN;
    END

	    IF @primaryTitle IS NULL OR LEN(@primaryTitle) = 0
    BEGIN
        RAISERROR('Title is required', 16, 1);
        RETURN;
    END

    IF @primaryImage IS NULL OR LEN(@primaryImage) = 0
    BEGIN
        RAISERROR('Image is required', 16, 1);
        RETURN;
    END 

    IF @year IS NULL
    BEGIN
        RAISERROR('Year is required', 16, 1);
        RETURN;
    END 

    IF @releaseDate IS NULL OR @releaseDate > GETDATE()
    BEGIN
        RAISERROR('Invalid release date', 16, 1);
        RETURN;
    END

    IF @language IS NULL OR LEN(@language) = 0
    BEGIN
        RAISERROR('Language is required', 16, 1);
        RETURN;
    END

    IF @budget IS NOT NULL AND @budget < 100000
    BEGIN
        RAISERROR('Budget must be at least 100000 if provided', 16, 1);
        RETURN;
    END

    IF @grossWorldwide IS NULL
    BEGIN
        SET @grossWorldwide = 0; 
    END 

    IF @isAdult IS NULL
    BEGIN
        SET @isAdult = 0; -- Default to FALSE
    END 

    IF @runtimeMinutes IS NULL OR @runtimeMinutes <= 0
    BEGIN
        RAISERROR('Runtime must be a positive number', 16, 1);
        RETURN;
    END


    -- Update parameters
 UPDATE MoviesTable
    SET
        url = @url,
        primaryTitle = @primaryTitle,
        description = @description,
        primaryImage = @primaryImage,
        year = @year,
        releaseDate = @releaseDate,
        language = @language,
        budget = @budget,
        grossWorldwide = @grossWorldwide,
        genres = @genres,
        isAdult = ISNULL(@isAdult, 0), 
        runtimeMinutes = @runtimeMinutes,
        averageRating = @averageRating,
        numVotes = @numVotes,
        priceToRent = @priceToRent
    WHERE id = @movieID;

	 PRINT 'Movie updated successfully';
END
GO

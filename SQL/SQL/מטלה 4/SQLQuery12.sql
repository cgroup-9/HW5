USE [igroup109_test2]
GO
/****** Object:  StoredProcedure [dbo].[SP_InsertMovie]    Script Date: 22/05/2025 17:02:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Eden>
-- Create date: <04-05-2025>
-- Description:	<Inserts movies into the database>
-- =============================================
ALTER PROCEDURE [dbo].[SP_InsertMovie]
    -- Add the parameters for the stored procedure here
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
    @numVotes INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

	-- VALIDATION

	 DECLARE @priceToRent INT = FLOOR(RAND() * 21) + 10;
	 INSERT INTO MoviesTable( url, primaryTitle, description, primaryImage, year, releaseDate, language, budget,
        grossWorldwide, genres, isAdult, runtimeMinutes, averageRating, numVotes, priceToRent, rentalCount, deletedAt )
     values ( @url, @primaryTitle, @description, @primaryImage, @year, @releaseDate, @language, @budget,
        @grossWorldwide, @genres, @isAdult, @runtimeMinutes, @averageRating, @numVotes, @priceToRent, 0, NULL )
END

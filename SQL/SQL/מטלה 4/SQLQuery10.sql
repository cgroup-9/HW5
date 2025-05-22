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
-- Description:	<Inserts a new rented movie to the database>
-- =============================================
ALTER PROCEDURE SP_RentMovie 
	@userId INT,
    @movieId INT,
    @rentStart DATETIME,
    @rentEnd DATETIME,
    @totalPrice FLOAT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

	IF NOT EXISTS (
        SELECT 1
        FROM MoviesTable
        WHERE id = @movieId AND deletedAt IS NULL
    )
    BEGIN
        RAISERROR('Movie not found or already deleted', 16, 1);
        RETURN;
    END

	IF NOT EXISTS (
        SELECT 1
        FROM UsersTable
        WHERE id = @userId AND active = 1
    )
    BEGIN
        RAISERROR('User not found or inactive', 16, 1);
        RETURN;
    END

	IF EXISTS (
        SELECT 1
        FROM RentedMovie
        WHERE movieId = @movieId AND (rentEnd IS NULL OR rentEnd > GETDATE())
    )
    BEGIN
        RAISERROR('Movie is already rented', 16, 1);
        RETURN;
    END

    -- Insert statements for procedure here
	INSERT INTO RentedMovie (userId, movieId, rentStart, rentEnd, totalPrice, deletedAt)
    values (@userId, @movieId, @rentStart, @rentEnd, @totalPrice, NULL);

	UPDATE MoviesTable
    SET rentalCount = rentalCount + 1,
        grossWorldwide = ISNULL(grossWorldwide, 0) + @totalPrice
    WHERE id = @movieId;

	PRINT 'Movie rented successfully';
END
GO

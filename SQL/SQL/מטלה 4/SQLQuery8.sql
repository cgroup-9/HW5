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
-- Description:	<Update users database>
-- =============================================
ALTER PROCEDURE SP_UpdateUser 
	@name NVARCHAR(100),
    @email NVARCHAR(100),
    @password NVARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

	IF NOT EXISTS (
        SELECT 1
        FROM UsersTable
        WHERE email = @email AND deletedAt IS NULL
    )
    BEGIN
        RAISERROR('User not found or deleted', 16, 1);
        RETURN;
    END

    UPDATE UsersTable
    SET name = @name,
        password = @password
    WHERE email = @email;

    SELECT 'User updated successfully' AS Result;
END
GO

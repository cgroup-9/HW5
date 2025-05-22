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
-- Description:	<Inserts a new user into the database>
-- =============================================
ALTER PROCEDURE SP_InsertUser
    @name NVARCHAR(100),
    @email NVARCHAR(100),
    @password NVARCHAR(100)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

    --IF EXISTS (SELECT 1 FROM UsersTable WHERE name = @name)
    --BEGIN
    --    SELECT 'Username already exists' AS Result;
    --    RETURN;
    --END

    --IF EXISTS (SELECT 1 FROM UsersTable WHERE email = @email)
    --BEGIN
    --    SELECT 'Email already exists' AS Result;
    --    RETURN;
    --END

    INSERT INTO UsersTable (name, email, password, active)
    VALUES (@name, @email, @password, 1);

    SELECT 'User registered successfully' AS Result;
END
GO

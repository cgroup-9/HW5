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
-- Create date: <05-05-25>
-- Description:	<Login method to the database>
-- =============================================
CREATE PROCEDURE SP_LoginUser
	    @email NVARCHAR(100),
        @password NVARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

    SELECT Id, Name, Email, Active
    FROM UsersTable
    WHERE Email = @email AND Password = @password AND Active = 1
END
GO

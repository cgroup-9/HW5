USE [igroup109_test2]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:     <Mika>
-- Create date: <16-05-25>
-- Description: Returns all active users
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetAllUsers]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        Name,
        Email,
        Password,
        Active
    FROM 
        UsersTable
    WHERE 
        Active = 1
END

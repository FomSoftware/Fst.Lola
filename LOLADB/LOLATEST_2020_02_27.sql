﻿/*
Deployment script for LOLATEST

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "LOLATEST"
:setvar DefaultFilePrefix "LOLATEST"
:setvar DefaultDataPath "C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\"
:setvar DefaultLogPath "C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\"

GO
:on error exit
GO
/*
Detect SQLCMD mode and disable script execution if SQLCMD mode is not supported.
To re-enable the script after enabling SQLCMD mode, execute the following:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'SQLCMD mode must be enabled to successfully execute this script.';
        SET NOEXEC ON;
    END


GO

IF (DB_ID(N'$(DatabaseName)') IS NOT NULL)
BEGIN
    DECLARE @rc      int,                       -- return code
            @fn      nvarchar(4000),            -- file name for back up
            @dir     nvarchar(4000)             -- backup directory

    EXEC @rc = [master].[dbo].[xp_instance_regread] N'HKEY_LOCAL_MACHINE', N'Software\Microsoft\MSSQLServer\MSSQLServer', N'BackupDirectory', @dir output, 'no_output'
    if (@rc = 0) SELECT @dir = @dir + N'\'

    IF (@dir IS NULL)
    BEGIN 
        EXEC @rc = [master].[dbo].[xp_instance_regread] N'HKEY_LOCAL_MACHINE', N'Software\Microsoft\MSSQLServer\MSSQLServer', N'DefaultData', @dir output, 'no_output'
        if (@rc = 0) SELECT @dir = @dir + N'\'
    END

    IF (@dir IS NULL)
    BEGIN
        EXEC @rc = [master].[dbo].[xp_instance_regread] N'HKEY_LOCAL_MACHINE', N'Software\Microsoft\MSSQLServer\Setup', N'SQLDataRoot', @dir output, 'no_output'
        if (@rc = 0) SELECT @dir = @dir + N'\Backup\'
    END

    IF (@dir IS NULL)
    BEGIN
        SELECT @dir = N'$(DefaultDataPath)'
    END

    SELECT  @fn = @dir + N'$(DatabaseName)' + N'-' + 
            CONVERT(nchar(8), GETDATE(), 112) + N'-' + 
            RIGHT(N'0' + RTRIM(CONVERT(nchar(2), DATEPART(hh, GETDATE()))), 2) + 
            RIGHT(N'0' + RTRIM(CONVERT(nchar(2), DATEPART(mi, getdate()))), 2) + 
            RIGHT(N'0' + RTRIM(CONVERT(nchar(2), DATEPART(ss, getdate()))), 2) + 
            N'.bak' 
            BACKUP DATABASE [$(DatabaseName)] TO DISK = @fn
END
GO
USE [$(DatabaseName)];


GO
PRINT N'Dropping [dbo].[DF_MessageMachine_IsVisible]...';


GO
ALTER TABLE [dbo].[MessageMachine] DROP CONSTRAINT [DF_MessageMachine_IsVisible];


GO
PRINT N'Dropping [dbo].[FK_HistoryMessage_State]...';


GO
ALTER TABLE [dbo].[HistoryMessage] DROP CONSTRAINT [FK_HistoryMessage_State];


GO
PRINT N'Dropping [dbo].[FK_MessageMachine_State]...';


GO
ALTER TABLE [dbo].[MessageMachine] DROP CONSTRAINT [FK_MessageMachine_State];


GO
PRINT N'Dropping [dbo].[usp_AggregationMessage]...';


GO
DROP PROCEDURE [dbo].[usp_AggregationMessage];


GO
PRINT N'Altering [dbo].[HistoryMessage]...';


GO
ALTER TABLE [dbo].[HistoryMessage] DROP COLUMN [Code], COLUMN [ElapsedTime], COLUMN [Group], COLUMN [StateId], COLUMN [Type];


GO
PRINT N'Altering [dbo].[MessageMachine]...';


GO
ALTER TABLE [dbo].[MessageMachine] DROP COLUMN [Code], COLUMN [ElapsedTime], COLUMN [Group], COLUMN [IsVisible], COLUMN [StateId], COLUMN [Type];


GO
PRINT N'Altering [api].[usp_HistoricizingMessages]...';


GO
-- =============================================
-- Author:		Marco Servillo
-- Create date:	2017-12-05
-- Description:	Storicizzazione giornaliera tabella Message
-- =============================================
ALTER PROCEDURE [api].[usp_HistoricizingMessages](@machineId INT)
AS
         BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
             SET NOCOUNT ON;

    -- Insert statements for procedure here
             DECLARE @date DATETIME;
	    
/**/

             SELECT @date = isnull(MAX(day), 0)
             FROM [HistoryMessage]
             WHERE MachineId = @machineId;
    
/* DAY AGGREGATION */

             WITH CompleteHistoryMessage
                  AS (SELECT dbo.MessagesIndex.MessageCode as Code,
                             0 AS Count,
                             0 AS ElapsedTime,
                             CAST(Day AS DATE) AS Day,
                             MachineId,
                             (DATEPART(year, CAST(Day AS DATE)) * 10000) + (DATEPART(month, CAST(Day AS DATE)) * 100) + DATEPART(day, CAST(Day AS DATE)) AS Period,
                             
							 Params,
                             'd' AS TypeHistory,
							 MessagesIndexId
                      FROM [dbo].[MessageMachine]
					  JOIN dbo.MessagesIndex on dbo.MessagesIndex.Id = dbo.MessageMachine.MessagesIndexId
                      CROSS JOIN
(
    SELECT Id
    FROM MessageType
    WHERE Id IN(11, 12)
) AS MessageType
                      WHERE CAST(Day AS DATE) >= @date
                            AND MachineId = @machineId
                      GROUP BY CAST(Day AS DATE),
                               MachineId,
							   Params,
                               
							   
							   MessagesIndexId,
							   dbo.MessagesIndex.MessageCode)
                  MERGE [dbo].[HistoryMessage] AS target
                  USING
(
    SELECT IIF(b.Day IS NOT NULL, b.Count, a.Count) Count,
		   IIF(b.Day IS NOT NULL, b.Params, a.Params) Params,
           ISNULL(b.Day, a.Day) Day,
           IIF(b.Day IS NOT NULL, b.MachineId, a.MachineId) MachineId,
           IIF(b.Day IS NOT NULL, b.Period, a.Period) Period,
		   IIF(b.Day IS NOT NULL, b.TypeHistory, a.TypeHistory) TypeHistory,
		   IIF(b.Day IS NOT NULL, b.MessagesIndexId, a.MessagesIndexId) MessagesIndexId
    FROM CompleteHistoryMessage a
         LEFT JOIN
(
    SELECT COUNT(1) AS Count,
           CAST(Day AS DATE) AS Day,
		   Params,
           MachineId,
           (DATEPART(year, CAST(Day AS DATE)) * 10000) + (DATEPART(month, CAST(Day AS DATE)) * 100) + DATEPART(day, CAST(Day AS DATE)) AS Period,

           'd' AS TypeHistory,
		   MessagesIndexId
    FROM dbo.MessageMachine
	JOIN dbo.MessagesIndex on dbo.MessagesIndex.Id = dbo.MessageMachine.MessagesIndexId
    WHERE CAST(Day AS DATE) >= @date
          AND MachineId = @machineId
    GROUP BY CAST(Day AS DATE),
             MachineId,
			 Params,
			 MessagesIndexId
) b ON a.Day = b.Day
       AND a.MachineId = b.MachineId

	   AND a.Params = b.Params
	   and a.MessagesIndexId = b.MessagesIndexId
	   where b.count > 0
)  AS source
ON target.Day = source.Day
   AND target.MachineId = source.MachineId
   AND target.TypeHistory = source.TypeHistory
   AND target.Params = source.Params
   and target.MessagesIndexId = source.MessagesIndexId
                      WHEN MATCHED
                      THEN UPDATE SET
                                      target.Count = source.Count,
									  target.MessagesIndexId = source.MessagesIndexId
                      WHEN NOT MATCHED BY TARGET
                      THEN
                        INSERT(
                               Count,
                               Day,
                               MachineId,
                               Period,
							   Params,   
							   
                               TypeHistory,
							   MessagesIndexId)
                        VALUES
(
 Count,
 Day,
 MachineId,
 Period,
 Params,
 
 TypeHistory,
 MessagesIndexId
);

/**/

         END;
GO
PRINT N'Refreshing [dbo].[usp_MesUserMachines]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[usp_MesUserMachines]';


GO
PRINT N'Refreshing [api].[usp_HistoricizingAll]...';


GO
EXECUTE sp_refreshsqlmodule N'[api].[usp_HistoricizingAll]';


GO
PRINT N'Update complete.';


GO
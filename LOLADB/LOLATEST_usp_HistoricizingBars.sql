USE [LOLATEST]
GO
/****** Object:  StoredProcedure [api].[usp_HistoricizingBars]    Script Date: 29/09/2020 12:07:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Marco Servillo, Alten Italia
-- Create date:	2018-02-06
-- Description:	Storicizzazione giornaliera tabella Bar
-- =============================================
ALTER PROCEDURE [api].[usp_HistoricizingBars](@machineId INT)
AS
         BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
             SET NOCOUNT ON;

     -- Insert statements for procedure here
             DECLARE @date DATETIME;
	    
/**/

             SELECT @date = isnull(MAX(day), 0)
             FROM [HistoryBar]
             WHERE MachineId = @machineId;
    
/* DAY AGGREGATION */

             MERGE [dbo].[HistoryBar] AS target
             USING
(
    SELECT COUNT(1) AS Count,
           CAST(StartTime AS DATE) AS Day,
           SUM(IIF(isOffcut = 0, Length, 0)) AS Length,
           MachineId,
           SUM(IIF(isOffcut = 1, 1, 0)) AS OffcutCount,
           SUM(IIF(isOffcut = 1, Length, 0)) AS OffcutLength,
           (DATEPART(year, CAST(StartTime AS DATE)) * 10000) + (DATEPART(month, CAST(StartTime AS DATE)) * 100) + DATEPART(day, CAST(StartTime AS DATE)) AS Period,
           NULL AS System,
           'd' AS TypeHistory
    FROM dbo.Bar
    WHERE CAST(StartTime AS DATE) >= @date
          AND MachineId = @machineId
    GROUP BY CAST(StartTime AS DATE),
             MachineId
) AS source
             ON target.Day = source.Day
                AND target.MachineId = source.MachineId
                AND target.TypeHistory = source.TypeHistory
                 WHEN MATCHED
                 THEN UPDATE SET
                                 target.Count = source.Count,
                                 target.Length = source.Length,
                                 target.OffcutCount = source.OffcutCount,
                                 target.OffcutLength = source.OffcutLength
                 WHEN NOT MATCHED BY TARGET
                 THEN
                   INSERT(Count,
                          Day,
                          Length,
                          MachineId,
                          OffcutCount,
                          OffcutLength,
                          Period,
                          System,
                          TypeHistory)
                   VALUES
(Count,
 Day,
 Length,
 MachineId,
 OffcutCount,
 OffcutLength,
 Period,
 System,
 TypeHistory
);

/**/

         END;

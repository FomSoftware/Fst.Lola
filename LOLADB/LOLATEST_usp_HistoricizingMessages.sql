USE [LOLATEST]
GO
/****** Object:  StoredProcedure [api].[usp_HistoricizingMessages]    Script Date: 29/09/2020 12:42:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
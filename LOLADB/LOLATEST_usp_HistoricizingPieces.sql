USE [LOLATEST]
GO
/****** Object:  StoredProcedure [api].[usp_HistoricizingPieces]    Script Date: 21/09/2020 10:26:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Marco Servillo, Alten Italia
-- Create date:	2018-01-31
-- Description:	Storicizzazione giornaliera tabella Piece
-- =============================================
ALTER PROCEDURE [api].[usp_HistoricizingPieces](@machineId INT)
AS
         BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
             SET NOCOUNT ON;

     -- Insert statements for procedure here
             DECLARE @date DATETIME;
	    
/**/

             SELECT @date = isnull(MAX(Day), 0)
             FROM [HistoryPiece]
             WHERE MachineId = @machineId;
    
/* DAY AGGREGATION */

             MERGE [dbo].[HistoryPiece] AS target
             USING
(
    SELECT SUM(IIF(isRedone = 0, 1, 0)) AS CompletedCount,
		   CAST(Day AS DATE) AS Day,
           SUM(ElapsedTime) AS ElapsedTime,
           SUM(ElapsedTimeProducing) AS ElapsedTimeProducing,
           SUM(ElapsedTimeCut) AS ElapsedTimeCut,
           SUM(ElapsedTimeWorking) AS ElapsedTimeWorking,
           SUM(ElapsedTimeTrim) AS ElapsedTimeTrim,
           MachineId,
           Operator,
           (DATEPART(year, CAST(Day AS DATE)) * 10000) + (DATEPART(month, CAST(Day AS DATE)) * 100) + DATEPART(day, CAST(Day AS DATE)) AS Period,   
		    SUM(IIF(isRedone = 0, Length, 0)) AS PieceLengthSum,
		   SUM(IIF(isRedone = 1, 1, 0)) AS RedoneCount,
           Shift,
           'd' AS TypeHistory
    FROM dbo.Piece
    WHERE CAST(Day AS DATE) >= @date
          AND MachineId = @machineId
    GROUP BY CAST(Day AS DATE),
             MachineId,
             CUBE(Operator, Shift)
) AS source
             ON target.Day = source.Day
                AND target.MachineId = source.MachineId
                AND (target.Operator = source.Operator
                     OR (target.Operator IS NULL
                         AND source.Operator IS NULL))
                AND (target.Shift = source.Shift
                     OR (target.Shift IS NULL
                         AND source.Shift IS NULL))
                AND target.TypeHistory = source.TypeHistory
                 WHEN MATCHED
                 THEN UPDATE SET
                                 target.CompletedCount = source.CompletedCount,
								 target.ElapsedTime = source.ElapsedTime,
                                 target.ElapsedTimeProducing = source.ElapsedTimeProducing,
                                 target.ElapsedTimeCut = source.ElapsedTimeCut,
                                 target.ElapsedTimeWorking = source.ElapsedTimeWorking,
                                 target.ElapsedTimeTrim = source.ElapsedTimeTrim,
								 target.PieceLengthSum = source.PieceLengthSum,
								 target.RedoneCount = source.RedoneCount
                 WHEN NOT MATCHED BY TARGET
                 THEN
                   INSERT(CompletedCount,
						  Day,
                          ElapsedTime,
                          ElapsedTimeProducing,
                          ElapsedTimeCut,
                          ElapsedTimeWorking,
                          ElapsedTimeTrim,
                          MachineId,
                          Operator,
                          Period,  
						  PieceLengthSum,
						  RedoneCount,
                          Shift,
                          TypeHistory)
                   VALUES
(CompletedCount,
 Day,
 ElapsedTime,
 ElapsedTimeProducing,
 ElapsedTimeCut,
 ElapsedTimeWorking,
 ElapsedTimeTrim,
 MachineId,
 Operator,
 Period,
 PieceLengthSum,
 RedoneCount,
 Shift,
 TypeHistory
);

/**/

         END;
USE [LOLATEST]
GO
/****** Object:  StoredProcedure [dbo].[usp_AggregationPiece]    Script Date: 17/09/2020 15:11:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Marco Servillo, Alten Italia
-- Create date:	2018-02-06
-- Description:	Aggregati day/week/month/quarter/year tabella Piece
-- =============================================
ALTER PROCEDURE [dbo].[usp_AggregationPiece] @machineId   INT,
                                             @startDate   DATETIME,
                                             @endDate     DATETIME,
                                             @aggregation INT      = 0,
                                             @dataType    INT      = 0
AS
         BEGIN

/************************************************************/

/************************ WORKAROUND ************************/

/************************************************************/

--             SET FMTONLY OFF;
--             CREATE TABLE #TempRisultato
--([Id]                   [INT] NOT NULL,
-- [CompletedCount]       [INT] NULL,
-- [Day]                  [DATETIME] NULL,
-- [ElapsedTime]          [BIGINT] NULL,
-- [ElapsedTimeProducing] [BIGINT] NULL,
-- [ElapsedTimeCut]       [BIGINT] NULL,
-- [ElapsedTimeWorking]   [BIGINT] NULL,
-- [ElapsedTimeTrim]      [BIGINT] NULL,
-- [MachineId]            [INT] NULL,
-- [Operator]             [VARCHAR](100) NULL,
-- [Period]               [INT] NULL,
-- [PieceLengthSum]       [INT] NULL,
-- [RedoneCount]          [INT] NULL,
-- [Shift]                [INT] NULL,
-- [System]               [VARCHAR](100) NULL,
-- [TypeHistory]          [CHAR](1) NULL,
--);

--/**/

--             SELECT *
--             FROM #TempRisultato;
/*********************************************************/

/************************ DEFAULT ************************/

/*********************************************************/

             SET NOCOUNT ON;
		   
/***********************************************************************/

/************************ DASHBOARD AGGREGATION ************************/

/***********************************************************************/

             IF @dataType = 0
                 BEGIN
			  
/**/

                     SELECT MAX(Id) AS Id,
                            SUM(CompletedCount) AS CompletedCount,
                            MAX(Day) AS Day,
                            SUM(ElapsedTime) AS ElapsedTime,
                            SUM(ElapsedTimeProducing) AS ElapsedTimeProducing,
                            SUM(ElapsedTimeCut) AS ElapsedTimeCut,
                            SUM(ElapsedTimeWorking) AS ElapsedTimeWorking,
                            SUM(ElapsedTimeTrim) AS ElapsedTimeTrim,                        
                            MachineId,
                            Operator,
                            NULL AS Period,
                            SUM(PieceLengthSum) AS PieceLengthSum,
                            SUM(RedoneCount) AS RedoneCount,
                            Shift,
							System,
                            'd' AS TypeHistory
                     FROM dbo.[HistoryPiece]
                     WHERE Day BETWEEN @startDate AND @endDate
                           AND MachineId = @machineId
                           AND Shift IS NULL
                           AND Operator IS NULL
                     GROUP BY MachineId,
                              Operator,
                              Shift,
							  System;
						
/**/

                 END;
                 ELSE
		   
/*********************************************************************/

/************************ HISTORY AGGREGATION ************************/

/*********************************************************************/

             IF @dataType = 1
                 BEGIN
			  
/*****************************************************************/

/************************ DAY AGGREGATION ************************/

/*****************************************************************/

                     IF @aggregation = 0
                         BEGIN
			  
/**/

                             SELECT a.[Id],
                                    [CompletedCount],
                                    [Day],
                                    [ElapsedTime],
                                    [ElapsedTimeProducing],
                                    [ElapsedTimeCut],
                                    [ElapsedTimeWorking],
                                    [ElapsedTimeTrim],
                                    [MachineId],
                                    [Operator],
                                    [Period],
                                    [PieceLengthSum],
                                    [RedoneCount],
                                    [Shift],
									[System],
                                    [TypeHistory]
                             FROM [dbo].[HistoryPiece] a
                             WHERE Day BETWEEN @startDate AND @endDate
                                   AND MachineId = @machineId
                                   AND Shift IS NULL
                                   AND Operator IS NULL;
			  
/**/

                         END;
                         ELSE
		   
/******************************************************************/

/************************ WEEK AGGREGATION ************************/

/******************************************************************/

                     IF @aggregation = 1
                         BEGIN
			  
/**/

                             SELECT MAX(Id) AS Id,
                                    SUM(CompletedCount) AS CompletedCount,
                                    MAX(Day) AS Day,
                                    SUM(ElapsedTime) AS ElapsedTime,
                                    SUM(ElapsedTimeProducing) AS ElapsedTimeProducing,
                                    SUM(ElapsedTimeCut) AS ElapsedTimeCut,
                                    SUM(ElapsedTimeWorking) AS ElapsedTimeWorking,
                                    SUM(ElapsedTimeTrim) AS ElapsedTimeTrim,
                                    MachineId,
                                    Operator,
                                    (DATEPART(YEAR, Day) * 100) + DATEPART(ISO_WEEK, Day) AS Period,
                                    SUM(PieceLengthSum) AS PieceLengthSum,
                                    SUM(RedoneCount) AS RedoneCount,
                                    Shift,
									System,
                                    'w' AS TypeHistory
                             FROM dbo.[HistoryPiece]
                             WHERE Day BETWEEN @startDate AND @endDate
                                   AND MachineId = @machineId
                                   AND Shift IS NULL
                                   AND Operator IS NULL
                             GROUP BY(DATEPART(YEAR, Day) * 100) + DATEPART(ISO_WEEK, Day),
                                     MachineId,
                                     Operator,
                                     Shift,
									 System;
						
/**/

                         END;
                         ELSE
		   
/*******************************************************************/

/************************ MONTH AGGREGATION ************************/

/*******************************************************************/

                     IF @aggregation = 2
                         BEGIN
			  
/**/

                             SELECT MAX(Id) AS Id,
                                    SUM(CompletedCount) AS CompletedCount,
                                    MAX(Day) AS Day,
                                    SUM(ElapsedTime) AS ElapsedTime,
                                    SUM(ElapsedTimeProducing) AS ElapsedTimeProducing,
                                    SUM(ElapsedTimeCut) AS ElapsedTimeCut,
                                    SUM(ElapsedTimeWorking) AS ElapsedTimeWorking,
                                    SUM(ElapsedTimeTrim) AS ElapsedTimeTrim,
                                    MachineId,
                                    Operator,
                                    (DATEPART(YEAR, Day) * 100) + DATEPART(MONTH, Day) AS Period,
                                    SUM(PieceLengthSum) AS PieceLengthSum,
                                    SUM(RedoneCount) AS RedoneCount,
                                    Shift,
									System,
                                    'm' AS TypeHistory
                             FROM dbo.[HistoryPiece]
                             WHERE Day BETWEEN @startDate AND @endDate
                                   AND MachineId = @machineId
                                   AND Shift IS NULL
                                   AND Operator IS NULL
                             GROUP BY(DATEPART(YEAR, Day) * 100) + DATEPART(MONTH, Day),
                                     MachineId,
                                     Operator,
                                     Shift,
									 System;
			  
/**/

                         END;
                         ELSE
		   
/*********************************************************************/

/************************ QUARTER AGGREGATION ************************/

/*********************************************************************/

                     IF @aggregation = 3
                         BEGIN
			  
/**/

                             SELECT MAX(Id) AS Id,
                                    SUM(CompletedCount) AS CompletedCount,
                                    MAX(Day) AS Day,
                                    SUM(ElapsedTime) AS ElapsedTime,
                                    SUM(ElapsedTimeProducing) AS ElapsedTimeProducing,
                                    SUM(ElapsedTimeCut) AS ElapsedTimeCut,
                                    SUM(ElapsedTimeWorking) AS ElapsedTimeWorking,
                                    SUM(ElapsedTimeTrim) AS ElapsedTimeTrim,
                                    MachineId,
                                    Operator,
                                    (DATEPART(YEAR, Day) * 100) + DATEPART(QUARTER, Day) AS Period,
                                    SUM(PieceLengthSum) AS PieceLengthSum,
                                    SUM(RedoneCount) AS RedoneCount,
                                    Shift,
									System,
                                    'q' AS TypeHistory
                             FROM dbo.[HistoryPiece]
                             WHERE Day BETWEEN @startDate AND @endDate
                                   AND MachineId = @machineId
                                   AND Shift IS NULL
                                   AND Operator IS NULL
                             GROUP BY(DATEPART(YEAR, Day) * 100) + DATEPART(QUARTER, Day),
                                     MachineId,
                                     Operator,
                                     Shift,
									 System;
			  
/**/

                         END;
                         ELSE
		   
/******************************************************************/

/************************ YEAR AGGREGATION ************************/

/******************************************************************/

                     IF @aggregation = 4
                         BEGIN
			  
/**/

                             SELECT MAX(Id) AS Id,
                                    SUM(CompletedCount) AS CompletedCount,
                                    MAX(Day) AS Day,
                                    SUM(ElapsedTime) AS ElapsedTime,
                                    SUM(ElapsedTimeProducing) AS ElapsedTimeProducing,
                                    SUM(ElapsedTimeCut) AS ElapsedTimeCut,
                                    SUM(ElapsedTimeWorking) AS ElapsedTimeWorking,
                                    SUM(ElapsedTimeTrim) AS ElapsedTimeTrim,
                                    MachineId,
                                    Operator,
                                    DATEPART(YEAR, Day) AS Period,
                                    SUM(PieceLengthSum) AS PieceLengthSum,
                                    SUM(RedoneCount) AS RedoneCount,
                                    Shift,
									System,
                                    'y' AS TypeHistory
                             FROM dbo.[HistoryPiece]
                             WHERE Day BETWEEN @startDate AND @endDate
                                   AND MachineId = @machineId
                                   AND Shift IS NULL
                                   AND Operator IS NULL
                             GROUP BY DATEPART(YEAR, Day),
                                      MachineId,
                                      Operator,
                                      Shift,
									  System;
			  
/**/

                         END;
                 END;
                 ELSE
		   
/*******************************************************************************/

/************************ OPERATOR OR SHIFT AGGREGATION ************************/

/*******************************************************************************/

             IF @dataType = 2
                OR @dataType = 3
                 BEGIN

/**/

                     SELECT MAX(Id) AS Id,
                            SUM(CompletedCount) AS CompletedCount,
                            MAX(Day) AS Day,
                            SUM(ElapsedTime) AS ElapsedTime,
                            SUM(ElapsedTimeProducing) AS ElapsedTimeProducing,
                            SUM(ElapsedTimeCut) AS ElapsedTimeCut,
                            SUM(ElapsedTimeWorking) AS ElapsedTimeWorking,
                            SUM(ElapsedTimeTrim) AS ElapsedTimeTrim,
                            MachineId,
                            Operator,
                            NULL AS Period,
                            SUM(PieceLengthSum) AS PieceLengthSum,
                            SUM(RedoneCount) AS RedoneCount,
                            Shift,
							System,
                            'd' AS TypeHistory
                     FROM dbo.[HistoryPiece]
                     WHERE Day BETWEEN @startDate AND @endDate
                           AND MachineId = @machineId
                           AND ((@dataType = 2
                                 AND Shift IS NULL
                                 AND Operator IS NOT NULL)
                                OR (@dataType = 3
                                    AND Shift IS NOT NULL
                                    AND Operator IS NULL))
                     GROUP BY MachineId,
                              Operator,
                              Shift,
							  System;
						
/**/

                 END;
         END;
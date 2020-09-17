USE [LOLATEST]
GO
/****** Object:  StoredProcedure [dbo].[usp_AggregationBar]    Script Date: 17/09/2020 14:26:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Marco Servillo, Alten Italia
-- Create date:	2018-02-06
-- Description:	Aggregati day/week/month/quarter/year tabella Bar
-- =============================================
ALTER PROCEDURE [dbo].[usp_AggregationBar] @machineId   INT,
                                           @startDate   DATETIME,
                                           @endDate     DATETIME,
                                           @aggregation INT      = 0
AS
         BEGIN

/************************************************************/

/************************ WORKAROUND ************************/

/************************************************************/

--             SET FMTONLY OFF;
--             CREATE TABLE #TempRisultato
--([Id]           [INT] NOT NULL,
-- [Count]        [INT] NULL,
-- [Day]          [DATETIME] NULL,
-- [Length]       [FLOAT] NULL,
-- [MachineId]    [INT] NULL,
-- [OffcutCount]  [INT] NULL,
-- [OffcutLength] [INT] NULL,
-- [Period]       [INT] NULL,
-- [System]       [VARCHAR](100) NULL,
-- [TypeHistory]  [CHAR](1) NULL,
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

             SELECT MAX(Id) AS Id,
                    COUNT(1) AS Count,
                    MAX(Day) AS Day,
                    SUM(Length) AS Length,
                    MachineId,
                    SUM(OffcutCount) AS OffcutCount,
                    SUM(OffcutLength) AS OffcutLength,
                    NULL AS Period,
                    NULL AS System,
                    'd' AS TypeHistory
             FROM dbo.[HistoryBar]
             WHERE Day BETWEEN @startDate AND @endDate
                   AND MachineId = @machineId
             GROUP BY MachineId;
			  
/**/

         END;

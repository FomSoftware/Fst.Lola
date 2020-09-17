USE [LOLATEST]
GO
/****** Object:  StoredProcedure [dbo].[usp_AggregationJob]    Script Date: 17/09/2020 14:20:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Marco Servillo, Alten Italia
-- Create date:	2018-02-06
-- Description:	Aggregati tabella Job
-- =============================================
ALTER PROCEDURE [dbo].[usp_AggregationJob] @machineId   INT,
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
--([Id]                [INT] NOT NULL,
-- [Code]              [VARCHAR](100) NULL,
-- [Day]               [DATETIME] NULL,
-- [ElapsedTime]       [BIGINT] NULL,
-- [MachineId]         [INT] NULL,
-- [Period]            [INT] NULL,
-- [PiecesProduced]    [INT] NULL,
-- [PiecesProducedDay] [INT] NULL,
-- [TotalPieces]       [INT] NULL,
-- [TypeHistory]       [CHAR](1) NULL
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

    SELECT 
	MAX(a.Id) AS Id, 
	a.Code, 
	MAX(a.Day) AS DAY, 
	sum(ElapsedTime) as ElapsedTime,
	a.MachineId,
	NULL AS Period,
	sum(a.PiecesProduced) as PiecesProduced,
	MAX(TotalPieces) as TotalPieces,
	'd' AS TypeHistory
FROM (
			  SELECT MAX(Id) AS Id,
                    Code,
                    MAX(Day) AS Day,					
                    MAX(ElapsedTime) AS ElapsedTime,
                    MachineId,
                    NULL AS Period,
                    MAX([PiecesProduced]) AS PiecesProduced,
                    TotalPieces,
                    'd' AS TypeHistory
             FROM dbo.[HistoryJob]
             WHERE Day BETWEEN @startDate AND @endDate
                   AND  MachineId = @machineId
             GROUP BY MachineId,
                      code,
                      TotalPieces
					  ) as a
group by a.MachineId, a.code
			  
			  
/**/

         END;

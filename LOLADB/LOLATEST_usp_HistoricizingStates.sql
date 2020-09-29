USE [LOLATEST]
GO
/****** Object:  StoredProcedure [api].[usp_HistoricizingStates]    Script Date: 29/09/2020 12:44:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Marco Servillo, Alten Italia
-- Create date:	2017-12-05
-- Description:	Storicizzazione giornaliera tabella State
-- =============================================
ALTER PROCEDURE [api].[usp_HistoricizingStates](@machineId INT)
AS
         BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
             SET NOCOUNT ON;
             DECLARE @date DATETIME;
	    
/**/

             SELECT @date = isnull(MAX(day), 0)
             FROM [HistoryState]
             WHERE MachineId = @machineId;
    
/* DAY AGGREGATION */

             WITH CompleteHistoryState
                  AS (SELECT CAST(StateMachine.Day AS DATE) Day,
                             0 AS ElapsedTime,
                             MachineId,
                             StateMachine.Operator,
                             IIF(State.Id = 1, 0, NULL) AS OverfeedAvg,
                             StateMachine.Shift,
                             (DATEPART(year, CAST(StateMachine.Day AS DATE)) * 10000) + (DATEPART(month, CAST(StateMachine.Day AS DATE)) * 100) + DATEPART(day, CAST(StateMachine.Day AS DATE)) AS Period,
                             State.Id AS StateId,
                             'd' AS TypeHistory
                      FROM dbo.StateMachine
                           CROSS JOIN State
                      WHERE MachineId = @machineId-- AND CAST(StateMachine.Day AS DATE) >= @date
                             
                      GROUP BY CAST(StateMachine.Day AS DATE),
                               MachineId,
                               CUBE(StateMachine.Operator, StateMachine.Shift),
                               State.Id)
                  MERGE [dbo].[HistoryState] AS target
                  USING
(
    SELECT ISNULL(b.Day, a.Day) Day,
           IIF(b.Day IS NOT NULL, b.ElapsedTime, a.ElapsedTime) ElapsedTime,
           IIF(b.Day IS NOT NULL, b.MachineId, a.MachineId) MachineId,
           IIF(b.Day IS NOT NULL, b.Operator, a.Operator) Operator,
           IIF(b.Day IS NOT NULL, b.OverfeedAvg, a.OverfeedAvg) OverfeedAvg,
           IIF(b.Day IS NOT NULL, b.Shift, a.Shift) Shift,
           IIF(b.Day IS NOT NULL, b.Period, a.Period) Period,
           IIF(b.Day IS NOT NULL, b.StateId, a.StateId) StateId,
           IIF(b.Day IS NOT NULL, b.TypeHistory, a.TypeHistory) TypeHistory
    FROM CompleteHistoryState a
         LEFT JOIN
(
    SELECT CAST(StateMachine.Day AS DATE) Day,
           SUM(StateMachine.ElapsedTime) AS ElapsedTime,
           MachineId,
           StateMachine.Operator,
		 IIF(StateId = 1, IIF(SUM(ElapsedTime) > 0, SUM(Overfeed * ElapsedTime) / SUM(ElapsedTime), 0), NULL) AS OverfeedAvg,
           StateMachine.Shift,
           (DATEPART(year, CAST(StateMachine.Day AS DATE)) * 10000) + (DATEPART(month, CAST(StateMachine.Day AS DATE)) * 100) + DATEPART(day, CAST(StateMachine.Day AS DATE)) AS Period,
           StateId,
           'd' AS TypeHistory
    FROM dbo.StateMachine
    WHERE MachineId = @machineId-- AND CAST(StateMachine.Day AS DATE) >= @date
    GROUP BY CAST(StateMachine.Day AS DATE),
             MachineId,
             CUBE(StateMachine.Operator, StateMachine.Shift),
             StateId
) b ON a.Day = b.Day
       AND a.MachineId = b.MachineId
       AND (a.Operator = b.Operator
            OR (a.Operator IS NULL
                AND b.Operator IS NULL))
       AND (a.Shift = b.Shift
            OR (a.Shift IS NULL
                AND b.Shift IS NULL))
       AND a.StateId = b.StateId
) AS source
ON target.Day = source.Day
   AND target.MachineId = source.MachineId
   AND (target.Operator = source.Operator
        OR (target.Operator IS NULL
            AND source.operator IS NULL))
   AND (target.Shift = source.Shift
        OR (target.Shift IS NULL
            AND source.Shift IS NULL))
   AND target.StateId = source.StateId
   AND target.TypeHistory = source.TypeHistory
                      WHEN MATCHED
                      THEN UPDATE SET
                                      target.ElapsedTime = source.ElapsedTime,
                                      target.OverfeedAvg = source.OverfeedAvg
                      WHEN NOT MATCHED BY TARGET
                      THEN
                        INSERT(Day,
                               ElapsedTime,
                               MachineId,
                               Operator,
                               OverfeedAvg,
                               Period,
                               Shift,
                               StateId,
                               TypeHistory)
                        VALUES
(Day,
 ElapsedTime,
 MachineId,
 Operator,
 OverfeedAvg,
 Period,
 Shift,
 StateId,
 TypeHistory
);

/**/

         END;
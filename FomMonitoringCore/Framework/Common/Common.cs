
using FomMonitoringCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FomMonitoringCore.Framework.Common
{
    public class Common
    {
        public const string Administrator = "e7d3e769f3f593dadcb8634cc5b09fc90dd3a61c4a06a79cb0923662fe6fae6b";
        public const string Operator = "291101a07fe980e93b900ae85c9eb824f9e7e93d0d754be7440b9386b615cad7";
        public const string HeadWorkshop = "ede02f346410664d40c90884131c4ed86aa5fe2b6ea6d711b4c02d6f0282c1df";
        public const string Assistance = "1b451b10ab6a95f1e24e659edc7315c7fd2930d8db77cbeea3315fdf4fe6a5c6";
        public const string Customer = "bf3763383aaf43069885db20b386631c6d5d8b8481df2a26769e9de5fe2f9c82";
        public const string Demo = "8a2cc0673b1c428315fe84c0138d95c3ddda30baf81e7d9aa821f1ca47098193";

        /// <summary>
        /// Get the startOfWeek date of the specified year and week number
        /// </summary>
        /// <param name="year">Date</param>
        /// <param name="week">Week</param>
        /// <param name="startOfWeek">Day of the week</param>
        /// <returns>Start of week's date</returns>
        public static DateTime StartOfWeek(int year, int week, DayOfWeek startOfWeek)
        {
            DateTime firstDayOfYear = new DateTime(year, 1, 1);
            int weekFirstOfYear = firstDayOfYear.GetWeekNumber();
            DateTime firstDayOfFirstWeek = firstDayOfYear.StartOfWeek(startOfWeek);
            return firstDayOfFirstWeek.AddDays((week - weekFirstOfYear) * 7);
        }

        /// <summary>
        /// Get the startOfWeek date of the specified year and week number
        /// </summary>
        /// <param name="dt">Date</param>
        /// <param name="week">Week</param>
        /// <param name="lastOfWeek">Day of the week</param>
        /// <returns>Last of week's date</returns>
        public static DateTime LastOfWeek(int year, int week, DayOfWeek lastOfWeek)
        {
            DateTime firstDayOfYear = new DateTime(year, 1, 1);
            int weekFirstOfYear = firstDayOfYear.GetWeekNumber();
            DateTime firstDayOfFirstWeek = firstDayOfYear.LastOfWeek(lastOfWeek);
            return firstDayOfFirstWeek.AddDays((week - weekFirstOfYear) * 7);
        }

        /// <summary>
        /// Ritorna il valore percentuale del value passato come parametro
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="total">total</param>
        /// <returns>valore percentuale</returns>
        public static decimal GetPercentage(long? value, long? total)
        {
            decimal result = 0;

            if (value != null && total != null)
            {
                if (total != 0)
                    result = (decimal)value * 100 / (decimal)total;
            }

            return result;
        }

        public static decimal GetPercentage(double? value, double? total)
        {
            decimal result = 0;

            if (value != null && total != null)
            {
                if (total != 0)
                    result = (decimal)value * 100 / (decimal)total;
            }

            return result;
        }

        /// <summary>
        /// Ritorna il valore percentuale del value passato come parametro
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="total">total</param>
        /// <returns>valore percentuale</returns>
        public static decimal GetPercentage(int value, int total)
        {
            if (total == 0)
                return 0;
            else
                return (decimal)value * 100 / (decimal)total;
        }

        /// <summary>
        /// Ritorna il valore dei pezzi fatti all'ora (pezzi/tempo)
        /// </summary>
        /// <param name="pieces">pieces</param>
        /// <param name="time">time</param>
        /// <returns>valore int</returns>
        public static int? GetRatioProductivity(int? pieces, long? time)
        {
            int? result = null;

            if (pieces != null && time != null)
            {
                if (time == 0)
                    result = 0;
                else
                {
                    double TotalHours = new TimeSpan(time.Value).TotalHours;

                    /*if (TotalHours < 1)
                        TotalHours = 1;*/

                    result = ((decimal)(pieces.Value / TotalHours)).RoundToInt();
                }
            }

            return result;
        }

        /// <summary>
        /// Ritorna la granularità con cui mostrare i dati di trend
        /// </summary>
        /// <param name="StartDate">Date</param>
        /// <param name="EndDate">Week</param>
        /// <returns>Tipo del periodo</returns>
        public static enAggregation GetAggregationType(DateTime StartDate, DateTime EndDate)
        {
            enAggregation granularity = enAggregation.Day;

            int diffDays = (EndDate - StartDate).Days;
            int diffMonth = EndDate.MonthDifference(StartDate);
            int diffYear = EndDate.Year - StartDate.Year;

            if (diffDays >= 0 && diffDays < 7)
                granularity = enAggregation.Day;
            else if (diffDays >= 7 && diffMonth <= 1)
                granularity = enAggregation.Week;
            else if (diffMonth > 1 && diffMonth < 7)
                granularity = enAggregation.Month;
            else if (diffMonth >= 7)
                granularity = enAggregation.Quarter;
            else if (diffYear > 1)
                granularity = enAggregation.Year;

            return granularity;
        }

        /// <summary>
        /// Ritorna la data inziale del trend sula base della data di fine e la granularità
        /// </summary>
        /// <param name="EndDate">EndDate</param>
        /// <param name="Granularity">Granularity</param>
        /// <returns>Data di inizio</returns>

        public static DateTime GetStartDateTrend(DateTime EndDate, DateTime StartDate, enAggregation Granularity)
        {

            var daysDiff = (EndDate - StartDate).Days;

            if(daysDiff < 6)
            {
                return EndDate.AddDays(-6).Date;
            }
            else
            {
                return StartDate;
            }
        }

        /// <summary>
        /// Ritorna l'unità di misura del tempo da utilizzare nei grafici
        /// </summary>
        /// <param name="elapsed">trascorso medio</param>
        /// <returns>Unità di misura</returns>
        public static enMeasurement GetTimeMeasurement(long elapsed)
        {
            enMeasurement measurement = enMeasurement.Seconds;

            TimeSpan ElapsedDateTime = new TimeSpan(elapsed);

            if (ElapsedDateTime.TotalSeconds >= 0 && ElapsedDateTime.TotalMinutes < 1)
                measurement = enMeasurement.Seconds;
            else if (ElapsedDateTime.TotalMinutes >= 1 && ElapsedDateTime.TotalHours < 1)
                measurement = enMeasurement.Minutes;
            else if (ElapsedDateTime.TotalHours >= 1 && ElapsedDateTime.TotalDays < 1)
                measurement = enMeasurement.Hours;
            else if (ElapsedDateTime.TotalDays >= 1)
                measurement = enMeasurement.Days;

            return measurement;
        }

        /// <summary>
        /// Converte i valori di elapsed time in base all'unità di misura del tempo
        /// </summary>
        /// <param name="elapsedList"></param>
        /// <returns>Dati su unità di tempo</returns>
        public static List<int> ConvertElapsedByMeasurement(List<long> elapsedList, enMeasurement measurement)
        {
            List<double> result = new List<double>();

            switch (measurement)
            {
                case enMeasurement.Seconds:
                    result = elapsedList.Select(s => new TimeSpan(s).TotalSeconds).ToList();
                    break;
                case enMeasurement.Minutes:
                    result = elapsedList.Select(s => new TimeSpan(s).TotalMinutes).ToList();
                    break;
                case enMeasurement.Hours:
                    result = elapsedList.Select(s => new TimeSpan(s).TotalHours).ToList();
                    break;
                case enMeasurement.Days:
                    result = elapsedList.Select(s => new TimeSpan(s).TotalDays).ToList();
                    break;
                default:
                    break;
            }
            //return result.Select(s => s.RoundToInt()).ToList();
            //is-1254
            return result.Select(s => (int)Math.Floor(s)).ToList();
        }
    }
}

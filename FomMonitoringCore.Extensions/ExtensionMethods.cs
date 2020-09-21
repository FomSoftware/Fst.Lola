using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringCore.Extensions
{
    public static class ExtensionMethods
    {



        /// <summary>
        /// Get the startOfWeek date of the specified date
        /// </summary>
        /// <param name="dt">Date</param>
        /// <param name="startOfWeek">Day of the week</param>
        /// <returns>Start of week's date</returns>
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }

        /// <summary>
        /// Get the lastOfWeek date of the specified date
        /// </summary>
        /// <param name="dt">Date</param>
        /// <param name="lastOfWeek">Day of the week</param>
        /// <returns>Last of week's date</returns>
        public static DateTime LastOfWeek(this DateTime dt, DayOfWeek lastOfWeek)
        {
            int diff = lastOfWeek - dt.DayOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(diff).Date;
        }

        /// <summary>
        /// Get the last day of month date of the specified date
        /// </summary>
        /// <param name="dt">Date</param>
        /// <returns>Last day of month date</returns>
        public static DateTime LastOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month + 1, 1).AddDays(-1).Date;
        }

        /// <summary>
        /// Get the week number of the specified date
        /// </summary>
        /// <param name="dt">Date</param>
        /// <returns>Week number</returns>
        public static int GetWeekNumber(this DateTime dt)
        {
            // Get jan 1st of the year
            DateTime startOfYear = dt.AddDays(-dt.Day + 1).AddMonths(-dt.Month + 1);
            // Get dec 31st of the year
            DateTime endOfYear = startOfYear.AddYears(1).AddDays(-1);
            // ISO 8601 weeks start with Monday 
            // The first week of a year includes the first Thursday 
            // DayOfWeek returns 0 for sunday up to 6 for saturday
            int[] iso8601Correction = { 6, 7, 8, 9, 10, 4, 5 };
            int nds = dt.Subtract(startOfYear).Days + iso8601Correction[(int)startOfYear.DayOfWeek];
            int wk = nds / 7;
            int result = 0;
            switch (wk)
            {
                case 0:
                    // Return weeknumber of dec 31st of the previous year
                    result = GetWeekNumber(startOfYear.AddDays(-1));
                    break;
                case 53:
                    // If dec 31st falls before thursday it is week 01 of next year
                    if (endOfYear.DayOfWeek < DayOfWeek.Thursday)
                    {
                        result = 1;
                    }
                    else
                    {
                        result = wk;
                    }
                    break;
                default:
                    result = wk;
                    break;
            }
            return result;
        }

        public static int GetQuarter(this DateTime dt)
        {
            int result = 0;
            result = (int)Math.Ceiling((decimal)dt.Month / 3);
            return result;
        }


    }
}

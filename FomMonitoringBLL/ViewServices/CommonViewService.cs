using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringBLL.ViewServices
{
    public class CommonViewService
    {
        public static KPIViewModel getKpiViewModel(decimal? Value, double? GreenThreshold, double? YellowThreshold)
        {
            ThresholdViewModel threshold = new ThresholdViewModel();
            threshold.green = GreenThreshold;
            threshold.yellow = YellowThreshold;

            KPIViewModel result = new KPIViewModel();
            result.value = Value;
            result.threshold = threshold;

            return result;
        }


        public static TimeViewModel getTimeViewModel(long? ElapsedTime)
        {
            TimeViewModel result = new TimeViewModel();

            if (ElapsedTime != null)
            {
                if (ElapsedTime < 0)
                    ElapsedTime = 0;

                TimeSpan ElapsedDateTime = new TimeSpan(ElapsedTime.Value);

                result.days = ElapsedDateTime.Days == 0 ? null : ElapsedDateTime.Days.ToString();
                result.hours = ElapsedDateTime.Hours == 0 ? null : ElapsedDateTime.Hours.ToString();
                result.minutes = ElapsedDateTime.Minutes == 0 ? null : ElapsedDateTime.Minutes.ToString();
                result.seconds = ElapsedDateTime.Seconds.ToString();

                result.elapsed = ElapsedTime.Value;
            }

            return result;
        }


        public static List<string> GetTimeCategories(List<DateTime> days, enAggregation granularity)
        {
            List<string> categories = new List<string>();

            switch (granularity)
            {
                case enAggregation.Day:
                    categories = days.Select(s => s.ToString("dd/MM")).ToList();
                    break;
                case enAggregation.Week:
                    categories = days.Select(s => string.Format("w{0}", s.GetWeekNumber())).Distinct().ToList();
                    break;
                case enAggregation.Month:
                    categories = days.Select(s => s.ToString("MMM", CultureInfo.InvariantCulture)).Distinct().ToList();
                    break;
                case enAggregation.Quarter:
                    break;
                case enAggregation.Year:
                    categories = days.Select(s => s.ToString("yyyy", CultureInfo.InvariantCulture)).Distinct().ToList();
                    break;
                default:
                    break;
            }

            return categories;
        }

        public static enToolType GetTypeTool(ToolMachineModel tool)
        {
            enToolType type = enToolType.Breaking;

            if (tool.IsBroken)
                type = enToolType.Breaking;

            if (tool.IsRevised)
                type = enToolType.Replacement;

            return type;
        }


        public static string GetColorState(enState state)
        {
            string Color = string.Empty;

            switch (state)
            {
                case enState.Production:
                    Color = "#A5CC48";
                    break;
                case enState.Pause:
                    Color = "#FFE941";
                    break;
                case enState.Error:
                    Color = "#cc3333";
                    break;
                case enState.Manual:
                    Color = "#98C2ED";
                    break;
                default:
                    Color = "#aaa";
                    break;
            }

            return Color;
        }


        public static string GetColorChart(enSerieProd serie)
        {
            string Color = string.Empty;

            switch (serie)
            {
                case enSerieProd.Efficiency:
                    Color = "#A5CC48";
                    break;
                case enSerieProd.GrossTime:
                    Color = "#588FA4";
                    break;
                case enSerieProd.NetTime:
                    Color = "#AEDFDE";
                    break;
            }

            return Color;
        }


        public static string GetColorAlarm(enState state)
        {
            string Color = string.Empty;

            switch (state)
            {
                case enState.Pause:
                    Color = "#ec0";
                    break;
                case enState.Error:
                    Color = "#cc3333";
                    break;
                default:
                    Color = "#aaa";
                    break;
            }

            return Color;
        }

  
    }
}

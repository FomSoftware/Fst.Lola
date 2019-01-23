using FomMonitoringResources;
using System.ComponentModel.DataAnnotations;

namespace FomMonitoringBLL.ViewModel
{
    public class I18nViewModel
    {
       
    }

    public class CalendarI18nModel
    {
        public string Today
        {
            get
            {
                return Resource.Today;
            }
        }

        public string Yesterday
        {
            get
            {
                return Resource.Yesterday;
            }
        }

        public string Last7Days
        {
            get
            {
                return Resource.Last7Days;
            }
        }

        public string Last30Days
        {
            get
            {
                return Resource.Last30Days;
            }
        }

        public string ThisMonth
        {
            get
            {
                return Resource.ThisMonth;
            }
        }

        public string LastMonth
        {
            get
            {
                return Resource.LastMonth;
            }
        }

        public string ApplyLabel
        {
            get
            {
                return Resource.ApplyLabel;
            }
        }

        public string CancelLabel
        {
            get
            {
                return Resource.CancelLabel;
            }
        }

        public string CustomRangeLabel
        {
            get
            {
                return Resource.CustomRangeLabel;
            }
        }
    }

    public class EfficiencyI18nModel
    {
        
    }

    public class ProductivityI18nModel
    {
     
    }

    public class AlarmI18nModel
    {
      
    }

    public class SpindleI18nModel
    {
      
    }

    public class ToolI18nModel
    {
     
    }

    public class JobI18nModel
    {
       
    }

}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FomMonitoringCore.SqlServer;

namespace FomMonitoringCore.Framework.Model
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public bool RememberMe { get; set; }


        public List<Languages> AllLanguages { get; set; }

        public Languages ActualLanguage { get; set; }

        public List<Faq> ExternalFaqs { get; set; }
    }
}
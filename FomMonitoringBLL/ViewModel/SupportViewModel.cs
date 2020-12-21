using System.Collections.Generic;
using System.IO;
using System.Web;

namespace FomMonitoringBLL.ViewModel
{
    public class SupportViewModel
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Azienda { get; set; }
        public string Prefisso { get; set; }
        public string Telefono { get; set; }
        public string NomeMacchina { get; set; }
        public string Seriale { get; set; }
        public string Testo { get; set; }
        public HttpPostedFileBase[] File { get; set; }

    }
}

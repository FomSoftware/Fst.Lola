using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UserManager.Service.Concrete
{
    public class UtilServices
    {
        public static string RenderControl(Table table)
        {
            var sb = new StringBuilder();
            var tw = new StringWriter(sb);
            var hw = new HtmlTextWriter(tw, string.Empty);
            hw.NewLine = string.Empty;

            table.RenderControl(hw);
            return sb.ToString();
        }
    }
}

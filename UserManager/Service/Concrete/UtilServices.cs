﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UserManager.Service.Concrete
{
    public class UtilServices
    {
        public static string RenderControl(Table table)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter tw = new StringWriter(sb);
            HtmlTextWriter hw = new HtmlTextWriter(tw, string.Empty);
            hw.NewLine = string.Empty;

            table.RenderControl(hw);
            return sb.ToString();
        }
    }
}
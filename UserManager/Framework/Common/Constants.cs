using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UserManager.Framework.Common
{
     public static class Constants
    {
        //Gestione QueryString
        public const string EscapeQuestionMark = "%3F";
        public const string EscapeCommercialE = "%26";
        public const string EscapePlus = "%2B";

        public const string LogoutToken = "Logout";
        public const string CheckUserSessionToken = "CheckUserSession";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;

namespace UserManager.Framework.Util
{
    public static class ExtensionMethods
    {

        /// <summary>
        /// Restituisce l'indirizzo IP della connessione dell'utente che si sta autenticando
        /// </summary>
        /// <returns>Restituisce </returns>
        public static string GetUserConnectionIP()
        {
            return System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }

        #region Guid Manager

        private static Regex isGuid = new Regex(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$", RegexOptions.Compiled);

        /// <summary>
        /// Controllolla se il GUID passato è valido
        /// </summary>
        /// <param name="candidate">La stringa candicata ad essere controllata</param>
        /// <param name="output">Restituisce il GUID sul quale è stato effettuato un parsing</param>
        /// <returns>True se il GUID è valido, restituisce False se il GUID non è valido</returns>
        public static bool CheckGuidIsValid(string candidate, out Nullable<Guid> output)
        {
            bool isValid = false;
            output = Guid.Empty;
            if (candidate != null)
            {
                if (isGuid.IsMatch(candidate))
                {
                    output = new Guid(candidate);
                    isValid = true;
                }
            }
            return isValid;
        }

        /// <summary>
        /// Controlla se il GUID è valido e se non è di tipo Empty
        /// </summary>
        /// <param name="candidate">La stringa candicata ad essere controllata</param>
        /// <param name="output">Restituisce il GUID sul quale è stato effettuato un parsing</param>
        /// <returns>True se il GUID è valido, restituisce False se il GUID non è valido avendo cura anche del controllo dell'empty</returns>
        public static bool CheckGuidIsValidAndNotEmpty(string candidate, out Nullable<Guid> output)
        {
            bool isValid = CheckGuidIsValid(candidate, out output);
            if (Guid.Equals(output, Guid.Empty)) { output = null; isValid = false; }
            return isValid;
        }

        #endregion

    }
}

using CommonCore.Service;
using FomMonitoringCore.Framework.Common;
using System;
using System.Net;
using System.Net.Http;

namespace FomMonitoringCore.Service.APIClient.Concrete
{
    internal class CUrlService
    {
        /// <summary>
        /// Costruisce l'URL che recupera i dati
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns>L'URL sotto forma di stringa</returns>
        private static string BuildURL(string queryString)
        {
            string result = string.Empty;
            try
            {
                string baseUrl = ApplicationSettingService.GetWebConfigKey("UrlApi");
                result = string.Concat(baseUrl, queryString);
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), queryString);
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        /// <summary>
        /// Esegue richieste GET dell'URL passato come parametro
        /// </summary>
        /// <param name="queryString"></param>
        /// <param name="cycles"></param>
        /// <returns>Stringa Json dei dati raccolti, altrimenti stringa vuota</returns>
        public static string ExecutePrincipalCUrl(string queryString, int cycles = 1)
        {
            string result = string.Empty;
            HttpResponseMessage response = null;
            if (cycles > 0)
            {
                AsyncHelpersService.RunSync(async () =>
                {
                    string url = string.Empty;
                    try
                    {
                        response = new HttpResponseMessage();
                        url = BuildURL(queryString);

                        HttpClientHandler authtHandler = new HttpClientHandler()
                        {
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential("mservillo", "", "vm-bsc02"),

                        };
                        using (HttpClient client = new HttpClient(authtHandler))
                        {
                            client.Timeout = new TimeSpan(0, 8, 20);
                            response = await client.GetAsync(url);
                            if (response.IsSuccessStatusCode)
                            {
                                result = await response.Content.ReadAsStringAsync();
                            }
                            else
                            {
                                response.EnsureSuccessStatusCode();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!ex.Message.Contains("500"))
                        {
                            cycles--;
                        }
                        else
                        {
                            cycles = 0;
                        }
                        string errMessage = string.Format(ex.GetStringLog(), queryString, cycles.ToString());
                        LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
                        Console.Error.WriteLine(errMessage);
                        Console.Error.WriteLine();
                        if (cycles > 0)
                        {
                            result = ExecutePrincipalCUrl(queryString, cycles);
                        }
                    }
                });
            }
            return result;
        }
    }
}

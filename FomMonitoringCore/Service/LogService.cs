using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringCore.Service
{
    public static class LogService
    {
        #region Variable Private

        /// <summary>
        /// Livello di Severity
        /// </summary>
        public enum TypeLevel
        {
            Debug = 1,
            Info = 2,
            Warning = 3,
            Error = 4,
            Fatal = 5
        };

        #endregion

        #region Method Static

        /// <summary>
        /// Restituisce un'istanza Logger
        /// </summary>
        /// <returns></returns>
        private static ILog GetLogger()
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();
                return LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            }
            catch
            {
                return null;
            }

        }

        /// <summary>
        /// Scrive il file di Log
        /// </summary>
        /// <param name="message">Messaggio da scrivere</param>
        public static void WriteLog(String message)
        {
            GetLogger().Info(message);
        }

        /// <summary>
        /// Scrive il file di Log
        /// </summary>
        /// <param name="message">Messaggio da scrivere</param>
        /// <param name="level">Livello di gravità</param>
        public static void WriteLog(String message, TypeLevel level)
        {
            switch ((int)level)
            {
                case 1:
                    GetLogger().Debug(message);
                    break;
                case 2:
                    GetLogger().Info(message);
                    break;
                case 3:
                    GetLogger().Warn(message);
                    break;
                case 4:
                    GetLogger().Error(message);
                    break;
                case 5:
                    GetLogger().Fatal(message);
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="exception"></param>
        public static void WriteLog(String message, TypeLevel level, Exception exception)
        {
            switch ((int)level)
            {
                case 1:
                    GetLogger().Debug(message, exception);
                    break;
                case 2:
                    GetLogger().Info(message, exception);
                    break;
                case 3:
                    GetLogger().Warn(message, exception);
                    break;
                case 4:
                    GetLogger().Error(message, exception);
                    break;
                case 5:
                    GetLogger().Fatal(message, exception);
                    break;
            }
        }

        #endregion
    }
}

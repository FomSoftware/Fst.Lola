using FomMonitoringCore.DAL;
using FomMonitoringCore.Service;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FomMonitoringCore.Framework.Common
{
    public class ReadMessages
    {
        static string dbConn = ApplicationSettingService.GetWebConfigKey("DbMessagesConnectionString");

        public static int ReadMessageScope(string code, FST_FomMonitoringEntities ent)
        {
            int result = 0;


            //dbConn = @"Provider=Microsoft.ACE.OLEDB.10.0;Data Source=C:\Lola\Fst.Lola\DatabaseMessaggiLola\messaggiLOLA.mdb;Persist Security Info=False;";
            /*dbConn = "DSN=prova;";
            using (OdbcConnection connection = new OdbcConnection(dbConn))
            {               
                var query = "Select scope From messaggi where Id = \'@id\'";
                OdbcCommand command = new OdbcCommand(query);
                OdbcParameter param0 = new OdbcParameter("@id", OdbcType.VarChar);
                param0.Value = code;
                command.Parameters.Add(param0);
                // Set the Connection to the new OleDbConnection.
                command.Connection = connection;
                // Open the connection and execute the insert command.
                try
                {
                    connection.Open();
                    using (OdbcDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            result = reader.GetInt32(0);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    result = 0;
                }
                finally
                {
                    connection.Close();
                }

            }*/

            result = ent.Database.SqlQuery<int>("Select scope from AnagMessages where id = @id", new SqlParameter("@id", code)).FirstOrDefault();

            return result;
        }

        public static string GetMessageDescription(string code, string parameters, string language)
        {
            string result = "";
            using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
            {
                string separator = "#-#";
                result = ent.Database.SqlQuery<string>($"Select {language} from AnagMessages where id = @id", new SqlParameter("@id", code)).FirstOrDefault();

                if (result.IndexOf(separator) > -1)
                {
                    string res = "";
                    string[] splitted = Regex.Split(result, separator);
                    string[] splittedParams = parameters.Split(',');
                    if (splitted.Length < splittedParams.Length)
                        return result;

                    for (int i = 0; i < splitted.Length; i++)
                    {
                        if (i < splittedParams.Length)
                            res += splitted[i] + splittedParams[i];
                        else
                            res += splitted[i];
                    }
                    result = res;
                }
            }

            return result;
        }       

        public static string ReplaceFirstOccurrence(string source, string find, string replace)
        {
            int place = source.IndexOf(find);
            string result = source.Remove(place, find.Length).Insert(place, replace);

            return result;
        }

    }
}


using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using MyTrackerLibrary.DataAccess;
namespace MyTrackerLibrary
{
    public static class GlobalConfig
    {
        public static IDataConnection Connection { get; private set; }
        public static void InitializeConnections(DatabaseType db)
        {
            if (db==DatabaseType.Sql)
            {
                // TODO Set up the SQL Connector properly
                SqlConnector sql = new SqlConnector();
                Connection=sql;
            }
            else if (db==DatabaseType.TextFile)
            {
                // TODO Create the Text connection
                TextConnector text = new TextConnector();
                Connection=text;
            }
        }

        public static string CnnString(string name)
        {
            
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
}

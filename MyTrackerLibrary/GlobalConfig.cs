
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using MyTrackerLibrary.DataAccess;
namespace MyTrackerLibrary
{
    public static class GlobalConfig
    {

        public const string PrizesFile = "PrizeModels.csv";
        public const string PeopleFile = "PersonModels.csv";
        public const string TeamsFile = "TeamModels.csv";
        public const string TournamentsFile = "TournamentModels.csv";
        public const string MatchupsFile = "MatchupModels.csv";
        public const string MatchupEntriesFile = "MatchupEntryModels.csv";
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

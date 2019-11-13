

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using MyTrackerLibrary.Models;

namespace MyTrackerLibrary.DataAccess
{
   

    public class SqlConnector : IDataConnection
    {

        private const String db = "Tournaments";
        
        /// <summary>
        /// Saves a new prize to the database
        /// </summary>
        /// <param name="model">The prize information.</param>
        /// <returns>The prize information, including the unique identifier.</returns>
        public void CreatePrize(PrizeModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {

                var p = new DynamicParameters();
                p.Add("@PlaceNumber", model.PlaceNumber);
                p.Add("@PlaceName", model.PlaceName);
                p.Add("@PrizeAmount", model.PrizeAmount);
                p.Add("@PrizePercentage", model.PrizePercentage);
                p.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spPrizes_Insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@Id");

            }
            
        }

        /// <summary>
        /// Saves a new person to the database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void CreatePerson(PersonModel model)
        {

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {

                var p = new DynamicParameters();
                p.Add("@FirstName", model.FirstName);
                p.Add("@LastName", model.LastName);
                p.Add("@EmailAddress", model.EmailAddress);
                p.Add("@CellphoneNumber", model.CellphoneNumber);
                p.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spPeople_Insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@Id");



            }

        }

        /// <summary>
        /// Saves a new team to the database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        public void CreateTeam(TeamModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {

                var t = new DynamicParameters();
                t.Add("@Team_Name", model.Team_Name);
                t.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spTeams_Insert", t, commandType: CommandType.StoredProcedure);

                model.Id = t.Get<int>("@Id");

                foreach (PersonModel tm in model.TeamMembers)
                {
                    t  = new DynamicParameters();
                    t.Add("@Team_Id", model.Id);
                    t.Add("@Person_Id", tm.Id);
                    t.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                    connection.Execute("dbo.spTeam_Members_Insert", t, commandType: CommandType.StoredProcedure);
                }

            }

        }

        /// <summary>
        /// Saves a new tournament to the database
        /// </summary>
        /// <param name="model"></param>
        public void CreateTournament(TournamentModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {

                SaveTournament(connection, model);

                SaveTournamentPrizes(connection, model);

                SaveTournamentEntries(connection, model);

                SaveTournamentRounds(connection, model);

                

            }

            TournamentLogic.UpdateTournamentResults(model);



        }
        /// <summary>
        /// Inserts a new Tournament entry to the Tournament table
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="model"></param>
        private void SaveTournament(IDbConnection connection,TournamentModel model)
        {
            var t = new DynamicParameters();
            t.Add("@TournamentName", model.TournamentName);
            t.Add("@EntryFee", model.EntryFee);
            t.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

            connection.Execute("dbo.spTournaments_Insert", t, commandType: CommandType.StoredProcedure);

            model.Id = t.Get<int>("@Id");


        }

        /// <summary>
        /// Inserts the tournaments prizes to the database
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="model"></param>
        private void SaveTournamentPrizes(IDbConnection connection, TournamentModel model)
        {
            foreach (PrizeModel pz in model.Prizes)
            {
                var t = new DynamicParameters();
                t.Add("@Tournament_Id", model.Id);
                t.Add("@Prize_Id", pz.Id);
                t.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spTournamentPrizes_Insert", t, commandType: CommandType.StoredProcedure);

            }

        }
        /// <summary>
        /// Inserts the tournaments entries to the database
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="model"></param>
        private void SaveTournamentEntries(IDbConnection connection, TournamentModel model)
        {
            foreach (TeamModel tm in model.EnteredTeams)
            {
               var t = new DynamicParameters();
                t.Add("@Team_Id", tm.Id);
                t.Add("@Tournament_Id", model.Id);
                t.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spTournamentEntries_Insert", t, commandType: CommandType.StoredProcedure);
            }

        }

        /// <summary>
        /// Saves all the tournament information to the database.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="model"></param>
        private void SaveTournamentRounds(IDbConnection connection, TournamentModel model)
        {
                

                foreach ( List<MatchupModel> round in model.Rounds) // goes through each round
                {
                    foreach(MatchupModel matchup in round) //saving all the matchups
                    {
                        
                        var t = new DynamicParameters();                         
                        t.Add("@Tournament_Id", model.Id);
                        t.Add("@MatchupRound", matchup.MatchupRound);
                        t.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                        connection.Execute("dbo.spMatchups_Insert", t, commandType: CommandType.StoredProcedure);

                        matchup.Id = t.Get<int>("@Id");

                        foreach (MatchupEntryModel entry in matchup.Entries) //saving the entries for each matchup
                        {
                            t = new DynamicParameters();
                            t.Add("@Matchup_Id", matchup.Id);
                            if (entry.ParentMatchup==null)
                                t.Add("@ParentMatchup_Id", null);
                            else
                                t.Add("@ParentMatchup_Id",entry.ParentMatchup.Id);
                            if (entry.TeamCompeting == null)
                                t.Add("@TeamCompeting_Id", null);
                            else
                                t.Add("@TeamCompeting_Id", entry.TeamCompeting.Id);
                            t.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                           
                            connection.Execute("dbo.spMatchupEntries_Insert", t, commandType: CommandType.StoredProcedure);

                            entry.Id = t.Get<int>("@Id");
                    }

                        
                    }
                }

        }

        /// <summary>
        /// Returns all the people saved in the database
        /// </summary>
        /// <returns>A list with every person</returns>
        public List<PersonModel> GetPerson_All()
        {
            List<PersonModel> output;
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<PersonModel>("dbo.spPeople_GetAll").ToList();
            }

            return output;
        }
        /// <summary>
        /// Returns all the teams saved in the database
        /// </summary>
        /// <returns>A list with every team</returns>
        public List<TeamModel> GetTeam_All()
        {
            List<TeamModel> output;
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<TeamModel>("dbo.spTeams_GetAll").ToList();
                foreach(TeamModel t in output)
                {
                    var p = new DynamicParameters();
                    p.Add("@Team_Id", t.Id);
                    t.TeamMembers= connection.Query<PersonModel>("dbo.spTeamMembers_GetByTeam",p, commandType: CommandType.StoredProcedure).ToList();
                }
            }

            return output;
        }
        /// <summary>
        /// Returns all the tournaments saved in the database
        /// </summary>
        /// <returns>A list with every tournament</returns>
        public List<TournamentModel> GetTournament_All()
        {
            List<TournamentModel> tournaments;
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                tournaments = connection.Query<TournamentModel>("dbo.spTournaments_GetAll").ToList();


                foreach (TournamentModel t in tournaments)
                {
                    var p = new DynamicParameters();
                    p.Add("Tournament_Id", t.Id);
                    t.Prizes = connection.Query<PrizeModel>("dbo.spPrizes_GetByTournament", p, commandType: CommandType.StoredProcedure).ToList();
             
                    t.EnteredTeams = connection.Query<TeamModel>("dbo.spTeams_GetByTournament", p, commandType: CommandType.StoredProcedure).ToList();

                    foreach (TeamModel team in t.EnteredTeams)
                    {
                        p = new DynamicParameters();
                        p.Add("@Team_Id", team.Id);
                        team.TeamMembers = connection.Query<PersonModel>("dbo.spTeamMembers_GetByTeam", p, commandType: CommandType.StoredProcedure).ToList();
                    }

                    p = new DynamicParameters();
                    p.Add("Tournament_Id", t.Id);
                    List <MatchupModel> matchups = connection.Query<MatchupModel>("dbo.spMatchups_GetByTournament", p, commandType: CommandType.StoredProcedure).ToList();

                    foreach (MatchupModel m in matchups)
                    {
                        p = new DynamicParameters();
                        p.Add("Matchup_Id", m.Id);

                        m.Entries = connection.Query<MatchupEntryModel>("dbo.spMatchupEntries_GetByMatchup", p, commandType: CommandType.StoredProcedure).ToList();

                        if (m.Winner_Id > 0)                       
                            m.Winner = t.EnteredTeams.Where(x => x.Id == m.Winner_Id).First();                       
                        
                        foreach (MatchupEntryModel entry in m.Entries)
                        {
                            if (entry.TeamCompeting_Id>0)
                             entry.TeamCompeting = t.EnteredTeams.Where(x => x.Id == entry.TeamCompeting_Id).First();

                            if (entry.ParentMatchup_Id>0)
                             entry.ParentMatchup = matchups.Where(x => x.Id == entry.ParentMatchup_Id).First();

                        }
                    }

                    List<MatchupModel> currRow = new List<MatchupModel>();
                    int currRound = 1;
                    foreach (MatchupModel m in matchups)
                    {
                        if (m.MatchupRound > currRound)
                        {
                            t.Rounds.Add(currRow);
                            currRow = new List<MatchupModel>();
                            currRound++;
                        }
                        
                        currRow.Add(m);
                    }

                    t.Rounds.Add(currRow);

                }

            }

            return tournaments;
        }
        /// <summary>
        /// Finds the input matchup in the database and updates its info
        /// </summary>
        /// <param name="model"></param>
        public void UpdateMatchup(MatchupModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var t = new DynamicParameters();
                if (model.Winner != null)
                {
                    t.Add("@Matchup_Id", model.Id);
                    t.Add("@Winner_Id", model.Winner.Id);

                    connection.Execute("dbo.spMatchups_Update", t, commandType: CommandType.StoredProcedure);

                }
                foreach (MatchupEntryModel me in model.Entries)
                {
                    if (me.TeamCompeting != null)
                    {
                        t = new DynamicParameters();
                        t.Add("@Entry_Id", me.Id);
                        t.Add("@TeamCompeting_Id", me.TeamCompeting.Id);
                        t.Add("@Score", me.Score);


                        connection.Execute("dbo.spMatchupEntries_Update", t, commandType: CommandType.StoredProcedure);
                    }
                }
            }
        }
    }
}

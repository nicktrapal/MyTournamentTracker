

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
        //TODO make the create prize method actually save to the database
        /// <summary>
        /// Saves a new prize to the database
        /// </summary>
        /// <param name="model">The prize information.</param>
        /// <returns>The prize information, including the unique identifier.</returns>
        public PrizeModel CreatePrize(PrizeModel model)
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

            return model;
        }

        public PersonModel CreatePerson(PersonModel model)
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

            return model;
        }



        public TeamModel CreateTeam(TeamModel model)
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


            return model;
        }

        public void CreateTournament(TournamentModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {

                SaveTournament(connection, model);

                SavePrizes(connection, model);

                SaveEntries(connection, model);

            }

        }

        private void SaveTournament(IDbConnection connection,TournamentModel model)
        {
            var t = new DynamicParameters();
            t.Add("@TournamentName", model.TournamentName);
            t.Add("@EntryFee", model.EntryFee);
            t.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

            connection.Execute("dbo.spTournaments_Insert", t, commandType: CommandType.StoredProcedure);

            model.Id = t.Get<int>("@Id");


        }

        private void SavePrizes(IDbConnection connection, TournamentModel model)
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

        private void SaveEntries(IDbConnection connection, TournamentModel model)
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

            public List<PersonModel> GetPerson_All()
        {
            List<PersonModel> output;
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<PersonModel>("dbo.spPeople_GetAll").ToList();
            }

            return output;
        }

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

      
    }
}

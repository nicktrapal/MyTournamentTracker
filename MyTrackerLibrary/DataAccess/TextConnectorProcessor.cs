using MyTrackerLibrary.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrackerLibrary.DataAccess.TextHelpers
{
    public static class TextConnectorProcessor
    {
        public static String FullFilePath(this string fileName) 
        {
            return $"{ ConfigurationManager.AppSettings["filePath"]}\\{fileName}";
        }
        public static List<String> LoadFile(this string file)
        {
            if (!File.Exists(file))
            {
                return new List<string>();
            }
            return File.ReadAllLines(file).ToList();
        }

        public static List<PrizeModel> ConvertToPrizeModels(this List<String> lines)
        {
            List<PrizeModel> output = new List<PrizeModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                PrizeModel p = new PrizeModel();
                p.Id = int.Parse(cols[0]);
                p.PlaceNumber = int.Parse(cols[1]);
                p.PlaceName = cols[2];
                p.PrizeAmount = decimal.Parse(cols[3]);
                p.PrizePercentage = double.Parse(cols[4]);
                output.Add(p);

            }

            return output;
        }

        public static List<PersonModel> ConvertToPersonModels(this List<String> lines)
        {
            List<PersonModel> output = new List<PersonModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                PersonModel p = new PersonModel();
                p.Id = int.Parse(cols[0]);
                p.FirstName = cols[1];
                p.LastName = cols[2];
                p.EmailAddress = cols[3];
                p.CellphoneNumber = cols[4];
                output.Add(p);

            }

            return output;
        }


        public static List<TeamModel> ConvertToTeamModels(this List<String> lines,string peopleFileName)
        {
            //id,team name, list of member ids seperated by the pipe (|)
            //e.g. 3,Nick's Team,1|3|5
            List<TeamModel> output = new List<TeamModel>();
            List<PersonModel> people = peopleFileName.FullFilePath().LoadFile().ConvertToPersonModels();


            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                TeamModel t = new TeamModel();
                t.Id = int.Parse(cols[0]);
                t.Team_Name= cols[1];
                string[] teamMembersIds_ = cols[2].Split('|');
                foreach (string id in teamMembersIds_)
                {
                    t.TeamMembers.Add(people.Where(x => x.Id == int.Parse(id)).First());
                }
                output.Add(t);
                

            }

            return output;
        }


        public static List<TournamentModel> ConvertToTournamentModels(this List<String> lines, string teamsFileName, string peopleFileName,string prizesFilename)
        {
            //FORMAT Id,TournamentName,EntryFee , list of Team ids seperated by the pipe ,list of Prize ids seperated by the pipe (|) ,list of rounds seperated by the pipe (|)  -- each round is a list of Matchup ids seperated by the carrot (^)
            //e.g. 3,Super bowl,100,1|3|5,1|2|3,1^3^5| 2^4^6 |5^7^8
            List<TournamentModel> output = new List<TournamentModel>();
            List<TeamModel> teams = teamsFileName.FullFilePath().LoadFile().ConvertToTeamModels(peopleFileName);
            List<PrizeModel> prizes = prizesFilename.FullFilePath().LoadFile().ConvertToPrizeModels();


            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                TournamentModel tm = new TournamentModel();
                tm.Id = int.Parse(cols[0]);
                tm.TournamentName= cols[1];
                tm.EntryFee = decimal.Parse(cols[2]);

                string[] teamIds = cols[3].Split('|');
                foreach (string id in teamIds)
                {
                    tm.EnteredTeams.Add(teams.Where(x => x.Id == int.Parse(id)).First());
                }

                string[] prizeIds = cols[4].Split('|');
                foreach (string id in prizeIds)
                {
                    tm.Prizes.Add(prizes.Where(x => x.Id == int.Parse(id)).First());
                }

                string[] rounds = cols[5].Split('|');
                foreach (string round in rounds )
                {
                    string[] matchupIds = round.Split('^');
                }
                
               

                output.Add(tm);


            }

            return output;
        }


        public static void SaveToPrizeFile(this List<PrizeModel> prizes, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (PrizeModel prize in prizes)
            {
                lines.Add(prize.Id + "," + prize.PlaceNumber + "," + prize.PlaceName + "," + prize.PrizeAmount + "," + prize.PrizePercentage);
            }

            File.WriteAllLines(fileName.FullFilePath(),lines);


        }

       
        public static void SaveToPersonFile(this List<PersonModel> people, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (PersonModel person in people)
            {
                lines.Add(person.Id + "," + person.FirstName + "," + person.LastName + "," + person.EmailAddress + "," + person.CellphoneNumber);
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);



        }

        public static void SaveToTeamFile(this List<TeamModel> teams, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (TeamModel t in teams)
            {
                lines.Add(t.Id + "," + t.Team_Name + "," + ConvertPeopleListToString(t.TeamMembers));
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        public static void SaveToTournamentFile(this List<TournamentModel> tournaments, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (TournamentModel tm in tournaments)
            {
                lines.Add(tm.Id + "," + tm.TournamentName + "," + tm.EntryFee + "," +  ConvertTeamsListToString(tm.EnteredTeams) + "," + ConvertPrizesListToString(tm.Prizes) + "," + ConvertRoundsListToString(tm.Rounds));
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        /// <summary>
        /// Converts a list of people to a string with their Ids.
        /// </summary>
        /// <param name="people"></param>
        /// <returns>A string with the ids of the people that are in the input List.</returns>
        private static string ConvertPeopleListToString(List<PersonModel> people)
        {
            String output = "";

            if (people.Count == 0)
                return "";

            foreach(PersonModel p in people)
            {
                output+= $"{p.Id }|";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        /// <summary>
        /// Converts a list of teams to a string with their Ids.
        /// </summary>
        /// <param name="teams"></param>
        /// <returns>A string with the ids of the teams that are in the input List.</returns>
        private static string ConvertTeamsListToString(List<TeamModel> teams)
        {
            String output = "";

            if (teams.Count == 0)
                return "";

            foreach (TeamModel t in teams)
            {
                output += $"{t.Id }|";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        /// <summary>
        /// Converts a list of prizes to a string with their Ids.
        /// </summary>
        /// <param name="prizes"></param>
        /// <returns>A string with the ids of the prizes that are in the input List.</returns>
        private static string ConvertPrizesListToString(List<PrizeModel> prizes)
        {
            String output = "";

            if (prizes.Count == 0)
                return "";

            foreach (PrizeModel p in prizes)
            {
                output += $"{p.Id }|";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        
        private static string ConvertRoundsListToString(List<List<MatchupModel>> rounds)
        {
            //Rounds example: 1 ^ 3 ^ 5 | 2 ^ 4 ^ 6 | 5 ^ 7 ^ 8 The numbers are matchup model ids
            String output = "";

            if (rounds.Count == 0)
                return "";

            foreach (List < MatchupModel > round in rounds)
            {
                output += $"{ConvertMatchupListToString(round) }|";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        private static string ConvertMatchupListToString(List<MatchupModel> matchups)
        {
            String output = "";

            if (matchups.Count == 0)
                return "";

            foreach (MatchupModel m in matchups)
            {
                output += $"{m.Id }^";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }
    }
}

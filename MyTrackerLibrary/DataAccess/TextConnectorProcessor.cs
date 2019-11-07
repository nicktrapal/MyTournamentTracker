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
        /// <summary>
        /// Returns the full path of the given text file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>The full path</returns>
        public static String FullFilePath(this string fileName) 
        {
            return $"{ ConfigurationManager.AppSettings["filePath"]}\\{fileName}";
        }
        /// <summary>
        /// Loads the contents of the file to a String
        /// </summary>
        /// <param name="file"></param>
        /// <returns>A string with all the file data in it</returns>
        public static List<String> LoadFile(this string file)
        {
            if (!File.Exists(file))
            {
                return new List<string>();
            }
            return File.ReadAllLines(file).ToList();
        }
        /// <summary>
        /// Converts the lines of a PrizeModel textfile to a list of PrizeModel objects
        /// </summary>
        /// <param name="lines"></param>
        /// <returns>The list of PrizeModel objects</returns>
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
        /// <summary>
        /// Converts the lines of a PersonModel textfile to a list of PesonModel objects
        /// </summary>
        /// <param name="lines"></param>
        /// <returns>The list of PersonModel objects</returns>
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
        /// <summary>
        /// Converts the lines of a TeamModel textfile to a list of TeanModel objects
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="peopleFileName"></param>
        /// <returns>The list of TeamModel objects</returns>
        public static List<TeamModel> ConvertToTeamModels(this List<String> lines,string peopleFileName)
        {
            //Textfile line structure :  id,team name, list of member ids seperated by the pipe (|)
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
        /// <summary>
        /// Converts the lines of a MatchupEntryModel textfile to a list of MatchupEntryModel objects
        /// </summary>
        /// <param name="lines"></param>
        /// <returns>The list of MatchupEntryModel objects</returns>
        public static List<MatchupEntryModel> ConvertToMatchupEntryModels(this List<string> lines)
        {
            //Textfile line structure : id,TeamCompeting_Id,Score,ParentMatchup_Id
            List<MatchupEntryModel> matchupEntries = new List<MatchupEntryModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                MatchupEntryModel m = new MatchupEntryModel();
                m.Id = int.Parse(cols[0]);
                m.TeamCompeting = LookupTeamById(int.Parse(cols[1]));
                m.Score = double.Parse(cols[2]);
                int parentId = 0;
                if (int.TryParse(cols[3], out parentId))
                    m.ParentMatchup = LookupMatchupById(parentId);
                else
                    m.ParentMatchup = null;

                matchupEntries.Add(m);

            }

            return matchupEntries;
        }

        /// <summary>
        /// Converts the lines of a MatchupModel textfile to a list of MatchupModel objects
        /// </summary>
        /// <param name="lines"></param>
        /// <returns>The list of MatchupModel objects</returns>
        public static List<MatchupModel> ConvertToMatchupModels(this List<String> lines)
        {
            //Textfile line structure :matchup_id,list of entry ids seperated by the pipe (|),winner_id,matchupRound
            List<MatchupModel> output = new List<MatchupModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                MatchupModel m = new MatchupModel();
                m.Id = int.Parse(cols[0]);
                m.Entries = ConvertStringToMatchupEntryModels(cols[1]);
                m.Winner = LookupTeamById(int.Parse(cols[2]));
                m.MatchupRound = int.Parse(cols[3]);
                output.Add(m);

            }

            return output;
        }
        /// <summary>
        /// Converts ids of entries from a MatchupModel textfile to a list of MatchupEntryModel objects
        /// </summary>
        /// <param name="input"></param>
        /// <returns>The list of MatchupEntryModel objects</returns>
        public static List<MatchupEntryModel> ConvertStringToMatchupEntryModels(string input)
        {
            //id1||id2|id3
            string[] ids = input.Split('|');
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();
            List<MatchupEntryModel> entries = GlobalConfig.MatchupEntriesFile.FullFilePath().LoadFile().ConvertToMatchupEntryModels();

            foreach (String id in ids)
            {
                output.Add(entries.Where(x => x.Id == int.Parse(id)).First());
            }

            return output;
        }
        /// <summary>
        /// Converts the lines of a TournamentModel textfile to a list of TournamentModel objects
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="teamsFileName"></param>
        /// <param name="peopleFileName"></param>
        /// <param name="prizesFilename"></param>
        /// <returns>The list of TournamentModel objects</returns>
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

        /// <summary>
        /// Saves a list of PrizeModel objects to the PrizeModel textfile by writing all the object info to the file.
        /// </summary>
        /// <param name="prizes"></param>
        /// <param name="fileName"></param>
        public static void SaveToPrizeFile(this List<PrizeModel> prizes, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (PrizeModel prize in prizes)
            {
                lines.Add(prize.Id + "," + prize.PlaceNumber + "," + prize.PlaceName + "," + prize.PrizeAmount + "," + prize.PrizePercentage);
            }

            File.WriteAllLines(fileName.FullFilePath(),lines);


        }

        /// <summary>
        /// Saves a list of PersonModel objects to the PersonModel textfile by writing all the object info to the file.
        /// </summary>
        /// <param name="people"></param>
        /// <param name="fileName"></param>
        public static void SaveToPersonFile(this List<PersonModel> people, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (PersonModel person in people)
            {
                lines.Add(person.Id + "," + person.FirstName + "," + person.LastName + "," + person.EmailAddress + "," + person.CellphoneNumber);
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);



        }
        /// <summary>
        /// Saves a list of TeamModel objects to the TeamModel textfile by writing all the object info to the file.
        /// </summary>
        /// <param name="teams"></param>
        /// <param name="fileName"></param>
        public static void SaveToTeamFile(this List<TeamModel> teams, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (TeamModel t in teams)
            {
                lines.Add(t.Id + "," + t.Team_Name + "," + ConvertPeopleListToString(t.TeamMembers));
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="matchupsFile"></param>
        /// <param name="matchupEntriesFile"></param>
        public static void SaveRoundsToFile(this TournamentModel model, string matchupsFile, string matchupEntriesFile)
        {
            //Loop through each Round 
            //Loop through each Matchup
            //Get the id for the new matchup and save the record
            //Loop through each Entry , get the id and save it

            foreach ( List <MatchupModel> round in model.Rounds)
            {
                foreach ( MatchupModel matchup in round)
                {
                    //Load all of the matchups from file
                    //Get the top id and add one
                    //Store the id
                    //Save the matchup record 
                    matchup.SaveMatchupToFile(matchupsFile,matchupEntriesFile);

                    
                }
            }
        }

        /// <summary>
        /// Saves a list of MatchupModel objects to the MatchupModel textfile by writing all the object info to the file.
        /// </summary>
        /// <param name="matchup"></param>
        /// <param name="matchupsFile"></param>
        /// <param name="matchupEntriesFile"></param>        
        public static void SaveMatchupToFile(this MatchupModel matchup, String matchupsFile,String matchupEntriesFile)
        {
            List<MatchupModel> matchups = GlobalConfig.MatchupsFile.FullFilePath().LoadFile().ConvertToMatchupModels();

            int currentId = 1;
            if (matchups.Count > 0)
                currentId = matchups.OrderByDescending(x => x.Id).First().Id + 1;
            matchup.Id = currentId;

            foreach (MatchupEntryModel entry in matchup.Entries)
            {
                entry.SaveEntryToFile();
            }

            List<string> lines = new List<string>();

            //Textfile line structure : id,entries (pipe delimited by id e.g. 1|2|3),winner_id,matchupRound
            foreach (MatchupModel m in matchups)
            {
                string winner = "";
                if (m.Winner != null)
                    winner = m.Winner.Id.ToString();

                lines.Add($"{m.Id},{ConvertMatchupEntryListToString(m.Entries)},{winner},{m.MatchupRound}");
            }

            File.WriteAllLines(GlobalConfig.MatchupsFile.FullFilePath(), lines);
        }
        /// <summary>
        /// Saves a new MatchupEntryModel object to the MatchupEntryModel textfile along with the already existing entries.
        /// </summary>
        /// <param name="entry"></param>
        public static void SaveEntryToFile(this MatchupEntryModel entry)
        {
            List<MatchupEntryModel> entries = GlobalConfig.MatchupEntriesFile.FullFilePath().LoadFile().ConvertToMatchupEntryModels();

            int currentId = 1;

            if (entries.Count > 0)
                currentId = entries.OrderByDescending(x => x.Id).First().Id + 1;

            entry.Id = currentId;
            entries.Add(entry);//saves the new

            List<string> lines = new List<string>();

            //Textfile line structure : id,TeamCompeting,Score,ParentMatchup
            foreach (MatchupEntryModel e in entries)
            {
                string parent = "";
                if (e.ParentMatchup != null)
                    parent = e.ParentMatchup.Id.ToString();

                lines.Add(e.Id + "," + e.TeamCompeting.Id + "," + e.Score + "," + e.ParentMatchup.Id);
            }

            File.WriteAllLines(GlobalConfig.MatchupEntriesFile.FullFilePath(), lines);
        }
        /// <summary>
        /// Saves a list of TournamentModel objects to the TournamentModel textfile by writing all the object info to the file.
        /// </summary>
        /// <param name="tournaments"></param>
        /// <param name="fileName"></param>
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
        /// <summary>
        /// Converts a list of MatchupEntries to a string with their Ids.
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        private static string ConvertMatchupEntryListToString(List<MatchupEntryModel> entries)
        {
            String output = "";

            if (entries.Count == 0)
                return "";

            foreach (MatchupEntryModel e in entries)
            {
                output += $"{e.Id }|";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }
        /// <summary>
        /// Converts a list of rounds to a string that represents each round accordingly.
        /// </summary>
        /// <param name="rounds"></param>
        /// <returns>A string that includes round information for all the rounds</returns>
        private static string ConvertRoundsListToString(List<List<MatchupModel>> rounds)
        {
            //Rounds example: 1 ^ 3 ^ 5 | 2 ^ 4 ^ 6 | 5 ^ 7 ^ 8 The numbers are matchup model ids
            String output = "";

            if (rounds.Count == 0)
                return "";

            foreach (List <MatchupModel> round in rounds)
            {
                output += $"{ConvertMatchupListToString(round) }|";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }
        /// <summary>
        /// Converts a list of MatchupModel to a string with their Ids.
        /// </summary>
        /// <param name="matchups"></param>
        /// <returns>A string with the ids of the MatchupModels that are in the input List.</returns>
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
        /// <summary>
        /// Finds the TeamModel object that is identified by the input id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The TeamModel object with the input id</returns>
        private static TeamModel LookupTeamById(int id)
        {
            List<TeamModel> teams = GlobalConfig.TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels(GlobalConfig.PeopleFile);

            return teams.Where(x => x.Id == id).First();
        }
        /// <summary>
        /// Finds the MatchupModel object that is identified by the input id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The MatchupModel object with the input id</returns>
        private static MatchupModel LookupMatchupById(int id)
        {
            List<MatchupModel> matchups = GlobalConfig.MatchupsFile.FullFilePath().LoadFile().ConvertToMatchupModels();

            return matchups.Where(x => x.Id == id).First();
        }
    }
}

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
        public static String FullFilePath(this string fileName) //PrizeModels.csv
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
            //id,team name, list of ids seperated by the pipe 
            //3,Nick's Team,1|3|5
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
    }
}

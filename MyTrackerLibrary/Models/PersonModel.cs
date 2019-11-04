using System;
using System.Collections.Generic;
using System.Text;

namespace MyTrackerLibrary.Models
{
    public class PersonModel
    {
        /// <summary>
        /// The unique identifier for this person.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Represents the first name of this person.
        /// </summary>
        public String FirstName { get; set; }
        /// <summary>
        /// Represents the last name of this person.
        /// </summary>
        public String LastName { get; set; }
        /// <summary>
        /// Represents the email address of this person.
        /// </summary>
        public String EmailAddress { get; set; }
        /// <summary>
        /// Represents the cellphone number of this person.
        /// </summary>
        public String CellphoneNumber { get; set; }

        public String FullName
        {
            get {
                return FirstName + " " + LastName;
                }
        }

        public PersonModel()
        {

        }

        public PersonModel(string firstName,string lastName,string emailAddress,string cellphoneNumber)
        {
            FirstName = firstName;
            LastName = lastName;
            EmailAddress = emailAddress;
            CellphoneNumber = cellphoneNumber;
        }
    }
}

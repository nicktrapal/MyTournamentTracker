using System;
using System.Collections.Generic;
using System.Text;

namespace MyTrackerLibrary.Models
{
    public class PrizeModel
    {
        /// <summary>
        /// The unique identifier for the prize
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Represents the place number you have to get in order to earn the prize.
        /// </summary>
        public int PlaceNumber { get; set; }
        /// <summary>
        ///  Represents the place name you have to get in order to earn the prize.
        /// </summary>
        public String PlaceName { get; set; }
        /// <summary>
        /// Represents the amount  of money this prize earns you.
        /// </summary>
        public decimal PrizeAmount { get; set; }
        /// <summary>
        /// Represents the percentage of tournament money this prize earns you.
        /// </summary>
        public double PrizePercentage { get; set; }

        public PrizeModel()
        {

        }

        public PrizeModel(String placeNumber,String placeName,String prizeAmount,String prizePercentage)
        {
            PlaceName = placeName;

            int placeNumberValue=0;
            int.TryParse(placeNumber, out placeNumberValue);
            PlaceNumber = placeNumberValue;

            decimal prizeAmountValue = 0;
            decimal.TryParse(prizeAmount, out prizeAmountValue);
            PrizeAmount = prizeAmountValue;

            double prizePercentageValue = 0;
            double.TryParse(prizePercentage, out prizePercentageValue);
            PrizePercentage = prizePercentageValue;


        }
    }
}

using MyTrackerLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrackerUI
{
    public interface IPrizeRequester
    {

        void PrizeComplete(PrizeModel model);
    }
}

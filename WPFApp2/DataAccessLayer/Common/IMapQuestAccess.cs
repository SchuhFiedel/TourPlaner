using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourFinder.DataAccessLayer.Common
{
    interface IMapQuestAccess
    {
        public Task<Tuple<string,float>> GetRouteSaveImg(string startLocation, string endLocation);
    }
}

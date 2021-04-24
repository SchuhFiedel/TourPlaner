using System;
using System.Collections.Generic;
using TourFinder.DataAccessLayer.Common;
using TourFinder.Models;

namespace TourFinder.BusinessLayer
{
    class BusinessLayerManager
    {
        DataLayerAccessManager DLAM = new DataLayerAccessManager();

        public Tour CreateNewTour(string name, string startLocation, string endLocation, string description = null)
        {
            return DLAM.CreateNewTour(name, startLocation, endLocation, description);
        }

        public List<Tour> GetAllToursFromDB()
        {
            return DLAM.GetToursFromDB();
        }

        public List<Log> CreateNewLogAndRefreshTourLogList()
        {
            throw new NotImplementedException();
        }

        public List<Log> DeleteLogGetList()
        {
            throw new NotImplementedException();
        }


    }
}

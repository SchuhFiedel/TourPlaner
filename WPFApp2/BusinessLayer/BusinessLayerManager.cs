using System;
using System.Collections.Generic;
using TourFinder.DataAccessLayer.Common;
using TourFinder.Models;

namespace TourFinder.BusinessLayer
{
    class BusinessLayerManager
    {
        DataLayerAccessManager DLAM = DataLayerAccessManager.GetInstance();

        public Tour CreateNewTour(string name, string startLocation, string endLocation, string description = null)
        {
            return DLAM.CreateNewTour(name, startLocation, endLocation, description);
        }

        public List<Tour> GetAllToursFromDB()
        {
            return (List<Tour>)DLAM.GetToursFromDB();
        }

        public List<Log> CreateNewLogAndRefreshTourLogList()
        {
            throw new NotImplementedException();
        }

        public List<Log> DeleteLogGetList()
        {
            throw new NotImplementedException();
        }


        public void ExportToursToJSON(List<Tour> tourlist)
        {
            DLAM.ExportToursToJSONFile(tourlist);
        }

        public List<Tour> ImportToursFromJSON()
        {
            return (List<Tour>)DLAM.ImportToursFromJSONFile();
        }
    }
}

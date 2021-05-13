using System;
using System.Collections.Generic;
using TourFinder.DataAccessLayer.Common;
using TourFinder.Models;
using TourFinder.BusinessLayer.PDF;
using System.Diagnostics;

namespace TourFinder.BusinessLayer
{
    class BusinessLayerManager
    {
        DataLayerAccessManager DLAM = DataLayerAccessManager.GetInstance();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("BusinessLayerManager.cs");

        public List<Tour> GetAllToursFromDB()
        {
            return (List<Tour>)DLAM.GetToursFromDB();
        }


        //CREATE TOUR, LOG
        public Tour CreateNewTour(string name, string startLocation, string endLocation, string description = null)
        {
            if (description == null) description = "";
            return DLAM.CreateNewTour(name, startLocation, endLocation, description);
        }

        public List<Log> CreateNewLogGetLogList(Tour tour, string date, string report = "\"\"", int distance = 0, string duration = "\"\"",
                                                            int rating = 0, int steps = 0, float weightkg = 0, string bloodpreassure = "\"\"",
                                                            string feeling = "\"\"", string weather = "\"\"")
        {
            return (List<Log>)DLAM.CreateNewLogAndRefreshTourLogList(tour, date, report, distance, duration,rating, steps, weightkg,bloodpreassure,feeling,weather);
        }

        //UPDATE
        public List<Tour> UpdateTourGetList(Tour tour, string name = null, string description = null)
        {
            return (List<Tour>)DLAM.UpdateTourAndRefreshTourList(tour, name, description);
        }

        public List<Log> UpdateLogGetList(Log log, string date, string report = "", int distance = 0, string duration = "",
                                                            int rating = 0, int steps = 0, float weightkg = 0, string bloodpreassure = "",
                                                            string feeling = "", string weather = "")
        {
            return (List<Log>)DLAM.UpdateLogAndRefreshTourLogList(log, report, distance, duration, rating, steps, weightkg, bloodpreassure, feeling, weather);
        }

        //DELETE TOUR, LOG
        public List<Log> DeleteLogGetList(Log log)
        {
            return (List<Log>)DLAM.DeleteLogAndRefreshTourLogList(log);
        }

        public List<Tour> DeleteTourGetList(Tour tour)
        {
            return (List<Tour>)DLAM.DeleteTourAndRefreshTourList(tour);
        }

        //COPY
        public List<Log> CopyLogGetList(Log log)
        {
            return (List<Log>)DLAM.CopyLogAndRefreshTourLogList(log);
        }

        public List<Tour> CopyTourGetList(Tour tour)
        {
            return (List<Tour>)DLAM.CopyTourAndRefreshTourList(tour);
        }


        //EXPORT INPORT
        public void ExportToursToJSON(IEnumerable<Tour> tourlist)
        {
            DLAM.ExportToursToJSONFile(tourlist);
        }

        public List<Tour> ImportToursFromJSON()
        {
            return (List<Tour>)DLAM.ImportToursFromJSONFile();
        }


        //EXPORT PDF
        public void PrintTourDataToPDF(Tour tour)
        {
            try
            {
                log.Info(String.Format("Print Tour To PDF: {0}", tour.ID));
                PDFMaker.MakeTourPDF(tour);
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                log.Info(String.Format(e.Message));
            }
        }

        public void PrintAllToursToPDF(IEnumerable<Tour> tourlist)
        {
            try
            {
                log.Info(String.Format("Print All Tours To PDF"));
                PDFMaker.MakeAllTourPDF(tourlist);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                log.Info(String.Format(e.Message));
            }
        }

        public void PrintSummary(IEnumerable<Tour> tourlist)
        {
            try
            {
                log.Info(String.Format("Print Summary To PDF"));
                PDFMaker.MakeTourSummaryPDF(tourlist);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                log.Info(String.Format(e.Message));
            }
        }

        //DELETE UNUSED IMAGES
        public int DeleteUnusedImages(int savedImageCounter)
        {
            return DLAM.DeleteUnusedTourImages(savedImageCounter);
        }
    }
}

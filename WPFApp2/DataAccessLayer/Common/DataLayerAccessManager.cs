using System;
using System.Collections.Generic;
using System.Diagnostics;
using TourFinder.DataAccessLayer.DAObject;
using TourFinder.DataAccessLayer.MapQuestConnector;
using TourFinder.DataAccessLayer.PostgresSqlDB;
using TourFinder.DataAccessLayer.FileSystem;
using TourFinder.Models;
using System.Collections.ObjectModel;


namespace TourFinder.DataAccessLayer.Common
{
    public class DataLayerAccessManager
    {
        IDBAccess database;
        ITourDAO tourDAO;
        ITourLogDAO logDAO;
        IMapQuestAccess mapQuest;
        Filemanager fileManager;
        private static DataLayerAccessManager instance;

         DataLayerAccessManager()
        {
            database = PostgresSqlConnector.Instance();
            tourDAO = new TourSqlPostgresDAO(database);
            logDAO = new LogSqlPostgresDAO(database, tourDAO);
            fileManager = Filemanager.GetInstance(tourDAO, logDAO);
            mapQuest = RestDataClass.Instance();
        }

        public static DataLayerAccessManager GetInstance()
        {
            if(instance == null)
            {
                instance = new DataLayerAccessManager();
            }
            return instance;
        }

        public IEnumerable<Tour> GetToursFromDB()
        {
            IEnumerable<Tour> tourList = new List<Tour>();
            try { 
                Debug.WriteLine("GET ALL TOURS FROM DB");
                tourList = tourDAO.GetTours();
                foreach(Tour x in tourList)
                { 
                    List<Log> loglist = (List<Log>)logDAO.GetLogsOfTour(x);
                    foreach (Log y in loglist)
                        y.Tour = x;
                    x.LogList = new ObservableCollection<Log>(loglist);
                    Debug.WriteLine(x);
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return tourList;
        }

        //CREATE
        public Tour CreateNewTour(string name, string startLocation, string endLocation, string description = "")
        {
            Tuple<string, float> tmpTuple = new Tuple<string, float>("0", 0);
            try
            {
                tmpTuple = mapQuest.GetRouteSaveImg(startLocation, endLocation);
            }catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            string mapImagePath = tmpTuple.Item1;
            float distance = tmpTuple.Item2;

            return tourDAO.AddNewTour(name, startLocation, endLocation, distance, mapImagePath, description);
        }

        public IEnumerable<Log> CreateNewLogAndRefreshTourLogList(Tour tour, string date, string report = "", int distance = 0, string duration = "",
                                                            int rating = 0, int steps = 0, float weightkg = 0, string bloodpreassure = "", 
                                                            string feeling = "", string weather = "")
        {
            try
            {
                logDAO.AddNewTourLog(tour, date, report, distance, duration, rating, steps, weightkg, bloodpreassure, feeling, weather);
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            
            return logDAO.GetLogsOfTour(tour);
        }

        //UPDATE
        public IEnumerable<Log> UpdateLogAndRefreshTourLogList(Log oldLog, string report = "", int distance = 0, string duration = "", 
                                                        int rating = 0, int steps = 0, float weightkg = 0, string bloodpreassure = "", 
                                                        string feeling = "", string weather = "")
        {
            try { 
                logDAO.UpdateLog(oldLog, report,distance,duration,rating,steps,weightkg,bloodpreassure,feeling,weather);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return  logDAO.GetLogsOfTour(oldLog.Tour);
        }

        public IEnumerable<Tour> UpdateTourAndRefreshTourList(Tour tour, string name = null, string description = null)
        {
            try
            {
                if (name == null && tour.Name == name)
                {
                    name = tour.Name;
                }
                if (description == null && tour.Description == description)
                {
                    description = tour.Description;
                }

                tourDAO.UpdateTour(tour.ID, name, description);
            }catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            
            return GetToursFromDB();
        }

        //COPY
        public IEnumerable<Log> CopyLogAndRefreshTourLogList(Log oldLog)
        {
            try
            {
                logDAO.CopyLog(oldLog);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return logDAO.GetLogsOfTour(oldLog.Tour);
        }

        public IEnumerable<Tour> CopyTourAndRefreshTourList(Tour oldTour)
        {
            try
            {
                tourDAO.CopyTour(oldTour);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return GetToursFromDB();
        }

        //DELETE 
        public IEnumerable<Log> DeleteLogAndRefreshTourLogList(Log oldlog)
        {
            try
            {
                logDAO.DeleteLog(oldlog);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return logDAO.GetLogsOfTour(oldlog.Tour);
        }

        public IEnumerable<Tour> DeleteTourAndRefreshTourList(Tour oldTour)
        {
            try
            {
                if (tourDAO.DeleteTour(oldTour) > 0)
                {
                    foreach (Log x in oldTour.LogList)
                    {
                        logDAO.DeleteLog(x);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return GetToursFromDB();
        }

        public int DeleteUnusedTourImages(int savedImageCounter)
        {

            List<string> dbImages = (List<string>)tourDAO.GetAllTourImages();
            if (savedImageCounter != dbImages.Count)
            {
                List<string> fileSystemImages = (List<string>)fileManager.GetAllImagesFromFolder();
                try
                {
                    foreach (string image in fileSystemImages)
                    {
                        if (!dbImages.Contains(image))
                        {
                            fileManager.DeleteImage(image);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            return savedImageCounter = ((List<string>)fileManager.GetAllImagesFromFolder()).Count;
        }

        //IMPORT EXPORT
        public IEnumerable<Tour> ImportToursFromJSONFile(string path = "TourData.json")
        {
            return fileManager.ImportToursFromFile(path);
        }
        public void ExportToursToJSONFile(IEnumerable<Tour> tourlist, string path = "TourData.json")
        {
            fileManager.ExportToursToFile(tourlist, path);
        }

    }
    
}

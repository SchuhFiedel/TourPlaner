﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TourFinder.DataAccessLayer.DAObject;
using TourFinder.DataAccessLayer.MapQuestConnector;
using TourFinder.DataAccessLayer.PostgresSqlDB;
using TourFinder.DataAccessLayer.FileSystem;
using TourFinder.Models;

namespace TourFinder.DataAccessLayer.Common
{
    public class DataLayerAccessManager
    {
        IDBAccess database;
        ITourDAO tourDAO;
        ITourLogDAO logDAO;
        IMapQuestAccess mapQuest;
        FileExInporter fileManager;
        private static DataLayerAccessManager instance;

         DataLayerAccessManager()
        {
            database = PostgresSqlConnector.Instance();
            fileManager = FileExInporter.GetInstance();
            tourDAO = new TourSqlPostgresDAO(database);
            logDAO = new LogSqlPostgresDAO(database, tourDAO);
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
            Debug.WriteLine("GET ALL TOURS FROM DB");
            return (List<Tour>)tourDAO.GetTours();
        }

        //CREATE
        public Tour CreateNewTour(string name, string startLocation, string endLocation, string description = "")
        {
            Tuple<string, float> tmpTuple = mapQuest.GetRouteSaveImg(startLocation, endLocation);

            string mapImagePath = tmpTuple.Item1;
            float distance = tmpTuple.Item2;
                       

            return tourDAO.AddNewTour(name, startLocation, endLocation, distance, mapImagePath, description);
        }

        public IEnumerable<Log> CreateNewLogAndRefreshTourLogList(Tour tour, string date, string report = "", int distance = 0, string duration = "",
                                                            int rating = 0, int steps = 0, float weightkg = 0, string bloodpreassure = "", 
                                                            string feeling = "", string weather = "")
        {
            logDAO.AddNewItemLog(tour, date, report, distance, duration, rating, steps, weightkg, bloodpreassure, feeling, weather);
            return (List<Log>)logDAO.GetLogsOfTour(tour);
        }

        //UPDATE
        public IEnumerable<Log> UpdateLogAndRefreshTourLogList(Log oldLog, string date = "", string report = "", int distance = 0, string duration = "", 
                                                        int rating = 0, int steps = 0, float weightkg = 0, string bloodpreassure = "", 
                                                        string feeling = "", string weather = "")
        {
            throw new NotImplementedException();
           // return new List<Log>();
        }

        public IEnumerable<Tour> UpdateTourAndRefreshTourList(Tour tour, string name = null, string description = null)
        {
            if(name == null && tour.Name == name)
            {
                name = tour.Name;
            }
            if(description == null && tour.Description == description)
            {
                description = tour.Description;
            }

            tourDAO.UpdateTour(tour.ID, name, description);
            return tourDAO.GetTours(); 
        }

        //COPY
        public IEnumerable<Log> CopyLogAndRefreshTourLogList(Log oldLog)
        {
            logDAO.CopyLog(oldLog);
            return logDAO.GetLogsOfTour(oldLog.Tour);
        }

        public IEnumerable<Tour> CopyTourAndRefreshTourList(Tour oldTour)
        {
            tourDAO.CopyTour(oldTour);
            return tourDAO.GetTours();
        }

        //DELETE 
        public IEnumerable<Log> DeleteLogAndRefreshTourLogList(Log oldlog)
        {
            logDAO.DeleteLog(oldlog);
            return logDAO.GetLogsOfTour(oldlog.Tour);
        }

        public IEnumerable<Tour> DeleteTourAndRefreshTourList(Tour oldTour)
        {
            tourDAO.DeleteTour(oldTour);
            return tourDAO.GetTours();
        }

        //IMPORT EXPORT
        public IEnumerable<Tour> ImportToursFromJSONFile()
        {
            return fileManager.ImportToursFromFile();
        }

        public void ExportToursToJSONFile(IEnumerable<Tour> tourlist)
        {
            fileManager.ExportToursToFile(tourlist);
        }
    }
}

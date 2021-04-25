using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TourFinder.DataAccessLayer.DAObject;
using TourFinder.DataAccessLayer.MapQuestConnector;
using TourFinder.DataAccessLayer.PostgresSqlDB;
using TourFinder.Models;

namespace TourFinder.DataAccessLayer.Common
{
    public class DataLayerAccessManager
    {
        IDBAccess database;
        ITourDAO tourDAO;
        ITourLogDAO logDAO;
        IMapQuestAccess mapQuest;

        public DataLayerAccessManager()
        {
            database = PostgresSqlConnector.Instance();
            tourDAO = new TourSqlPostgresDAO(database);
            logDAO = new LogSqlPostgresDAO(database, tourDAO);
            mapQuest = RestDataClass.Instance();
        }

        public Tour CreateNewTour(string name, string startLocation, string endLocation, string description = "")
        {
            Task<Tuple<string,float>> tmpTask =  mapQuest.GetRouteSaveImg(startLocation, endLocation);
            Task.WaitAll(tmpTask);

            Tuple<string, float> tmpTuple = tmpTask.Result;
            string mapImagePath = tmpTuple.Item1;
            float distance = tmpTuple.Item2;
                       

            return tourDAO.AddNewTour(name, startLocation, endLocation, distance, mapImagePath, description);
        }

        public List<Log> CreateNewLogAndRefreshTourLogList(Tour tour, string date, string report = "", int distance = 0, string duration = "", int rating = 0, int steps = 0, float weightkg = 0, string bloodpreassure = "", string feeling = "", string weather = "")
        {
            logDAO.AddNewItemLog(tour, date, report, distance, duration, rating, steps, weightkg, bloodpreassure, feeling, weather);
            return (List<Log>)logDAO.GetLogsOfTour(tour);
        }

        public List<Log> UpdateLogAndRefreshTourLogList(Log oldLog, string date = "", string report = "", int distance = 0, string duration = "", int rating = 0, int steps = 0, float weightkg = 0, string bloodpreassure = "", string feeling = "", string weather = "")
        {
            throw new NotImplementedException();
           // return new List<Log>();
        }

        public List<Tour> GetToursFromDB()
        {
            Debug.WriteLine("GET ALL TOURS FROM DB");
            return (List<Tour>)tourDAO.GetTours();
        }

        public List<Log> DeleteLogAndRefreshTourLogList()
        {
            throw new NotImplementedException();
        }

        //MoRE To COME 
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using TourFinder.DataAccessLayer.Common;
using TourFinder.DataAccessLayer.DAObject;
using TourFinder.Models;

namespace TourFinder.DataAccessLayer.PostgresSqlDB
{
    class LogSqlPostgresDAO : ITourLogDAO
    {
        private const string SQL_FIND_BY_ID = "SELECT * FROM logs WHERE id = @id";
        private const string SQL_FIND_BY_TOUR = "SELECT * FROM logs WHERE tourId = @id";
        private const string SQL_INSERT_NEW_LOG = "INSERT INTO logs " +
            "(tourid, date, report, distance, duration, rating, steps, weightkg, bloodpreassure, feeling, weather) " +
            "VALUES (@tourid, @date, @report, @distance, @duration, @rating, @steps, @weightkg, @bloodpreassure, @feeling, @weather) " +
            "RETURNING id";
        private const string SQL_DELETE_LOG = "DELETE FROM logs WHERE id = @id;";
        private const string SQL_UPDATE_LOG = "UPDATE logs SET report = @report, " +
                                                "distance = @distance, duration = @duration, " +
                                                "rating = @rating, steps = @steps, weightkg = @weightkg," +
                                                "bloodpreassure = @bloodpreassure, feeling = @feeling, weather = @weather " +
                                                "WHERE id = @id";

        private IDBAccess database;
        private ITourDAO tourDAO;

        public LogSqlPostgresDAO(IDBAccess database, ITourDAO tourDAO)
        {
            this.database = database;
            this.tourDAO = tourDAO;
        }

        public Log AddNewTourLog(Tour tour, string date, string report = "\"\"", int distance = 0, string duration = "\"\"", 
                                int rating = 0, int steps = 0, float weightkg = 0, string bloodpreassure = "\"\"",
                                string feeling = "\"\"", string weather = "\"\"")
        {
            DbCommand command = database.CreateCommand(SQL_INSERT_NEW_LOG);
            database.DefineParameter<int>(command, "@tourid", DbType.Int32, tour.ID);
            database.DefineParameter<string>(command, "@date", DbType.String, date);
            database.DefineParameter<string>(command, "@report", DbType.String, report);
            database.DefineParameter<int?>(command, "@distance", DbType.Int32, distance);
            database.DefineParameter<string>(command, "@duration", DbType.String, duration);
            database.DefineParameter<int?>(command, "@rating", DbType.Int32, rating);
            database.DefineParameter<int?>(command, "@steps", DbType.Int32, steps);
            database.DefineParameter<float?>(command, "@weightkg", DbType.Decimal, weightkg);
            database.DefineParameter<string>(command, "@bloodpreassure", DbType.String, bloodpreassure);
            database.DefineParameter<string>(command, "@feeling", DbType.String, feeling);
            database.DefineParameter<string>(command, "@weather", DbType.String, weather);

            return FindById(database.ExecuteScalar(command));
        }

        public Log FindById(int logId)
        {
            DbCommand command = database.CreateCommand(SQL_FIND_BY_ID);
            database.DefineParameter<int>(command, "@id", DbType.Int32, logId);

            IEnumerable<Log> loglist = QueryLogsFromDB(command);
            return loglist.FirstOrDefault();
        }

        public IEnumerable<Log> GetLogsOfTour(Tour tour)
        {
            DbCommand command = database.CreateCommand(SQL_FIND_BY_TOUR);
            database.DefineParameter<int>(command, "@id", DbType.Int32, tour.ID);

            List<Log> loglist = (List<Log>)QueryLogsFromDB(command);
            foreach(Log log in loglist)
            {
                log.Tour = tour;
            }

            return loglist;
        }

        private IEnumerable<Log> QueryLogsFromDB(DbCommand command)
        {
            List<Log> loglist = new List<Log>();

            using (IDataReader reader = database.ExecuteReader(command))
            {
                while (reader.Read())
                {
                    loglist.Add(new Log()
                    {
                        ID = (int)reader["id"],
                        Date = (string)reader["date"],
                        Report = (string)reader["report"],
                        Duration = (string)reader["duration"],
                        Rating = (int)(long)reader["rating"],
                        Steps = (int)(long)reader["steps"],
                        Distance = (int)(long)reader["distance"],
                        WeightKG = Convert.ToSingle(reader["weightkg"]),
                        BloodPreassure = (string)reader["bloodpreassure"],
                        Feeling = (string)reader["feeling"],
                        Weather = (string)reader["feeling"],
                    }) ;
                }
            }
            return loglist;
        }

        public int CopyLog(Log oldLog)
        {
            DbCommand command = database.CreateCommand(SQL_INSERT_NEW_LOG);
            database.DefineParameter<int>(command, "@tourid", DbType.Int32, oldLog.Tour.ID);
            database.DefineParameter<string>(command, "@date", DbType.String, oldLog.Date);
            database.DefineParameter<string>(command, "@report", DbType.String, oldLog.Report);
            database.DefineParameter<int>(command, "@distance", DbType.Int32, oldLog.Distance);
            database.DefineParameter<string>(command, "@duration", DbType.String, oldLog.Duration);
            database.DefineParameter<int>(command, "@rating", DbType.Int32, oldLog.Rating);
            database.DefineParameter<int?>(command, "@steps", DbType.Int32, oldLog.Steps);
            database.DefineParameter<float?>(command, "@weightkg", DbType.Decimal, oldLog.WeightKG);
            database.DefineParameter<string>(command, "@bloodpreassure", DbType.String, oldLog.BloodPreassure);
            database.DefineParameter<string>(command, "@feeling", DbType.String, oldLog.Feeling);
            database.DefineParameter<string>(command, "@weather", DbType.String, oldLog.Weather);

            
            return database.ExecuteScalar(command);
        }

        public int DeleteLog(Log oldLog)
        {
            DbCommand command = database.CreateCommand(SQL_DELETE_LOG);
            database.DefineParameter<int>(command, "@id", DbType.Int32, oldLog.ID);

            return database.ExecuteScalar(command);
        }
        public int UpdateLog(Log oldLog, string report = "\"\"", int distance = 0, string druration = "\"\"",
                            int rating = 0, int steps = 0, float weightkg = 0, string bloodpreassure = "\"\"",
                            string feeling = "\"\"", string weather = "\"\"")
        {
            DbCommand command = database.CreateCommand(SQL_UPDATE_LOG);
            database.DefineParameter<int>(command, "@id", DbType.Int32, oldLog.ID);
            database.DefineParameter<string>(command, "@report", DbType.String, report);
            database.DefineParameter<int>(command, "@distance", DbType.Int32, distance);
            database.DefineParameter<string>(command, "@duration", DbType.String, druration);
            database.DefineParameter<int>(command, "@rating", DbType.Int32, rating);
            database.DefineParameter<int?>(command, "@steps", DbType.Int32, steps);
            database.DefineParameter<float?>(command, "@weightkg", DbType.Decimal, weightkg);
            database.DefineParameter<string>(command, "@bloodpreassure", DbType.String, bloodpreassure);
            database.DefineParameter<string>(command, "@feeling", DbType.String, feeling);
            database.DefineParameter<string>(command, "@weather", DbType.String, weather);

            return database.ExecuteScalar(command);
        }

    }
}

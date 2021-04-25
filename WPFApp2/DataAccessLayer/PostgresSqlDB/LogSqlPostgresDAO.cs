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
        private IDBAccess database;
        private ITourDAO tourDAO;

        public LogSqlPostgresDAO(IDBAccess database, ITourDAO tourDAO)
        {
            this.database = database;
            this.tourDAO = tourDAO;
        }

        public Log AddNewItemLog(Tour tour, string date, string report = "\"\"", int distance = 0, string duration = "\"\"", 
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

            return QueryLogsFromDB(command);
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
                        Tour = tourDAO.FindByID((int)reader["tourid"]),
                        Date = (string)reader["date"],
                        Report = (string)reader["report"],
                        Duration = (string)reader["duration"],
                        Rating = (int)reader["rating"],
                        Steps = (int)reader["steps"],
                        Distance = (float)reader["distance"],
                        WeightKG = (float)reader["weightkg"],
                        BloodPreassure = (string)reader["bloodpreassure"],
                        Feeling = (string)reader["feeling"],
                        Weather = (string)reader["feeling"],
                    }) ;
                }
            }
            return loglist;
        }
    }
}

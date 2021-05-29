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
    public class TourSqlPostgresDAO : ITourDAO
    {
        private IDBAccess database;
        private const string SQL_FIND_BY_ID = "SELECT * FROM Tours WHERE id = @id;";
        private const string SQL_GET_ALL_ITeMS = "SELECT * FROM tours;";
        private const string SQL_INSTERT_NEW_ITEM = "INSERT INTO tours (name, startlocation, endlocation, description, distance, mapimagepath) " +
                                                    "VALUES (@name, @startlocation, @endlocation, @description, @distance, @mapimagepath) " +
                                                    "RETURNING id;";
        private const string SQL_UPDATE_TOUR = "UPDATE Tours SET name = @newname, description = @newdescription WHERE id = @tourid;";
        private const string SQL_CHECK_IMG = "SELECT mapimagepath FROM Tours";
        private const string SQL_DELETE_TOUR = "DELETE FROM Tours WHERE id = @tourid;";

        public TourSqlPostgresDAO(IDBAccess database)
        {
            this.database = database;
        }

        public Tour FindByID(int itemId)
        {
            DbCommand command = database.CreateCommand(SQL_FIND_BY_ID);
            database.DefineParameter<int>(command, "@id", DbType.Int32, itemId);

            IEnumerable<Tour> tourList = QueryToursFromDatabase(command);
            return tourList.FirstOrDefault();
        }

        public IEnumerable<Tour> GetTours()
        {
            DbCommand command = database.CreateCommand(SQL_GET_ALL_ITeMS);

            return QueryToursFromDatabase(command);
        }

        private IEnumerable<Tour> QueryToursFromDatabase(DbCommand command)
        {
            List<Tour> tourList = new List<Tour>();

            using (IDataReader reader = database.ExecuteReader(command))
            {
                while (reader.Read())
                {
                    if(reader["id"] != null)
                    {
                        Tour tmp = new Tour()
                        {
                            ID = (int)(long)reader["id"],
                            Name = (string)reader["name"],
                            Description = (string)reader["description"],
                            StartLocation = (string)reader["startlocation"],
                            EndLocation = (string)reader["endlocation"],
                            MapImagePath = (string)reader["mapimagepath"],
                            Distance = Convert.ToSingle(reader["distance"])
                        };
                        tourList.Add(tmp);
                    }
                }
            }
        return tourList;
        }

        public Tour AddNewTour(string name, string startLocation, string endLocation, float distance, string mapImagePath, string description = "\"\"")
        {
            DbCommand command = database.CreateCommand(SQL_INSTERT_NEW_ITEM);
            database.DefineParameter<string>(command, "@name", DbType.String, name);
            database.DefineParameter<string>(command, "@startlocation", DbType.String, startLocation);
            database.DefineParameter<string>(command, "@endlocation", DbType.String, endLocation);
            database.DefineParameter<float>(command, "@distance", DbType.Decimal, distance);
            database.DefineParameter<string>(command, "@mapimagepath", DbType.String, mapImagePath);
            database.DefineParameter<string>(command, "@description", DbType.String, description);

            return FindByID(database.ExecuteScalar(command));
        }

        public Tour AddNewTour(Tour tour)
        {
            return AddNewTour(tour.Name, tour.StartLocation, tour.EndLocation, tour.Distance, tour.MapImagePath, tour.Description);
        }

        public int CopyTour(Tour oldTour)
        {
            DbCommand command = database.CreateCommand(SQL_INSTERT_NEW_ITEM);
            database.DefineParameter<string>(command, "@name", DbType.String, oldTour.Name + "_Copy");
            database.DefineParameter<string>(command, "@startlocation", DbType.String, oldTour.StartLocation);
            database.DefineParameter<string>(command, "@endlocation", DbType.String, oldTour.EndLocation);
            database.DefineParameter<float>(command, "@distance", DbType.Decimal, oldTour.Distance);
            database.DefineParameter<string>(command, "@description", DbType.String, oldTour.Description);
            database.DefineParameter<string>(command, "@mapimagepath", DbType.String, oldTour.MapImagePath);

            return database.ExecuteScalar(command);
        }

        public int UpdateTour(int tourid, string newName, string newDescription)
        {
            DbCommand command = database.CreateCommand(SQL_UPDATE_TOUR);
            database.DefineParameter<int>(command, "@tourid", DbType.Int32, tourid);
            database.DefineParameter<string>(command, "@newname", DbType.String, newName);
            database.DefineParameter<string>(command, "@newdescription", DbType.String, newDescription);

            return database.ExecuteScalar(command);
        }

        public int DeleteTour(Tour oldTour)
        {
            DbCommand command = database.CreateCommand(SQL_DELETE_TOUR);
            database.DefineParameter<int>(command, "@tourid", DbType.Int32, oldTour.ID);

            return database.ExecuteScalar(command);
        }

        public IEnumerable<string> GetAllTourImages()
        {
            List<string> mapPathList = new List<string>();
            DbCommand command = database.CreateCommand(SQL_CHECK_IMG);

            using (IDataReader reader = database.ExecuteReader(command))
            {
                while (reader.Read())
                {
                    if (!mapPathList.Contains((string)reader["mapimagepath"]))
                    {
                        mapPathList.Add((string)reader["mapimagepath"]);
                    }
                }
            }
            return mapPathList;
        }

        
    }
}

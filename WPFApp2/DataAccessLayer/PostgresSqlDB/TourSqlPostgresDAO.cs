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
        private ITourLogDAO logDAO;
        private const string SQL_FIND_BY_ID = "SELECT * FROM Tours WHERE id = @id";
        private const string SQL_GET_ALL_ITeMS = "SELECT * FROM tours";
        private const string SQL_INSTERT_NEW_ITEM = "INSERT INTO tours (name, startlocation, endlocation, description, distance, mapimagepath) " +
                                                    "VALUES (@name, @startlocation, @endlocation, @description, @distance, @mapimagepath) " +
                                                    "RETURNING id;";

        public TourSqlPostgresDAO(IDBAccess database)
        {
            this.database = database;
        }
        public Tour AddNewTour(string name, string startLocation, string endLocation, float distance, string mapImagePath, string description = null)
        {
            DbCommand command = database.CreateCommand(SQL_INSTERT_NEW_ITEM);
            database.DefineParameter<string>(command, "@name", DbType.String , name);
            database.DefineParameter<string>(command, "@startlocation", DbType.String, startLocation);
            database.DefineParameter<string>(command, "@endlocation", DbType.String, endLocation);
            database.DefineParameter<float>(command, "@distance", DbType.Decimal, distance);
            database.DefineParameter<string>(command, "@description", DbType.String, description);
            database.DefineParameter<string>(command, "@mapimagepath", DbType.String, mapImagePath);

            return FindByID(database.ExecuteScalar(command));
        }

        public Tour AddNewItem(Tour copyTour)
        {
            DbCommand command = database.CreateCommand(SQL_INSTERT_NEW_ITEM);
            database.DefineParameter<string>(command, "@name", DbType.String, copyTour.Name);
            database.DefineParameter<string>(command, "@startlocation", DbType.String, copyTour.StartLocation);
            database.DefineParameter<string>(command, "@endlocation", DbType.String, copyTour.EndLocation);
            database.DefineParameter<float>(command, "@distance", DbType.Decimal, copyTour.Distance);
            database.DefineParameter<string>(command, "@description", DbType.String, copyTour.Description);
            database.DefineParameter<string>(command, "@mapimagepath", DbType.String, copyTour.MapImagePath);

            return FindByID(database.ExecuteScalar(command));
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

            //query Tour from database
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
    }
}

using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Npgsql;
using TourFinder.DataAccessLayer.Common;

namespace TourFinder.DataAccessLayer.PostgresSqlDB
{
    public class PostgresSqlConnector : IDBAccess
    {
        private DbConnection connection;
        static PostgresSqlConnector instance = null;
        //string sqlConnectionString = "Host=localhost;Username=postgres;Password=postgres;Database=tourplanner;Pooling=false";
        private static string connectionString;

        private PostgresSqlConnector()
        {
            connectionString = ConfigurationManager.ConnectionStrings["PostgresSQLConnectionString"].ConnectionString;
            connection = CreateConnection();
        }

        public static PostgresSqlConnector Instance()
        {
            if(instance == null)
            {
                instance = new PostgresSqlConnector();
            }
            return instance;
        }

        public DbCommand CreateCommand(string genericCommandText)
        {
            return new NpgsqlCommand(genericCommandText);
        }

        public int DeclareParameter(DbCommand command, string name, DbType type)
        {
            if (!command.Parameters.Contains(name))
            {
                int index = command.Parameters.Add(new NpgsqlParameter(name, type));
                return index;
            }
            throw new ArgumentException(string.Format("Parameter {0} already exists.",name));
        }

        public void DefineParameter<T>(DbCommand command, string name, DbType type, T value)
        {
            int index = DeclareParameter(command, name, type);
            command.Parameters[index].Value = value;
        }

        public void SetParameter<T> (DbCommand command, string name, T value)
        {
            if (command.Parameters.Contains(name))
            {
                command.Parameters[name].Value = value;
                return;
            }
            throw new ArgumentException(string.Format("Parameter {0} does not exist.", name));
        }

        public IDataReader ExecuteReader(DbCommand command)
        {
            command.Connection = connection;
            return command.ExecuteReader();
        }

        public int ExecuteScalar(DbCommand command)
        {
            command.Connection = connection;
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public  DbConnection CreateConnection()
        {
            DbConnection connection = new NpgsqlConnection(connectionString);
            Task tmp =  connection.OpenAsync();
            tmp.Wait();
            return connection;
        }

        public void CloseConnection()
        {
            connection.Close();
        }

        public string GetAllCardsFromDB()
        {
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM cards", (NpgsqlConnection)connection);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            string output = "Cards:\n";
            while (reader.Read())
            {
                  output += (
                                    reader.GetInt32(0) + " " + //CardID
                                    reader.GetString(1) + " " + //Cardname
                                    reader.GetString(2) + " " + //CardInfo
                                    Convert.ToInt32(reader.GetBoolean(3)) + " " + //CardType
                                    reader.GetInt32(4) + " " + //ElementType
                                    reader.GetInt32(5) + " " + //SpecialType
                                    reader.GetInt32(6) + " " + //MaxHP
                                    reader.GetInt32(7) + " " + //MaxAP
                                    reader.GetInt32(8) + " " + //MaxDP
                                    reader.GetBoolean(9) + "\n"//piercing
                                    ); 
            }
            reader.Close();
            Console.WriteLine("Got all them cards Mate!");
            return output;
        }
    }
}

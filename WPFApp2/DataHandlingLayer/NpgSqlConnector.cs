using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace TourFinder
{
    class NpgSqlConnector : IDBConnector
    {
        private NpgsqlConnection connection;
        static NpgSqlConnector instance = null;
        
        private NpgSqlConnector()
        {
            Connect();
        }

        public static NpgSqlConnector Instance()
        {
            if(instance == null)
            {
                instance = new NpgSqlConnector();
            }
            return instance;
        }

        private void Connect()
        {
            string sqlConnectionString = "Host=localhost;Username=postgres;Password=postgres;Database=mtcg;Pooling=false";

            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(sqlConnectionString);
                connection.Open();
                Console.WriteLine("No Error - Connection to Database Established!");
                this.connection = connection;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void CloseConnection()
        {
            connection.Close();
        }

        public string GetAllCardsFromDB()
        {
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM cards", connection);
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

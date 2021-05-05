using System.Data;
using System.Data.Common;

namespace TourFinder.DataAccessLayer.Common
{
    public interface IDBAccess
    {
        public string GetAllCardsFromDB();


        public DbConnection CreateConnection();
        public void CloseConnection();
        DbCommand CreateCommand(string genericCommandText);

        int DeclareParameter(DbCommand command, string name, DbType type);
        void DefineParameter<T>(DbCommand command, string name, DbType type, T value);
        void SetParameter<T>(DbCommand command, string name, T value);

        IDataReader ExecuteReader(DbCommand command); // für alles wo daten zurückkommen
        int ExecuteScalar(DbCommand command); //für alles wo keine daten zurückkommen (also zb nur success)

        //void DefineParameter<T>(DbCommand command, string v, DbType int32, object id);
    }
}

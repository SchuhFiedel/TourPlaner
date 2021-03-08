using System;
using System.Collections.Generic;
using System.Text;


namespace TourFinder.BackendStuff.DB
{
    interface IDBConnector
    {
        public string GetAllCardsFromDB();
        public void CloseConnection();
    }
}

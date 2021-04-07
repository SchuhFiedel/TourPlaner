using System;
using System.Collections.Generic;
using System.Text;


namespace TourFinder
{
    interface IDBConnector
    {
        public string GetAllCardsFromDB();
        public void CloseConnection();
    }
}

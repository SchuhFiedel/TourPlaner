using NUnit.Framework;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using TourFinder;
using TourFinder.DataAccessLayer.Common;
using TourFinder.DataAccessLayer.PostgresSqlDB;
using TourFinder.Models;

namespace NUnitTestProject1
{
    class Tests
    {
        [Test]
        public void TourAddUtilityTest()
        {
            TourAddUtility tourAddUtility = new TourAddUtility();
            tourAddUtility.Description = "Description";
            tourAddUtility.Name = "Name";
            Tour tour = new Tour() { Description = tourAddUtility.Description, Name = tourAddUtility.Name };
            Assert.IsTrue(tour.Name == tourAddUtility.Name && tour.Description == tourAddUtility.Description);
        }

        /*
        [Test]
        public void DataLayerAccessManagerTest ()
        {
            DataLayerAccessManager DLAM = new DataLayerAccessManager();
            //Debug.WriteLine(DLAM.GetToursFromDB().ToString());
            Assert.IsNotNull(DLAM);
        }

        /*
        [Test]
        public void misc()
        {
            var tmp = ConfigurationManager.ConnectionStrings;
            Assert.IsNotNull(ConfigurationManager.ConnectionStrings.Count);
        }*/
    }
}

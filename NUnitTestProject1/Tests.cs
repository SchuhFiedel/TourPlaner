using NUnit.Framework;
using TourFinder.DataAccessLayer.Common;
using TourFinder.Models;
using TourFinder.BusinessLayer;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using TourFinder.DataAccessLayer.FileSystem;
using TourFinder.DataAccessLayer.PostgresSqlDB;
using TourFinder.BusinessLayer.PDF;
using TourFinder.DataAccessLayer.MapQuestConnector;

namespace NUnitTestProject1
{
    class MoreGeneralTests
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
        
        [Test]
        public void DataLayerAccessManagerTest ()
        {
            DataLayerAccessManager DLAM = DataLayerAccessManager.GetInstance();
            Assert.IsNotNull(DLAM);
        }

        [Test]
        public void BusinessLayerManagerTest()
        {
            BusinessLayerManager blm = new BusinessLayerManager();
            Assert.IsNotNull(blm);
        }

        [Test]
        public void GetToursFromDB_BusinessLayer()
        {
            BusinessLayerManager blm = new();
            List<Tour> tourlist = blm.GetAllToursFromDB();
            Assert.IsTrue(tourlist.Count > 0);
        }

        [Test]
        public void GetToursFromDB_DataLayerAccessManager()
        {
            DataLayerAccessManager DLAM  = DataLayerAccessManager.GetInstance();
            List<Tour> tourlist = new List<Tour>(DLAM.GetToursFromDB());
            Assert.IsTrue(tourlist.Count > 0);
        }

        [Test]
        public void GetToursFromDB_CheckLogs_BusinessLayer()
        {
            BusinessLayerManager blm = new();
            List<Tour> tourlist = blm.GetAllToursFromDB();

            foreach(Tour tour in tourlist)
            {
                if(tour.LogList.Count > 0)
                {
                    Assert.Pass();
                }
            }
            Assert.Fail();
        }

        [Test]
        public void GetToursFromDB_CheckLogs_DataAccessLayer()
        {
            DataLayerAccessManager DAL = DataLayerAccessManager.GetInstance();
            List<Tour> tourlist = (List<Tour>)DAL.GetToursFromDB();

            foreach (Tour tour in tourlist)
            {
                if (tour.LogList.Count > 0)
                {
                    Assert.Pass();
                }
            }
            Assert.Fail();
        }

        [Test]
        public void FileExport_DataLayerAccessManager()
        {
            DataLayerAccessManager DLAM = DataLayerAccessManager.GetInstance();
            string path = "JsonExportTest.json";
            DLAM.ExportToursToJSONFile(MakeTestTourList(), path);
            Assert.IsTrue(File.Exists(ConfigurationManager.AppSettings["FileFolder"] + path));
            File.Delete(ConfigurationManager.AppSettings["FileFolder"] + path);
        }

        [Test]
        public void GetImagesFromFolder()
        {
            var db = PostgresSqlConnector.Instance();
            var tourdao = new TourSqlPostgresDAO(db);
            Filemanager fm = Filemanager.GetInstance(tourdao, new LogSqlPostgresDAO(db));
            Assert.IsNotNull(fm.GetAllImagesFromFolder());
        }

        [Test]
        public void DeleteUnusedTourImages()
        {
            DataLayerAccessManager DLAM = DataLayerAccessManager.GetInstance();
            Assert.IsTrue(DLAM.DeleteUnusedTourImages(0) != 0);
        }

        [Test]
        public void TourPDFTest()
        {
            string filename = "TestTourPDF";
            PDFMaker.MakeTourPDF(((List<Tour>)MakeTestTourList())[0], filename);
            Assert.IsTrue(File.Exists(ConfigurationManager.AppSettings["FileFolder"] + filename + ".pdf"));
            //File.Delete(ConfigurationManager.AppSettings["FileFolder"] + path);
        }
        
        [Test]
        public void TourListPDFTest()
        {
            string filename = "TestTourListPDF";
            PDFMaker.MakeAllTourPDF((List<Tour>)MakeTestTourList() , filename);
            Assert.IsTrue(File.Exists(ConfigurationManager.AppSettings["FileFolder"] + filename + ".pdf"));
            //File.Delete(ConfigurationManager.AppSettings["FileFolder"] + path);
        }

        [Test]
        public void TourSummaryPDFTest()
        {
            string filename = "TestTourSummary";
            PDFMaker.MakeTourSummaryPDF((List<Tour>)MakeTestTourList(), filename);
            Assert.IsTrue(File.Exists(ConfigurationManager.AppSettings["FileFolder"] + filename + ".pdf"));
            //File.Delete(ConfigurationManager.AppSettings["FileFolder"] + path);
        }

        [Test]
        public void InstanceOfRestDataClass()
        {
            RestDataClass RDC = RestDataClass.Instance();
            Assert.IsNotNull(RDC);
        }

        [Test]
        public void SearchTest_BLM()
        {
            BusinessLayerManager BLM = new BusinessLayerManager();
            BLM.CreateNewTour("SEARCHTEST1", "Stephansplatz,Vienna,Austria", "Mariahilfer-Straße,Vienna,Austria", "");
            List<Tour> searchList = (List<Tour>)BLM.Search("SEARCHTEST1");
            Assert.AreEqual(1, searchList.Count);
            BLM.DeleteTourGetList(searchList[0]);
        }

        [Test]
        public void GetAllImagesFromDB()
        {
            BusinessLayerManager BLM = new BusinessLayerManager();
            List<Tour> tourlist = BLM.GetAllToursFromDB();
            TourSqlPostgresDAO tourdao = new TourSqlPostgresDAO(PostgresSqlConnector.Instance());
            List<string> imageList = (List<string>)tourdao.GetAllTourImages();
            Assert.AreEqual(tourlist.Count, imageList.Count);
        }

        //Misc
        IEnumerable<Tour> MakeTestTourList()
        {
            List<Tour> tourlist = new List<Tour>()
            {
                new Tour()
                {
                    ID = 80,
                    Name = "TestTourOne",
                    Description = "TestTestTest",
                    StartLocation = "Vienna,Austria",
                    EndLocation = "Karl-Renner-Ring,Vienna,Austria",
                    Distance = 3,
                    MapImagePath = "Map80.jpg",
                    LogList = new System.Collections.ObjectModel.ObservableCollection<Log>()
                    {
                        new Log()
                        {
                            ID = 150,
                            Distance = 1,
                            Date = "00.00.0000",
                            Duration = "5",
                            Report = "Great Stuff",
                            Steps = 1500
                        },
                        new Log()
                        {
                            ID = 250,
                            Distance = 1,
                            Date = "00.00.0000",
                            Duration = "5",
                            Report = "Great Stuff",
                            Steps = 1500
                        }
                    }
                },
                new Tour()
                {
                    ID = 888,
                    Name = "TestTourTwo",
                    Description = "TestTestTest",
                    StartLocation = "Vienna,Austria",
                    EndLocation = "Mariahilferstraße,Vienna,Austria",
                    Distance = 3,
                    MapImagePath = "Map888.jpg",
                    LogList = new System.Collections.ObjectModel.ObservableCollection<Log>()
                    {
                        new Log()
                        {
                            ID = 355,
                            Distance = 1,
                            Date = "00.00.0000",
                            Duration = "5",
                            Report = "Great Stuff",
                            Steps = 1500
                        },
                        new Log()
                        {
                            ID = 450,
                            Distance = 1,
                            Date = "00.00.0000",
                            Duration = "5",
                            Report = "Great Stuff",
                            Steps = 1500
                        },
                        new Log()
                        {
                            ID = 455,
                            Distance = 1,
                            Date = "00.00.0000",
                            Duration = "5",
                            Report = "Great Stuff",
                            Steps = 1500
                        },
                        new Log()
                        {
                            ID = 550,
                            Distance = 1,
                            Date = "00.00.0000",
                            Duration = "5",
                            Report = "Great Stuff",
                            Steps = 1500
                        },
                        new Log()
                        {
                            ID = 555,
                            Distance = 1,
                            Date = "00.00.0000",
                            Duration = "5",
                            Report = "Great Stuff",
                            Steps = 1500
                        },
                        new Log()
                        {
                            ID = 650,
                            Distance = 1,
                            Date = "00.00.0000",
                            Duration = "5",
                            Report = "Great Stuff",
                            Steps = 1500
                        }
                    }
                }
            };
            foreach (Tour tour in tourlist)
            {
                foreach (Log log in tour.LogList)
                {
                    log.Tour = tour;
                }
            }
            return tourlist;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using TourFinder.DataAccessLayer.DAObject;
using TourFinder.Models;

namespace TourFinder.DataAccessLayer.FileSystem
{
    public class Filemanager
    {
        private string filefolder;
        private static Filemanager instance;
        private static ITourDAO tourDAO;
        private static ITourLogDAO logDAO;

        private Filemanager(ITourDAO newTourDAO, ITourLogDAO newLogDAo)
        {
            filefolder = ConfigurationManager.AppSettings["FileFolder"];
            tourDAO = newTourDAO;
            logDAO = newLogDAo;
        }

        static public Filemanager GetInstance(ITourDAO newTourDAO, ITourLogDAO newLogDAo)
        {
            if(instance == null)
            {
                instance = new Filemanager(newTourDAO, newLogDAo);
            }
            return instance;
        }

        public IEnumerable<Tour> ImportToursFromFile(string fileName)
        {
            try
            {
                JsonSerializerOptions options = new() { WriteIndented = true, MaxDepth = 0, ReferenceHandler = ReferenceHandler.Preserve };
                string fileString = File.ReadAllText(filefolder + fileName);

                
                List<Tour> tourList = JsonSerializer.Deserialize<List<Tour>>(fileString, options);

                foreach (Tour tour in tourList)
                {
                    Tour tmptour = tourDAO.AddNewTour(tour);
                    if (tour.LogList != null)
                    {
                        foreach (Log log in tour.LogList)
                        {
                            log.Tour = tmptour;
                            logDAO.AddNewTourLog(log.Tour, log);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString() + " Source: " + e.Source + "\n" + e.Message);
            }

            return tourDAO.GetTours();
        }

        public void ExportToursToFile(IEnumerable<Tour> tourList, string filename)
        {
            try
            {
                JsonSerializerOptions options = new() { WriteIndented = true, MaxDepth = 0, ReferenceHandler = ReferenceHandler.Preserve};
                string jsonString = JsonSerializer.Serialize<IEnumerable<Tour>>(tourList, options);

                FileInfo file = new FileInfo(filefolder);
                file.Directory.Create();

                File.WriteAllText(filefolder + filename, jsonString);
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.ToString() + " Source: " + e.Source + "\n" + e.Message);
            }
        }

        public IEnumerable<string> GetAllImagesFromFolder()
        {
            List<string> allFiles = new List<string>();

            DirectoryInfo d = new DirectoryInfo(ConfigurationManager.AppSettings["ImageFolder"]);
            FileInfo[] files = d.GetFiles("*jpg");
            foreach (var file in files)
            {
                allFiles.Add(file.Name);
            }
            return allFiles;
        }

        public void DeleteImage(string image)
        {
            string path = ConfigurationManager.AppSettings["ImageFolder"] + image;
            File.Delete(path);
        }
    }
}

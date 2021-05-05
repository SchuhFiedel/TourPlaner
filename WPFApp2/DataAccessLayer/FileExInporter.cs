using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourFinder.Models;

namespace TourFinder.DataAccessLayer.FileSystem
{
    public class FileExInporter
    {
        private string filefolder;
        private static FileExInporter instance;

        private FileExInporter()
        {
            filefolder = ConfigurationManager.AppSettings["FileFolder"];
        }

        static public FileExInporter GetInstance()
        {
            if(instance == null)
            {
                instance = new FileExInporter();
            }
            return instance;
        }

        public IEnumerable<Tour> ImportToursFromFile()
        {


            throw new NotImplementedException();
            
        }

        public void ExportToursToFile(IEnumerable<Tour> tourList)
        {
            throw new NotImplementedException();
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

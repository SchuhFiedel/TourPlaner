using System;
using System.Collections.Generic;
using System.Configuration;
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
    }
}

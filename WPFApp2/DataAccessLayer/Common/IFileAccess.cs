using System.Collections.Generic;
using System.IO;
using TourFinder.Models;

namespace TourFinder.DataAccessLayer.Common
{
    public interface IFileAccess
    {
        int CreateNewTourFile(string filename, Tour tour);
        int CreateNewLogFile(string filename, Log log, Tour tour);
        IEnumerable<FileInfo> SearchFiles(string searchTerm, ModelTypes searchType);
        IEnumerable<FileInfo> GetAllFiles(ModelTypes searchType);
    }
}

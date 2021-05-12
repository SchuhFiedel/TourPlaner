using System.Collections.Generic;
using TourFinder.Models;

namespace TourFinder.DataAccessLayer.DAObject
{
    public interface ITourLogDAO
    {
        Log FindById(int logId);
        
        IEnumerable<Log> GetLogsOfTour(Tour tour);
        Log AddNewTourLog(Tour tour, string date, string report = "\"\"", int distance = 0, string druration = "\"\"",
                            int rating = 0, int steps = 0, float weightkg = 0, string bloodpreassure = "\"\"",
                            string feeling = "\"\"", string weather = "\"\"");
        Log AddNewTourLog(Tour tour, Log log);
        int UpdateLog(Log oldLog, string report = "\"\"", int distance = 0, string druration = "\"\"",
                            int rating = 0, int steps = 0, float weightkg = 0, string bloodpreassure = "\"\"",
                            string feeling = "\"\"", string weather = "\"\"");
        int CopyLog(Log oldLog);
        int DeleteLog(Log oldLog);

    }
}

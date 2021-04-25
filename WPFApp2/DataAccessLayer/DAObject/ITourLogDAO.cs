using System.Collections.Generic;
using TourFinder.Models;

namespace TourFinder.DataAccessLayer.DAObject
{
    public interface ITourLogDAO
    {
        Log FindById(int logId);
        Log AddNewItemLog(Tour tour, string date, string report = "\"\"", int distance = 0, string druration = "\"\"",
                            int rating = 0, int steps = 0, float weightkg = 0, string bloodpreassure = "\"\"", 
                            string feeling = "\"\"", string weather = "\"\"");
        IEnumerable<Log> GetLogsOfTour(Tour tour);
    }
}

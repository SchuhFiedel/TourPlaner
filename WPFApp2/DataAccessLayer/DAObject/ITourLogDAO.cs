using System.Collections.Generic;
using TourFinder.Models;

namespace TourFinder.DataAccessLayer.DAObject
{
    public interface ITourLogDAO
    {
        Log FindById(int logId);
        Log AddNewItemLog(Tour tour, string date, string report = null, int? distance = null,
                                 string duration = null, int? rating = null, int? steps = null, float? weightkg = null,
                                 string bloodpreassure = null, string feeling = null, string weather = null);
        IEnumerable<Log> GetLogsOfTour(Tour tour);
    }
}

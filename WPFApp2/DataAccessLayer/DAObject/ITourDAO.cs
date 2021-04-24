using System.Collections.Generic;
using TourFinder.Models;

namespace TourFinder.DataAccessLayer.DAObject
{
    public interface ITourDAO
    {
        Tour FindByID(int itemId);
        //WAS BRAuch ich ALLES UNBEDINGT IM TOUR OBJEKT?
        Tour AddNewTour(string name, string startLocation, string endLocation, float distance, string mapImagePath, string description = null);
        Tour AddNewItem(Tour copyTour);
        IEnumerable<Tour> GetTours();
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourFinder.Models
{
    public class Tour
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public string Description { get; set; }
        public float Distance { get; set; }
        public ObservableCollection<Log> LogList { get; set; }
        public string MapImagePath { get; set; }
    }
}

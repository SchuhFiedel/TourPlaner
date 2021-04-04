using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourFinder
{
    public class Tour
    {
        public string Name { get; set; }
        public int Distance { get; set; }
        public string Description { get; set; }
        public ObservableCollection<Log> LogList { get; set; }
        public string MapImagePath { get; set; }
    }
}

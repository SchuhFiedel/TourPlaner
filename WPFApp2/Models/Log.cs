namespace TourFinder.Models
{
    public class Log
    {
        public int ID { get; set; }
        public Tour Tour { get; set; }
        public string Date { get; set; }
		public string Report { get; set; }
		public int Distance { get; set; }
        public string Duration { get; set; }
		public int Rating { get; set;}
		public int Steps { get; set; }
		public float WeightKG { get; set; }
		public string BloodPreassure { get; set; }
        public string Feeling { get; set; }
        public string Weather { get; set; }
    }
}

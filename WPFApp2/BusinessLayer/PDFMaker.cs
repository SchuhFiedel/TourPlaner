using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Tables;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using TourFinder.Models;


namespace TourFinder.BusinessLayer.PDF
{
    static class PDFMaker
    {
        public static string imagePath = ConfigurationManager.AppSettings["ImageFolder"];
        public static PdfFont headerFont = new PdfStandardFont(PdfFontFamily.Helvetica, 20);
        public static PdfFont normalFont = new PdfStandardFont(PdfFontFamily.Helvetica, 11);
        public static string filepath = ConfigurationManager.AppSettings["FileFolder"];
        public static string FALLBACKIMAGE = ConfigurationManager.AppSettings["FallbackImage"];

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("PDFMaker.cs");

        public static void MakeTourPDF(Tour tour)
        {
            
            string file = filepath + tour.Name + ".pdf"; // aus .config auslesen

            FileStream fs = File.Open(file, FileMode.Create);
            PdfDocument document = new PdfDocument();

            PdfPage page = document.Pages.Add();

            PdfGraphics graphics = page.Graphics;

            //MAKE TEXT
            graphics.DrawString("Tour: " + tour.Name, headerFont, PdfBrushes.Black, new PointF(0, 0));
            graphics.DrawString("From: " + tour.StartLocation + "\n To: " + tour.EndLocation, normalFont, PdfBrushes.Black, new PointF(0, 30));
            graphics.DrawString("Description:\n" + tour.Description, normalFont, PdfBrushes.Black, new PointF(0, 60));
            

            PdfBitmap image;
            //MAKE IMAGE
            try
            {
                string path = imagePath + tour.MapImagePath;
                FileStream imgStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                image = new PdfBitmap(imgStream);
            }
            catch(Exception e)
            {
                log.Info(String.Format("No Image Found for Tour: {0} \n  Error: {1}", tour.ID,e.Message ));
                string path = FALLBACKIMAGE;
                FileStream imgStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                image = new PdfBitmap(imgStream);
            }

            graphics.DrawImage(image, 300, 30, 200, 200);

            if (tour.LogList.Count > 0)
            {
                //MAKE TABLE
                PdfLightTable table = new PdfLightTable();
                table.DataSourceType = PdfLightTableDataSourceType.External;
                List<object> datalist = new List<object>();

                foreach (Log log in tour.LogList)
                {
                    object tmp = new { Date = log.Date, Report = log.Report, Distance = log.Distance, Rating = log.Rating, Steps = log.Steps, Duration = log.Duration, WeightKG = log.WeightKG, Feeling = log.Feeling, Weather = log.Weather, BloodPreassure = log.BloodPreassure };
                    datalist.Add(tmp);
                }

                table.Style.ShowHeader = true;
                table.DataSource = datalist;
                table.Draw(page, new PointF(0, 250));
            }

            document.Save(fs);

            document.Close(true);
            fs.Close();

            //open after creating
            var p = new Process { StartInfo = new ProcessStartInfo(file) { UseShellExecute = true } };
            p.Start();


            return;
        }
        
        public static void MakeAllTourPDF(IEnumerable<Tour> tourlist)
        {

            string file = filepath + "AllTours.pdf"; // aus .config auslesen

            FileStream fs = File.Open(file, FileMode.Create);
            PdfDocument document = new PdfDocument();

            foreach (Tour tour in tourlist)
            {
                PdfPage page = document.Pages.Add();

                PdfGraphics graphics = page.Graphics;

                //MAKE TEXT
                graphics.DrawString("Tour: " + tour.Name, headerFont, PdfBrushes.Black, new PointF(0, 0));
                graphics.DrawString("From: " + tour.StartLocation + "\n To: " + tour.EndLocation, normalFont, PdfBrushes.Black, new PointF(0, 30));
                graphics.DrawString("Description:\n" + tour.Description, normalFont, PdfBrushes.Black, new PointF(0, 60));

                PdfBitmap image;

                //MAKE IMAGE
                try
                {
                    string path = imagePath + tour.MapImagePath;
                    FileStream imgStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                    image = new PdfBitmap(imgStream);
                }
                catch (Exception e)
                {
                    log.Info(String.Format("No Image Found for Tour: {0} \n  Error: {1}", tour.ID, e.Message));
                    string path = FALLBACKIMAGE;
                    FileStream imgStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                    image = new PdfBitmap(imgStream);
                }

                graphics.DrawImage(image, 300, 30, 200, 200);

                if (tour.LogList.Count > 0)
                {
                    //MAKE TABLE
                    PdfLightTable table = new PdfLightTable();
                    table.DataSourceType = PdfLightTableDataSourceType.External;
                    List<object> datalist = new List<object>();

                    foreach (Log log in tour.LogList)
                    {
                        object tmp = new { Date = log.Date, Report = log.Report, Distance = log.Distance, Rating = log.Rating, Steps = log.Steps, Duration = log.Duration, WeightKG = log.WeightKG, Feeling = log.Feeling, Weather = log.Weather, BloodPreassure = log.BloodPreassure };
                        datalist.Add(tmp);
                    }

                    table.Style.ShowHeader = true;
                    table.DataSource = datalist;
                    table.Draw(page, new PointF(0, 250));
                }
            }
            
            document.Save(fs);

            document.Close(true);
            fs.Close();

            //open after creating
            var p = new Process { StartInfo = new ProcessStartInfo(file) { UseShellExecute = true } };
            p.Start();

            return;
        }

        public static void MakeTourSummaryPDF(IEnumerable<Tour> tourlist)
        {
            string file = filepath + "Summary.pdf"; // aus .config auslesen
            List<Tour> alltours = new List<Tour>(tourlist);
            

            FileStream fs = File.Open(file, FileMode.Create);
            PdfDocument document = new PdfDocument();

            PdfPage page = document.Pages.Add();
            PdfGraphics graphics = page.Graphics;

            float totalDistance = 0;
            int totalNumberLogs = 0;
            int totalSteps = 0;
            float totalWeight = 0;
            float totalRating = 0;
            float totalLogDistance = 0;

            foreach (Tour tour in alltours)
            {
                totalDistance += tour.Distance;
                totalNumberLogs += tour.LogList.Count;
                foreach(Log log in tour.LogList)
                {
                    totalSteps += log.Steps;
                    //For Average
                    totalWeight += log.WeightKG;
                    totalRating += log.Rating;
                    totalLogDistance += log.Distance;
                }
            }

            float avgWeight = 0;
            float avgRating = 0;
            float avgLogDistance = 0;
            float avgTourDistance = 0;

            //Calculate Averages
            if (totalWeight != 0) avgWeight = totalWeight / totalNumberLogs; 
            if (totalRating != 0) avgRating = totalRating / totalNumberLogs; 
            if (totalLogDistance != 0) avgLogDistance = totalLogDistance / totalNumberLogs; 
            if (totalDistance != 0) avgTourDistance = totalDistance / alltours.Count; 

            //Draw PDF
            graphics.DrawString("Number of Tours: " + alltours.Count , headerFont, PdfBrushes.Black, new PointF(0, 0));
            graphics.DrawString("Total Distance: " + totalDistance + "km", normalFont, PdfBrushes.Black, new PointF(0, 30));
            graphics.DrawString("Average Distance " + avgTourDistance + "km", normalFont, PdfBrushes.Black, new PointF(0, 60));

            graphics.DrawString("Total Number of Logs: " + totalNumberLogs , normalFont, PdfBrushes.Black, new PointF(0, 90));
            graphics.DrawString("Total Steps: " + totalSteps, normalFont, PdfBrushes.Black, new PointF(0, 120));
            graphics.DrawString("Average Weight: " + avgWeight + "kg", normalFont, PdfBrushes.Black, new PointF(0, 150));
            graphics.DrawString("Average Rating: " + avgRating + "/10", normalFont, PdfBrushes.Black, new PointF(0, 180));
            graphics.DrawString("Average Distance Walked: " + avgLogDistance + "km", normalFont, PdfBrushes.Black, new PointF(0, 210));


            document.Save(fs);

            document.Close(true);
            fs.Close();

            //open after creating
            var p = new Process { StartInfo = new ProcessStartInfo(file) { UseShellExecute = true } };
            p.Start();
        }
    }
}


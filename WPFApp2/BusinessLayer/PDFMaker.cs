using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Tables;
using System;
using System.Collections.Generic;
using System.Configuration;
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
                string path = FALLBACKIMAGE;
                FileStream imgStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                image = new PdfBitmap(imgStream);
            }
            
            graphics.DrawImage(image,300,30,200,200);

            //MAKE TABLE
            PdfLightTable table = new PdfLightTable();
            table.DataSourceType = PdfLightTableDataSourceType.External;
            List<object> datalist = new List<object>();

            foreach (Log log in tour.LogList)
            {
                object tmp = new { Date = log.Date, Report = log.Report, Distance = log.Distance, Rating = log.Rating, Steps = log.Steps, Duration = log.Duration, WeightKG = log.WeightKG, Feeling = log.Feeling, Weather = log.Weather , BloodPreassure = log.BloodPreassure};
                datalist.Add(tmp);
            }

            table.Style.ShowHeader = true;
            table.DataSource = datalist;
            table.Draw(page, new PointF(0, 250));

            document.Save(fs);

            document.Close(true);
            fs.Close();
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
                    string path = FALLBACKIMAGE;
                    FileStream imgStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                    image = new PdfBitmap(imgStream);
                }

                graphics.DrawImage(image, 300, 30, 200, 200);

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
            return;
        }
    }
}


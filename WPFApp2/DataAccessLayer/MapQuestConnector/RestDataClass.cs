using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net;
using System.Configuration;
using TourFinder.DataAccessLayer.Common;

//How to use HTTPClient https://zetcode.com/csharp/httpclient/

namespace TourFinder.DataAccessLayer.MapQuestConnector
{
    class RestDataClass : IMapQuestAccess
    {
        private static RestDataClass instance = null;
        private static HttpClient httpClient = null;
        //private static string key = "RtfQrP95xMphxahgFEmU8ZQkHBQfC4eu";
        
        private static string mapquestDirectionsUri = "http://www.mapquestapi.com/directions/v2/route?key=";
        private static string mapquestStaticMapUri = "http://www.mapquestapi.com/staticmap/v5/map?key=";
        private static string internalPath;
        private static string key;

        public static RestDataClass Instance()
        {
            if (instance == null)
            {
                instance = new RestDataClass();
            }
            return instance;
        }


        private RestDataClass()
        {
            if(httpClient == null)
            {
                httpClient = new HttpClient();
                internalPath = ConfigurationManager.AppSettings["ImageFolder"];
                key = ConfigurationManager.AppSettings["MapQuestAPIKey"];
            }
        }

        /// <summary>
        /// Get new Route information, save image and return img location in path
        /// </summary>
        /// <param name="startLocation"></param>
        /// <param name="endLocation"></param>
        /// <returns><string>newImageLocation</string>, <string>Distance</string></returns>
        public async Task<Tuple<string, float>> GetRouteSaveImg(string startLocation, string endLocation)
        {
            try {
                string respBody = httpClient
                                    .GetStringAsync(mapquestDirectionsUri + key + "&from="+startLocation + "&to="+endLocation)
                                    .Result;
                //httpClient.GetByteArrayAsync();

                //Debug.WriteLine("OUTPUT FROM WEBSITE_\n" +respBody);

                Task filetask = File.WriteAllTextAsync("..\\..\\..\\..\\WriteLines.json", respBody);
                filetask.Wait();

                JObject mapData = JObject.Parse(respBody);
                string ul_lat = (string)mapData["route"]["boundingBox"]["ul"]["lat"].ToString().Replace(",", ".");
                string ul_lng = (string)mapData["route"]["boundingBox"]["ul"]["lng"].ToString().Replace(",", ".");
                string lr_lat = (string)mapData["route"]["boundingBox"]["lr"]["lat"].ToString().Replace(",", ".");
                string lr_lng = (string)mapData["route"]["boundingBox"]["lr"]["lng"].ToString().Replace(",", ".");

                float distance = float.Parse(mapData["route"]["distance"].ToString());

                string sessionId = (string)mapData["route"]["sessionId"].ToString();

                string boundingBox = ul_lat + "," + ul_lng + "," + lr_lat + "," + lr_lng;

                string saveImgPath = await GetAndSaveImage(boundingBox, sessionId);

                return new Tuple<string, float>(saveImgPath, distance);
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine("Exception Caught!!");
                Debug.WriteLine("Message :{0} ", e.Message);
                return new Tuple<string,float>(e.Message,0);
            }
        }

        /// <summary>
        /// Gets image from MapQuestAPI, saves it to path and returns path string
        /// </summary>
        /// <param name="boundingBox"></param>
        /// <param name="sessionID"></param>
        /// <returns><string>newImageLocation</string></returns>
        private async Task<string> GetAndSaveImage(string boundingBox, string sessionID)
        {
            System.IO.Directory.CreateDirectory(internalPath);

            string myFile = "0";
            int intFileNameValue = 0;
            string fileNameValue = "";
            try
            {
                DirectoryInfo directory = new DirectoryInfo(internalPath);
                myFile = directory.GetFiles()
                                    .OrderByDescending(f => f.LastWriteTime)
                                    .First()
                                    .Name;
                
                foreach (char i in myFile)
                {
                    if (char.IsDigit(i))
                    {
                        fileNameValue += i;
                    }
                }
                intFileNameValue = int.Parse(fileNameValue) + 1;
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }

            fileNameValue = intFileNameValue.ToString();

            string fileLocation = internalPath + $"\\map{fileNameValue}.jpg";

            //Debug.Print("We do be here though");
            using WebClient client = new();
            await client.DownloadFileTaskAsync(
                new Uri(mapquestStaticMapUri + key + $"&session={sessionID}&boundingBox={boundingBox}&format=jpg"),
                fileLocation);
            Debug.WriteLine(String.Format("SAVED IMAGE AT: {0}", fileLocation));

            return "map"+fileNameValue+".jpg";
        }
    }
}

using NUnit.Framework;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using TourFinder;
using TourFinder.Models;
using System.Configuration;
using System.Diagnostics;

namespace NUnitTestProject1
{
    public class JsonReadAndConfigTests
    {

        static string PATH = "..\\..\\..\\..\\WriteLines.json";
        string allText = File.ReadAllText(PATH);

        [SetUp]
        public void Setup()
        {
            string textWithEnter = allText.Replace("{", "\n{\n");
            textWithEnter = textWithEnter.Replace("}", "\n}\n");
            textWithEnter = textWithEnter.Replace(",", ",\n");
            File.WriteAllText("..\\..\\..\\..\\Cleanup.json", textWithEnter);
            string[] splitInComma = allText.Split(",");
        }

        [Test]
        public void JSonReadBoundingBoxTest()
        {
            JObject mapData = JObject.Parse(allText);
            string output = (string)mapData["route"]["boundingBox"]["lr"]["lng"].ToString();
            Assert.IsNotNull(output); 
        } 

        [Test]
        public void JSonReadSessionIDTest()
        {
            JObject mapData = JObject.Parse(allText);
            string output = (string)mapData["route"]["sessionId"].ToString();
            Assert.IsNotNull(output);
        }

        
        [Test]
        public void JSonReadDistanceTest()
        {
            JObject mapData = JObject.Parse(allText);
            string output = (string)mapData["route"]["distance"].ToString();
            Assert.IsNotNull(output);
        }

        [Test]
        public void AppConfigTest()
        {
            string path = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;
            TestContext.WriteLine($"Config is Expected at path: {path}");
            Assert.IsNotNull(path);
        }

        [Test]
        public void ConnectionStringTest()
        {
            Assert.IsNotNull(ConfigurationManager.ConnectionStrings["PostgresSQLConnectionString"].ConnectionString);
        }

        [Test]
        public void AppSettingsTest()
        {
            System.Collections.Specialized.NameValueCollection appsettings = ConfigurationManager.AppSettings;
            List<bool> assertList = new List<bool>();
            foreach(var set in appsettings)
            {
                assertList.Add(set != null);
                TestContext.WriteLine(set);
            }
            CollectionAssert.AllItemsAreNotNull(appsettings);
        }
    }
}
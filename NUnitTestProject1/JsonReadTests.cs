using NUnit.Framework;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using TourFinder;
using TourFinder.Models;

namespace NUnitTestProject1
{
    public class JsonReadTests
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

        
    }
}
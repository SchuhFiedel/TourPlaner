using NUnit.Framework;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using TourFinder;

namespace NUnitTestProject1
{
    public class Tests
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

        /*[Test]
        public void MakeNewFilenameWithAccendingID()
        {
            DirectoryInfo directory = new DirectoryInfo("..\\..\\..\\..\\WPFApp2\\img");
            string myFile = directory.GetFiles()
                                .OrderByDescending(f => f.LastWriteTime)
                                .First()
                                .Name;

            string value = "";
            foreach(char i in myFile)
            {
                if (char.IsDigit(i))
                {
                    value += i;
                }
            }
            int intValue = int.Parse(value);

            Assert.AreEqual(1, intValue);
        }*/

        [Test]
        public void TourAddUtilityTest()
        {
            TourAddUtility tourAddUtility = new TourAddUtility();
            tourAddUtility.Description = "Description";
            tourAddUtility.Name = "Name";
            Tour tour = new Tour() { Description = tourAddUtility.Description, Name = tourAddUtility.Name };
            Assert.IsTrue(tour.Name == tourAddUtility.Name && tour.Description == tourAddUtility.Description);
            
        }
    }
}
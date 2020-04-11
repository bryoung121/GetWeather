using System;
using System.Net;
using System.Xml;
using System.Timers;
using System.IO;


namespace GetTheWeather
{
    class MyService
    {
        private static System.Timers.Timer fivemintimeer;

        public void Start()
        {

            fivemintimeer = new System.Timers.Timer(300000);
            fivemintimeer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            fivemintimeer.Interval = 300000;
            fivemintimeer.Enabled = true;

        }
        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {

            WeatherData myWeather = new WeatherData("Dallas");
            myWeather.CheckWeather();

            string outputtext = $"Temp: {myWeather.Temp.ToString()}, Precipitation:{myWeather.Precipmode.ToString()}";

            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            FileStream fs = new FileStream(Path.Combine(docPath, "myWeather.txt"), FileMode.Append);

            using (StreamWriter outputFile = new StreamWriter(fs))
            {
                outputFile.WriteLine(outputtext);
                //Console.WriteLine(outputtext);
                outputFile.Close();
            }
            

        }
        public void Stop()
        {
           
            // write code here that runs when the Windows Service stops.  
        }
    }


    class WeatherData
    {
        public WeatherData(string City)
        {
            city = City;
        }
        private string city;
        private string temp;
        private Boolean precipmode;

        public void CheckWeather()
        {
            WeatherAPI DataAPI = new WeatherAPI(City);
            temp = DataAPI.GetTemp();
            Precipmode = DataAPI.GetPrecipMode();
        }

        public string City { get => city; set => city = value; }
        public string Temp { get => temp; set => temp = value; }
        public Boolean Precipmode { get => precipmode; set => precipmode = value; }
        
    }
    class WeatherAPI
    {
        public WeatherAPI(string city)
        {
            SetCurrentURL(city);
            xmlDocument = GetXML(CurrentURL);
        }

        public string GetTemp()
        {
            XmlNode temp_node = xmlDocument.SelectSingleNode("//temperature");
            XmlAttribute temp_value = temp_node.Attributes["value"];
            XmlAttribute temp_unit = temp_node.Attributes["unit"];
            string temp_string = temp_value.Value + " " + temp_unit.Value;
            return temp_string;
        }

        public bool GetPrecipMode()
        {
            XmlNode precip_node = xmlDocument.SelectSingleNode("//precipitation");
            XmlAttribute precip_value = precip_node.Attributes["mode"];
            string precip_string = precip_value.Value;
            Boolean blnprecip = precip_string != "no" ? true : false;

            return blnprecip;

        }

        private const string APIKEY = "973f84bd640a375bfa291ac0f7735e11";
        private string CurrentURL;
        private XmlDocument xmlDocument;

        private void SetCurrentURL(string location)
        {
            CurrentURL = "http://api.openweathermap.org/data/2.5/weather?q="
                + location + "&mode=xml&units=metric&APPID=" + APIKEY;
        }

        private XmlDocument GetXML(string CurrentURL)
        {
            using (WebClient client = new WebClient())
            {
                string xmlContent = client.DownloadString(CurrentURL);
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xmlContent);
                return xmlDocument;
            }
        }
    }
}


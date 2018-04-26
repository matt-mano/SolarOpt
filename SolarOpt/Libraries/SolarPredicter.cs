using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarOpt.Libraries
{
    public class SolarPredicter
    {
        //HERE are my sources for this
        //https://books.google.com/books?id=_2brYQqb_RYC&pg=PA26&lpg=PA26&dq=average+watts/m%5E2+surface+peak&source=bl&ots=ImzfwuObq1&sig=oWaRpcitRVbF5M0YApRiLwb-qoA&hl=en&sa=X&ved=0ahUKEwi41IKkxLPaAhUBWK0KHXLbB3EQ6AEIiQEwBw#v=onepage&q=average%20watts%2Fm%5E2%20surface%20peak&f=false
        //https://solarpowerrocks.com/solar-basics/how-do-solar-panels-work-in-cloudy-weather/


        /*
         * Assumptions:
         * 1kw/m^2 during sunlight
         * 25% efficiency during cloudy day
         * 100% efficiency during sunny day
         * 
         * panel is 15% efficient at converting power
         * 
         * full power during daylight
         * 
         * */
        public static PowerData MakePredictionAndGetAngles(string weather)
        {
            const double wattsPerM2 = 1000;
            const double coefficientForClouds = 0.25;
            const double panelAreaInM2 = 0.0687;
            const double panelEfficiency = 0.15;

            //Get the minutes of the day that are avaiulable
            ExcelPackage package = new ExcelPackage(new System.IO.FileInfo("wwwroot/xls/NOAA_Solar_Calculations_day.xlsx"));
            ExcelWorksheet sheet = package.Workbook.Worksheets[1];
            var start = sheet.Dimension.Start;
            var end = sheet.Dimension.End;
            //double solarMinutes = Convert.ToDouble(sheet.Cells[27, 2].Text);
            //DateTime sunrise = Convert.ToDateTime(sheet.Cells[25, 2].Text);
            //DateTime sunset = Convert.ToDateTime(sheet.Cells[26, 2].Text);
            double solarMinutes = 1000;
            PowerData data = new PowerData();

            //Now get all teh angle data
            List<DateTime> TimeFractions = new List<DateTime>();
            List<double> AngleH = new List<double>();
            List<double> AngleA = new List<double>();

            //Also get the coordinates
            var lat = Convert.ToDouble(sheet.Cells[3, 2].Text);
            var lng = Convert.ToDouble(sheet.Cells[4, 2].Text);
            int row = start.Row;

            //Skip headers
            row++;
            //Parse rows one by one
            while (row < end.Row)
            {
                if(row < 180 && row > 90)
                {
                    //Add the thing from this row to each
                    TimeFractions.Add(Convert.ToDateTime(sheet.Cells[row, 5].Text));
                    AngleH.Add(Convert.ToDouble(sheet.Cells[row, 33].Text));
                    AngleA.Add(Convert.ToDouble(sheet.Cells[row, 34].Text));


                }
                //increment row
                row++;
            }

            //Closes package
            package.Dispose();


            //Now get the weather
            OpenWeatherMap.OpenWeatherMapClient client = new OpenWeatherMap.OpenWeatherMapClient("933839ed82ba0ad5f89513a943b6f71c");
            OpenWeatherMap.Coordinates coord = new OpenWeatherMap.Coordinates();
            coord.Latitude = lat;
            coord.Longitude = lng;

            //This is lowkey a little bit dangerous
            var currentWeather = client.CurrentWeather.GetByCoordinates(coord).Result;

            
            double cloudyNess = currentWeather.Clouds.Value;
            data.City = currentWeather.City.Name;
            data.UrlForIcon = "http://openweathermap.org/img/w/" + currentWeather.Weather.Icon + ".png";
            data.CurrentWeather = currentWeather.Weather.Value;

            //Make the predictions
            if (cloudyNess < 50)
            {
                data.DailyPower = wattsPerM2 * panelAreaInM2 * panelEfficiency * (solarMinutes / 60.0) * ((1-cloudyNess) / 100);
                data.CurrentPower = wattsPerM2 * panelAreaInM2 * panelEfficiency;
            } else 
            {
                data.DailyPower = coefficientForClouds * wattsPerM2 * panelAreaInM2 * panelEfficiency * solarMinutes / 60.0;
                data.CurrentPower = coefficientForClouds * wattsPerM2 * panelAreaInM2 * panelEfficiency;
            }

            data.Times = TimeFractions;
            //Process 3d angles
            if (cloudyNess < 50)
            {
                data.AngleA = AngleA;
                data.AngleH = AngleH;
            }
            else
            {
                data.AngleH = new List<double>();
                data.AngleA = new List<double>();
                foreach(var t in data.Times)
                {
                    data.AngleA.Add(0);
                    data.AngleH.Add(0);
                }
            }

            data.X = new List<double>();
            data.Y = new List<double>();
            data.Z = new List<double>();

            foreach(var angle in AngleA)
            {
                data.X.Add(5 * Math.Sin(angle));
                data.Z.Add(5 * Math.Cos(angle));
            }

            foreach(var angle in AngleH)
            {
                data.Y.Add(5 * Math.Tan(angle));
            }
            data.countAngs = data.Y.Count;
            data.HoursOfSun = (int)solarMinutes / 60;
            return data;
        }

        public struct PowerData
        {
            public double DailyPower { get; set; }
            public double CurrentPower { get; set; }
            public string CurrentWeather { get; set; }
            public List<double> X { get; set; }
            public List<double> Y { get; set; }
            public List<double> Z { get; set; }
            public List<double> AngleH { get; set; }
            public List<double> AngleA { get; set; }
            public List<DateTime> Times { get; set; }
            public int countAngs;
            public string UrlForIcon { get; set; }
            public string City { get; set; }
            public int HoursOfSun { get; set; }
        }

        


    }
}

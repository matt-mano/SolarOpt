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
        public static PowerData MakePrediction(string weather)
        {
            const double wattsPerM2 = 1000;
            const double coefficientForClouds = 0.25;
            const double coefficientForPartialClouds = 0.75;
            const double panelAreaInM2 = 0.1;
            const double panelEfficiency = 0.15;

            //Get the minutes of 
            ExcelPackage package = new ExcelPackage(new System.IO.FileInfo("wwwroot/xls/NOAA_Solar_Calculations_day.xlsx"));
            ExcelWorksheet sheet = package.Workbook.Worksheets[1];
            var start = sheet.Dimension.Start;
            var end = sheet.Dimension.End;
            double solarMinutes = Convert.ToDouble(sheet.Cells[27, 2].Text);
            DateTime sunrise = Convert.ToDateTime(sheet.Cells[25, 2].Text);
            DateTime sunset = Convert.ToDateTime(sheet.Cells[26, 2].Text);
            PowerData data = new PowerData();

            if(weather == "sunny")
            {
                data.CurrentWeather = "SUNNY";
                data.DailyPower = wattsPerM2 * panelAreaInM2 * panelEfficiency * solarMinutes / 60.0;
                data.CurrentPower = wattsPerM2 * panelAreaInM2 * panelEfficiency;
            } else if (weather == "cloudy")
            {
                data.CurrentWeather = "CLOUDY";
                data.DailyPower = coefficientForClouds * wattsPerM2 * panelAreaInM2 * panelEfficiency * solarMinutes / 60.0;
                data.CurrentPower = coefficientForClouds * wattsPerM2 * panelAreaInM2 * panelEfficiency;
            } else if(weather == "PARTLY CLOUDY")
            {
                data.CurrentWeather = "CLOUDY";
                data.DailyPower = coefficientForPartialClouds * wattsPerM2 * panelAreaInM2 * panelEfficiency * solarMinutes / 60.0;
                data.CurrentPower = coefficientForPartialClouds * wattsPerM2 * panelAreaInM2 * panelEfficiency;
            }

            int row = start.Row;
            //Skip headers
            row++;
            //Parse rows one by one


            //Closes package
            package.Dispose();

            return data;


        }

        public struct PowerData
        {
            public double DailyPower;
            public double CurrentPower;
            public string CurrentWeather;
        }


    }
}

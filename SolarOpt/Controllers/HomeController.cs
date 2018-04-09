using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using SolarOpt.Libraries;
using SolarOpt.Models;


namespace SolarOpt.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            //This calls read
            MeghanCode.ExcelReader.Read();
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        public void UpdateLatLongInSpreadSheet(double lat, double lng, int timeZone)
        {
            //Open package and set values
            ExcelPackage package = new ExcelPackage(new System.IO.FileInfo("wwwroot/xls/NOAA_Solar_Calculations_day.xlsx"));
            ExcelWorksheet sheet = package.Workbook.Worksheets[1];

            sheet.Cells[3, 2].Value = lat;
            sheet.Cells[4, 2].Value = lng;
            sheet.Cells[5, 2].Value = timeZone;
            sheet.Cells[7, 2].Value = DateTime.Today.ToShortDateString();

            //Closes package
            package.Save();


            ////Init excel application and open NOAA_Solar_Calculations_day.xlsx
            //Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            //Workbook excelWorkbook = excelApp.Workbooks.Open("~/xls/NOAA_Solar_Calculations_day.xls", 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
            //Microsoft.Office.Interop.Excel.Worksheet excelWorksheet;
            //excelWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)excelWorkbook.Worksheets.get_Item(1);

            ////Enter user location data
            //Range latRange = (Range)excelWorksheet.Cells[3, 2];
            //latRange.Value = "50"; //Latitude
            //Range longRange = (Range)excelWorksheet.Cells[4, 2];
            //longRange.Value = "50"; //Longitude
            //Range timeZoneRange = (Range)excelWorksheet.Cells[5, 2];
            //timeZoneRange.Value = "3"; //Time Zone

            ////Close workbook and application
            //excelWorkbook.Save();
            //excelWorkbook.Close();
            //excelApp.Quit();

        }

        public JsonResult GetDataFromSpreadsheet()
        {

            ExcelPackage package = new ExcelPackage(new System.IO.FileInfo("wwwroot/xls/NOAA_Solar_Calculations_day.xlsx"));
            ExcelWorksheet sheet = package.Workbook.Worksheets[1];
            var start = sheet.Dimension.Start;
            var end = sheet.Dimension.End;
            List<DateTime> TimeFractions = new List<DateTime>();
            List<double> AngleH = new List<double>();
            List<double> AngleA = new List<double>();

            int row = start.Row;
            //Skip headers
            row++;
            //Parse rows one by one
            while(row < end.Row)
            {
                //Add the thing from this row to each
                TimeFractions.Add(Convert.ToDateTime(sheet.Cells[row, 5].Text));
                AngleH.Add(Convert.ToDouble(sheet.Cells[row, 33].Text));
                AngleA.Add(Convert.ToDouble(sheet.Cells[row, 33].Text));
                
                //increment row
                row++;
            }
           
            //Closes package
            package.Dispose();

            //Return
            return Json(new { Time = TimeFractions, AngleH = AngleH, AngleA = AngleA });

        }

        public JsonResult GetLatLongFromSpreadsheet()
        {
            //Open package and get values
            ExcelPackage package = new ExcelPackage(new System.IO.FileInfo("wwwroot/xls/NOAA_Solar_Calculations_day.xlsx"));
            ExcelWorksheet sheet = package.Workbook.Worksheets[1];

            var lat = Convert.ToDouble(sheet.Cells[3, 2].Text);
            var lng = Convert.ToDouble(sheet.Cells[4, 2].Text);

            //Closes package
            package.Save();

            return (Json(new LatLong(lat, lng)));
        }



        //Here is the class to return for reference/testing
        public class LatLong
        {

            //Define public members
            public double Lat { get; set; }
            public double Lng { get; set; }

            //Public constructor (Wasn't software 2 fun!)
            public LatLong(double lat, double lng)
            {
                this.Lat = lat;
                this.Lng = lng;
            }
        }


        [HttpPost]
        public void TCPRequester()
        {
            var ArduinoTalker = new Talk2Arduino();
            ArduinoTalker.TCPListen();
        }

        [HttpPost]
        public JsonResult ChartDemo()
        {
            return DefaultInfoGenerator.IndexChartDataGenerator.randomChart();
        }
    }
}

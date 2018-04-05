using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Office.Interop.Excel;
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



        public void UpdateLatLongInSpreadSheet()
        {
            //Init excel application and open NOAA_Solar_Calculations_day.xlsx
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Workbook excelWorkbook = excelApp.Workbooks.Open("~/xls/NOAA_Solar_Calculations_day.xls", 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
            Microsoft.Office.Interop.Excel.Worksheet excelWorksheet;
            excelWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)excelWorkbook.Worksheets.get_Item(1);

            //Enter user location data
            Range latRange = (Range)excelWorksheet.Cells[3, 2];
            latRange.Value = "50"; //Latitude
            Range longRange = (Range)excelWorksheet.Cells[4, 2];
            longRange.Value = "50"; //Longitude
            Range timeZoneRange = (Range)excelWorksheet.Cells[5, 2];
            timeZoneRange.Value = "3"; //Time Zone

            //Close workbook and application
            excelWorkbook.Save();
            excelWorkbook.Close();
            excelApp.Quit();

        }

        public JsonResult GetDataFromSpreadsheet()
        {
            
            //Init excel application and open NOAA_Solar_Calculations_day.xlsx
            Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Workbook excelWorkbook = excelApp.Workbooks.Open("~/xls/NOAA_Solar_Calculations_day.xls", 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
            Worksheet excelWorksheet;
            excelWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)excelWorkbook.Worksheets.get_Item(1);

            //Extract time and angle data
            Range TimeCol = (Range)excelWorksheet.UsedRange.Columns[5];
            Array TimeVals = (Array)TimeCol.Cells.Value;
            string[] Time = TimeVals.OfType<object>().Select(o => o.ToString()).ToArray();

            Range AngHCol = (Range)excelWorksheet.UsedRange.Columns[33];
            Array AngHVals = (Array)AngHCol.Cells.Value;
            string[] AngH = AngHVals.OfType<object>().Select(o => o.ToString()).ToArray();

            Range AngACol = (Range)excelWorksheet.UsedRange.Columns[33];
            Array AngAVals = (Array)AngACol.Cells.Value;
            string[] AngA = AngAVals.OfType<object>().Select(o => o.ToString()).ToArray();
            Console.WriteLine(AngH[1]);

            //Close workbook and application
            excelWorkbook.Save();
            excelWorkbook.Close();
            excelApp.Quit();

            //Create an anonymous object that holds the three arrays
            //OBJECT ORIENTED BULLSHIT INCOMING DO NOT QUESTION THE MAGIC
            return Json(new { Time = TimeCol, AngleH = AngHCol, AngleA = AngACol });

        }

        public JsonResult GetLatLongFromSpreadsheet()
        {
            return Json(getLatLongFromExcel());
        }

        //Could you finish implementing this method?
        public LatLong getLatLongFromExcel()
        {
            /*
             * Do some magic to get lat and long from the sheet
             * Store the lat and long in the variables below
             * And then I wrote the object oriented crap
             */
            double lat = 39.9612;
            double lng = -82.9988;

            //Return a new LatLong object with the specified parameters
            return (new LatLong(lat, lng));

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
        public JsonResult ChartDemo()
        {
            return DefaultInfoGenerator.IndexChartDataGenerator.randomChart();
        }
    }
}

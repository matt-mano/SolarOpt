using Microsoft.AspNetCore.Mvc;
using SolarOpt.Models;
using System;
using System.Collections.Generic;
using System.Web.Helpers;

namespace SolarOpt.Libraries
{
    public class DefaultInfoGenerator
    {
        public class DefaultAngleGenerator : IAngleGenerator
        {
            public List<ServoAngle> getAnglesForToday(double latitude, double longitude)
            {
                //Use graduations to determine the number of steps in a day
                const int graduations = 48;


                //Get the steps from today to tomorrow
                List<ServoAngle> result = new List<ServoAngle>();
                DateTime next  = DateTime.Now.AddHours(6);
                DateTime end = DateTime.Now.AddHours(18);
                double difference = (next - end).TotalMinutes;
                double step = difference / graduations;
                double degreeStep = 180 / graduations;
                int stepNo = 0;

                //Populate dictionary from 6:00 to 18:, even rotations over the steps
                while(next < end)
                {
                    //Add new servoangle to list
                    ServoAngle angle = new ServoAngle();
                    angle.Angle = Convert.ToSingle(stepNo * degreeStep);
                    angle.Timestamp = next;
                    angle.UnitId = 1;
                    result.Add(angle);

                    //Move up indices
                    stepNo++;
                    next.AddMinutes(step);
                }

                return result;

            }
        }

        public class IndexChartDataGenerator
        {
            public static JsonResult randomChart()
            {
                List<dataLine> data = new List<dataLine>();
                Random random = new Random();
                int i = 0;
                while (i < 24)
                {
                    dataLine dataPoint = new dataLine();
                    dataPoint.Time = DateTime.Now.AddHours(i);
                    dataPoint.Value = random.NextDouble() * 100;
                    data.Add(dataPoint);
                    i++;
                }
                return Json(data);
            }
                
                private class dataLine
                {
                    public DateTime Time { get; set; }
                    public double Value { get; set; }
                }

            }
        }
    }


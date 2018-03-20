using Microsoft.AspNetCore.Mvc;
using SolarOpt.Models;
using System;
using System.Collections.Generic;
using System.Web.Helpers;
using Newtonsoft.Json;

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
                List<DateTime> times = new List<DateTime>();
                List<double> values = new List<double>();

                Random random = new Random();
                int i = 0;
                while (i < 24)
                {
                    times.Add(DateTime.Now.AddHours(i));
                    values.Add(random.NextDouble() * 100);
                    i++;
                }
                ChartData data = new ChartData();
                data.Times = times;
                data.Values = values;

                return new JsonResult(data);
            }
                
                private class ChartData
                {
                    public List<DateTime> Times { get; set; }
                    public List<double> Values { get; set; }
                }

            }
        }
    }


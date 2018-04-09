using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using OfficeOpenXml;
using SolarOpt.Libraries;
using SolarOpt.Models;

namespace SolarOpt.Libraries
{
    public class DataForTCP
    {
        public List<DateTime> dates { get; set; }
        public List<double> angleH { get; set; }
        public List<double> angleA { get; set; }

    }
    public class Talk2Arduino
    {
        public TcpListener server = null;
        public IPAddress GetIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            throw new Exception("No IP found");
        }

        //Listen for incoming TCP Requests
        public void TCPListen()
        {
            try
            {
                //Init server
                Int32 port1 = 12345;
                IPAddress localIP = GetIP();
                server = new TcpListener(localIP, port1);
                server.Start();

                //Reading buffer
                //Byte[] bytes = new Byte[256];
                //String data = null;

                //Listen
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    //data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;
                    DataForTCP data = GetDataFromSpreadsheetTCP();
              
                    // Send back a response.
                    //byte[] msg = 
                    //stream.Write(msg, 0, msg.Length);
                    // Loop to receive all the data sent by the client.
                    /*
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        // Process the data sent by the client.
                        data = data.ToUpper();

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
                    }*/

                    // Shutdown and end connection
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }


            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }


        public DataForTCP GetDataFromSpreadsheetTCP()
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
            while (row < end.Row)
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
            var Data = new DataForTCP();
            Data.dates = TimeFractions;
            Data.angleH = AngleH;
            Data.angleA = AngleA;
            return Data;
        }


    }
}

using System;
using System.Management;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Configuration;
using System.Printing;

namespace RawMat.Utilities
{
    public class printerCls
    {

        public static class myPrinters
        {
            [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern bool SetDefaultPrinter(string Name);
        }

        public void setPrinter()
        {
            myPrinters.SetDefaultPrinter(ConfigurationManager.AppSettings["PrinterDefault"]);
        }

        public string GetDefaultPrinter()
        {
            PrinterSettings settings = new PrinterSettings();
            return settings.PrinterName;
        }

        public bool IsPrinterReady(string printerName)
        {
            string query = $"SELECT * FROM Win32_Printer WHERE Name = '{printerName.Replace("\\", "\\\\")}'";

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject printer in searcher.Get())
                {
                    if (printer["WorkOffline"] != null &&
                        bool.TryParse(printer["WorkOffline"].ToString(), out bool isOffline))
                    {
                        return !isOffline;
                    }
                }
            }
            return false;
        }

        public int checkPrinter(out string printerName)
        {
            int Result = 0;
            printerName = "Not Found";

            ManagementObjectCollection MgmtCollection;
            ManagementObjectSearcher MgmtSearcher;

            //Perform the search for printers and return the listing as a collection
            MgmtSearcher = new ManagementObjectSearcher("Select * from Win32_Printer");
            MgmtCollection = MgmtSearcher.Get();
            //Get local print server
            var server = new LocalPrintServer();

            foreach (ManagementObject objWMI in MgmtCollection)
            {
                if (((bool?)objWMI["Default"]) ?? false)
                {
                    printerName = objWMI["Name"].ToString();
                    Result = 1;
                }
            }

            return Result;
        }

    }

   

}

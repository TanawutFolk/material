using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Data;

namespace RawMat.Utilities
{
    public class NetworkInfoCls
    {
        public string GetHostName()
        {
            try
            {
                return Dns.GetHostName();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting hostname: " + ex.Message);
                return string.Empty;
            }
        }

        public string GetIPv4Address()
        {
            try
            {
                string hostName = GetHostName();
                if (string.IsNullOrEmpty(hostName))
                    return "Hostname not found";

                IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
                foreach (IPAddress ip in hostEntry.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                return "No IPv4 address found";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting IPv4 address: " + ex.Message);
                return "Error";
            }
        }

        public string GetIPv6Address()
        {
            try
            {
                string hostName = GetHostName();
                if (string.IsNullOrEmpty(hostName))
                    return "Hostname not found";

                IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
                foreach (IPAddress ip in hostEntry.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        return ip.ToString();
                    }
                }
                return "No IPv6 address found";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting IPv6 address: " + ex.Message);
                return "Error";
            }
        }

        public string GetIPActive()
        {
            string ipActive = "";
            // ดึงรายการ network interfaces ที่ active และไม่ใช่ loopback
            var activeInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.OperationalStatus == OperationalStatus.Up &&
                             ni.NetworkInterfaceType != NetworkInterfaceType.Loopback);

            foreach (var ni in activeInterfaces)
            {
                var ipProps = ni.GetIPProperties();
                var gateways = ipProps.GatewayAddresses;
                if (gateways.Any())
                {
                    var ipAddress = ipProps.UnicastAddresses
                        .FirstOrDefault(addr => addr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                    if (ipAddress != null)
                    {
                        //Console.WriteLine($"Interface: {ni.Name}, Type: {ni.NetworkInterfaceType}, IP: {ipAddress.Address}");
                        ipActive = $"{ipAddress.Address}";
                        
                    }
                }
            }
            return ipActive;
        }
        // ฟังก์ชันใหม่: ดึง IP Address ของ WiFi และ Ethernet คืนค่าเป็น DataTable
        public DataTable GetNetworkInterfacesIP()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("wifi", typeof(string));
            dt.Columns.Add("ethernet", typeof(string));

            string wifiIP = null;
            string ethernetIP = null;

            try
            {
                // ดึงรายการ network interfaces ที่ active และไม่ใช่ loopback
                var activeInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(ni => ni.OperationalStatus == OperationalStatus.Up &&
                                 ni.NetworkInterfaceType != NetworkInterfaceType.Loopback);

                foreach (var ni in activeInterfaces)
                {
                    var ipProps = ni.GetIPProperties();
                    var gateways = ipProps.GatewayAddresses;
                    if (gateways.Any())
                    {
                        var ipAddress = ipProps.UnicastAddresses
                            .FirstOrDefault(addr => addr.Address.AddressFamily == AddressFamily.InterNetwork);
                        if (ipAddress != null)
                        {
                            string ip = ipAddress.Address.ToString();
                            // ตรวจสอบประเภทของ interface
                            if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                            {
                                wifiIP = ip;
                            }
                            else if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                            {
                                ethernetIP = ip;
                            }
                        }
                    }
                }

                // เพิ่มข้อมูลลง DataTable
                dt.Rows.Add(wifiIP, ethernetIP);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting network interfaces IP: " + ex.Message);
                // คืนค่า DataTable ที่มีคอลัมน์ว่างในกรณีเกิดข้อผิดพลาด
                dt.Rows.Add(null, null);
            }

            return dt;
        }

        // ฟังก์ชันใหม่: ดึง Computer Name
        public string GetComputerName()
        {
            try
            {
                return Environment.MachineName;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting computer name: " + ex.Message);
                return string.Empty;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Multitool
{
    internal class Program
    {
        static string webhook = URLKeys.webhook;
        static List<string> IPs = new List<string>();

        static async Task Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.Title = "Multi-tool";
                Banner();
                Menu();

                ConsoleKeyInfo input = Console.ReadKey();
                char option = input.KeyChar;
                Console.Clear();
                switch (option)
                {
                    case '1':
                        print("Discord Webhook Message: \n Send a message to my private Discord Server\n\n");
                        WebhookMessage();
                        break;
                    case '2':
                        print("MAC Address Lookup: \n Gets MAC address of every NICs \n\n");
                        GetLocalMacAddress();
                        break;
                    case '3':
                        print("IP Scanner: \n Gets every IP on the local network\n\n");
                        await IPScan();
                        break;
                    case '4':
                        print("DNS Record Names: \n Gets every domain names that we're visited\n\n");
                        DNSRecordNames();
                        break;
                    case '5':
                        print("Saved Wi-Fi Extractor: \n Gets every saved Wi-Fi passwords\n\n");
                        CheckWiFiProfiles();
                        break;
                    case '9':
                        Banner();
                        print("Thank you for using this");
                        return;
                    default:
                        Console.WriteLine("\nInvalid Input. Please type the number\n\n");
                        break;
                }

                Console.WriteLine("Finished");
                Console.ReadKey();
            }


        }
        
        static void Banner()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(@"   
                              _ _   _      _____            _ 
                  /\/\  _   _| | |_(_)    /__   \___   ___ | |
                 /    \| | | | | __| |_____ / /\/ _ \ / _ \| |
                / /\/\ \ |_| | | |_| |_____/ / | (_) | (_) | |
                \/    \/\__,_|_|\__|_|     \/   \___/ \___/|_|
                                                                        - Made by:
                                                                            M3galodn81");
            Console.ResetColor();
        }

        static void Menu()
        {
            Console.WriteLine(@"1. Send Webhook Message");
            Console.WriteLine(@"2. MAC Address Lookup");
            Console.WriteLine(@"3. IP Scan");
            Console.WriteLine(@"4. Get DNS Record Names");
            Console.WriteLine(@"5. Get Saved WiFi Passwords");
            Console.WriteLine(@"9. Exit");
        }

        static async void WebhookMessage()
        {
           
            //Console.WriteLine("Webhook URL: ");
            //string webhook = Console.ReadLine();

            Console.Write("Message: ");
            string message = Console.ReadLine();

            string json = $"{{\"content\":\"{message}\"}}";

            using (HttpClient client = new HttpClient())
            {
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                await client.PostAsync(webhook, content);
            }
        }

        #region IP Scan
        static async Task IPScan()
        {
            string subnet = "192.168.1.";  // Adjust based on your network
            int timeout = 100;  // ms

            Console.WriteLine("Scanning local network...");

            var tasks = new Task[254];
            for (int i = 1; i <= 254; i++)
            {
                string ip = subnet + i;
                tasks[i - 1] = Task.Run(() => PingIP(ip, timeout));
            }

            await Task.WhenAll(tasks);

        }

        static async Task PingIP(string ip, int timeout)
        {
            IPs.Clear();
            using (Ping ping = new Ping())
            {
                try
                {
                    PingReply reply = await ping.SendPingAsync(ip, timeout);
                    if (reply.Status == IPStatus.Success)
                    {
                        IPs.Add(ip);
                        //await ReverseDNSLookup(ip);
                        Console.WriteLine($"{ip}");
                        
                    }
                }
                catch { /* Ignore unreachable hosts */ }
            }
        }

        static async Task ReverseDNSLookup(string ip)
        {
            try
            {
                var host = await Dns.GetHostEntryAsync(ip);
                Console.WriteLine($"{ip} - {host.HostName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ip} - DNS Lookup Failed: {ex.Message}");
            }
        }
        #endregion

        #region MAC lookup
        static void GetLocalMacAddress()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    Console.WriteLine($"MAC Address for {nic.Name}: {nic.GetPhysicalAddress()}");
                }
            }
        }
        #endregion

        #region Get Wifi Password
        static void GetSavedWiFiPassword()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    Console.WriteLine($"MAC Address for {nic.Name}: {nic.GetPhysicalAddress()}");
                }
            }
        }
        #endregion

        #region Get History
        static void DNSRecordNames()
        {
            List<string> dnsRecords = GetDnsRecordNames();
            foreach (var record in dnsRecords)
            {
                Console.WriteLine("[+] " + record);
            }
        }

        static List<string> GetDnsRecordNames()
        {
            List<string> recordNames = new List<string>();

            ProcessStartInfo psi = new ProcessStartInfo("ipconfig", "/displaydns")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(psi))
            using (var reader = process.StandardOutput)
            {
                string output = reader.ReadToEnd();

                // Debugging raw output
                //Console.WriteLine("--- Raw Output ---\n");
                //Console.WriteLine(output);

                var matches = Regex.Matches(output, @"Record Name\s*[:.]+\s*(.+)");
                foreach (Match match in matches)
                {
                    recordNames.Add(match.Groups[1].Value.Trim());
                }
            }

            return recordNames;
        }

        #endregion

        #region Get WiFi Passwords
        static void CheckWiFiProfiles()
        {
            List<string> profiles = GetWiFiProfiles();
            foreach (var profile in profiles)
            {
                string password = GetWiFiPassword(profile);
                Console.WriteLine($"[+] {profile} - Password: {password}");
            }
        }

        static List<string> GetWiFiProfiles()
        {
            List<string> profiles = new List<string>();
            ProcessStartInfo psi = new ProcessStartInfo("netsh", "wlan show profile")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process p = Process.Start(psi))
            using (var reader = p.StandardOutput)
            {
                string output = reader.ReadToEnd();
                var matches = Regex.Matches(output, @"All User Profile\s*:\s*(.+)");
                foreach (Match match in matches)
                {
                    profiles.Add(match.Groups[1].Value.Trim());
                }
            }

            return profiles;
        }

        static string GetWiFiPassword(string profileName)
        {
            ProcessStartInfo psi = new ProcessStartInfo("netsh", $"wlan show profile name=\"{profileName}\" key=clear")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process p = Process.Start(psi))
            using (var reader = p.StandardOutput)
            {
                string output = reader.ReadToEnd();
                var match = Regex.Match(output, @"Key Content\s*:\s*(.+)");
                return match.Success ? match.Groups[1].Value.Trim() : "N/A";
            }
        }
        #endregion
    
        static void print(string text)
        {
            Console.WriteLine(text);
        }
    
    }
}



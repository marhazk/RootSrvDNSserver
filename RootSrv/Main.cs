using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Threading.Tasks;


namespace RootSrv
{
    public class RootSrv : System.ServiceProcess.ServiceBase
    {
        private static string servName = "RootSync";
        private static string defaultDNS = "rootsync.pub.haztech.com.my";
        private EventLog eventLog;

        public Aera.FileDB datadb;
        public Aera.FileDB hosts;
        public Aera.FileDB newhosts;
        public Aera.INI ini;
        public int mins = 15;
        public string _mins = "10";
        public int totalSync = 0;
        public System.Threading.Thread client;
        public bool running = false;
        public static string inifile = @".\setting.ini";
        public string syncurl = "http://pub.haztech.com.my/data.txt";
        public string hostsfile = "c:\\Windows\\System32\\drivers\\etc\\hosts";

        public string defIP = "0.0.0.0";

        public string ires = "";
        public string ores = "";
        public RootSrv()
        {
            this.ServiceName = servName;

            string source = "ResSync";
            eventLog = new EventLog();
            eventLog.Source = source;
        }

        public void cliThread()
        {
            try
            {
                int num = 1;
                int success = 0;
                int fail = 0;
                //AeraLib = new Aera.Aera();
                while (running)
                {
                    ores = "";
                    ires = "";
                    success = 0;
                    fail = 0;
                    try
                    {

                        ini = new Aera.INI();
                        eventLog.WriteEntry("Loaded AeraClass Library version: " + new Aera.Aera().version());

                        eventLog.WriteEntry("Checking if INI is exists");
                        if (ini.Exists())
                        {
                            eventLog.WriteEntry("Reading INI that exists");
                            syncurl = ini.ReadValue("sync", "url");
                            _mins = ini.ReadValue("sync", "minute_sync");
                        }
                        else
                        {
                            _mins = mins.ToString();
                            eventLog.WriteEntry("Reading from default value, INI file unexists", EventLogEntryType.Warning);
                        }
                        eventLog.WriteEntry("Value detected: " + syncurl +
                            "\r\nValue detected: " + _mins);
                        if (_mins.Length > 0)
                            mins = Convert.ToInt32(_mins);
                        else
                        {
                            eventLog.WriteEntry("Failed to convert from INI file. Default set 10 minutes.", EventLogEntryType.Warning);
                            mins = 10;
                        }
                        datadb = new Aera.FileDB(new Uri(@"" + syncurl));
                        ores = datadb.Read(0);
                        datadb.Dispose();
                        ini.WriteValue("sync", "ores", ores);

                        if (defIP != ores)
                        {
                            defIP = ores;
                            eventLog.WriteEntry("New IP Detected: " + ores + " (NEW) " + defIP + " (OLD)");


                            hosts = new Aera.FileDB(@"" + hostsfile, true);
                            hosts.ReplaceLine(defaultDNS, ores + "\t" + defaultDNS);
                            hosts.Update();
                        }
                        success++;
                    }
                    catch (Exception e)
                    {
                        eventLog.WriteEntry("ERROR: " + e.ToString(), EventLogEntryType.Error);
                        fail++;
                    }
                    num++;
                    eventLog.WriteEntry("Delay for " + mins + " minutes");
                    //System.Threading.Thread.Sleep(55000);
                    System.Threading.Thread.Sleep(mins * 55000);
                }
            }
            catch (Exception e)
            {
                eventLog.WriteEntry("Error: " + e.ToString(), EventLogEntryType.Error);
            }
        }
        public void startProgress()
        {
            try
            {
                running = true;
                eventLog.WriteEntry("starting thread!");
                client = new System.Threading.Thread(new System.Threading.ThreadStart(cliThread));
                client.Start();
            }
            catch
            {
                //client.Abort();
            }
        }




        static void Main(string[] arg)
        {
            if (arg.Length < 1)
            {
                try
                {
                    System.ServiceProcess.ServiceBase[] ServicesToRun;
                    ServicesToRun = new System.ServiceProcess.ServiceBase[] { new RootSrv() };
                    System.ServiceProcess.ServiceBase.Run(ServicesToRun);
                }
                catch { }
            }
            else
            {
                try
                {

                    if (arg[0] == "install")
                    {
                        try
                        {
                            ServiceInstaller.InstallAndStart(servName, servName, System.Reflection.Assembly.GetExecutingAssembly().Location);
                            //ServiceInstaller.InstallAndStart("MyServiceName", "MyServiceDisplayName", System.Reflection.Assembly.GetExecutingAssembly().Location + System.AppDomain.CurrentDomain.FriendlyName);
                            System.Console.WriteLine("Successfully installed");
                        }
                        catch
                        {
                            System.Console.WriteLine("Failed to install");
                        }
                    }
                    else if (arg[0] == "uninstall")
                    {
                        try
                        {
                            ServiceInstaller.Uninstall(servName);
                            Console.WriteLine("Successfully uninstalled");
                        }
                        catch
                        {
                            Console.WriteLine("Failed to uninstall");
                        }
                    }
                    else
                    {
                    }
                }
                catch { }
            }
        }
        protected override void OnStart(string[] args)
        {

            try
            {
                startProgress();
            }
            catch { }
            eventLog.WriteEntry("starting up!");
        }
        protected override void OnStop()
        {
            eventLog.WriteEntry("shutting down!");

            running = false;
            try
            {
                client.Abort();
            }
            catch { }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace demo
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer(); // name space(using System.Timers;)  
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            WriteToFile("Service is started at " + DateTime.Now);
            //timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            //timer.Interval = 5000; //number in milisecinds  
            //timer.Enabled = true;

            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            var scheduleDateTime = DateTime.Today.AddHours(16).AddMinutes(10);
            var scheduleInterval = scheduleDateTime.Subtract(DateTime.Now).TotalSeconds * 1000;
            if (scheduleInterval < 0)
            {
                scheduleInterval += new TimeSpan(24, 0, 0).TotalSeconds * 1000;
            }
            timer.Interval = scheduleInterval;
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            WriteToFile("Service is stopped at " + DateTime.Now);
        }

        //private void OnElapsedTime(object source, ElapsedEventArgs e)
        //{
        //    WriteToFile("Service is recall at " + DateTime.Now);
        //}

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            if (timer.Interval != 24 * 60 * 60 * 1000)
            
            {
                timer.Interval = 24 * 60 * 60 * 1000; //Reset the timer    
            }
            string ApiData = new WebClient().DownloadString("https://localhost:7070/LoginMVC/Index");
            WriteToFile($"Salesforce Api called : Api Data1");
            string ApiData1 = new WebClient().DownloadString("https://localhost:7070/Marketing/Mail");
            WriteToFile($"Mail Api called : Api Data2");
        }
        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}
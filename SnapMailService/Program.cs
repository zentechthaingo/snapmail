using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using BLL.Common;
using Quartz;
using Quartz.Impl;

namespace OneButtonAppService
{
    static class Program
    {
        public static ISchedulerFactory SchedulerFactory;
        public static IScheduler Scheduler;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            SchedulerFactory = new StdSchedulerFactory();
            Scheduler = SchedulerFactory.GetScheduler();
            Scheduler.Start();
//#if(!DEBUG)
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new Service1() 
            };
            ServiceBase.Run(ServicesToRun);
//#else
            //Service1 myServ = new Service1();
            //myServ.Start();
//#endif
        }

        private static void CurrentDomain_UnhandledException(Object sender, UnhandledExceptionEventArgs e)
        {
            if (e != null && e.ExceptionObject != null)
            {
                Utils.FileLog("Exception Occured..." + e.ExceptionObject.ToString());
            }
        }
    }
}

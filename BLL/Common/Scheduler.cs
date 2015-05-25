using System;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Auth;
using Quartz;
using Quartz.Impl;

namespace BLL.Common
{
    public class Scheduler
    {

        //public Scheduler()
        //{
        //    try
        //    {
        //        // Create the table client.
        //        StorageCredentials creds = new StorageCredentials("snapmails", "K34cW5qYo2LBovgfMQMTQMQvdDv84LWWJucWpmwHoN5lL81/r2Ct8ZFx1UqmhMq9GlMN84uYXYIH6xTndB8Eqw==");
        //        CloudStorageAccount storageAccount = new CloudStorageAccount(creds, useHttps: true);
        //        CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
        //        CloudTable tblEmails = tableClient.GetTableReference("Emails");
        //        tblEmails.CreateIfNotExists();

        //        SnapmailData snapmail = new SnapmailData()
        //        {
        //            JobKey = "jk1",
        //            JobGroup = "jg1",
        //            UserId = "shakeelanjum",
        //            Cameras = "window",
        //            Sender = "evercamio@gmail.com",
        //            Subject = "Hi Shakeel, this is a daily email",
        //            Recipients = "shakeel.anjum@camba.tv",
        //            NotifyDays = "sat, sun",
        //            NotifyHH = 10,
        //            NotifyMM = 00,
        //            Status = 0
        //        };

        //        EmailData email = new EmailData()
        //        {
        //            ETag = "*",
        //            PartitionKey = "P1",
        //            RowKey = Guid.NewGuid().ToString(),
        //            UserID = "shakeelanjum",
        //            Email = "shakeel.anjum@mhlabs.net",
        //            Timestamp = DateTimeOffset.Now,
        //        };
        //    }
        //    catch(Exception x)
        //    {
        //        Console.WriteLine(x.ToString());
        //    }
        //}

        //static void Main(string[] args)
        //{
        //    //Common.Logging.LogManager.Adapter = new Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter { Level = Common.Logging.LogLevel.Info };
            
        //    // Now let's start our scheduler; you could perform
        //    // any processing or bootstrapping code here before
        //    // you start it but it must be started to schedule
        //    // any jobs
        //    Scheduler.Start();

        //    // Schedule a test job, that will run instantly
        //    //ScheduleInstantJob(CreateTestJob("SendTestEmail", "testgroup"));
        //    // Tirgger instantly
        //    //Scheduler.TriggerJob(new JobKey("SendTestEmail", "testgroup"));

        //    //// Schedule a daily job
        //    ScheduleDailyJob(CreateDailyJob("SendDailyEmail", "dailygroup"));
        //    //Scheduler.TriggerJob(new JobKey("SendDailyEmail", "dailygroup"));

        //    // some sleep to show what's happening
        //    Thread.Sleep(TimeSpan.FromSeconds(60));

        //    Scheduler.Shutdown();

        //    Console.ReadKey();
        //}

        private static IJobDetail CreateDailyJob(string jobKey, string jobGroup)
        {
            IJobDetail _detail = JobBuilder.Create<SnapmailJob>()
                .WithIdentity(jobKey, jobGroup)   // Here we can assign a friendly name to our job        
                .Build();                       // And now we build the job detail

            SnapmailData data = new SnapmailData()
            {
                JobKey = jobKey,
                JobGroup = jobGroup,
                UserId = "shakeelanjum",
                Cameras = "window",
                Sender = "evercamio@gmail.com",
                Subject = "Hi Shakeel, this is a daily email",
                Recipients = "shakeel.anjum@camba.tv",
                NotifyDays = "sat, sun",
                NotifyHH = 10,
                NotifyMM = 00,
                Status = 0
            };
            _detail.JobDataMap.Add("JobData", data);

            return _detail;
        }

        private static void ScheduleDailyJob(IJobDetail _jobDetails)
        {
            // Let's create a trigger that fires immediately
            ITrigger trigger = TriggerBuilder.Create()
                .WithDescription("Every 10 Seconds from 9AM UTC")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .RepeatForever()
                    .WithIntervalInHours(24)
                )
                .Build();

            // Ask the scheduler to schedule our EmailJob
            Scheduler.ScheduleJob(_jobDetails, trigger);
        }
    }

    public class EmailData : TableEntity
    {
        public string UserID { get; set; }
        public string Email { get; set; }
    }

    public class SnapmailData : TableEntity
    {
        public string JobKey { get; set; }
        public string JobGroup { get; set; }
        /// <summary>
        /// evercam-user-id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// camera1id, camera2id, camera3id, ...
        /// </summary>
        public string Cameras { get; set; }
        public string Sender { get; set; }
        /// <summary>
        /// Hi {user-name}, here are your snapshots
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// email@domain.com; email@domain.com, ...
        /// </summary>
        public string Recipients { get; set; }
        public string Message { get; set; }
        /// <summary>
        /// sat, sun, mon, ...
        /// </summary>
        public string NotifyDays { get; set; }
        /// <summary>
        /// 14
        /// </summary>
        public int NotifyHH { get; set; }
        /// <summary>
        /// 30
        /// </summary>
        public int NotifyMM { get; set; }
        public int Status { get; set; }
    }

    /// <summary>
    /// Snapemail job, yet to be implemented
    /// </summary>
    [DisallowConcurrentExecution]
    public class SnapmailJob : IJob
    {
        public SnapmailJob()
        { }

        public void Execute(IJobExecutionContext context)
        {
            var mail = context.MergedJobDataMap["JobData"] as SnapmailData;
            
            Console.WriteLine("Job: " + mail.JobKey + " [" + DateTime.UtcNow.ToString() + "]");
            Console.WriteLine("Sub: " + mail.Subject);
            Console.WriteLine("Msg: " + mail.Message);
            Console.WriteLine("");
        }
    }

    public class SnapmailerSettings
    {
        public static bool SendTestEmail
        {
            get { return bool.Parse(ConfigurationSettings.AppSettings["SendTestEmail"]); }
        }

        public static string SMTPServer
        {
            get { return ConfigurationSettings.AppSettings["SMTPServer"]; }
        }

        public static string SMTPEmail
        {
            get { return ConfigurationSettings.AppSettings["SMTPEmail"]; }
        }

        public static string SMTPPassword
        {
            get { return ConfigurationSettings.AppSettings["SMTPPassword"]; }
        }
    }
}

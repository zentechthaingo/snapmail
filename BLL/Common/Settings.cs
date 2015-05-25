using System;
using System.Linq;
using System.Configuration;

namespace BLL.Common
{
    public class Settings
    {
        public static string SnapmailExePath
        {
            get { return ConfigurationSettings.AppSettings["SnapmailExePath"]; }
        }

        public static string EvercamClientName
        {
            get { return ConfigurationSettings.AppSettings["EvercamClientName"]; }
        }

        public static string EvercamClientID
        {
            get { return ConfigurationSettings.AppSettings["EvercamClientID"]; }
        }

        public static string EvercamClientSecret
        {
            get { return ConfigurationSettings.AppSettings["EvercamClientSecret"]; }
        }

        public static string EvercamClientUri
        {
            get { return ConfigurationSettings.AppSettings["EvercamClientUri"]; }
        }

        public static string EvercamTestToken
        {
            get { return ConfigurationSettings.AppSettings["EvercamTestToken"]; }
        }

        public static bool EvercamSandboxMode
        {
            get { return bool.Parse(ConfigurationSettings.AppSettings["EvercamSandboxMode"]); }
        }

        public static string CloudStorageAccount
        {
            get { return ConfigurationSettings.AppSettings["CloudStorageAccount"]; }
        }

        public static string StorageAccountKey
        {
            get { return ConfigurationSettings.AppSettings["StorageAccountKey"]; }
        }

        public static string PartitionKey
        {
            get { return ConfigurationSettings.AppSettings["PartitionKey"]; }
        }

        public static string SnapmailsTable
        {
            get { return ConfigurationSettings.AppSettings["SnapmailsTable"]; }
        }

        public static string EmailsTable
        {
            get { return ConfigurationSettings.AppSettings["EmailsTable"]; }
        }

        public static string ConnectionString
        {
            get { return ConfigurationSettings.AppSettings["ConnectionString"]; }
        }

        public static string ServerUrl
        {
            get { return ConfigurationSettings.AppSettings["ServerUrl"]; }
        }

        public static string EmailSource
        {
            get { return ConfigurationSettings.AppSettings["EmailSource"]; }
        }

        public static string SmtpEmail
        {
            get { return ConfigurationSettings.AppSettings["SmtpEmail"]; }
        }

        public static string SmtpUser
        {
            get { return ConfigurationSettings.AppSettings["SmtpUser"]; }
        }

        public static string SmtpPassword
        {
            get { return ConfigurationSettings.AppSettings["SmtpPassword"]; }
        }

        public static string SmtpServer
        {
            get { return ConfigurationSettings.AppSettings["SmtpServer"]; }
        }

        public static string SmtpServerPort
        {
            get { return ConfigurationSettings.AppSettings["SmtpServerPort"]; }
        }

        public static string TestEmail
        {
            get { return ConfigurationSettings.AppSettings["TestEmail"]; }
        }

        public static string EmailSubject
        {
            get { return ConfigurationSettings.AppSettings["EmailSubject"]; }
        }

        public static string EmailMessage
        {
            get { return ConfigurationSettings.AppSettings["EmailMessage"]; }
        }

        public static string QuartzDailyJobKey
        {
            get { return ConfigurationSettings.AppSettings["QuartzDailyJobKey"]; }
        }

        public static string QuartzDailyJobGroup
        {
            get { return ConfigurationSettings.AppSettings["QuartzDailyJobGroup"]; }
        }

        public static string QuartzDailyJobData
        {
            get { return ConfigurationSettings.AppSettings["QuartzDailyJobData"]; }
        }

        public static string QuartzDailyJobTrigger
        {
            get { return ConfigurationSettings.AppSettings["QuartzDailyJobTrigger"]; }
        }

        public static int CheckInterval
        {
            get { return int.Parse(ConfigurationSettings.AppSettings["CheckInterval"]); }
        }

        public static string TempImagePath
        {
            get { return ConfigurationSettings.AppSettings["TempImagePath"]; }
        }
    }
}

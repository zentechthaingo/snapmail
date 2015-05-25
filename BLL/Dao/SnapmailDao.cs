using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BLL.Entities;
using BLL.Common;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Auth;

namespace BLL.Dao
{
    public class SnapmailDao
    {
        static string STORAGE_ACCOUNT = Settings.CloudStorageAccount;
        static string STORAGE_ACCOUNT_KEY = Settings.StorageAccountKey;
        static string SNAPMAIL_PARTITION_KEY = Settings.PartitionKey;
        static string SNAPMAIL_TABLE_NAME = Settings.SnapmailsTable;
        static string EMAIL_TABLE_NAME = Settings.EmailsTable;

        private static CloudTable GetAzureTable(string tableName)
        {
            StorageCredentials creds = new StorageCredentials(STORAGE_ACCOUNT, STORAGE_ACCOUNT_KEY);
            CloudStorageAccount storageAccount = new CloudStorageAccount(creds, useHttps: true);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable tbl = tableClient.GetTableReference(tableName);
            tbl.CreateIfNotExists();
            return tbl;
        }

        public static SnapmailRowData Get(string key)
        {
            try
            {
                CloudTable tblSnapmails = GetAzureTable(SNAPMAIL_TABLE_NAME);
                TableOperation retrieveEmail = TableOperation.Retrieve<SnapmailRowData>(SNAPMAIL_PARTITION_KEY, key);
                var retrieveResult = tblSnapmails.Execute(retrieveEmail);
                if (retrieveResult != null)
                    return (SnapmailRowData)retrieveResult.Result;

                return new SnapmailRowData();
            }
            catch (Exception x)
            {
                return new SnapmailRowData();
            }
        }

        public static List<SnapmailRowData> GetAll(string user)
        {
            List<SnapmailRowData> all = new List<SnapmailRowData>();
            try
            {
                CloudTable tblSnapmails = GetAzureTable(SNAPMAIL_TABLE_NAME);
                TableQuery<SnapmailRowData> qryGetUserEmails = new TableQuery<SnapmailRowData>().Where(
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, Settings.PartitionKey),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("UserId", QueryComparisons.Equal, user)));

                var qryResults = tblSnapmails.ExecuteQuery(qryGetUserEmails);
                foreach (SnapmailRowData result in qryResults)
                    all.Add(result);
            }
            catch (Exception x)
            {
                
            }
            return all.Where(s => s.IsActive).OrderBy(s => s.CameraNames).ToList<SnapmailRowData>();
        }

        public static List<SnapmailRowData> GetAll()
        {
            List<SnapmailRowData> all = new List<SnapmailRowData>();
            try
            {
                CloudTable tblSnapmails = GetAzureTable(SNAPMAIL_TABLE_NAME);
                TableQuery<SnapmailRowData> qryGetUserEmails = new TableQuery<SnapmailRowData>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, Settings.PartitionKey));

                var qryResults = tblSnapmails.ExecuteQuery(qryGetUserEmails);
                foreach (SnapmailRowData result in qryResults)
                    all.Add(result);
            }
            catch (Exception x)
            {

            }
            return all.OrderBy(s => s.CameraNames).ToList<SnapmailRowData>();
        }

        public static List<SnapmailRowData> GetScheduled(bool scheduled)
        {
            List<SnapmailRowData> all = new List<SnapmailRowData>();
            try
            {
                CloudTable tblSnapmails = GetAzureTable(SNAPMAIL_TABLE_NAME);
                TableQuery<SnapmailRowData> qryGetUserEmails = new TableQuery<SnapmailRowData>().Where(
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, Settings.PartitionKey),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("IsScheduled", QueryComparisons.Equal, scheduled.ToString())));

                var qryResults = tblSnapmails.ExecuteQuery(qryGetUserEmails);
                foreach (SnapmailRowData result in qryResults)
                    all.Add(result);
            }
            catch (Exception x)
            {

            }
            return all;
        }

        public static SnapmailRowData Insert(SnapmailRowData snapmail)
        {
            try
            {
                CloudTable tblSnapmails = GetAzureTable(SNAPMAIL_TABLE_NAME);
                TableOperation insertEmail = TableOperation.Insert(snapmail);
                tblSnapmails.Execute(insertEmail);
                return Get(snapmail.RowKey);
            }
            catch (Exception x)
            {
                return snapmail;
            }
        }

        public static SnapmailRowData Update(string key, SnapmailRowData snapmail)
        {
            try
            {
                CloudTable tblSnapmails = GetAzureTable(SNAPMAIL_TABLE_NAME);
                SnapmailRowData old = Get(key);
                SnapmailRowData mail = new SnapmailRowData() 
                {
                    RowKey = key,
                    PartitionKey = snapmail.PartitionKey,
                    ETag = snapmail.ETag,
                    Timestamp = snapmail.Timestamp,
                    Cameras = snapmail.Cameras,
                    CameraNames = snapmail.CameraNames,
                    Message = old.Message,
                    NotifyDays = snapmail.NotifyDays,
                    NotifyTime = snapmail.NotifyTime,
                    TimeZone = snapmail.TimeZone,
                    Recipients = snapmail.Recipients,
                    Subject = old.Subject,
                    UserId = snapmail.UserId,
                    UserName = snapmail.UserName,
                    IsActive = old.IsActive,
                    IsScheduled = old.IsScheduled,
                    AccessToken = snapmail.AccessToken,
                    RefreshToken = snapmail.RefreshToken,
                    TokenExpires = snapmail.TokenExpires,
                    TokenCreated = snapmail.TokenCreated
                };

                TableOperation updateEmail = TableOperation.Replace(mail);
                var result = tblSnapmails.Execute(updateEmail);
                return Get(key);
            }
            catch (Exception x)
            {
                return snapmail;
            }
        }

        public static SnapmailRowData Unsubscribe(string key, string email)
        {
            CloudTable tblSnapmails = GetAzureTable(SNAPMAIL_TABLE_NAME);
            SnapmailRowData snapmail = Get(key);
            try
            {
                string emails = snapmail.Recipients;
                emails = emails.ToLower().Remove(emails.IndexOf(email.ToLower()), email.Length);
                
                snapmail.Recipients = emails;

                TableOperation updateEmail = TableOperation.Replace(snapmail);
                var result = tblSnapmails.Execute(updateEmail);
                return Get(key);
            }
            catch (Exception x)
            {
                return snapmail;
            }
        }

        public static SnapmailRowData UpdateEmail(string key, string email, DateTime lastSent)
        {
            CloudTable tblSnapmails = GetAzureTable(SNAPMAIL_TABLE_NAME);
            SnapmailRowData snapmail = Get(key);
            try
            {
                snapmail.SentMail = email;
                snapmail.LastSent = lastSent.ToString();

                TableOperation updateEmail = TableOperation.Replace(snapmail);
                var result = tblSnapmails.Execute(updateEmail);
                return Get(key);
            }
            catch (Exception x)
            {
                return snapmail;
            }
        }

        public static SnapmailRowData UpdateScheduled(string key, bool scheduled)
        {
            CloudTable tblSnapmails = GetAzureTable(SNAPMAIL_TABLE_NAME);
            SnapmailRowData snapmail = Get(key);
            try
            {
                snapmail.IsScheduled = scheduled;

                TableOperation updateEmail = TableOperation.Replace(snapmail);
                var result = tblSnapmails.Execute(updateEmail);
                return Get(key);
            }
            catch (Exception x)
            {
                return snapmail;
            }
        }

        public static SnapmailRowData UpdateSnapmailToken(string key, string access_token)
        {
            CloudTable tblSnapmails = GetAzureTable(SNAPMAIL_TABLE_NAME);
            SnapmailRowData snapmail = Get(key);
            try
            {
                snapmail.AccessToken = access_token;

                TableOperation updateToken = TableOperation.Replace(snapmail);
                var result = tblSnapmails.Execute(updateToken);
                return Get(key);
            }
            catch (Exception x)
            {
                return snapmail;
            }
        }

        public static List<SnapmailRowData> UpdateUserToken(string user, string access_token)
        {
            List<SnapmailRowData> all = new List<SnapmailRowData>();
            try
            {
                CloudTable tblSnapmails = GetAzureTable(SNAPMAIL_TABLE_NAME);
                TableQuery<SnapmailRowData> qryGetUserEmails = new TableQuery<SnapmailRowData>().Where(
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, Settings.PartitionKey),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("UserId", QueryComparisons.Equal, user)));

                var qryResults = tblSnapmails.ExecuteQuery(qryGetUserEmails).Where(s => s.IsActive);
                foreach (SnapmailRowData data in qryResults)
                {
                    if (string.IsNullOrEmpty(data.AccessToken))
                    {
                        data.AccessToken = access_token;

                        TableOperation updateToken = TableOperation.Replace(data);
                        var result = tblSnapmails.Execute(updateToken);
                    }
                    all.Add(data);
                }
            }
            catch (Exception x)
            {

            }

            return all;
        }

        public static bool Delete(string key, bool active)
        {
            CloudTable tblSnapmails = GetAzureTable(SNAPMAIL_TABLE_NAME);
            SnapmailRowData snapmail = Get(key);
            try
            {
                snapmail.IsActive = active;

                TableOperation deleteEmail = TableOperation.Replace(snapmail);
                var result = tblSnapmails.Execute(deleteEmail);

                return true;
            }
            catch (Exception x)
            {
                return false;
            }
        }
    }
}

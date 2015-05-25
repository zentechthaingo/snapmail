using System;
using System.Collections.Generic;

using BLL.Entities;
using BLL.Common;

namespace OneButtonApi.Models
{
    public class SnapmailModel
    {
        /// <summary>
        /// evercam-user-id
        /// </summary>
        public string user_id { get; set; }
        /// <summary>
        /// evercam-user-name
        /// </summary>
        public string user_name { get; set; }
        /// <summary>
        /// camera1id, camera2id, camera3id, ...
        /// </summary>
        public string cameras { get; set; }
        /// <summary>
        /// camera1 name, camera2 name, camera3 name, ...
        /// </summary>
        public string camera_names { get; set; }
        /// <summary>
        /// email@domain.com; email@domain.com, ...
        /// </summary>
        public string recipients { get; set; }
        /// <summary>
        /// sat, sun, mon, ...
        /// </summary>
        public string notify_days { get; set; }
        /// <summary>
        /// Notification Time e.g. 14:30
        /// </summary>
        public string notify_time { get; set; }
        /// <summary>
        /// TimeZone
        /// </summary>
        public string timezone { get; set; }
        /// <summary>
        /// Should activate notification sending
        /// </summary>
        public bool is_active { get; set; }
        /// <summary>
        /// evercam-access-token
        /// </summary>
        public string access_token { get; set; }

        public static SnapmailRowData ToRowData(SnapmailModel model)
        {
            return new SnapmailRowData() 
            { 
                ETag = "*",
                PartitionKey = Settings.PartitionKey,
                RowKey = Guid.NewGuid().ToString(),
                Timestamp = DateTimeOffset.Now,
                Subject = Settings.EmailSubject,
                Message = Settings.EmailMessage,
                UserId = model.user_id,
                UserName = model.user_name,
                Cameras = model.cameras,
                CameraNames = model.camera_names,
                Recipients = model.recipients,
                NotifyDays = model.notify_days,
                NotifyTime = model.notify_time,
                TimeZone = model.timezone,
                SentMail = "",
                LastSent = DateTime.MinValue.ToString(),
                IsActive = true,
                AccessToken = model.access_token
            };
        }

        public static SnapmailRowData ToRowData(string key, SnapmailModel model)
        {
            return new SnapmailRowData()
            {
                ETag = "*",
                PartitionKey = Settings.PartitionKey,
                RowKey = key,
                Timestamp = DateTimeOffset.Now,
                UserId = model.user_id,
                UserName = model.user_name,
                Cameras = model.cameras,
                CameraNames = model.camera_names,
                Recipients = model.recipients,
                NotifyDays = model.notify_days,
                NotifyTime = model.notify_time,
                TimeZone = model.timezone,
                AccessToken = model.access_token
            };
        }

        public static SnapmailInfoModel ToInfoModel(SnapmailRowData data)
        {
            return new SnapmailInfoModel()
            {
                key = data.RowKey,
                user_id = data.UserId,
                
                cameras = data.Cameras,
                camera_names = data.CameraNames,
                recipients = data.Recipients,
                notify_days = data.NotifyDays,
                notify_time = data.NotifyTime,
                timezone = data.TimeZone,
                last_sent = data.LastSent != null ? data.LastSent.ToString() : DateTime.MinValue.ToString(),
                sent_mail = data.SentMail,
                is_active = data.IsActive,
                is_scheduled = data.IsScheduled,
                timestamp = data.Timestamp.ToString(),
                access_token = data.AccessToken
            };
        }

        public static List<SnapmailInfoModel> ToInfoModel(List<SnapmailRowData> list)
        {

            List<SnapmailInfoModel> result = new List<SnapmailInfoModel>();
            foreach(var data in list)
            {
                result.Add(new SnapmailInfoModel()
                {
                    key = data.RowKey,
                    user_id = data.UserId,
                    user_name = data.UserName,
                    cameras = data.Cameras,
                    camera_names = data.CameraNames,
                    recipients = data.Recipients,
                    notify_days = data.NotifyDays,
                    notify_time = data.NotifyTime,
                    timezone = data.TimeZone,
                    last_sent = data.LastSent != null ? data.LastSent.ToString() : DateTime.MinValue.ToString(),
                    sent_mail = data.SentMail,
                    is_active = data.IsActive,
                    is_scheduled = data.IsScheduled,
                    timestamp = data.Timestamp.ToString(),
                    access_token = data.AccessToken
                });
            }
            return result;
        }
    }

    public class SnapmailInfoModel
    {
        /// <summary>
        /// Key
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// evercam-user-id
        /// </summary>
        public string user_id { get; set; }
        /// <summary>
        /// evercam-user-name
        /// </summary>
        public string user_name { get; set; }
        /// <summary>
        /// camera1id, camera2id, camera3id, ...
        /// </summary>
        public string cameras { get; set; }
        /// <summary>
        /// camera1 name, camera2 name, camera3 name, ...
        /// </summary>
        public string camera_names { get; set; }
        /// <summary>
        /// email@domain.com; email@domain.com, ...
        /// </summary>
        public string recipients { get; set; }
        /// <summary>
        /// sat, sun, mon, ...
        /// </summary>
        public string notify_days { get; set; }
        /// <summary>
        /// Number of notification to send
        /// </summary>
        public string notify_time { get; set; }
        /// <summary>
        /// TimeZone
        /// </summary>
        public string timezone { get; set; }
        /// <summary>
        /// When was last notification sent
        /// </summary>
        public string last_sent { get; set; }
        /// <summary>
        /// Contents of last email notification sent
        /// </summary>
        public string sent_mail { get; set; }
        /// <summary>
        /// Is notification sending activated
        /// </summary>
        public bool is_active { get; set; }
        /// <summary>
        /// Indicates if notification has been scheduled or not
        /// </summary>
        public bool is_scheduled { get; set; }
        /// <summary>
        /// Timestamp
        /// </summary>
        public string timestamp { get; set; }
        /// <summary>
        /// evercam-access-token
        /// </summary>
        public string access_token { get; set; }
    }

    public class MailAddress
    {
        /// <summary>
        /// email@address.com
        /// </summary>
        public string email { get; set; }
    }
}
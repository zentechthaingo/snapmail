using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace BLL.Entities
{
    public class SnapmailRowData : TableEntity
    {
        /// <summary>
        /// evercam-user-id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// evercam-user-name
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// { camera1id,camera2id,camera3id, ... }
        /// </summary>
        public string Cameras { get; set; }
        /// <summary>
        /// { camera1 name , timezone}, camera2id, camera3id, ... }
        /// </summary>
        public string CameraNames { get; set; }
        /// <summary>
        /// Your Scheduled SnapMail @ {notify_time}
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// email@domain.com; email@domain.com, ...
        /// </summary>
        public string Recipients { get; set; }
        /// <summary>
        /// Notification email message, e.g. Hi {user-name} Here are your snapshots. {snapshots} Evercam.io
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// sat, sun, mon, ...
        /// </summary>
        public string NotifyDays { get; set; }
        /// <summary>
        /// Notification Time of day
        /// </summary>
        public string NotifyTime { get; set; }
        /// <summary>
        /// Timezone
        /// </summary>
        public string TimeZone { get; set; }
        /// <summary>
        /// Contents of last email notification sent
        /// </summary>
        public string SentMail { get; set; }
        /// <summary>
        /// When was last notification sent
        /// </summary>
        public string LastSent { get; set; }
        /// <summary>
        /// Send notifications till its Active
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Checks if this SnapMail is not yet scheduled
        /// </summary>
        public bool IsScheduled { get; set; }
        /// <summary>
        /// evercam-access-token
        /// </summary>
        public string AccessToken { get; set; }
        /// <summary>
        /// evercam-refresh-token
        /// </summary>
        public string RefreshToken { get; set; }
        /// <summary>
        /// evercam-token-expires-at
        /// </summary>
        public string TokenExpires { get; set; }
        /// <summary>
        /// evercam-token-created-at
        /// </summary>
        public string TokenCreated { get; set; }
    }
}
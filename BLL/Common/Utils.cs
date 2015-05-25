using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net;
using System.Net.Mail;
using BLL.Entities;
using BLL.Dao;

namespace BLL.Common
{
    public class Utils
    {
        public static DateTime SQLMinDate = new DateTime(1900, 1, 1, 12, 0, 0);     // changed from 1753    to fix utc
        public static DateTime SQLMaxDate = new DateTime(8888, 12, 31, 23, 59, 59); // changed from 9999    conversion errors
        private const string Dictionary = "abcdefghiklmonpqrstuxzwy";
        private const string Dictionary2 = "abcdefghiklmonpqrstuxzwy1234567890";

        public static string RemoveSymbols(string str)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (char c in str)
                {
                    if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '-' || c == '_')
                        sb.Append(c);
                }
            }
            catch { }
            return sb.ToString();
        }

        public static string CleanFileName(string str)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (char c in str)
                {
                    if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_')
                        sb.Append(c);
                }
            }
            catch { }
            return sb.ToString();
        }

        public static string GenerateRandomString(int length)
        {
            string s = "";
            Random r = new Random();
            for (int i = 0; i < length; i++)
            {
                int idx = r.Next(Dictionary.Length);
                s += Dictionary[idx];
            }
            return s;
        }

        public static string GeneratePassCode(int length)
        {
            string s = "";
            Random r = new Random();
            for (int i = 0; i < length; i++)
            {
                int idx = r.Next(Dictionary2.Length);
                s += Dictionary2[idx];
            }
            return s;
        }

        public static DayOfWeek[] GetDaysOfWeek(string days)
        {
            string[] dd = days.Split(',', ' ');
            DayOfWeek[] ddd = new DayOfWeek[dd.Length];
            for (int i = 0; i < dd.Length; i++)
            {
                if (dd[i].ToLower().Equals("sat"))
                    ddd[i] = DayOfWeek.Saturday;
                else if (dd[i].ToLower().Equals("sun"))
                    ddd[i] = DayOfWeek.Sunday;
                else if (dd[i].ToLower().Equals("mon"))
                    ddd[i] = DayOfWeek.Monday;
                else if (dd[i].ToLower().Equals("tue"))
                    ddd[i] = DayOfWeek.Tuesday;
                else if (dd[i].ToLower().Equals("wed"))
                    ddd[i] = DayOfWeek.Wednesday;
                else if (dd[i].ToLower().Equals("thu"))
                    ddd[i] = DayOfWeek.Thursday;
                else if (dd[i].ToLower().Equals("fri"))
                    ddd[i] = DayOfWeek.Friday;
            }
            return ddd;
        }

        public static bool IsTimeBetween(DateTime time, DateTime? fromTime, DateTime? toTime)
        {
            if (fromTime == null || toTime == null)
                return true;

            double sec = (time - new DateTime(time.Year, time.Month, time.Day)).TotalMilliseconds;
            double fromSec =
                (fromTime.Value - new DateTime(fromTime.Value.Year, fromTime.Value.Month, fromTime.Value.Day)).
                    TotalMilliseconds;
            double toSec =
                (toTime.Value - new DateTime(toTime.Value.Year, toTime.Value.Month, toTime.Value.Day)).TotalMilliseconds;

            if (fromSec == toSec && toSec == 0)
                return true;

            if (toSec > fromSec)
                return sec >= fromSec && sec <= toSec;
            return sec >= toSec || sec <= fromSec; // if range is 23.00 - 5.00
        }

        public static TimeZoneInfo GetTimeZoneInfo(string tz)
        {
            TimeZoneInfo tzi = String.IsNullOrEmpty(tz) ? TimeZoneInfo.Utc : TimeZoneInfo.FindSystemTimeZoneById(tz);
            return tzi;
        }

        public static DateTime ConvertToUtc(DateTime dt, string timezone, bool useTryCatch = false)
        {
            TimeZoneInfo tzi = GetTimeZoneInfo(timezone);
            if (useTryCatch)
            {
                try
                {
                    return TimeZoneInfo.ConvertTimeToUtc(dt, tzi);
                }
                catch (Exception)
                {
                    // Possibly we get this error: The supplied DateTime represents an invalid time.  
                    // For example, when the clock is adjusted forward, any time in the period that is skipped is invalid.
                    // so we move time to one hour earlier
                    return TimeZoneInfo.ConvertTimeToUtc(dt.AddHours(-1), tzi);
                }
            }
            return TimeZoneInfo.ConvertTimeToUtc(dt, tzi);
        }

        public static int ConvertHourToUtc(int hour, string timezone)
        {
            TimeZoneInfo tzi = GetTimeZoneInfo(timezone);
            return TimeZoneInfo.ConvertTimeToUtc(new DateTime(2011, 01, 01, hour, 0, 0), tzi).Hour;
        }

        public static DateTime ConvertFromUtc(DateTime dt, string timezone)
        {
            TimeZoneInfo tzi = GetTimeZoneInfo(timezone);
            return TimeZoneInfo.ConvertTimeFromUtc(dt, tzi);
        }

        public static double GetBaseUtcOffsetMilliseconds(string timezone)
        {
            if (string.IsNullOrEmpty(timezone))
                return 0;
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            return tz.BaseUtcOffset.TotalMilliseconds;
        }

        public static double GetTimeZoneOffSetMilliseconds(string timezone)
        {
            if (string.IsNullOrEmpty(timezone))
                return 0;
            var now = DateTime.UtcNow;
            var dt = ConvertFromUtc(now, timezone);
            return (dt - now).TotalMilliseconds;
        }

        public static double GetTimeZoneOffSetHours(string timezone)
        {
            if (string.IsNullOrEmpty(timezone))
                return 0;
            var now = DateTime.UtcNow;
            var dt = ConvertFromUtc(now, timezone);
            return (dt - now).TotalHours;
        }

        public static string GetCameraDayEndTimeInUtc(DateTime time, string timeZone)
        {
            string endTime = "";
            DateTime utcTime = ConvertToUtc(time, timeZone);
            if (time.Hour == 0 && time.Minute == 0 && time.Second == 0 && time.Millisecond == 0)
                endTime = utcTime.AddSeconds(86399).ToString("yyyyMMddHHmmssfff");
            //there are total 86400 seconds in a day, 86399 is used to keep the day same
            return endTime;
        }

        public static string GetNiceTime(DateTime dt)
        {
            TimeSpan ts = DateTime.UtcNow - dt;
            if (ts.TotalMinutes < 60)
                return "" + (int)ts.TotalMinutes + " minutes";
            if (ts.TotalHours < 24)
                return "" + (int)ts.TotalHours + " hours";
            if (ts.TotalDays < 30)
                return "" + (int)ts.TotalDays + " days";
            if (ts.TotalDays < 365)
                return "" + (int)ts.TotalDays / 30 + " months";
            if (ts.TotalDays > 365)
                return "" + (int)ts.TotalDays / 365 + " years";
            return "";
        }

        public static string GetSpace(long size)
        {
            if (size < 1024 * 1024)
                return "" + ((double)size / 1024).ToString("f1") + "Kb";
            if (size < 1024 * 1024 * 1024)
                return "" + ((double)size / (1024 * 1024)).ToString("f1") + "Mb";
            if (size > 1024 * 1024 * 1024)
            {
                double gbs = (double)size / (1024 * 1024 * 1024);
                if (gbs > 1024)
                {
                    double tbs = gbs / 1024;
                    return "" + tbs.ToString("f1") + "Tb";
                }
                return "" + gbs.ToString("f1") + "Gb";
            }
            return size + "B";
        }

        public static Image MakeIconImage(string sourceFile, int radius)
        {
            using (Image image = Image.FromFile(sourceFile)) {
                radius *= 2;
                Bitmap RoundedImage = new Bitmap(image.Width, image.Height);
                Color backColor = RoundedImage.GetPixel(0, 0);
                Graphics g = Graphics.FromImage(RoundedImage);

                g.Clear(backColor);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Brush brush = new TextureBrush(image);
                GraphicsPath gp = new GraphicsPath();
                gp.AddArc(0, 0, radius, radius, 180, 90);
                gp.AddArc(0 + RoundedImage.Width - radius, 0, radius, radius, 270, 90);
                gp.AddArc(0 + RoundedImage.Width - radius, 0 + RoundedImage.Height - radius, radius, radius, 0, 90);
                gp.AddArc(0, 0 + RoundedImage.Height - radius, radius, radius, 90, 90);
                g.FillPath(brush, gp);

                RoundedImage.MakeTransparent(backColor);

                return RoundedImage;
            }
        }

        public static string MakeSquareImage(string sourceFile)
        {
            string destFile = sourceFile;
            using (Image image = Image.FromFile(sourceFile))
            {
                int targetWidth = image.Width, targetHeight = image.Height;

                if (image.Width == image.Height)
                    return sourceFile;

                if (image.Width > image.Height)
                {
                    targetWidth = image.Height;
                    targetHeight = image.Height;
                }
                if (image.Width < image.Height)
                {
                    targetWidth = image.Width;
                    targetHeight = image.Width;
                }

                int x = image.Width / 2 - targetWidth / 2;
                int y = image.Height / 2 - targetHeight / 2;

                Rectangle cropArea = new Rectangle(x, y, targetWidth, targetHeight);

                using (Bitmap original = new Bitmap(image))
                {
                    using (Bitmap bitmap = original.Clone(cropArea, original.PixelFormat))
                    {
                        FileInfo info = new FileInfo(sourceFile);
                        bitmap.Save(destFile = Path.Combine(info.DirectoryName, "_" + info.Name));
                    }
                }
            }
            return destFile;
        }

        public static void FileLog(string msg, string key)
        {
            try
            {
                System.IO.Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

                if (!Directory.Exists(logPath))
                    Directory.CreateDirectory(logPath);

                string logFile = Path.Combine(logPath, DateTime.UtcNow.ToString("yyyy-MM-dd") + "_" + CleanFileName(key) + ".txt");

                StreamWriter file = new StreamWriter(logFile, true);
                file.WriteLine(DateTime.UtcNow + "\t" + msg + "\n");
                file.Close();
            }
            catch (Exception) { }
        }

        public static void FileLog(string msg)
        {
            try
            {
                System.IO.Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

                if (!Directory.Exists(logPath))
                    Directory.CreateDirectory(logPath);

                string logFile = Path.Combine(logPath, DateTime.UtcNow.ToString("yyyy-MM-dd") + ".txt");

                StreamWriter file = new StreamWriter(logFile, true);
                file.WriteLine(DateTime.UtcNow + "\t" + msg + "\n");
                file.Close();
            }
            catch (Exception) { }
        }

        public static void SendMail(string subject, string message, string recipients)
        {
            SmtpClient smtp = new SmtpClient
            {
                Host = Settings.SmtpServer,
                Port = int.Parse(Settings.SmtpServerPort),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(Settings.SmtpUser, Settings.SmtpPassword)
            };

            MailMessage mail = new MailMessage
            {
                From = new MailAddress(Settings.SmtpEmail, Settings.EmailSource),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            try
            {
                string[] emails = recipients.Split(new char[] { ';', ',', ' ' });
                foreach (string email in emails)
                {
                    //mail.To.Clear();
                    if (!string.IsNullOrEmpty(email))
                        mail.To.Add(new MailAddress(email));
                }
                if (mail.To.Count > 0)
                    smtp.Send(mail);
            }
            catch (Exception x)
            {

            }
        }

        public static void SendMail(string subject, string message, string recipients, List<string> attachments)
        {
            SmtpClient smtp = new SmtpClient
            {
                Host = Settings.SmtpServer,
                Port = int.Parse(Settings.SmtpServerPort),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(Settings.SmtpUser, Settings.SmtpPassword)
            };

            MailMessage mail = new MailMessage
            {
                From = new MailAddress(Settings.SmtpEmail, Settings.EmailSource),
                Subject = subject,
                IsBodyHtml = true
            };
            
            try
            {
                foreach(string s in attachments)
                    mail.Attachments.Add(new Attachment(s));

                string[] emails = recipients.Split(new char[] { ';', ',', ' ' });
                foreach (string email in emails)
                {
                    mail.To.Clear();
                    if (!string.IsNullOrEmpty(email))
                    {
                        mail.Body = message.Replace("{email}", email);
                        mail.To.Add(new MailAddress(email));
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception x)
            {
                
            }
        }

        public static SnapshotData DownloadImage(string url, string username, string password, bool useCredentials)
        {
            SnapshotData snapshot = new SnapshotData();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;
            
            if (useCredentials)
                request.Credentials = new NetworkCredential(username, password);

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (MemoryStream ms = new MemoryStream(60000))
                    {
                        if (response.ContentType.Contains("image") && stream != null)
                        {
                            stream.CopyTo(ms);
                            snapshot.Data = ms.ToArray();
                            snapshot.ContentType = response.Headers["Content-Type"];
                        }
                    }
                }
            }

            return snapshot;
        }
    }

    public class SnapshotData
    {
        public byte[] Data { get; set; }
        public string ContentType { get; set; }
    }
}

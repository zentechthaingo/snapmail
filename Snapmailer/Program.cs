using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BLL;
using BLL.Entities;
using BLL.Dao;
using BLL.Common;
using EvercamV2;

namespace Snapmailer
{
    class Program
    {
        static SnapmailRowData data = new SnapmailRowData();
        static Evercam evercam = new Evercam();

        static void Main(string[] args)
        {
            Evercam.SANDBOX = Settings.EvercamSandboxMode;
            
            //// for testing
            //args = new string[1];
            //args[0] = "81f3e1dc-85b5-4ca3-bd4a-4636f8e66602";
            //data = SnapmailDao.Get(args[0]);

            if (!string.IsNullOrEmpty(args[0]))
            {
                while ((data = SnapmailDao.Get(args[0])) != null && data.IsActive && !string.IsNullOrEmpty(data.Recipients))
                {
                    if (!string.IsNullOrEmpty(data.AccessToken))
                    {
                        evercam = new Evercam(data.AccessToken);
                        string[] cred = data.AccessToken.Split(':');
                        if (cred.Length >= 2)
                            evercam = new Evercam(cred[0], cred[1]);
                        int hh = int.Parse(data.NotifyTime.Substring(0, 2));
                        int mm = int.Parse(data.NotifyTime.Substring(3, 2));
                        DateTime scheduled = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, hh, mm, 0);
                        scheduled = Utils.ConvertToUtc(scheduled, data.TimeZone, true);

                        DayOfWeek[] days = Utils.GetDaysOfWeek(data.NotifyDays);
                        if (days.Contains<DayOfWeek>(DateTime.UtcNow.DayOfWeek))
                        {
                            if (DateTime.UtcNow >= scheduled && DateTime.UtcNow <= scheduled.AddMinutes(1))
                            {
                                Utils.FileLog("User: " + data.UserName + ", Cameras: " + data.Cameras, data.RowKey);

                                List<string> attachments = new List<string>();
                                bool anyImages = false;
                                bool anyErrors = false;
                                bool anyDebugs = false;
                                string images = "";
                                string errors = "<ul>";
                                string debugs = "<ul>";
                                string[] cc = data.Cameras.Split(',', ' ');
                                foreach (string c in cc)
                                {
                                    bool gotImage = false;
                                    string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
                                    string temppath = Path.Combine(Settings.TempImagePath, c + timestamp + ".jpg");
                                
                                    Camera camera = new Camera();
                                    byte[] image = null;
                                    for (int i = 1; i <= Settings.TryCount; i++)
                                    {
                                        try
                                        {
                                            if (camera == null || string.IsNullOrEmpty(camera.ID))
                                                camera = evercam.GetCamera(c);

                                            // store and returns live snapshot on evercam
                                            image = evercam.CreateSnapshot(c, Settings.EvercamClientName, true).ToBytes();
                                            Utils.FileLog("Image data retrieved (try#" + i + "): " + data.RowKey, data.RowKey);

                                            if (image != null && Storage.SaveFile(temppath, image))
                                            {
                                                attachments.Add(temppath);
                                                temppath = WebUtility.UrlDecode(Path.Combine(Settings.TempImagePath, c + timestamp + ".jpg")).Replace(@"/", @"\\");
                                                temppath = temppath.Replace(Settings.TempImagePath.Replace(@"/", @"\\"), Settings.ServerUrl + @"images/");
                                                images += "<br /><img src='" + temppath + "' width='100%' /> <br /><br /><strong>" + camera.Name + "</strong> (" + camera.ID + ") - See the live view on Evercam by <a target='_blank' href='https:////dashboard.evercam.io/v1/cameras/" + c + "/live'>clicking here</a><br />";

                                                anyImages = true;
                                                gotImage = true;

                                                break;
                                            }
                                            else
                                            {
                                                debugs += "<li> <i>Image could not be saved from <a target='_blank' href='https:////dashboard.evercam.io/cameras/" + c + "'>" + c + "</a></i>";
                                                Utils.FileLog("Image could not be saved from Camera " + c, data.RowKey);
                                                anyDebugs = true;
                                            }
                                        }
                                        catch (Exception x)
                                        {
                                            anyDebugs = true;
                                            debugs += "<li> <i>Image could not be saved from <a target='_blank' href='https:////dashboard.evercam.io/cameras/" + c + "'>" + c + "</a></i>. [Error: " + x.Message + "]";
                                            Utils.FileLog("Error (try#" + i + "): " + data.RowKey + ": " + x.ToString(), data.RowKey);

                                            if (x.Message.Contains("offline"))
                                                break;

                                            if (i < Settings.TryCount)
                                                Thread.Sleep(Settings.RetryInterval);    // 15 seconds
                                        }
                                    }

                                    if (!gotImage)
                                    {
                                        // download latest snapshot from evercam
                                        try
                                        {
                                            Snapshot snap = evercam.GetLatestSnapshot(c, true);
                                            image = snap.ToBytes();
                                            DateTime last = EvercamV2.Utility.ToWindowsDateTime(snap.CreatedAt);
                                            
                                            // assuming that snapshot timestamp is in camera timezone
                                            DateTime utc = Utils.ConvertToUtc(last, camera.Timezone, true);
                                            if (utc > DateTime.UtcNow.AddMinutes(-5))
                                            {
                                                if (Storage.SaveFile(temppath, image))
                                                {
                                                    attachments.Add(temppath);
                                                    temppath = WebUtility.UrlDecode(Path.Combine(Settings.TempImagePath, c + timestamp + ".jpg")).Replace(@"/", @"\\");
                                                    temppath = temppath.Replace(Settings.TempImagePath.Replace(@"/", @"\\"), Settings.ServerUrl + @"images/");
                                                    images += "<br /><img src='" + temppath + "' width='100%' /> <br /><br /><strong>" + camera.Name + "</strong> (" + camera.ID + ") - See the live view on Evercam by <a target='_blank' href='https:////dashboard.evercam.io/v1/cameras/" + c + "/live'>clicking here</a><br />";

                                                    anyImages = true;
                                                }
                                                else
                                                {
                                                    errors += "<li> <i>Could not retrieve an image from <a target='_blank' href='https:////dashboard.evercam.io/cameras/" + c + "'>" + c + "</a></i>";
                                                    debugs += "<li> <i>Latest image could not be retrieved from <a target='_blank' href='https:////dashboard.evercam.io/cameras/" + c + "'>" + c + "</a></i>";
                                                    Utils.FileLog("Latest image could not be saved from Camera " + c, data.RowKey);

                                                    anyErrors = anyDebugs = true;
                                                }
                                            }
                                            else
                                            {
                                                attachments.Add(temppath);
                                                temppath = WebUtility.UrlDecode(Path.Combine(Settings.TempImagePath, c + timestamp + ".jpg")).Replace(@"/", @"\\");
                                                temppath = temppath.Replace(Settings.TempImagePath.Replace(@"/", @"\\"), Settings.ServerUrl + @"images/");

                                                errors += "<li> <i>Could not retrieve an image from <a target='_blank' href='https:////dashboard.evercam.io/cameras/" + c + "'>" + c + "</a></i>";
                                                errors += "<br /><i>Here is the last image we received from this camera</i><br /><img src='" + temppath + "' width='50%' /></li>";

                                                debugs += "<li> <i>Latest image could not be retrieved from <a target='_blank' href='https:////dashboard.evercam.io/cameras/" + c + "'>" + c + "</a></i>";
                                                debugs += "<br /><i>Here is the last image we received from this camera @ " + last.ToString() + ":</i><br /><img src='" + temppath + "' width='50%' /></li>";
                                                Utils.FileLog("Latest image is too old from Camera " + c, data.RowKey);

                                                anyErrors = anyDebugs = true;
                                            }
                                        }
                                        catch (Exception x)
                                        {
                                            errors += "<li> <i>Could not retrieve an image from <a target='_blank' href='https:////dashboard.evercam.io/cameras/" + c + "'>" + c + "</a></i>";
                                            debugs += "<li> <i>Latest image could not be retrieved from <a target='_blank' href='https:////dashboard.evercam.io/cameras/" + c + "'>" + c + "</a>. [Error: " + x.Message + "]</i>";
                                            Utils.FileLog("Latest image could not be retrieved from Camera " + c, data.RowKey);
                                            anyErrors = anyDebugs = true;
                                        }
                                    }
                                }
                                errors += "</ul>";
                                debugs += "</ul>";
                                
                                string message = "";
                                string debug = "";
                                if (anyImages) {
                                    message = data.Message.Replace("{br}", "<br />").Replace("{snapshots}", images + (!anyErrors ? "" : "<br />But...<br />" + errors)).Replace("{unsubscribe}", "<center style='font-size:11px'>If you want to change your Snapmail settings, <a target='_blank' href='" + Settings.ServerUrl + "?user=" + data.UserId + "'>click here</a>.<br />If you would prefer not to receive future emails for this Scheduled SnapMail @ " + data.NotifyTime + ", you may <a target='_blank' href='" + Settings.ServerUrl + "Unsubscribe.html?id=" + data.RowKey + "&email={email}'>unsubscribe here</a>.</center>");
                                    debug = data.Message.Replace("{br}", "<br />").Replace("{snapshots}", images + (!anyDebugs ? "" : "<br />But...<br />" + debugs)).Replace("{unsubscribe}", "<center style='font-size:11px'>If you want to change your Snapmail settings, <a target='_blank' href='" + Settings.ServerUrl + "?user=" + data.UserId + "'>click here</a>.<br />If you would prefer not to receive future emails for this Scheduled SnapMail @ " + data.NotifyTime + ", you may <a target='_blank' href='" + Settings.ServerUrl + "Unsubscribe.html?id=" + data.RowKey + "&email={email}'>unsubscribe here</a>.</center>");
                                }
                                else
                                {
                                    message = data.Message.Replace("{br}", "<br />").Replace("Here's the snapshot(s) from your cameras.", "").Replace("{snapshots}", (!anyErrors ? "" : errors)).Replace("{unsubscribe}", "<center style='font-size:11px'>If you want to change your Snapmail settings, <a target='_blank' href='" + Settings.ServerUrl + "?user=" + data.UserId + "'>click here</a>.<br />If you would prefer not to receive future emails for this Scheduled SnapMail @ " + data.NotifyTime + ", you may <a target='_blank' href='" + Settings.ServerUrl + "Unsubscribe.html?id=" + data.RowKey + "&email={email}'>unsubscribe here</a>.</center>");
                                    debug = data.Message.Replace("{br}", "<br />").Replace("Here's the snapshot(s) from your cameras.", "").Replace("{snapshots}", (!anyDebugs ? "" : debugs)).Replace("{unsubscribe}", "<center style='font-size:11px'>If you want to change your Snapmail settings, <a target='_blank' href='" + Settings.ServerUrl + "?user=" + data.UserId + "'>click here</a>.<br />If you would prefer not to receive future emails for this Scheduled SnapMail @ " + data.NotifyTime + ", you may <a target='_blank' href='" + Settings.ServerUrl + "Unsubscribe.html?id=" + data.RowKey + "&email={email}'>unsubscribe here</a>.</center>");
                                }
                                
                                // Finally send email
                                Utils.SendMail(data.Subject.Replace("{notify_time}", data.NotifyTime), message, data.Recipients, attachments);
                                
                                if (!string.IsNullOrEmpty(Settings.DebugEmail) && anyDebugs)
                                    Utils.SendMail("[DEBUG] " + data.Subject.Replace("{notify_time}", data.NotifyTime), debug, Settings.DebugEmail, attachments);
                                
                                SnapmailDao.UpdateEmail(data.RowKey, message.Replace("{email}", data.Recipients), DateTime.UtcNow);

                                Utils.FileLog("SendMail: " + message, data.RowKey);
                            }
                            else
                            {
                                Utils.FileLog("Schedule out of time @ UTC " + scheduled.ToString(), data.RowKey);
                            }
                        }
                        else
                        {
                            Utils.FileLog("Schedule out of days @ " + data.NotifyDays, data.RowKey);
                        }
                    }
                    else
                    {
                        Utils.FileLog("Evercam Access Token Not Found @ " + data.RowKey, data.RowKey);
                    }

                    if (!data.IsScheduled)
                        SnapmailDao.UpdateScheduled(data.RowKey, true);
                    
                    Thread.Sleep(Settings.CheckInterval);
                }

                if (data != null && !string.IsNullOrEmpty(data.RowKey))
                    Utils.FileLog("Exiting Snapmailer...", data.RowKey);
                else
                    Utils.FileLog("Exiting Snapmailer (no data)...", args[0]);
            }
        }
    }
}

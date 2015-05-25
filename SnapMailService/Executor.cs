using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Configuration;
using System.Collections;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using BLL.Dao;
using BLL.Entities;
using BLL.Common;
using Quartz;
using Quartz.Impl;
using Microsoft.WindowsAzure.Storage.Table;

namespace OneButtonAppService
{
    public class Executor
    {
        bool isServiceRunning = true;
        string SnapmailExePath = "";

        public void Execute()
        {
            Utils.FileLog("Service execution started...");
            
            while (isServiceRunning)
            {
                Utils.FileLog(Environment.NewLine + "Fetching pending snapmails...");
                List<SnapmailRowData> snapmails = SnapmailDao.GetAll();

                if (snapmails.Count > 0)
                {
                    List<string> keys = new List<string>();
                    foreach (SnapmailRowData data in snapmails)
                    {
                        ///// TESTING ONLY
                        //if (data.UserId != "shakeelanjum")
                        //    continue;

                        if (string.IsNullOrEmpty(data.AccessToken))
                        {
                            Utils.FileLog("Skipping snapmail (" + data.RowKey + ") No access token");
                            continue;
                        }
                        if (string.IsNullOrEmpty(data.Recipients))
                        {
                            Utils.FileLog("Skipping snapmail (" + data.RowKey + ") No more recipients");
                            continue;
                        }

                        if (string.IsNullOrEmpty((SnapmailExePath = CopySnapmailer(data.RowKey))))
                        {
                            Utils.FileLog("Skipping snapmail (" + data.RowKey + ") Unable to create copy of Snapmailer.exe");
                            continue;
                        }

                        StartSnapmailer(data);
                    }
                }
                Thread.Sleep(Settings.CheckInterval);
            }
        }

        private int StartSnapmailer(SnapmailRowData snapmail)
        {
            try
            {
                int pid = ProcessRunning(snapmail);
                
                if (snapmail.IsActive && !snapmail.IsScheduled)
                {
                    ProcessStartInfo process = new ProcessStartInfo(SnapmailExePath, snapmail.RowKey);
                    process.UseShellExecute = true;
                    process.WindowStyle = ProcessWindowStyle.Normal;

                    Process currentProcess = Process.Start(process);

                    Utils.FileLog("Executor.StartSnapmailer(" + snapmail.RowKey + ")");

                    return currentProcess.Id;
                }
                else if (snapmail.IsActive && snapmail.IsScheduled)
                {
                    if (pid == 0)
                    {
                        ProcessStartInfo process = new ProcessStartInfo(SnapmailExePath, snapmail.RowKey);
                        process.UseShellExecute = true;
                        process.WindowStyle = ProcessWindowStyle.Normal;

                        Process currentProcess = Process.Start(process);

                        Utils.FileLog("Executor.StartSnapmailer(" + snapmail.RowKey + ") - re");

                        return currentProcess.Id;
                    }
                }
                else if (!snapmail.IsActive && snapmail.IsScheduled)
                {
                    if (pid > 0)
                        KillProcess(pid, snapmail.RowKey);
                        //SnapmailDao.UpdateScheduled(snapmail.RowKey, false);
                }
                else if (!snapmail.IsActive && !snapmail.IsScheduled)
                {
                    if (pid > 0)
                        KillProcess(pid, snapmail.RowKey);
                }
            }
            catch (Exception x)
            {
                Utils.FileLog("Executor.StartSnapmailer(" + snapmail.RowKey + ") Error: " + x.Message);
            }
            return 0;
        }

        protected string CopySnapmailer(string key)
        {
            string ExeFile = Path.Combine(Settings.SnapmailExePath, "Snapmailer.exe");
            string ConfigFile = Path.Combine(Settings.SnapmailExePath, "Snapmailer.exe.config");
            string PathDest = Path.Combine(Settings.SnapmailExePath, "snapmailer_" + Utils.CleanFileName(key) + ".exe");
            string ConfigDest = Path.Combine(Settings.SnapmailExePath, "snapmailer_" + Utils.CleanFileName(key) + ".exe.config");
            try
            {
                // if already exists (and process is running) 
                // then kill the process and delete its exe
                if (File.Exists(PathDest))
                {
                    //KillProcess(ProcessRunning("snapmailer_" + key), 0);
                    try
                    {
                        File.Delete(PathDest);
                        File.Delete(ConfigDest);

                        File.Copy(ExeFile, PathDest, true);
                        File.Copy(ConfigFile, ConfigDest, true);
                    }
                    catch (Exception x)
                    {
                        //Utils.FileLog("CopySnapmailer(" + key + ") Error in File.Delete/File.Copy: " + x.Message);
                    }
                }
                else if (!File.Exists(PathDest))
                {
                    File.Copy(ExeFile, PathDest, true);
                    File.Copy(ConfigFile, ConfigDest, true);
                }
            }
            catch (Exception x)
            {
                return "";
            }
            return PathDest;
        }

        private int ProcessRunning(SnapmailRowData snapmail)
        {
            try
            {
                int id = 0;
                    Process[] processlist = Process.GetProcesses();
                    foreach (Process process in processlist)
                {
                    if (process.ProcessName.ToLower().StartsWith("snapmailer_"))
                    {
                        string _id = process.ProcessName.Substring(
                            process.ProcessName.IndexOf("_") + 1,
                            process.ProcessName.Length - (process.ProcessName.IndexOf("_") + 1));
                        if (_id == Utils.CleanFileName(snapmail.RowKey))
                        {
                            if (process.Responding)
                            {
                                id = process.Id;
                                break;
                            }
                            else
                            {
                                id = 0;
                                KillProcess(process.Id, snapmail.RowKey);
                                break;
                            }
                        }
                    }
                }
                return id;
            }
            catch (Exception x)
            {
                Console.WriteLine("ProcessRunning: " + snapmail.RowKey);
                return 0;
            }
        }

        private bool KillProcess(int pid, string key)
        {
            if (pid == 0) return false;
            try
            {
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = "taskkill.exe";
                start.Arguments = "/pid " + pid + " /F";
                start.UseShellExecute = false;

                Process process = new Process();
                start.CreateNoWindow = true;
                process.StartInfo = start;
                process.Start();
                process.WaitForExit(500);

                Utils.FileLog("KillProcess: " + key);

                return true;
            }
            catch (Exception x)
            {
                Utils.FileLog("KillProcess (" + key + ") Error: " + x.Message);
                return false;
            }
        }

        private bool KillProcess(string key)
        {
            Process[] processlist = Process.GetProcesses();
            foreach (Process process in processlist)
            {
                if (process.ProcessName.ToLower().StartsWith("snapmailer_"))
                {
                    string _id = process.ProcessName.Substring(
                        process.ProcessName.IndexOf("_") + 1,
                        process.ProcessName.Length - (process.ProcessName.IndexOf("_") + 1));
                    if (_id == Utils.CleanFileName(key))
                    {
                        try
                        {
                            ProcessStartInfo start = new ProcessStartInfo();
                            start.FileName = "taskkill.exe";
                            start.Arguments = "/pid " + process.Id + " /F";
                            start.UseShellExecute = false;

                            Process p = new Process();
                            start.CreateNoWindow = true;
                            p.StartInfo = start;
                            p.Start();
                            p.WaitForExit(500);

                            Utils.FileLog("KillProcess: #" + key);

                            return true;
                        }
                        catch (Exception x)
                        {
                            Utils.FileLog("KillProcess (" + key + ") Error: " + x.Message);
                        }
                    }
                }
            }
            return false;
        }

        public void StopExecution()
        {
            isServiceRunning = false;
            Process[] processlist = Process.GetProcesses();
            foreach (Process process in processlist)
            {
                if (process.ProcessName.ToLower().StartsWith("snapmailer_"))
                {
                    Console.WriteLine("kill " + process.ProcessName);
                    KillProcess(process.Id, process.ProcessName);
                }
            }
        }
    }
}

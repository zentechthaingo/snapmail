﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using BLL.Entities;
using BLL.Common;

namespace BLL.Dao
{
    public class LogDao
    {
        public static int Insert(Log log)
        {
            string query = @"INSERT INTO [dbo].[Logs] " +
                           "([CameraId],[UserId],[Message],[Details],[Type],[CreatedAt]) " +
                           "VALUES " +
                           "(@CameraId,@UserId,@Message,@Details,@Type,@CreatedAt) " +
                           "SELECT CAST(scope_identity() AS int)";
            try
            {
                var p2 = new SqlParameter("@CameraId", (log.CameraId == null ? "" : log.CameraId));
                var p3 = new SqlParameter("@UserId", (log.UserId == null ? "" : log.UserId));
                var p4 = new SqlParameter("@Message", (string.IsNullOrEmpty(log.Message) ? "" : log.Message));
                var p5 = new SqlParameter("@Details", (string.IsNullOrEmpty(log.Details) ? "" : log.Details));
                var p6 = new SqlParameter("@Type", log.Type);
                var p7 = new SqlParameter("@CreatedAt", DateTime.Now);
                
                var list = new[] { p2, p3, p4, p5, p6, p7 };
                var cmd = new SqlCommand { CommandText = query, CommandType = CommandType.Text };
                cmd.Parameters.AddRange(list);
                Connection.OpenConnection();
                cmd.Connection = Connection.DbConnection;
                int result = (int)cmd.ExecuteScalar();
                Connection.CloseConnection();
                cmd.Dispose();
                return result;
            }
            catch (Exception ex)
            {
                string msg = ("LogsDao Insert(Log log) " + ex.Message);
                return 0;
            }
            finally
            { Connection.CloseConnection(); }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using BLL.Entities;
using BLL.Common;

namespace BLL.Dao
{
    public class AppDao
    {
        public static App Get(int id)
        {
            var app = new App();
            try
            {
                const string sql = "Select * FROM Apps WHERE id=@id AND IsDeleted=0 ORDER BY Id DESC";
                var p1 = new SqlParameter("@id", id);
                var cmd = new SqlCommand { CommandText = sql, CommandType = CommandType.Text };
                cmd.Parameters.Add(p1);
                cmd.Connection = Connection.DbConnection;
                Connection.OpenConnection();
                var dr = GetListFromDataReader(cmd.ExecuteReader());

                if (dr.Count > 0) app = dr.FirstOrDefault();

                Connection.CloseConnection();
                return app;
            }
            catch (Exception ex)
            {
                string msg = string.Format("AppDao Get(int id) Id={0}<br />{1}", id, ex.Message);
                return app;
            }
            finally
            {
                Connection.CloseConnection();
            }
        }

        public static App Get(string code)
        {
            var app = new App();
            try
            {
                const string sql = "Select * FROM Apps WHERE code=@code AND IsDeleted=0 ORDER BY Id DESC";
                var p1 = new SqlParameter("@code", code);
                var cmd = new SqlCommand { CommandText = sql, CommandType = CommandType.Text };
                cmd.Parameters.Add(p1);
                cmd.Connection = Connection.DbConnection;
                Connection.OpenConnection();
                var dr = GetListFromDataReader(cmd.ExecuteReader());

                if (dr.Count > 0) app = dr.FirstOrDefault();

                Connection.CloseConnection();
                return app;
            }
            catch (Exception ex)
            {
                string msg = string.Format("AppDao Get(string code) Id={0}<br />{1}", code, ex.Message);
                return app;
            }
            finally
            {
                Connection.CloseConnection();
            }
        }

        public static List<App> GetList(AppStatus? status)
        {
            try
            {
                string s = (status.HasValue ? " AND Status = " + ((int)status.Value) : "");
                string sql = "Select * FROM Apps WHERE IsDeleted=0" + s + " ORDER BY Id DESC";
                var cmd = new SqlCommand { CommandText = sql, CommandType = CommandType.Text };
                cmd.Connection = Connection.DbConnection;
                Connection.OpenConnection();
                var dr = GetListFromDataReader(cmd.ExecuteReader());
                Connection.CloseConnection();
                return dr;
            }
            catch (Exception ex)
            {
                string msg = string.Format("AppDao GetList(AppStatus? status) <br />{1}", ex.Message);
                return new List<App>();
            }
            finally
            {
                Connection.CloseConnection();
            }
        }

        public static List<App> GetList(int count, AppStatus? status)
        {
            try
            {
                string s = (status.HasValue ? " AND Status = " + ((int)status.Value) : "");
                string sql = "Select TOP (" + count + ") * FROM Apps WHERE IsDeleted=0" + s + " ORDER BY Id ASC";
                var cmd = new SqlCommand { CommandText = sql, CommandType = CommandType.Text };
                cmd.Connection = Connection.DbConnection;
                Connection.OpenConnection();
                var dr = GetListFromDataReader(cmd.ExecuteReader());
                Connection.CloseConnection();
                return dr;
            }
            catch (Exception ex)
            {
                string msg = string.Format("AppDao GetList(AppStatus? status) <br />{1}", ex.Message);
                return new List<App>();
            }
            finally
            {
                Connection.CloseConnection();
            }
        }

        public static List<App> GetListByUserId(string userId, AppStatus? status)
        {
            try
            {
                string s = (status.HasValue ? " AND Status = " + ((int)status.Value) : "");
                string sql = "Select * FROM Apps WHERE UserId = '" + userId + "' AND IsDeleted=0" + s + " ORDER BY Id DESC";
                var cmd = new SqlCommand { CommandText = sql, CommandType = CommandType.Text };
                cmd.Connection = Connection.DbConnection;
                Connection.OpenConnection();
                var dr = GetListFromDataReader(cmd.ExecuteReader());
                Connection.CloseConnection();
                return dr;
            }
            catch (Exception ex)
            {
                string msg = string.Format("AppDao GetListByUserId(string userId, AppStatus? status) <br />{1}", ex.Message);
                return new List<App>();
            }
            finally
            {
                Connection.CloseConnection();
            }
        }

        public static List<App> GetListByCameraId(string cameraId, AppStatus? status)
        {
            try
            {
                string s = (status.HasValue ? " AND Status = " + ((int)status.Value) : "");
                string sql = "Select * FROM Apps WHERE CameraId = '" + cameraId + "' AND IsDeleted=0" + s + " ORDER BY Id DESC";
                var cmd = new SqlCommand { CommandText = sql, CommandType = CommandType.Text };
                cmd.Connection = Connection.DbConnection;
                Connection.OpenConnection();
                var dr = GetListFromDataReader(cmd.ExecuteReader());
                Connection.CloseConnection();
                return dr;
            }
            catch (Exception ex)
            {
                string msg = string.Format("AppDao GetListByCameraId(string cameraId, AppStatus? status) <br />{1}", ex.Message);
                return new List<App>();
            }
            finally
            {
                Connection.CloseConnection();
            }
        }

        public static int Insert(App app)
        {
            string query = @"INSERT INTO [dbo].[Apps] " +
                           "([UserId],[CameraId],[Code],[Title],[Email],[Status],[IconFile],[ThumbFile],[AppFile],[IsDeleted],[CreatedAt],[ModifiedAt],[CameraUrl],[CameraUser],[CameraPass]) " +
                           "VALUES " +
                           "(@UserId,@CameraId,@Code,@Title,@Email,@Status,@IconFile,@ThumbFile,@AppFile,@IsDeleted,@CreatedAt,@ModifiedAt,@CameraUrl,@CameraUser,@CameraPass) " +
                           "SELECT CAST(scope_identity() AS int)";
            try
            {
                var p1 = new SqlParameter("@CameraId", app.CameraId);
                var p2 = new SqlParameter("@UserId", app.UserId);
                var p3 = new SqlParameter("@Code", string.IsNullOrEmpty(app.Code) ? Utils.GeneratePassCode(10) : app.Code);
                var p4 = new SqlParameter("@Title", app.Title);
                var p5 = new SqlParameter("@Status", app.Status);
                var p6 = new SqlParameter("@Email", (string.IsNullOrEmpty(app.Email) ? "" : app.Email));
                var p7 = new SqlParameter("@IconFile", (string.IsNullOrEmpty(app.IconFile) ? "" : app.IconFile));
                var p8 = new SqlParameter("@ThumbFile", (string.IsNullOrEmpty(app.ThumbFile) ? "" : app.ThumbFile));
                var p9 = new SqlParameter("@AppFile", (string.IsNullOrEmpty(app.AppFile) ? "" : app.AppFile));
                var p10 = new SqlParameter("@IsDeleted", app.IsDeleted);
                var p11 = new SqlParameter("@CreatedAt", DateTime.Now);
                var p12 = new SqlParameter("@ModifiedAt", DateTime.Now);
                var p13 = new SqlParameter("@CameraUrl", (string.IsNullOrEmpty(app.CameraUrl) ? "" : app.CameraUrl));
                var p14 = new SqlParameter("@CameraUser", (string.IsNullOrEmpty(app.CameraUser) ? "" : app.CameraUser));
                var p15 = new SqlParameter("@CameraPass", (string.IsNullOrEmpty(app.CameraPassword) ? "" : app.CameraPassword));

                var list = new[] { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15 };
                var cmd = new SqlCommand { CommandText = query, CommandType = CommandType.Text };
                cmd.Parameters.AddRange(list);
                Connection.OpenConnection();
                cmd.Connection = Connection.DbConnection;
                app.ID = (int)cmd.ExecuteScalar();
                Connection.CloseConnection();
                cmd.Dispose();
                return app.ID;
            }
            catch (Exception ex)
            {
                string msg = ("AppDao Insert(App app) " + ex.Message);
                return 0;
            }
            finally
            { Connection.CloseConnection(); }
        }

        public static bool Update(App app)
        {
            string query = @"UPDATE [dbo].[Apps] " +
                           "SET [Title]=@Title, [ThumbFile]=@ThumbFile, [IconFile]=@IconFile, [AppFile]=@AppFile, [Email]=@Email, [Status]=@Status, [ModifiedAt]=@ModifiedAt, [CameraUrl]=@CameraUrl, [CameraUser]=@CameraUser, [CameraPass]=@CameraPass " +
                           "WHERE (Code = '" + app.Code + "')";
            try
            {
                var p1 = new SqlParameter("@Title", app.Title);
                var p2 = new SqlParameter("@ThumbFile", (string.IsNullOrEmpty(app.ThumbFile) ? "" : app.ThumbFile));
                var p3 = new SqlParameter("@IconFile", (string.IsNullOrEmpty(app.IconFile) ? "" : app.IconFile));
                var p4 = new SqlParameter("@AppFile", (string.IsNullOrEmpty(app.AppFile) ? "" : app.AppFile));
                var p5 = new SqlParameter("@Email", (string.IsNullOrEmpty(app.Email) ? "" : app.Email));
                var p6 = new SqlParameter("@Status", app.Status);
                var p7 = new SqlParameter("@ModifiedAt", DateTime.Now);
                var p8 = new SqlParameter("@CameraUrl", (string.IsNullOrEmpty(app.CameraUrl) ? "" : app.CameraUrl));
                var p9 = new SqlParameter("@CameraUser", (string.IsNullOrEmpty(app.CameraUser) ? "" : app.CameraUser));
                var p10 = new SqlParameter("@CameraPass", (string.IsNullOrEmpty(app.CameraPassword) ? "" : app.CameraPassword));
                
                var list = new[] { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10 };
                var cmd = new SqlCommand { CommandText = query, CommandType = CommandType.Text };
                cmd.Parameters.AddRange(list);
                Connection.OpenConnection();
                cmd.Connection = Connection.DbConnection;
                bool result = (cmd.ExecuteNonQuery() > 0);
                Connection.CloseConnection();
                cmd.Dispose();
                return result;
            }
            catch (Exception ex)
            {
                string msg = ("AppsDao Update(App app) " + ex.Message);
                return false;
            }
            finally
            { Connection.CloseConnection(); }
        }

        public static bool UpdateStatus(string code, int status)
        {
            string query = @"UPDATE [dbo].[Apps] " +
                           "SET [Status]=@Status, [ModifiedAt]=@ModifiedAt " +
                           "WHERE (Code = '" + code + "')";
            try
            {
                var p1 = new SqlParameter("@Status", status);
                var p2 = new SqlParameter("@ModifiedAt", DateTime.Now);
                
                var list = new[] { p1, p2 };
                var cmd = new SqlCommand { CommandText = query, CommandType = CommandType.Text };
                cmd.Parameters.AddRange(list);
                Connection.OpenConnection();
                cmd.Connection = Connection.DbConnection;
                bool result = (cmd.ExecuteNonQuery() > 0);
                Connection.CloseConnection();
                cmd.Dispose();
                return result;
            }
            catch (Exception ex)
            {
                string msg = ("AppsDao UpdateStatus(string code, int status) " + ex.Message);
                return false;
            }
            finally
            { Connection.CloseConnection(); }
        }

        public static bool UpdateStatus(string code, int status, string appUrl)
        {
            string query = @"UPDATE [dbo].[Apps] " +
                           "SET [Status]=@Status, [AppFile]=@AppFile, [ModifiedAt]=@ModifiedAt " +
                           "WHERE (Code = '" + code + "')";
            try
            {
                var p1 = new SqlParameter("@Status", status);
                var p2 = new SqlParameter("@AppFile", appUrl);
                var p3 = new SqlParameter("@ModifiedAt", DateTime.Now);

                var list = new[] { p1, p2, p3 };
                var cmd = new SqlCommand { CommandText = query, CommandType = CommandType.Text };
                cmd.Parameters.AddRange(list);
                Connection.OpenConnection();
                cmd.Connection = Connection.DbConnection;
                bool result = (cmd.ExecuteNonQuery() > 0);
                Connection.CloseConnection();
                cmd.Dispose();
                return result;
            }
            catch (Exception ex)
            {
                string msg = ("AppsDao UpdateStatus(string code, int status) " + ex.Message);
                return false;
            }
            finally
            { Connection.CloseConnection(); }
        }

        public static bool Delete(string code)
        {
            string query = @"UPDATE [dbo].[Apps] " +
                           "SET [IsDeleted] = 1 " +
                           "WHERE (Code = '" + code + "')";
            try
            {
                var cmd = new SqlCommand { CommandText = query, CommandType = CommandType.Text };
                Connection.OpenConnection();
                cmd.Connection = Connection.DbConnection;
                bool result = (cmd.ExecuteNonQuery() > 0);
                Connection.CloseConnection();
                cmd.Dispose();
                return result;
            }
            catch (Exception ex)
            {
                string msg = ("AppDao Delete(string code) " + ex.Message);
                return false;
            }
            finally
            { Connection.CloseConnection(); }
        }

        private static List<App> GetListFromDataReader(SqlDataReader dr)
        {
            List<App> apps = new List<App>();
            while (dr.Read())
            {
                var app = new App();
                if (!dr.IsDBNull(dr.GetOrdinal("Id")))
                    app.ID = dr.GetInt32(dr.GetOrdinal("Id"));

                if (!dr.IsDBNull(dr.GetOrdinal("Code")))
                    app.Code = dr["Code"].ToString();
                if (!dr.IsDBNull(dr.GetOrdinal("UserId")))
                    app.UserId = dr["UserId"].ToString();
                if (!dr.IsDBNull(dr.GetOrdinal("CameraId")))
                    app.CameraId = dr["CameraId"].ToString();
                if (!dr.IsDBNull(dr.GetOrdinal("CameraUrl")))
                    app.CameraUrl = dr["CameraUrl"].ToString();
                if (!dr.IsDBNull(dr.GetOrdinal("CameraUser")))
                    app.CameraUser = dr["CameraUser"].ToString();
                if (!dr.IsDBNull(dr.GetOrdinal("CameraPass")))
                    app.CameraPassword = dr["CameraPass"].ToString();

                if (!dr.IsDBNull(dr.GetOrdinal("Title")))
                    app.Title = dr["Title"].ToString();
                if (!dr.IsDBNull(dr.GetOrdinal("ThumbFile")))
                    app.ThumbFile = dr["ThumbFile"].ToString();
                if (!dr.IsDBNull(dr.GetOrdinal("IconFile")))
                    app.IconFile = dr["IconFile"].ToString();
                if (!dr.IsDBNull(dr.GetOrdinal("AppFile")))
                    app.AppFile = dr["AppFile"].ToString();
                if (!dr.IsDBNull(dr.GetOrdinal("Email")))
                    app.Email = dr["Email"].ToString();
                if (!dr.IsDBNull(dr.GetOrdinal("Status")))
                    app.Status = dr.GetInt32(dr.GetOrdinal("Status"));
                if (!dr.IsDBNull(dr.GetOrdinal("IsDeleted")))
                    app.IsDeleted = dr.GetBoolean(dr.GetOrdinal("IsDeleted"));
                if (!dr.IsDBNull(dr.GetOrdinal("ModifiedAt")))
                    app.ModifiedAt = dr.GetDateTime(dr.GetOrdinal("ModifiedAt"));
                if (!dr.IsDBNull(dr.GetOrdinal("CreatedAt")))
                    app.CreatedAt = dr.GetDateTime(dr.GetOrdinal("CreatedAt"));

                apps.Add(app);
            }
            dr.Close();
            dr.Dispose();
            return apps;
        }
    }
}

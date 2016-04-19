using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Web.Http;
using System.Web.Helpers;
using System.Web.Security;
using System.Web.Http.Cors;
using System.Net.Http.Headers;
using System.Collections.Generic;
using OneButtonApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using EvercamV2;
using Quartz;
using Quartz.Impl;
using BLL.Dao;
using BLL.Entities;
using BLL.Common;
using Microsoft.WindowsAzure.Storage.Table;

namespace OneButtonApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SnapmailsController : ApiController
    {
        [Route("api/snapmails/{key}")]
        [HttpGet]
        public SnapmailInfoModel Get(string key)
        {
            return SnapmailModel.ToInfoModel(SnapmailDao.Get(key));
        }

        [Route("api/snapmails/{key}/snapshots")]
        [HttpGet]
        public List<Snapshot> GetSnapshots(string key)
        {
            SnapmailRowData snapmail = SnapmailDao.Get(key);
            Evercam.SANDBOX = Settings.EvercamSandboxMode;
            Evercam evercam = new Evercam(snapmail.AccessToken);
            string[] cred = snapmail.AccessToken.Split(':');
            if (cred.Length >= 2)
                evercam = new Evercam(cred[0], cred[1]);
            List<Snapshot> snaps = new List<Snapshot>();
            string[] camids = snapmail.Cameras.Split(',');
            List<Camera> cams = evercam.GetCameras(null, snapmail.UserId, true);
            foreach (Camera c in cams)
            {
                var results = Array.FindAll(camids, s => s.Equals(c.ID));
                if (results.Length > 0 && c.Thumbnail != null)
                {
                    Snapshot snap = new Snapshot() { Data = c.Thumbnail };
                    snaps.Add(snap);
                }
            }
            return snaps;
        }

        [Route("api/snapmails/users/{id}")]
        [HttpGet]
        public List<SnapmailInfoModel> GetAll(string id)
        {
            return SnapmailModel.ToInfoModel(SnapmailDao.GetAll(id));
        }

        [Route("api/snapmails")]
        [HttpPost]
        public SnapmailInfoModel Post(SnapmailModel data)
        {
            SnapmailRowData rowdata = SnapmailModel.ToRowData(data);
            return SnapmailModel.ToInfoModel(SnapmailDao.Insert(rowdata));
        }

        [Route("api/snapmails/{key}")]
        [HttpPut]
        public SnapmailInfoModel Put(string key, SnapmailModel data)
        {
            return SnapmailModel.ToInfoModel(SnapmailDao.Update(key, SnapmailModel.ToRowData(key, data)));
        }

        [Route("api/snapmails/{key}/unsubscribe")]
        [HttpPost]
        public SnapmailInfoModel Unsubscribe(string key, [FromBody] MailAddress data)
        {
            return SnapmailModel.ToInfoModel(SnapmailDao.Unsubscribe(key, data.email));
        }

        [Route("api/snapmails/{key}")]
        [HttpDelete]
        public HttpResponseMessage Delete(string key)
        {
            return Common.Utility.GetResponseMessage(SnapmailDao.Delete(key, false).ToString(), HttpStatusCode.OK);
        }

        #region Utils

        /// <summary>
        /// Get Evercam token details from given token_endpoint
        /// </summary>
        /// <param name="data">See sample request data below</param>
        /// <returns>See sample response data below</returns>
        [Route("api/tokeninfo")]
        [HttpPost]
        public TokenUserModel GetTokenUser([FromBody]TokenUrlModel data)
        {
            try
            {
                data.token_endpoint = data.token_endpoint.Replace("dashboard", "api");
                string result;
                WebRequest r = WebRequest.Create(data.token_endpoint);
                r.Method = "GET";
                using (var response = (HttpWebResponse)r.GetResponse())
                {
                    result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    response.Close();
                }
                TokenUserModel token = JsonConvert.DeserializeObject<TokenUserModel>(result);
                
                ////// UPDATES USER ACCESS TOKEN AGAINST ALL SNAPMAILS ////////
                SnapmailDao.UpdateUserToken(token.userid, token.access_token);
                ///////////////////////////////////////////////////////////////
                return token;
            }
            catch (Exception x) { throw new HttpResponseException(HttpStatusCode.InternalServerError); }
        }

        #endregion
    }
}
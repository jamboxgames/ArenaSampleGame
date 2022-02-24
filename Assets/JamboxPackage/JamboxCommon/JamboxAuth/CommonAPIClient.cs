namespace Jambox.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Jambox.Server;
    using Jambox.Common.TinyJson;

    public class CommonAPIClient
    {
        public IHttpAdapter HttpAdapter;
        public int Timeout { get; set; }
        public Uri _baseUri;
        private string appVersion = UnityEngine.Application.version;
        private string SDKVersion = "2.0.1";
        private string _platform = UnityEngine.Application.platform.ToString();
        public CommonAPIClient ()
        {

        }
        public CommonAPIClient(Uri baseUri, IHttpAdapter httpAdapter, int timeout = 10)
        {
            _baseUri = baseUri;
            HttpAdapter = httpAdapter;
            Timeout = timeout;
        }

        public async Task<IApiSession> AuthenticateUser(String GameID, String UserID, String UserName, String AppSecret)
        {
            if (String.IsNullOrEmpty(appVersion))
            {
                appVersion = UnityEngine.Application.version;
            }
            var urlpath = "/v1/authenticate";
            var queryParams = "";

            String androidID = UnityEngine.SystemInfo.deviceUniqueIdentifier;
            String IOSAdvdID = "";// TourneyHelper.Instance.AdverTisingID;
            TimeZoneInfo infos = TimeZoneInfo.Local;
            UnityDebug.Debug.Log("Time Zone Info : DisplayName : " + infos.DisplayName + "  BaseUtcOffset : " + infos.BaseUtcOffset.ToString()
                            + " Id : " + infos.Id + "   StandardName : " + infos.StandardName);
            String country = infos.StandardName;
            String timezone = infos.BaseUtcOffset.ToString();
            String deviceName = UnityEngine.SystemInfo.deviceName;
            String DeviceManufacturer = UnityEngine.SystemInfo.deviceModel;

            UnityDebug.Debug.Log("AppVersion : " + appVersion + "  AndroidID : " + androidID + "  IOSAdvdID : " + IOSAdvdID + " Timezone : "
                + timezone + " DeviceName : " + deviceName + " DeviceManufacturer : " + DeviceManufacturer + " Country : " + country);

            var uri = new UriBuilder(_baseUri)
            {
                Path = urlpath,
                Query = queryParams
            }.Uri;
            var method = "POST";
            var headers = new Dictionary<string, string>();
            var header = string.Concat("Bearer ", UserID);
            headers.Add("Authorization", header);
            headers.Add("version", UnityEngine.Application.version);
            headers.Add("platform", _platform);

            //login track details
            Dictionary<String, String> DataLogin = new Dictionary<String, String>();
            DataLogin.Add("platform", _platform);
            DataLogin.Add("version", appVersion);
            DataLogin.Add("country", country);
            DataLogin.Add("android_id", androidID);
            DataLogin.Add("advd_id", IOSAdvdID);
            DataLogin.Add("timezone", timezone);
            DataLogin.Add("device_name", deviceName);
            DataLogin.Add("device_manufacturer", DeviceManufacturer);
            DataLogin.Add("SDK_Version", SDKVersion);
            var jsonLoginData = DataLogin.ToJson();

            //Auth details
            Dictionary<String, String> DataNew2 = new Dictionary<String, String>();
            DataNew2.Add("gameid", GameID);
            DataNew2.Add("gameuserid", UserID);
            DataNew2.Add("username", UserName);
            DataNew2.Add("appsecret", AppSecret);
            DataNew2.Add("logindata", jsonLoginData);

            byte[] content = null;
            var jsonNew = DataNew2.ToJson();
            content = Encoding.UTF8.GetBytes(jsonNew);
            var contents = await HttpAdapter.SendAsync(method, uri, headers, content, Timeout);
            UnityDebug.Debug.Log("Contents From server in authenticate : " + contents);
            return contents.FromJson<ApiSession>();
        }

        public async Task<IAPIUpdateUserData> updateUserDetailsOnServer(string authToken, string Name, int avatarId, string avatarGroup)
        {
            var urlpath = "/v1/updateuserinfo";
            var queryParams = "";
            var uri = new UriBuilder(_baseUri)
            {
                Path = urlpath,
                Query = queryParams
            }.Uri;

            var method = "POST";
            var headers = new Dictionary<string, string>();
            var header = string.Concat("Bearer ", authToken);
            headers.Add("Authorization", header);
            headers.Add("userid", authToken);
            headers.Add("version", appVersion);
            headers.Add("platform", _platform);

            Dictionary<String, object> DataNew2 = new Dictionary<String, object>();
            DataNew2.Add("username", Name);
            DataNew2.Add("avatar_group_id", avatarGroup);
            DataNew2.Add("avatar_index_id", avatarId);

            byte[] content = null;
            var jsonNew = DataNew2.ToJson();
            content = Encoding.UTF8.GetBytes(jsonNew);
            var contents = await HttpAdapter.SendAsync(method, uri, headers, content, Timeout);
            return contents.FromJson<APIUpdateUserData>();
        }
    }
}

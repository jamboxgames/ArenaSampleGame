namespace JBX.Leaderboard.Server
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Jambox.Common.TinyJson;
    using JBX.Leaderboard.Data;
    using Jambox.Server;

    internal class JBXLeaderboardAPIClient
    {
        public readonly IHttpAdapter HttpAdapter;
        public int Timeout { get; set; }
        private readonly Uri _baseUri;
        private string appVersion = UnityEngine.Application.version;
        private string SDKVersion = "1.9";
        private string _platform = UnityEngine.Application.platform.ToString();

        public JBXLeaderboardAPIClient(Uri baseUri, IHttpAdapter httpAdapter, int timeout = 10)
        {
            _baseUri = baseUri;
            HttpAdapter = httpAdapter;
            Timeout = timeout;
        }

        public async Task<ILeaderboardList> LeaderboardList(String authToken)
        {
            var urlpath = "/v1/leaderboards/list";
            var queryParams = "";
            var uri = new UriBuilder(_baseUri)
            {
                Path = urlpath,
                Query = queryParams
            }.Uri;

            var method = "GET";
            var headers = new Dictionary<string, string>();
            var header = string.Concat("Bearer ", authToken);
            headers.Add("Authorization", header);
            headers.Add("userid", authToken);
            headers.Add("version", appVersion);
            headers.Add("platform", _platform);
            byte[] content = null;
            var contents = await HttpAdapter.SendAsync(method, uri, headers, content, Timeout);
            return contents.FromJson<ApiLeaderboardList>();
        }

        public async Task<IApiLeaderRecordList> LBRecord(String authToken, string LBId, String PartitionKey)
        {
            var urlpath = "/v1/leaderboards/members";
            var queryParams = "lb_id=" + LBId + "&partition_key=" + PartitionKey;
            var uri = new UriBuilder(_baseUri)
            {
                Path = urlpath,
                Query = queryParams
            }.Uri;

            //var uri = new UriBuilder(string.Format("{0}?{1}", _baseUri.Concat(urlpath), queryParamsNew)).Uri;
            var method = "GET";
            var headers = new Dictionary<string, string>();
            var header = string.Concat("Bearer ", authToken);
            headers.Add("Authorization", header);
            headers.Add("userid", authToken);
            headers.Add("version", appVersion);
            headers.Add("platform", _platform);
            byte[] content = null;
            var contents = await HttpAdapter.SendAsync(method, uri, headers, content, Timeout);
            return contents.FromJson<ApiLeaderRecordList>();
        }

        public async Task<IApiLeaderAroundMe> LBRecordAround(String authToken, string LBId, String PartitionKey)
        {
            var urlpath = "/v1/leaderboards/aroundowner";
            var queryParams = "lb_id=" + LBId + "&partition_key=" + PartitionKey;
            var uri = new UriBuilder(_baseUri)
            {
                Path = urlpath,
                Query = queryParams
            }.Uri;

            //var uri = new UriBuilder(string.Format("{0}?{1}", _baseUri.Concat(urlpath), queryParamsNew)).Uri;
            var method = "GET";
            var headers = new Dictionary<string, string>();
            var header = string.Concat("Bearer ", authToken);
            headers.Add("Authorization", header);
            headers.Add("userid", authToken);
            headers.Add("version", appVersion);
            headers.Add("platform", _platform);
            byte[] content = null;
            var contents = await HttpAdapter.SendAsync(method, uri, headers, content, Timeout);
            return contents.FromJson<ApiLeaderAroundMe>();
        }

        public async Task<ISubmitScore> SubmitLBRecord(string authToken, String LBID, String PartitionKey, long score, string DisplayScore)
        {
            UnityDebug.Debug.Log("appVersion :  " + appVersion);
            var urlpath = "/v1/leaderboards/score";
            //urlpath = urlpath.Replace("{lb_id}", Uri.EscapeDataString(LBID));
            //urlpath = urlpath.Replace("{part_key}", Uri.EscapeDataString(PartitionKey));
            var queryParams = "lb_id=" + LBID + "&partition_key=" + PartitionKey;
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
            DataNew2.Add("score", score);
            DataNew2.Add("display_score", DisplayScore);
            byte[] content = null;
            var jsonNew = DataNew2.ToJson();
            UnityDebug.Debug.Log("SENDING : " + jsonNew);
            content = Encoding.UTF8.GetBytes(jsonNew);
            var contents = await HttpAdapter.SendAsync(method, uri, headers, content, Timeout);
            UnityDebug.Debug.Log("GetUserProfileDet Data Returned ; " + contents.ToString());
            return contents.FromJson<ApiSubmitScore>();
        }

        public async Task<IApiLeaderRecordList> PreviousLeaderBoard(String authToken, String LBId)
        {
            var urlpath = "/v1/leaderboards/lastplayed";
            urlpath = urlpath.Replace("{lb_id}", Uri.EscapeDataString(LBId));
            var queryParams = "lb_id=" + LBId;
            var uri = new UriBuilder(_baseUri)
            {
                Path = urlpath,
                Query = queryParams
            }.Uri;

            var method = "GET";
            var headers = new Dictionary<string, string>();
            var header = string.Concat("Bearer ", authToken);
            headers.Add("Authorization", header);
            headers.Add("userid", authToken);
            headers.Add("version", appVersion);
            headers.Add("platform", _platform);
            byte[] content = null;
            var contents = await HttpAdapter.SendAsync(method, uri, headers, content, Timeout);
            return contents.FromJson<ApiLeaderRecordList>();
        }

        public async Task<IPendingReward> PendingLBReward(String authToken)
        {
            var urlpath = "/v1/leaderboards/unclaimedreward";
            var queryParams = "";
            var uri = new UriBuilder(_baseUri)
            {
                Path = urlpath,
                Query = queryParams
            }.Uri;
            //String Score = score.ToString();
            var method = "GET";
            var headers = new Dictionary<string, string>();
            var header = string.Concat("Bearer ", authToken);
            headers.Add("Authorization", header);
            headers.Add("userid", authToken);
            headers.Add("version", appVersion);
            headers.Add("platform", _platform);
            byte[] content = null;
            var contents = await HttpAdapter.SendAsync(method, uri, headers, content, Timeout);
            return contents.FromJson<ApiPendingReward>();
        }

        public async Task<IClaimReward> ClaimLBReward(String authToken, String LBId, bool ClaimAll)
        {
            var urlpath = "/v1/leaderboards/claimreward";
            //urlpath = urlpath.Replace("{lb_id}", Uri.EscapeDataString(LBId));
            var queryParams = "lb_id=" + LBId;
            var uri = new UriBuilder(_baseUri)
            {
                Path = urlpath,
                Query = queryParams
            }.Uri;
            //String Score = score.ToString();
            var method = "POST";
            var headers = new Dictionary<string, string>();
            var header = string.Concat("Bearer ", authToken);
            headers.Add("Authorization", header);
            headers.Add("userid", authToken);
            headers.Add("version", appVersion);
            headers.Add("platform", _platform);
            Dictionary<String, object> DataNew2 = new Dictionary<String, object>();
            DataNew2.Add("lb_id", LBId);
            if (ClaimAll)
                DataNew2.Add("claim_type", "all");
            else
                DataNew2.Add("claim_type", "one");

            byte[] content = null;
            var jsonNew = DataNew2.ToJson();
            UnityDebug.Debug.Log("SENDING : " + jsonNew);
            content = Encoding.UTF8.GetBytes(jsonNew);
            var contents = await HttpAdapter.SendAsync(method, uri, headers, content, Timeout, true);
            return contents.FromJson<ApiClaimReward>();
        }
    }
}
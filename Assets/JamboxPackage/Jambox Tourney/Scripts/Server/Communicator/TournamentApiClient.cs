namespace Jambox.Tourney.Server
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Jambox.Common.TinyJson;
    using Jambox.Tourney.Data;
    using Jambox.Tourney.Connector;
    using UnityEngine;
    using Jambox.Server;
    using Jambox.Common;

    /// <summary>
    /// The low level client for the Jambox API.
    /// </summary>
    internal class TournamentApiClient
    {
        private readonly IHttpAdapter HttpAdapter;
        private int Timeout { get; set; }
        private readonly Uri _baseUri;
        private string appVersion = Application.version;
        private string SDKVersion = ArenaSDKVersion.VersionString;
        private string _platform = UnityEngine.Application.platform.ToString();
        private string _userLevel = "";

        public TournamentApiClient(Uri baseUri, IHttpAdapter httpAdapter, int timeout = 10)
        {
            _baseUri = baseUri;
            HttpAdapter = httpAdapter;
            Timeout = timeout;
        }

        /// <summary>
        /// A healthcheck which load balancers can use to check the service.
        /// </summary>
        public async Task HealthcheckAsync(
            string bearerToken)
        {
            var urlpath = "/healthcheck";
            var queryParams = "";
            var uri = new UriBuilder(_baseUri)
            {
                Path = urlpath,
                Query = queryParams
            }.Uri;
            var method = "GET";
            var headers = new Dictionary<string, string>();
            var header = string.Concat("Bearer ", bearerToken);
            headers.Add("Authorization", header);
            byte[] content = null;
            await HttpAdapter.SendAsync(method, uri, headers, content, Timeout);
        }

        /// <summary>
        /// Fetch the current user's account.
        /// </summary>
        public async Task<IApiAccount> GetAccountAsync(
            string bearerToken)
        {
            var urlpath = "/v2/account";
            var queryParams = "";
            var uri = new UriBuilder(_baseUri)
            {
                Path = urlpath,
                Query = queryParams
            }.Uri;

            var method = "GET";
            var headers = new Dictionary<string, string>();
            var header = string.Concat("Bearer ", bearerToken);
            headers.Add("Authorization", header);
            byte[] content = null;
            var contents = await HttpAdapter.SendAsync(method, uri, headers, content, Timeout);
            return contents.FromJson<ApiAccount>();
        }

        /// <summary>
        /// Refresh a user's session using a refresh token retrieved from a previous authentication request.
        /// </summary>
        public async Task<IApiSession> SessionRefreshAsync( string basicAuthUsername, string basicAuthPassword, ApiSessionRefreshRequest body)
        {
            if (body == null)
            {
                throw new ArgumentException("'body' is required but was null.");
            }
            var urlpath = "/v1/account/session/refresh";
            var queryParams = "";
            var uri = new UriBuilder(_baseUri)
            {
                Path = urlpath,
                Query = queryParams
            }.Uri;
            var method = "POST";
            var headers = new Dictionary<string, string>();
            var credentials = Encoding.UTF8.GetBytes(basicAuthUsername + ":" + basicAuthPassword);
            var header = string.Concat("Basic ", Convert.ToBase64String(credentials));
            headers.Add("Authorization", header);

            byte[] content = null;
            var jsonBody = body.ToJson();
            content = Encoding.UTF8.GetBytes(jsonBody);
            var contents = await HttpAdapter.SendAsync(method, uri, headers, content, Timeout);
            return contents.FromJson<ApiSession>();
        }

        /// <summary>
        /// Log out a session, invalidate a refresh token, or log out all sessions/refresh tokens for a user.
        /// </summary>

        public async Task SessionLogoutAsync(string bearerToken, ApiSessionLogoutRequest body)
        {
            if (body == null)
            {
                throw new ArgumentException("'body' is required but was null.");
            }

            var urlpath = "/v1/session/logout";
            var queryParams = "";
            var uri = new UriBuilder(_baseUri)
            {
                Path = urlpath,
                Query = queryParams
            }.Uri;

            var method = "POST";
            var headers = new Dictionary<string, string>();
            var header = string.Concat("Bearer ", bearerToken);
            headers.Add("Authorization", header);
            byte[] content = null;
            var jsonBody = body.ToJson();
            content = Encoding.UTF8.GetBytes(jsonBody);
            await HttpAdapter.SendAsync(method, uri, headers, content, Timeout);
        }

        /// <summary>
        /// This method will fetch All the available tourneyList.
        /// </summary>
        /// <param name="authtoken">This will be the unique auth token sent to server for different games</param>
        /// <returns></returns>
        public async Task<IApiTourneyList> getTourneyDetail(string authtoken, string UserLevel)
        {
            var urlpath = "/v1/gettournamentdetails";
            var queryParams = "";

            var uri = new UriBuilder(_baseUri)
            {
                Path = urlpath,
                Query = queryParams
            }.Uri;
            _userLevel = UserLevel;
            var method = "GET";
            var headers = new Dictionary<string, string>();
            var header = string.Concat("Bearer ", authtoken);
            headers.Add("Authorization", header);
            headers.Add("userid", authtoken);
            headers.Add("version", appVersion);
            headers.Add("platform", _platform);

            //Dictionary<String, String> DataNew2 = new Dictionary<String, String>();
            //DataNew2.Add("user_level", _userLevel);
            byte[] content = null;
            //var jsonBody = DataNew2.ToJson();
            //content = Encoding.UTF8.GetBytes(jsonBody);
            var contents = await HttpAdapter.SendAsync(method, uri, headers, content, Timeout);
            return contents.FromJson<ApiTourneyList>();
        }

        /// <summary>
        /// This method will call the server to join in any tournament.
        /// </summary>
        /// <param name="authtoken">This will be the unique auth token sent by server for different games and users</param>
        /// <param name="tourneyID"> the Id of the tournament user wants to join</param>
        /// <returns></returns>
        public async Task<IAPIJoinTourney> JoinTourney(string authtoken, string tourneyID)
        {
            var urlpath = "/v1/jointournament/{tourID}";
            urlpath = urlpath.Replace("{tourID}", Uri.EscapeDataString(tourneyID));
            var queryParams = "";
            var uri = new UriBuilder(_baseUri)
            {
                Path = urlpath,
                Query = queryParams
            }.Uri;

            var method = "POST";
            var headers = new Dictionary<string, string>();
            var header = string.Concat("Bearer ", authtoken);
            headers.Add("Authorization", header);
            headers.Add("userid", authtoken);
            headers.Add("version", appVersion);
            headers.Add("platform", _platform);
            Dictionary<String, String> DataNew2 = new Dictionary<String, String>();
            DataNew2.Add("user_level", _userLevel);
            byte[] content = null;
            var jsonBody = DataNew2.ToJson();
            content = Encoding.UTF8.GetBytes(jsonBody);
            var contents = await HttpAdapter.SendAsync(method, uri, headers, content, Timeout);
            return contents.FromJson<APIJoinTourney>();
        }

        /// <summary>
        /// This method will call the server to join in any tournament.
        /// </summary>
        /// <param name="authtoken">This will be the unique auth token sent by server for different games and users</param>
        /// <param name="duelID"> the Id of the tournament user wants to join</param>
        /// <returns></returns>
        public async Task<APIJoinDuel> JoinDuel(string authtoken, string duelID)
        {
            var urlpath = "/v1/joinduel/{duelID}";
            urlpath = urlpath.Replace("{duelID}", Uri.EscapeDataString(duelID));
            var queryParams = "";
            var uri = new UriBuilder(_baseUri)
            {
                Path = urlpath,
                Query = queryParams
            }.Uri;

            var method = "POST";
            var headers = new Dictionary<string, string>();
            var header = string.Concat("Bearer ", authtoken);
            headers.Add("Authorization", header);
            headers.Add("userid", authtoken);
            headers.Add("version", appVersion);
            headers.Add("platform", _platform);
            Dictionary<String, String> DataNew2 = new Dictionary<String, String>();
            DataNew2.Add("user_level", _userLevel);
            byte[] content = null;
            var jsonBody = DataNew2.ToJson();
            content = Encoding.UTF8.GetBytes(jsonBody);
            var contents = await HttpAdapter.SendAsync(method, uri, headers, content, Timeout);
            UnityDebug.Debug.Log("Content from JoinDuel -> " + contents);
            APIJoinDuel jsonData = contents.FromJson<APIJoinDuel>();
            return jsonData;
        }

        /// <summary>
        /// This method will be called when user attempts any tourney to play the game
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="tourneyId"></param>
        /// <param name="attemptType"></param>
        /// <returns></returns>
        public async Task<IApiPlayTourney> PlayTourney(string authToken, string tourneyId, string buy_type)
        {
            var urlpath = "/v1/playtournament/{tourID}";
            urlpath = urlpath.Replace("{tourID}", Uri.EscapeDataString(tourneyId));
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
            Dictionary<String, String> DataNew2 = new Dictionary<String, String>();
            DataNew2.Add("buy_type", buy_type);
            byte[] content = null;
            var jsonBody = DataNew2.ToJson();
            content = Encoding.UTF8.GetBytes(jsonBody);
            var contents = await HttpAdapter.SendAsync(method, uri, headers, content, Timeout);
            return contents.FromJson<ApiPlayTourney>();
        }

        /// <summary>
        /// When User Completes the game thois method will responsible to submit his score
        /// </summary>
        /// <param name="authToken">The unique authToken sent by server</param>
        /// <param name="score">The score achieved by user in current game completed</param>
        /// <returns></returns>
        public async Task<IApiSubmitScore> submitScore(string authToken, string LbID, long score, string displayScore, ReplayData replayData = null)
        {
            var urlpath = "/v1/submitscore/{LbId}";
            urlpath = urlpath.Replace("{LbId}", Uri.EscapeDataString(LbID));
            var queryParams = "";
            var uri = new UriBuilder(_baseUri)
            {
                Path = urlpath,
                Query = queryParams
            }.Uri;
            String Score = score.ToString();
            var method = "POST";
            var headers = new Dictionary<string, string>();
            var header = string.Concat("Bearer ", authToken);
            headers.Add("Authorization", header);
            headers.Add("userid", authToken);
            headers.Add("version", appVersion);
            headers.Add("platform", _platform);
            Dictionary<String, object> DataNew2 = new Dictionary<String, object>();
            DataNew2.Add("score", Score);
            DataNew2.Add("displayscore", displayScore);
            DataNew2.Add("replay_data", replayData);
            byte[] content = null;
            var jsonNew = DataNew2.ToJson();
            content = Encoding.UTF8.GetBytes(jsonNew);
            var contents = await HttpAdapter.SendAsync(method, uri, headers, content, Timeout);
            return contents.FromJson<ApiSubmitScore>();
        }

        /// <summary>
        /// When User Completes the game thois method will responsible to submit his score
        /// </summary>
        /// <param name="authToken">The unique authToken sent by server</param>
        /// <param name="score">The score achieved by user in current game completed</param>
        /// <param name="matchID">The score achieved by user in current game completed</param>
        /// <returns></returns>
        public async Task<IApiSubmitDuelScore> submitDuelScore(string authToken, string matchID, long score, string displayScore, ReplayData replayData = null)
        {
            var urlpath = "/v1/submitduelscore/{matchID}";
            urlpath = urlpath.Replace("{matchID}", Uri.EscapeDataString(matchID));
            var queryParams = "";
            var uri = new UriBuilder(_baseUri)
            {
                Path = urlpath,
                Query = queryParams
            }.Uri;
            String Score = score.ToString();
            var method = "POST";
            var headers = new Dictionary<string, string>();
            var header = string.Concat("Bearer ", authToken);
            headers.Add("Authorization", header);
            headers.Add("userid", authToken);
            headers.Add("version", appVersion);
            headers.Add("platform", _platform);
            UnityDebug.Debug.Log("userid : " + authToken);
            Dictionary<String, object> DataNew2 = new Dictionary<String, object>();
            DataNew2.Add("score", Score);
            DataNew2.Add("displayscore", displayScore);
            DataNew2.Add("replay_data", replayData);

            byte[] content = null;
            var jsonNew = DataNew2.ToJson();
            content = Encoding.UTF8.GetBytes(jsonNew);
            var contents = await HttpAdapter.SendAsync(method, uri, headers, content, Timeout);
            try
            {
                return contents.FromJson<ApiSubmitDuelScore>();
            }
            catch (Exception e)
            {
                UnityDebug.Debug.Log("Inside Parsing Duel result data : " + e.ToString());
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="lbId"></param>
        /// <returns></returns>
        public async Task<IApiLeaderRecordList> getLeaderBoard(string authToken, string lbId)
        {
            var urlpath = "/v1/leaderboard/{LbId}";
            urlpath = urlpath.Replace("{LbId}", Uri.EscapeDataString(lbId));
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
            headers.Add("LeaderBoardId", lbId);
            headers.Add("version", appVersion);
            headers.Add("platform", _platform);
            byte[] content = null;
            var contents = await HttpAdapter.SendAsync(method, uri, headers, content, Timeout);
            return contents.FromJson<ApiLeaderRecordList>();
        }

        public async Task<IAPICompTourneyList> getCompletedTourneyData(string authToken, string Cate)
        {
            var urlpath = "/v1/completedtournaments";
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
            headers.Add("category", Cate);
            headers.Add("version", appVersion);
            headers.Add("platform", _platform);
            byte[] content = null;
            var contents = await HttpAdapter.SendAsync(method, uri, headers, content, Timeout);
            try
            {
                return contents.FromJson<APICompTourneyList>();
            }
            catch (Exception ex)
            {
                UnityDebug.Debug.LogError("getCompletedTourneyData data parsing exception :  " + ex.ToString());
                return null;
            }
        }

        public async Task<IAPIClaimData> getClaimData(string authToken, string lbId)
        {
            var urlpath = "/v1/claimreward/{LbId}";
            urlpath = urlpath.Replace("{LbId}", Uri.EscapeDataString(lbId));
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
            headers.Add("LeaderBoardId", lbId);
            headers.Add("version", appVersion);
            headers.Add("platform", _platform);

            Dictionary<String, String> DataNew2 = new Dictionary<String, String>();
            byte[] content = null;
            var contents = await HttpAdapter.SendAsync(method, uri, headers, content, Timeout);
            return contents.FromJson<APIClaimData>();
        }

        public async Task<APICreateFriendly> createFriendly(string authToken, string tourneyName, int attempts, int duration)
        {
            var urlpath = "/v1/createtournament";
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
            DataNew2.Add("name", tourneyName);
            DataNew2.Add("duration", duration);
            DataNew2.Add("attempts", attempts);

            byte[] content = null;
            var jsonNew = DataNew2.ToJson();
            content = Encoding.UTF8.GetBytes(jsonNew);
            var contents = await HttpAdapter.SendAsync(method, uri, headers, content, Timeout);
            return contents.FromJson<APICreateFriendly>();
        }

        public async Task<APIJoinFriendly> joinFriendly(string authToken, string code)
        {
            var urlpath = "/v1/joinprivatetournament";
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
            Dictionary<String, String> DataNew2 = new Dictionary<String, String>();
            DataNew2.Add("code", code);
            byte[] content = null;
            var jsonNew = DataNew2.ToJson();
            content = Encoding.UTF8.GetBytes(jsonNew);
            var contents = await HttpAdapter.SendAsync(method, uri, headers, content, Timeout);
            return contents.FromJson<APIJoinFriendly>();
        }

        public async Task<IAPIFriendlyTourneyList> getFriendlyDetails(string authtoken)
        {
            var urlpath = "/v1/gettournamentdetails";
            var queryParams = "";
            var uri = new UriBuilder(_baseUri)
            {
                Path = urlpath,
                Query = queryParams
            }.Uri;

            var method = "GET";
            var headers = new Dictionary<string, string>();
            var header = string.Concat("Bearer ", authtoken);
            headers.Add("Authorization", header);
            headers.Add("userid", authtoken);
            headers.Add("version", appVersion);
            headers.Add("platform", _platform);
            byte[] content = null;
            var contents = await HttpAdapter.SendAsync(method, uri, headers, content, Timeout);
            return contents.FromJson<IAPIFriendlyTourneyList>();
        }

        /// <summary>
        /// This method will be called when user attempts any tourney to play the game
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="tourneyId"></param>
        /// <param name="attemptType"></param>
        /// <returns></returns>
        public async Task<IApiPlayFriendlyTourney> PlayFriendlyTourney(string authToken, string tourneyId)
        {
            var urlpath = "/v1/playtournament/{tourID}";
            urlpath = urlpath.Replace("{tourID}", Uri.EscapeDataString(tourneyId));
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
            byte[] content = null;
            var contents = await HttpAdapter.SendAsync(method, uri, headers, content, Timeout);
            return contents.FromJson<ApiPlayFriendlyTourney>();
        }

        public async Task<IAPICurrencyList> getCurrencyData(string authToken)
        {
            var urlpath = "/v1/getcurrencylist";
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
            return contents.FromJson<APICurrencyList>();
        }

        public async Task<IAPIUnclaimedRewards> UnclaimedRewards(string authToken)
        {
            var urlpath = "/v1/unclaimedrewards";
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
            return contents.FromJson<APIUnclaimedRewards>();
        }
    }
}

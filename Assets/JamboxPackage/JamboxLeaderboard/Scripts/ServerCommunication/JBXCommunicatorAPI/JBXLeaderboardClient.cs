namespace JBX.Leaderboard.Server
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;
    using UnityEngine;
    using JBX.Leaderboard.Data;
    using Jambox.Server;
    using Jambox.Common;

    public class JBXLeaderboardClient
    {
        private readonly JBXLeaderboardAPIClient _apiClient;
        public event Action OnLeaderBoardInit;
        public JBXLeaderboardClient(CommonClient baseClient)
        {
            _apiClient = new JBXLeaderboardAPIClient(new UriBuilder(baseClient.Scheme, baseClient.Host).Uri, HttpRequestAdapter.WithGzip(), baseClient.DefaultTimeout); 
        }

        public async Task<ILeaderboardList> GetLeaderboardList(string authToken)
        {
            Debug.Log("GetLeaderboardList Hit >>>>>>");
            return await _apiClient.LeaderboardList(authToken);
        }

        public async Task<IApiLeaderRecordList> GetLeaderBoardRecord(string authToken, string LBId, String PartitionKey)
        {
            Debug.Log("GetLeaderBoardRecord Hit >>>>>>");
            return await _apiClient.LBRecord(authToken, LBId, PartitionKey);
        }

        public async Task<IApiLeaderAroundMe> GetLeaderBoardAroundMe(string authToken, string LBId, String PartitionKey)
        {
            Debug.Log("GetLeaderBoardRecord Hit >>>>>>");
            return await _apiClient.LBRecordAround(authToken, LBId, PartitionKey);
        }

        public async Task<ISubmitScore> SubmitLBRecord(string authToken, String LBID, String PartitionKey, long score, string DisplayScore)
        {
            Debug.Log("GetUserProfile Hit >>>>>>");
            return await _apiClient.SubmitLBRecord(authToken, LBID, PartitionKey, score, DisplayScore);
        }

        public async Task<IApiLeaderRecordList> GetPreviousLeaderBoard(string authToken, String LBID)
        {
            Debug.Log("GetUserProfile Hit >>>>>>");
            return await _apiClient.PreviousLeaderBoard(authToken, LBID);
        }

        public async Task<IPendingReward> GetPendingLBReward(string authToken)
        {
            Debug.Log("GetUserProfile Hit >>>>>>");
            return await _apiClient.PendingLBReward(authToken);
        }

        public async Task<IClaimReward> ClaimLBReward(string authToken, String LBID, bool ClaimAll)
        {
            Debug.Log("GetUserProfile Hit >>>>>>");
            return await _apiClient.ClaimLBReward(authToken, LBID, ClaimAll);
        }
    }
}
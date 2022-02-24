// Copyright 2021 The Jambox Authors
namespace Jambox.Tourney.Server
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;
    using Jambox.Tourney.Data;
    using Jambox.Tourney.Connector;
    using Jambox.Server;
    using Jambox.Common;

    /// <inheritdoc cref="TournamentIClient"/>
    public class TournamentClient
    {
        
        internal readonly TournamentApiClient _apiClient;
        
        public TournamentClient(CommonClient baseClient)
        {            
            _apiClient = new TournamentApiClient(new UriBuilder(baseClient.Scheme, baseClient.Host).Uri, HttpRequestAdapter.WithGzip(), baseClient.DefaultTimeout);        
        }        

        /// <inheritdoc cref="GetTourneyList"/>
        public async Task<IApiTourneyList> GetTourneyList(string authToken)
        {
            return await _apiClient.getTourneyDetail(authToken);
        }

        /// <inheritdoc cref="JoinTournament"/>
        public async Task<IAPIJoinTourney> JoinTournament(string authToken, string TourneyId)
        {
            return await _apiClient.JoinTourney(authToken, TourneyId);
        }

        /// <inheritdoc cref="JoinDuel"/>
        public async Task<IAPIJoinDuel> JoinDuel(string authToken, string DuelId)
        {
            return await _apiClient.JoinDuel(authToken, DuelId);
        }

        ///<inheritdoc cref="PlayTourney">
        public async Task<IApiPlayTourney> PlayTourney(string authToken, string tourneyId, string attemptType)
        {
            return await _apiClient.PlayTourney(authToken, tourneyId, attemptType);
        }

        /// <inheritdoc cref="SubmitScore"/>
        public async Task<IApiSubmitScore> SubmitScore(string authToken, string tourneyID, long Score, ReplayData replayData = null)
        {
            return await _apiClient.submitScore(authToken, tourneyID, Score, replayData);
        }

        /// <inheritdoc cref="SubmitDuelScore"/>
        public async Task<IApiSubmitDuelScore> SubmitDuelScore(string authToken, string matchID, long Score, ReplayData replayData = null)
        {
            return await _apiClient.submitDuelScore(authToken, matchID, Score, replayData);
        }

        /// <inheritdoc cref="getLeaderBoard"/>
        public async Task<IApiLeaderRecordList> getLeaderBoard(string authToken, string lbId)
        {
            return await _apiClient.getLeaderBoard(authToken, lbId);
        }

        /// <inheritdoc cref="getCompletedTourneyData"/>
        public async Task<IAPICompTourneyList> getCompletedTourneyData(String authToken, string Cate)
        {
            return await _apiClient.getCompletedTourneyData(authToken, Cate);
        }

        public async Task<IAPIClaimData> GetClaimData(string authToken, string lbId)
        {
            return await _apiClient.getClaimData(authToken, lbId);
        }

        public async Task<IAPICreateFriendly> CreateFriendly(string authToken, string tourneyName, int attempts, int duration)
        {
            return await _apiClient.createFriendly(authToken, tourneyName, attempts, duration);
        }

        public async Task<IAPIJoinFriendly> JoinFriendly(string authToken, string code)
        {
            return await _apiClient.joinFriendly(authToken, code);
        }

        public async Task<IAPIFriendlyTourneyList> GetFriendlyDetails(string authToken)
        {
            return await _apiClient.getFriendlyDetails(authToken);
        }

        ///<inheritdoc cref="PlayFriendlyTourney">
        public async Task<IApiPlayFriendlyTourney> PlayFriendlyTourney(string authToken, string tourneyId)
        {
            return await _apiClient.PlayFriendlyTourney(authToken, tourneyId);
        }

        public async Task<IAPICurrencyList> GetCurrencyData(string authToken)
        {
            return await _apiClient.getCurrencyData(authToken);
        }

        public async Task<IAPIUnclaimedRewards> UnclaimedRewards(string authToken)
        {
            return await _apiClient.UnclaimedRewards(authToken);
        }
    }
}

// Copyright 2021 The Jambox Authors
namespace Jambox.Tourney.Server
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Jambox.Common;
    using Jambox.Tourney.Connector;
    using Jambox.Tourney.Data;

    /// <summary>
    /// A client for the API in Jambox server.
    /// </summary>
    public interface TournamentIClient : CommonIClient
    {
        ///// <summary>
        ///// True if the session should be refreshed with an active refresh token.
        ///// </summary>
        //bool AutoRefreshSession { get; }

        ///// <summary>
        ///// The host address of the server. Defaults to "127.0.0.1".
        ///// </summary>
        //string Host { get; }

        ///// <summary>
        ///// The port number of the server. Defaults to 7350.
        ///// </summary>
        //int Port { get; }

        ///// <summary>
        ///// The protocol scheme used to connect with the server. Must be either "http" or "https".
        ///// </summary>
        //string Scheme { get; }

        ///// <summary>
        ///// The key used to authenticate with the server without a session. Defaults to "defaultkey".
        ///// </summary>
        //string ServerKey { get; }

        ///// <summary>
        ///// Set the timeout in seconds on requests sent to the server.
        ///// </summary>
        //int Timeout { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="GameID"></param>
        ///// <param name="UserID"></param>
        ///// <param name="UserName"></param>
        ///// <param name="AppSecret"></param>
        ///// <returns></returns>
        //Task<IApiSession> AuthenticateUser(String GameID, String UserID, String UserName, String AppSecret);

        /// <summary>
        /// Get the total available tournamnet list from server
        /// </summary>
        /// <param name="authtoken"> the unique auth token will be sent to server. </param>
        /// <returns>A task which resolves to the list of tournament objects</returns>
        Task<IApiTourneyList> GetTourneyList(String authtoken);

        /// <summary>
        /// To Join the tournament hit this method
        /// </summary>
        /// <param name="authToken"> The unique token sent by server to each client</param>
        /// <param name="TourneyId">The Id of tournamnet Which user wants to join</param>
        /// <returns></returns>
        Task<IAPIJoinTourney> JoinTournament(String authToken, String TourneyId);


        /// <summary>
        /// To Join the tournament hit this method
        /// </summary>
        /// <param name="authToken"> The unique token sent by server to each client</param>
        /// <param name="DuelId">The Id of tournamnet Which user wants to join</param>
        /// <returns></returns>
        Task<IAPIJoinDuel> JoinDuel(String authToken, String DuelId);

        /// <summary>
        /// When user wants to play the tournament this method will verify that can user play the tourney ?
        /// </summary>
        /// <param name="authToken"> The unique token sent by server to each client</param>
        /// <param name="tourneyId">The Id of the tournament uswe is going to play</param>
        /// <param name="attemptType">The type of attempt user is using to playe the game</param>
        /// <returns></returns>
        Task<IApiPlayTourney> PlayTourney(string authToken, string tourneyId, string attemptType);

        /// <summary>
        /// After Completing every match user will submit his score to update his position o LeaderBoard
        /// </summary>
        /// <param name="authToken">The unique token sent by server to each client</param>
        /// <param name="Score">The Current score user has made in this game</param>
        /// <returns></returns>
        Task<IApiSubmitScore> SubmitScore(String authToken, string tourneyId, long Score, ReplayData replayData = null);

        /// <summary>
        /// After Completing every match user will submit his score to update his position o LeaderBoard
        /// </summary>
        /// <param name="authToken">The unique token sent by server to each client</param>
        /// <param name="Score">The Current score user has made in this game</param>
        /// <param name="matchId">The Current score user has made in this game</param>
        /// <returns></returns>
        Task<IApiSubmitDuelScore> SubmitDuelScore(String authToken, string matchId, long Score, ReplayData replayData = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken">The unique token sent by server to each client</param>
        /// <param name="leaderBoardID"> The unique leaderboardId corresponding to each tournament and related to particuylar user also.</param>
        /// <returns></returns>
        Task<IApiLeaderRecordList> getLeaderBoard(string authToken, string leaderBoardID);


        Task<IAPICompTourneyList> getCompletedTourneyData(String authToken, string Cate);


        Task<IAPIClaimData> GetClaimData(string authToken, string LbId);

        Task<IAPICreateFriendly> CreateFriendly(string authToken, string tourneyName, int attempts, int duration);

        Task<IAPIJoinFriendly> JoinFriendly(string authToken, string code);

        Task<IAPIFriendlyTourneyList> GetFriendlyDetails(string authToken);

        Task<IApiPlayFriendlyTourney> PlayFriendlyTourney(string authToken, string tourneyId);

        Task<IAPICurrencyList> GetCurrencyData(string authToken);

        Task<IAPIUpdateUserData> UpdateUserDetails(string authToken, string name, int avatarId, string avatarGroup);

        Task<IAPIUnclaimedRewards> UnclaimedRewards(string authToken);

    }
}
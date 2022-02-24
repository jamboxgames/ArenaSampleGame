namespace Jambox.Tourney.Connector
{
    using System.Collections;
    using System.Collections.Generic;
    using Jambox.Server;
    using Jambox.Tourney.Data;
    using System.Threading.Tasks;
    using Jambox.Common.TinyJson;
    using UnityEngine.Assertions;
    using System;
    using Jambox.Tourney.UI;
    using UnityEngine;
    using Jambox.Tourney.Server;
    using Jambox.Common;
    using Jambox.Common.Utility;

    public class CommunicationController : MonoSingleton<CommunicationController>
    {
        private TournamentClient _client;

        public string getMyuserId()
        {
            if (JamboxController.Instance.CurrentSession == null)
                return null;
            return JamboxController.Instance.CurrentSession.MyID;
        }

        public string getAuthToken()
        {
            if (JamboxController.Instance.CurrentSession == null)
                return string.Empty;
            return JamboxController.Instance.CurrentSession.Token;
        }

        //public bool isGameIdSet()
        //{
        //    if (String.IsNullOrEmpty(_gameID) || String.IsNullOrEmpty(_appSecret))
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        //public String getMyUserName()
        //{
        //    return _userName;
        //}

        public void Init(CommonClient baseClient)
        {            
                _client = new TournamentClient(baseClient);        
        }

        //public async Task StartSession(String UserName, String UserID)
        //{
        //    _userName = UserName;
        //    _userID = UserID;
        //    if (UserID == null)
        //    {
        //        _userID = UnityEngine.SystemInfo.deviceUniqueIdentifier;
        //    }
        //    if (UnityEngine.Application.internetReachability == UnityEngine.NetworkReachability.NotReachable)
        //    {
        //        return;
        //    }
        //    if (String.IsNullOrEmpty(_gameID) || String.IsNullOrEmpty(_appSecret))
        //    {
        //        Debug.LogError("You are trying to start session without being Set up SDK Variables ");
        //    }
        //    UnityDebug.Debug.Log("StartSession : _userID : " + _userID);
        //    JamboxController.Instance.CurrentSession = await _client.AuthenticateUser(_gameID, _userID, UserName, _appSecret);
        //    UnityDebug.Debug.Log("Session created Token of CurrentSession is  :  " + CurrentSession.Token +
        //        "  And Id is : " + CurrentSession.MyID);
        //    UserDataContainer.Instance.userName = CurrentSession.Name;
        //    UserDataContainer.Instance.MyAvatarURL = CurrentSession.MyAvatar;
        //    Enum.TryParse(CurrentSession.AvatarType, out UserDataContainer.Instance.AvatarGroup);
        //}
        //public async Task CreateSession(Action sessionCreated = null)
        //{
        //    UnityDebug.Debug.Log("CreateSession Hit >>>>>");
        //    JamboxController.Instance.CurrentSession = await _client.AuthenticateUser(_gameID, _userID, _userName, _appSecret);
        //    UnityDebug.Debug.Log("Session created Token of CurrentSession is  :  " + JamboxController.Instance.CurrentSession.Token +
        //        "  And Id is : " + JamboxController.Instance.CurrentSession.MyID);
        //    UserDataContainer.Instance.userName = CurrentSession.Name;
        //    UserDataContainer.Instance.MyAvatarURL = CurrentSession.MyAvatar;
        //    UserDataContainer.Instance.MyAvatarURL = CurrentSession.MyAvatar;
        //    Enum.TryParse(JamboxController.Instance.CurrentSession.AvatarType, out UserDataContainer.Instance.AvatarGroup);
        //    if (sessionCreated != null)
        //        sessionCreated();
        //}
        //public bool ChechkForSession()
        //{
        //    if (JamboxController.Instance.CurrentSession != null && !String.IsNullOrEmpty(JamboxController.Instance.CurrentSession.Token))
        //        return true;

        //    return false;
        //}
        //public bool CheckForNetwork()
        //{
        //    if (UnityEngine.Application.internetReachability == UnityEngine.NetworkReachability.NotReachable)
        //    {
        //        ShowNoNetworkDialogue();
        //        return false;
        //    }
        //    return true;
        //}
        public void ShowNoNetworkDialogue()
        {
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            //metadata.Add("Header", "No Internet");
            metadata.Add("Header", "Server Error");

            string errorString = "You don't have active internet connection. Please check your internet connection and try again.";
            metadata.Add("DialogBody", errorString);
            //metadata.Add("Btn1Name", "Retry");
            //metadata.Add("Btn2Name", "Home");
            metadata.Add("Btn1Name", "Home");
            UIPanelController.Instance.ShowPanel(Panels.DialoguePanel, Panels.None, metadata);
        }

        /// <summary>
        /// This method will be called from UI to get the tourney detail.
        /// </summary>
        /// <param name="authToken">The unique token which will be held for Communication.</param>
        /// <returns></returns>
        public async Task GetTourneydetail(string authToken, Action<IApiTourneyList> OnReceived)
        {
            if (!JamboxController.Instance.CheckForNetwork())
            {
                ShowNoNetworkDialogue();
                return;
            }

            if (!JamboxController.Instance.ChechkForSession())
            {
                await JamboxController.Instance.RefreshSession();
            }
            authToken = JamboxController.Instance.CurrentSession.Token;
            var result = await _client.GetTourneyList(authToken);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Tournaments);
            if (OnReceived != null)
                OnReceived(result);
        }

        /// <summary>
        /// The UI will call this method when user requests to join any particular Tourney.
        /// </summary>
        /// <param name="authToken">The unique token which will be held for Communication</param>
        /// <param name="tourneyId">The Unique Id of tournament held responsible for getting detail of any tournament.</param>
        /// <param name="OnReceived">The Action on completion of this particular task.</param>
        /// <returns></returns>
        public async Task JoinTourney(string authToken, String tourneyId, Action<IAPIJoinTourney> OnReceived)
        {
            if (!JamboxController.Instance.CheckForNetwork())
            {
                ShowNoNetworkDialogue();
                return;
            }

            if (!JamboxController.Instance.ChechkForSession())
            {
                await JamboxController.Instance.RefreshSession();
            }

            authToken = JamboxController.Instance.CurrentSession.Token;
            var result = await _client.JoinTournament(authToken, tourneyId);
            if (OnReceived != null)
                OnReceived(result);
        }

        /// <summary>
        /// The UI will call this method when user requests to join any particular Tourney.
        /// </summary>
        /// <param name="authToken">The unique token which will be held for Communication</param>
        /// <param name="duelId">The Unique Id of Duel held responsible for getting detail of any tournament.</param>
        /// <param name="OnReceived">The Action on completion of this particular task.</param>
        /// <returns></returns>
        public async Task JoinDuel(string authToken, String tourneyId, Action<IAPIJoinDuel> OnReceived)
        {
            if (!JamboxController.Instance.CheckForNetwork())
            {
                ShowNoNetworkDialogue();
                return;
            }

            if (!JamboxController.Instance.ChechkForSession())
            {
                await JamboxController.Instance.RefreshSession();
            }
            authToken = JamboxController.Instance.CurrentSession.Token;
            var result = await _client.JoinDuel(authToken, tourneyId);

            if (OnReceived != null)
                OnReceived(result);
        }

        /// <summary>
        /// The UI will call this method when user requests to play any particular Tourney.
        /// </summary>
        /// <param name="authToken">The unique token which will be held for Communication</param>
        /// <param name="tourneyId">The Unique Id of tournament held responsible for getting detail of any tournament</param>
        /// <param name="attemptType"></param>
        /// <param name="OnReceived">The Action on completion of this particular task</param>
        /// <returns></returns>
        public async Task PlayTourney(string authToken, string tourneyId, string attemptType, Action<IApiPlayTourney> OnReceived)
        {
            if (!JamboxController.Instance.CheckForNetwork())
            {
                ShowNoNetworkDialogue();
                return;
            }

            if (!JamboxController.Instance.ChechkForSession())
            {
                await JamboxController.Instance.RefreshSession();
            }
            authToken = JamboxController.Instance.CurrentSession.Token;
            var result = await _client.PlayTourney(authToken, tourneyId, attemptType);
            if (OnReceived != null)
                OnReceived(result);
        }

        /// <summary>
        /// The UI will call this method when they complete the game and submit their score.
        /// </summary>
        /// <param name="authToken">The unique token which will be held for Communication</param>
        /// <param name="score"></param>
        /// <param name="OnReceived">The Action on completion of this particular task</param>
        /// <returns></returns>
        public async Task SubmitScore(string authToken, string LbID, long score, Action<IApiSubmitScore> OnReceived, ReplayData replayData = null)
        {
            if (!JamboxController.Instance.CheckForNetwork())
            {
                ShowNoNetworkDialogue();
                return;
            }

            if (!JamboxController.Instance.ChechkForSession())
            {
                await JamboxController.Instance.RefreshSession();
            }
            authToken = JamboxController.Instance.CurrentSession.Token;
            var result = await _client.SubmitScore(authToken, LbID, score, replayData);
            if (OnReceived != null)
                OnReceived(result);
        }

        /// <summary>
        /// The UI will call this method when they complete the game and submit their score.
        /// </summary>
        /// <param name="authToken">The unique token which will be held for Communication</param>
        /// <param name="score"></param>
        /// <param name="OnReceived">The Action on completion of this particular task</param>
        /// <returns></returns>
        public async Task SubmitDuelScore(string authToken, string matchID, long score, Action<IApiSubmitDuelScore> OnReceived, ReplayData replayData = null)
        {
            if (!JamboxController.Instance.CheckForNetwork())
            {
                ShowNoNetworkDialogue();
                return;
            }

            if (!JamboxController.Instance.ChechkForSession())
            {
                await JamboxController.Instance.RefreshSession();
            }
            authToken = JamboxController.Instance.CurrentSession.Token;
            var result = await _client.SubmitDuelScore(authToken, matchID, score, replayData);
            if (OnReceived != null)
                OnReceived(result);
        }

        /// <summary>
        /// The UI will call this method when they complete the game and submit their score.
        /// </summary>
        /// <param name="authToken">The unique token which will be held for Communication</param>
        /// <param name="score"></param>
        /// <param name="OnReceived">The Action on completion of this particular task</param>
        /// <returns></returns>
        public async Task SubmitDuelScore(string authToken, string LbID, long score, Action<IApiSubmitScore> OnReceived, ReplayData replayData = null)
        {
            if (!JamboxController.Instance.CheckForNetwork())
            {
                ShowNoNetworkDialogue();
                return;
            }

            if (!JamboxController.Instance.ChechkForSession())
            {
                await JamboxController.Instance.RefreshSession();
            }
            authToken = JamboxController.Instance.CurrentSession.Token;
            var result = await _client.SubmitScore(authToken, LbID, score, replayData);
            if (OnReceived != null)
                OnReceived(result);
        }

        /// <summary>
        /// The UI will call this to fetch Leader board of Any particular tournament.
        /// </summary>
        /// <param name="authToken">The unique token which will be held for Communication</param>
        /// <param name="LBId"><The Unique LeaderboardId sent by server to get the leaderboard from server/param>
        /// <param name="OnReceived">The Action on completion of this particular task</param>
        /// <returns></returns>
        public async Task GetLeaderBoard(string authToken, String LBId, Action<IApiLeaderRecordList> OnReceived)
        {
            if (!JamboxController.Instance.CheckForNetwork())
            {
                ShowNoNetworkDialogue();
                return;
            }

            if (!JamboxController.Instance.ChechkForSession())
            {
                await JamboxController.Instance.RefreshSession();
            }
            authToken = JamboxController.Instance.CurrentSession.Token;
            var result = await _client.getLeaderBoard(authToken, LBId);
            if (OnReceived != null)
                OnReceived(result);
        }


        public async Task GetCompletedTourneyData(string authToken, string Cate, Action<IAPICompTourneyList> OnReceived)
        {
            if (!JamboxController.Instance.CheckForNetwork())
            {
                ShowNoNetworkDialogue();
                return;
            }

            if (!JamboxController.Instance.ChechkForSession())
            {
                await JamboxController.Instance.RefreshSession();
            }
            authToken = JamboxController.Instance.CurrentSession.Token;
            var result = await _client.getCompletedTourneyData(authToken, Cate);
            if (OnReceived != null)
                OnReceived(result);
        }


        public async Task GetClaim(string authToken, String LBId, Action<IAPIClaimData> OnReceived)
        {
            if (!JamboxController.Instance.CheckForNetwork())
            {
                ShowNoNetworkDialogue();
                return;
            }

            if (!JamboxController.Instance.ChechkForSession())
            {
                await JamboxController.Instance.RefreshSession();
            }
            authToken = JamboxController.Instance.CurrentSession.Token;
            var result = await _client.GetClaimData(authToken, LBId);
            if (OnReceived != null)
                OnReceived(result);
        }

        public async Task CreateFriendly(string authToken, String tourneyName, int attempts, int duration, Action<IAPICreateFriendly> OnReceived)
        {
            if (!JamboxController.Instance.CheckForNetwork())
            {
                ShowNoNetworkDialogue();
                return;
            }

            if (!JamboxController.Instance.ChechkForSession())
            {
                await JamboxController.Instance.RefreshSession();
            }
            authToken = JamboxController.Instance.CurrentSession.Token;
            var result = await _client.CreateFriendly(authToken, tourneyName, attempts, duration);
            if (OnReceived != null)
                OnReceived(result);
        }

        public async Task JoinFriendly(string authToken, String code, Action<IAPIJoinFriendly> OnReceived)
        {
            if (!JamboxController.Instance.CheckForNetwork())
            {
                ShowNoNetworkDialogue();
                return;
            }

            if (!JamboxController.Instance.ChechkForSession())
            {
                await JamboxController.Instance.RefreshSession();
            }
            authToken = JamboxController.Instance.CurrentSession.Token;
            var result = await _client.JoinFriendly(authToken, code);
            if (OnReceived != null)
                OnReceived(result);
        }

        public async Task GetFriendlyDetails(string authToken, Action<IAPIFriendlyTourneyList> OnReceived)
        {
            if (!JamboxController.Instance.CheckForNetwork())
            {
                ShowNoNetworkDialogue();
                return;
            }

            if (!JamboxController.Instance.ChechkForSession())
            {
                await JamboxController.Instance.RefreshSession();
            }
            authToken = JamboxController.Instance.CurrentSession.Token;
            var result = await _client.GetFriendlyDetails(authToken);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.friendlyTournaments);
            if (OnReceived != null)
                OnReceived(result);
        }

        public async Task PlayFriendlyTourney(string authToken, string tourneyId, Action<IApiPlayFriendlyTourney> OnReceived)
        {
            if (!JamboxController.Instance.CheckForNetwork())
            {
                ShowNoNetworkDialogue();
                return;
            }

            if (!JamboxController.Instance.ChechkForSession())
            {
                await JamboxController.Instance.RefreshSession();
            }
            authToken = JamboxController.Instance.CurrentSession.Token;
            var result = await _client.PlayFriendlyTourney(authToken, tourneyId);
            if (OnReceived != null)
                OnReceived(result);
        }

        public async Task GetCurrencyData(string authToken, Action<IAPICurrencyList> OnReceived)
        {
            if (!JamboxController.Instance.CheckForNetwork())
            {
                ShowNoNetworkDialogue();
                return;
            }

            if (!JamboxController.Instance.ChechkForSession())
            {
                await JamboxController.Instance.RefreshSession();
            }
            authToken = JamboxController.Instance.CurrentSession.Token;
            var result = await _client.GetCurrencyData(authToken);
            if (OnReceived != null)
                OnReceived(result);
        }

        public async Task UpdateUserDetails(String authToken, String name, int avatarId, string avatarGroup, Action<IAPIUpdateUserData> OnReceived)
        {
            if (!JamboxController.Instance.CheckForNetwork())
            {
                ShowNoNetworkDialogue();
                return;
            }
            var result = await JamboxController.Instance.UpdateUserDetails(authToken, name, avatarId, avatarGroup);
            if (OnReceived != null)
                OnReceived(result);
        }

        public async Task UnclaimedRewards(string authToken, Action<IAPIUnclaimedRewards> OnReceived)
        {
            if (!JamboxController.Instance.CheckForNetwork())
            {
                ShowNoNetworkDialogue();
                return;
            }

            if (!JamboxController.Instance.ChechkForSession())
            {
                await JamboxController.Instance.RefreshSession();
            }
            authToken = JamboxController.Instance.CurrentSession.Token;
            var result = await _client.UnclaimedRewards(authToken);
            if (OnReceived != null)
                OnReceived(result);
        }


    }
}

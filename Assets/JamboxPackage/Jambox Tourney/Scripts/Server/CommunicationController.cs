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

        public void Init(CommonClient baseClient)
        {            
                _client = new TournamentClient(baseClient);        
        }

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
        public async Task GetTourneydetail(Action<IApiTourneyList> OnReceived, Action<int, String> OnError)
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
            String authToken = JamboxController.Instance.CurrentSession.Token;
            try
            {
                var result = await _client.GetTourneyList(authToken, JamboxController.Instance.getUserLevel());
                if (OnReceived != null)
                    OnReceived(result);
            }
            catch (JamboxException Ex)
            {
                Debug.Log("Exception caught Hit >>>> Message : " + Ex.ErrorMsg + " errorCode : " + Ex.ErrorCode
                    + "  StackTrace : " + Ex.StackTrace);
                if (OnError != null)
                    OnError(Ex.ErrorCode, Ex.ErrorMsg);
            }
        }

        /// <summary>
        /// The UI will call this method when user requests to join any particular Tourney.
        /// </summary>
        /// <param name="authToken">The unique token which will be held for Communication</param>
        /// <param name="tourneyId">The Unique Id of tournament held responsible for getting detail of any tournament.</param>
        /// <param name="OnReceived">The Action on completion of this particular task.</param>
        /// <returns></returns>
        public async Task JoinTourney(String tourneyId, Action<IAPIJoinTourney> OnReceived, Action<int, String> OnError)
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

            String authToken = JamboxController.Instance.CurrentSession.Token;
            try
            {
                var result = await _client.JoinTournament(authToken, tourneyId);
                if (OnReceived != null)
                    OnReceived(result);
            }
            catch (JamboxException Ex)
            {
                Debug.Log("Exception caught Hit >>>> Message : " + Ex.ErrorMsg + " errorCode : " + Ex.ErrorCode
                    + "  StackTrace : " + Ex.StackTrace);
                if (OnError != null)
                    OnError(Ex.ErrorCode, Ex.ErrorMsg);
            }
        }

        /// <summary>
        /// The UI will call this method when user requests to join any particular Tourney.
        /// </summary>
        /// <param name="authToken">The unique token which will be held for Communication</param>
        /// <param name="duelId">The Unique Id of Duel held responsible for getting detail of any tournament.</param>
        /// <param name="OnReceived">The Action on completion of this particular task.</param>
        /// <returns></returns>
        public async Task JoinDuel(String tourneyId, Action<IAPIJoinDuel> OnReceived, Action<int, String> OnError)
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
            String authToken = JamboxController.Instance.CurrentSession.Token;
            try
            {
                var result = await _client.JoinDuel(authToken, tourneyId);

                if (OnReceived != null)
                    OnReceived(result);
            }
            catch (JamboxException Ex)
            {
                Debug.Log("Exception caught Hit >>>> Message : " + Ex.ErrorMsg + " errorCode : " + Ex.ErrorCode
                    + "  StackTrace : " + Ex.StackTrace);
                if (OnError != null)
                    OnError(Ex.ErrorCode, Ex.ErrorMsg);
            }
        }

        /// <summary>
        /// The UI will call this method when user requests to play any particular Tourney.
        /// </summary>
        /// <param name="authToken">The unique token which will be held for Communication</param>
        /// <param name="tourneyId">The Unique Id of tournament held responsible for getting detail of any tournament</param>
        /// <param name="attemptType"></param>
        /// <param name="OnReceived">The Action on completion of this particular task</param>
        /// <returns></returns>
        public async Task PlayTourney(string tourneyId, string attemptType, Action<IApiPlayTourney> OnReceived, Action<int, String> OnError)
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
            String authToken = JamboxController.Instance.CurrentSession.Token;
            try
            {
                var result = await _client.PlayTourney(authToken, tourneyId, attemptType);
                if (OnReceived != null)
                    OnReceived(result);
            }
            catch (JamboxException Ex)
            {
                Debug.Log("Exception caught Hit >>>> Message : " + Ex.ErrorMsg + " errorCode : " + Ex.ErrorCode
                    + "  StackTrace : " + Ex.StackTrace);
                if (OnError != null)
                    OnError(Ex.ErrorCode, Ex.ErrorMsg);
            }
        }

        /// <summary>
        /// The UI will call this method when they complete the game and submit their score.
        /// </summary>
        /// <param name="authToken">The unique token which will be held for Communication</param>
        /// <param name="score"></param>
        /// <param name="OnReceived">The Action on completion of this particular task</param>
        /// <returns></returns>
        public async Task SubmitScore(string LbID, long score, string displayScore, Action<IApiSubmitScore> OnReceived, Action<int, String> OnError, ReplayData replayData = null)
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
            String authToken = JamboxController.Instance.CurrentSession.Token;
            try
            {
                var result = await _client.SubmitScore(authToken, LbID, score, displayScore, replayData);
                if (OnReceived != null)
                    OnReceived(result);
            }
            catch (JamboxException Ex)
            {
                Debug.Log("Exception caught Hit >>>> Message : " + Ex.ErrorMsg + " errorCode : " + Ex.ErrorCode
                    + "  StackTrace : " + Ex.StackTrace);
                if (OnError != null)
                    OnError(Ex.ErrorCode, Ex.ErrorMsg);
            }
        }

        /// <summary>
        /// The UI will call this method when they complete the game and submit their score.
        /// </summary>
        /// <param name="authToken">The unique token which will be held for Communication</param>
        /// <param name="score"></param>
        /// <param name="OnReceived">The Action on completion of this particular task</param>
        /// <returns></returns>
        public async Task SubmitDuelScore(string matchID, long score, string displayScore, Action<IApiSubmitDuelScore> OnReceived, Action<int, String> OnError, ReplayData replayData = null)
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
            String authToken = JamboxController.Instance.CurrentSession.Token;
            try
            {
                var result = await _client.SubmitDuelScore(authToken, matchID, score, displayScore, replayData);
                if (OnReceived != null)
                    OnReceived(result);
            }
            catch (JamboxException Ex)
            {
                Debug.Log("Exception caught Hit >>>> Message : " + Ex.ErrorMsg + " errorCode : " + Ex.ErrorCode
                    + "  StackTrace : " + Ex.StackTrace);
                if (OnError != null)
                    OnError(Ex.ErrorCode, Ex.ErrorMsg);
            }
        }

        /// <summary>
        /// The UI will call this to fetch Leader board of Any particular tournament.
        /// </summary>
        /// <param name="authToken">The unique token which will be held for Communication</param>
        /// <param name="LBId"><The Unique LeaderboardId sent by server to get the leaderboard from server/param>
        /// <param name="OnReceived">The Action on completion of this particular task</param>
        /// <returns></returns>
        public async Task GetLeaderBoard(String LBId, Action<IApiLeaderRecordList> OnReceived, Action<int, String> OnError)
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
            String authToken = JamboxController.Instance.CurrentSession.Token;
            try
            {
                var result = await _client.getLeaderBoard(authToken, LBId);
                if (OnReceived != null)
                    OnReceived(result);
            }
            catch (JamboxException Ex)
            {
                Debug.Log("Exception caught Hit >>>> Message : " + Ex.ErrorMsg + " errorCode : " + Ex.ErrorCode
                    + "  StackTrace : " + Ex.StackTrace);
                if (OnError != null)
                    OnError(Ex.ErrorCode, Ex.ErrorMsg);
            }
        }


        public async Task GetCompletedTourneyData(string Cate, Action<IAPICompTourneyList> OnReceived, Action<int, String> OnError)
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
            String authToken = JamboxController.Instance.CurrentSession.Token;
            try
            {
                var result = await _client.getCompletedTourneyData(authToken, Cate);
                if (OnReceived != null)
                    OnReceived(result);
            }
            catch (JamboxException Ex)
            {
                Debug.Log("Exception caught Hit >>>> Message : " + Ex.ErrorMsg + " errorCode : " + Ex.ErrorCode
                    + "  StackTrace : " + Ex.StackTrace);
                if (OnError != null)
                    OnError(Ex.ErrorCode, Ex.ErrorMsg);
            }
        }


        public async Task GetClaim(String LBId, Action<IAPIClaimData> OnReceived, Action<int, String> OnError)
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
            String authToken = JamboxController.Instance.CurrentSession.Token;
            try
            {
                var result = await _client.GetClaimData(authToken, LBId);
                if (OnReceived != null)
                    OnReceived(result);
            }
            catch (JamboxException Ex)
            {
                Debug.Log("Exception caught Hit >>>> Message : " + Ex.ErrorMsg + " errorCode : " + Ex.ErrorCode
                    + "  StackTrace : " + Ex.StackTrace);
                if (OnError != null)
                    OnError(Ex.ErrorCode, Ex.ErrorMsg);
            }
        }

        public async Task CreateFriendly(String tourneyName, int attempts, int duration, Action<IAPICreateFriendly> OnReceived, Action<int, String> OnError)
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
            String authToken = JamboxController.Instance.CurrentSession.Token;
            try
            {
                var result = await _client.CreateFriendly(authToken, tourneyName, attempts, duration);
                if (OnReceived != null)
                    OnReceived(result);
            }
            catch (JamboxException Ex)
            {
                Debug.Log("Exception caught Hit >>>> Message : " + Ex.ErrorMsg + " errorCode : " + Ex.ErrorCode
                    + "  StackTrace : " + Ex.StackTrace);
                if (OnError != null)
                    OnError(Ex.ErrorCode, Ex.ErrorMsg);
            }
        }

        public async Task JoinFriendly(String code, Action<IAPIJoinFriendly> OnReceived, Action<int, String> OnError)
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
            String authToken = JamboxController.Instance.CurrentSession.Token;
            try
            {
                var result = await _client.JoinFriendly(authToken, code);
                if (OnReceived != null)
                    OnReceived(result);
            }
            catch (JamboxException Ex)
            {
                Debug.Log("Exception caught Hit >>>> Message : " + Ex.ErrorMsg + " errorCode : " + Ex.ErrorCode
                    + "  StackTrace : " + Ex.StackTrace);
                if (OnError != null)
                    OnError(Ex.ErrorCode, Ex.ErrorMsg);
            }
        }

        public async Task GetFriendlyDetails(Action<IAPIFriendlyTourneyList> OnReceived, Action<int, String> OnError)
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
            String authToken = JamboxController.Instance.CurrentSession.Token;
            try
            {
                var result = await _client.GetFriendlyDetails(authToken);
                Assert.IsNotNull(result);
                Assert.IsNotNull(result.friendlyTournaments);
                if (OnReceived != null)
                    OnReceived(result);
            }
            catch (JamboxException Ex)
            {
                Debug.Log("Exception caught Hit >>>> Message : " + Ex.ErrorMsg + " errorCode : " + Ex.ErrorCode
                    + "  StackTrace : " + Ex.StackTrace);
                if (OnError != null)
                    OnError(Ex.ErrorCode, Ex.ErrorMsg);
            }
        }

        public async Task PlayFriendlyTourney(string tourneyId, Action<IApiPlayFriendlyTourney> OnReceived, Action<int, String> OnError)
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
            String authToken = JamboxController.Instance.CurrentSession.Token;
            try
            {
                var result = await _client.PlayFriendlyTourney(authToken, tourneyId);
                if (OnReceived != null)
                    OnReceived(result);
            }
            catch (JamboxException Ex)
            {
                Debug.Log("Exception caught Hit >>>> Message : " + Ex.ErrorMsg + " errorCode : " + Ex.ErrorCode
                    + "  StackTrace : " + Ex.StackTrace);
                if (OnError != null)
                    OnError(Ex.ErrorCode, Ex.ErrorMsg);
            }
        }

        public async Task GetCurrencyData(Action<IAPICurrencyList> OnReceived, Action<int, String> OnError)
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
            String authToken = JamboxController.Instance.CurrentSession.Token;
            try
            {
                var result = await _client.GetCurrencyData(authToken);
                if (OnReceived != null)
                    OnReceived(result);
            }

            catch (JamboxException Ex)
            {
                Debug.Log("Exception caught Hit >>>> Message : " + Ex.ErrorMsg + " errorCode : " + Ex.ErrorCode
                    + "  StackTrace : " + Ex.StackTrace);
                if (OnError != null)
                    OnError(Ex.ErrorCode, Ex.ErrorMsg);
            }
        }

        public async Task UnclaimedRewards(Action<IAPIUnclaimedRewards> OnReceived, Action<int, String> OnError)
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
            String authToken = JamboxController.Instance.CurrentSession.Token;
            try
            {
                var result = await _client.UnclaimedRewards(authToken);
                if (OnReceived != null)
                    OnReceived(result);
            }

            catch (JamboxException Ex)
            {
                Debug.Log("Exception caught Hit >>>> Message : " + Ex.ErrorMsg + " errorCode : " + Ex.ErrorCode
                    + "  StackTrace : " + Ex.StackTrace);
                if (OnError != null)
                    OnError(Ex.ErrorCode, Ex.ErrorMsg);
            }
        }
    }
}

namespace JBX.Leaderboard.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Jambox.Common.Utility;
    using JBX.Leaderboard.Data;
    using JBX.Leaderboard.Server;
    using Jambox.Common;
    using UnityEngine;
    using Jambox.Tourney.Connector;

    public class JBXLeaderboardCommunicator : MonoSingleton <JBXLeaderboardCommunicator>
    {
        private JBXLeaderboardClient _client;

        public event Action OnLeaderBoardInit;

        public void Init (CommonClient baseClient)
        {
            _client = new JBXLeaderboardClient(baseClient);
            //_ = GetLeaderboardList((dataNew) => OnLeaderBoardListRcvd(dataNew));
        }

        //private void OnLeaderBoardListRcvd(ILeaderboardList LBListData)
        //{
        //    Debug.Log("OnLeaderBoardListRcvd Hit >>>");
        //    LeaderboardDataContain.Instance.UpdateLBMasterData(LBListData);
        //}

        private void ShowNoNetworkDialogue()
        {
            Debug.Log("ShowNoNetworkDialogue Hit >>>");
        }
        private void OnErrorFromServer(String ErrorKey, String ErrorMsg)
        {
            UnityDebug.Debug.Log("Error From Server Method Hit >>>" + ErrorKey  + "   ErrorMsg : "+ ErrorMsg);
            Dictionary<String, String> Errordata = new Dictionary<string, string>();
            Errordata.Add(ErrorKey, ErrorMsg);
            ArenaSDKEvent.Instance.OnErrorFromServer(Errordata);
            //JamboxRewardPanelController.Instance.ErrorFromServerRcvd(ErrorMsg);
        }

        /// <summary>
        /// This method will be called from UI to get the Sweep Stakes detail.
        /// </summary>
        /// <param name="authToken">The unique token which will be held for Communication.</param>
        /// <returns></returns>
        public async Task GetLeaderboardList(Action<ILeaderboardList> OnReceived)
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
            string authToken = JamboxController.Instance.CurrentSession.Token;
            try
            {
                var result = await _client.GetLeaderboardList(authToken);
                Debug.Log("GetLeaderboardList Of Communicator Hit2222 >>>");
                if (OnReceived != null)
                    OnReceived(result);
            }
            catch (Exception Ex)
            {
                Debug.Log("Exception caught Hit >>>> Message : " + Ex.Message
                    + " Stacktrace : " + Ex.StackTrace);
                OnErrorFromServer("GetLeaderboardList", Ex.Message);
            }
        }

        public async Task GetLeaderboardRecord(string LBId, string partitionKey, Action<IApiLeaderRecordList> OnReceived)
        {
            Debug.Log("GetLeaderboardRecord Of Communicator Hit");
            if (!JamboxController.Instance.CheckForNetwork())
            {
                ShowNoNetworkDialogue();
                return;
            }

            if (!JamboxController.Instance.ChechkForSession())
            {
                await JamboxController.Instance.RefreshSession();
            }
            string authToken = JamboxController.Instance.CurrentSession.Token;
            try
            {
                var result = await _client.GetLeaderBoardRecord(authToken, LBId, partitionKey);
                Debug.Log("GetLeaderboardRecord Of Communicator Hit2222 >>>");
                if (OnReceived != null)
                    OnReceived(result);
            }
            catch (Exception Ex)
            {
                Debug.Log("Exception caught Hit >>>> Message : " + Ex.Message
                     + " Stacktrace : " + Ex.StackTrace);
                OnErrorFromServer("GetLeaderboardRecord", Ex.Message);
            }
        }

        public async Task GetLeaderboardAroundMe(string LBId, string partitionKey, Action<IApiLeaderAroundMe> OnReceived)
        {
            Debug.Log("GetLeaderBoardAroundMe Of Communicator Hit");
            if (!JamboxController.Instance.CheckForNetwork())
            {
                ShowNoNetworkDialogue();
                return;
            }

            if (!JamboxController.Instance.ChechkForSession())
            {
                await JamboxController.Instance.RefreshSession();
            }
            string authToken = JamboxController.Instance.CurrentSession.Token;
            try
            {
                var result = await _client.GetLeaderBoardAroundMe(authToken, LBId, partitionKey);
                Debug.Log("GetLeaderBoardAroundMe Of Communicator Hit2222 >>>");
                if (OnReceived != null)
                    OnReceived(result);
            }
            catch (Exception Ex)
            {
                Debug.Log("Exception caught Hit >>>> Message : " + Ex.Message
                     + " Stacktrace : " + Ex.StackTrace);
                OnErrorFromServer("GetLeaderboardRecord", Ex.Message);
            }
        }

        public async Task SubmitLBRecord(string LBId, String PartitionKey, long score, string DisplayScore, Action<ISubmitScore> OnReceived)
        {
            Debug.Log("SubmitLBRecord Of Communicator Hit");
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
                Debug.Log("SubmitLBRecord Of Communicator Hit1111 : score : " + score);
                var result = await _client.SubmitLBRecord(authToken, LBId, PartitionKey, score, DisplayScore);
                Debug.Log("SubmitLBRecord Of Communicator Hit2222 >>>");
                if (OnReceived != null)
                    OnReceived(result);
            }
            catch (Exception Ex)
            {
                Debug.Log("Exception caught Hit >>>> Message : " + Ex.Message
                    + " Stacktrace : " + Ex.StackTrace);
                OnErrorFromServer("SubmitLBRecord", Ex.Message);
            }
        }

        public async Task GetPreviousLeaderBoard(string LBId, Action<IApiLeaderRecordList> OnReceived)
        {
            Debug.Log("GetPreviousLeaderBoard Of Communicator Hit");
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
                var result = await _client.GetPreviousLeaderBoard(authToken, LBId);
                Debug.Log("GetPreviousLeaderBoard Of Communicator Hit2222 >>>");
                if (OnReceived != null)
                    OnReceived(result);
            }
            catch (Exception Ex)
            {
                Debug.Log("Exception caught Hit >>>> Message : " + Ex.Message
                    + " Stacktrace : " + Ex.StackTrace);
                OnErrorFromServer("GetPreviousLeaderBoard", Ex.Message);
            }
        }

        public async Task GetPendingLBReward( Action<IPendingReward> OnReceived)
        {
            Debug.Log("GetPendingLBReward Of Communicator Hit");
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
                var result = await _client.GetPendingLBReward(authToken);
                Debug.Log("GetPendingLBReward Of Communicator Hit2222 >>>");
                if (OnReceived != null)
                    OnReceived(result);
            }
            catch (Exception Ex)
            {
                Debug.Log("Exception caught Hit >>>> Message : " + Ex.Message
                    + " Stacktrace : " + Ex.StackTrace);
                OnErrorFromServer("GetPendingLBReward", Ex.Message);
            }
        }

        public async Task ClaimLBReward(string LBId, bool ClaimAll, Action<IClaimReward> OnReceived)
        {
            Debug.Log("ClaimLBReward Of Communicator Hit");
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
                var result = await _client.ClaimLBReward(authToken, LBId, ClaimAll);
                Debug.Log("ClaimLBReward Of Communicator Hit2222 >>>");
                if (OnReceived != null)
                    OnReceived(result);
            }
            catch (Exception Ex)
            {
                Debug.Log("Exception caught Hit >>>> Message : " + Ex.Message
                    + " Stacktrace : " + Ex.StackTrace);
                OnErrorFromServer("ClaimLBReward", Ex.Message);
            }
        }
    }
}

namespace Jambox.Tourney.Connector
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Jambox.Common;
    using Jambox.Tourney.UI;
    using UnityEngine;
    using Jambox.Common.Utility;
    using Jambox.Tourney.Data;
    using Jambox.Common.TinyJson;

    public class ArenaSDKEvent : MonoSingleton<ArenaSDKEvent>
    {
        private bool isTourney = false;
        private Panels PrevPanel = Panels.None;
        private String tourneyID;
        public event Action<Match> OnPlay;
        public event Action OnBackToLobby;
        public event Action<int, string> OnPurchaseRequired;
        public event Action OnWatchAdRequired;
        public event Action<int, string, bool> UserMoneyUpdate;
        public event Action<Dictionary<String, String>> OnServerError;
        private ReplayRecorder replayRecorder = null;

        private IAPIReplayData replayTestData = null;
        private bool replayTestMode = false;
        private bool useConsecutiveReplayData = false;

        //******************* SDK API *******************//

        //Call this method to initialize the SDK

        public void InitializeArenaSdk(string userName = null, string userID = null)
        {
            Debug.Log("InitializeArenaSdk Hit >>>>");
            if (JamboxController.Instance.isGameIdSet())
            {
                CommunicationController.Instance.Init(JamboxController.Instance.BaseClient());
                _ = JamboxController.Instance.StartSession(userName, userID);
                if (UIPanelController.Instance != null)
                {
                    UIPanelController.Instance.SetParentPanel(JamboxSDKParams.Instance.gameObject);
                    UnityDebug.Debug.LogFormat("Arena SDK Version {0} is intialized successfully.", ArenaSDKVersion.VersionString);
                }
                else
                {
                    Debug.LogError("Error in SDK setup");
                }
            }
            else
            {
                Debug.LogError("You have not set either GameID or AppSecret code.");
            }
        }

        public void SubmitScore(int score, int subScore, string displayScore = "")
        {
            //check for tourney ID, if there is no tourney id set then developer might have called the API
            //for no tournament gameplay

            if(!CheckRequiredDetail("SubmitScore"))
            {
                return;
            }
            if (string.IsNullOrEmpty(tourneyID)){
                Debug.LogError("SubmitScore is called for non Tournament Gameplay");
                return;
            }
            StartCoroutine(WaitndSubmit(score, subScore, displayScore));
        }

        public void OpenArenaUI(Dictionary<string, long> currencyMap = null, Panels _targetpanel = Panels.TourneyPanel)
        {
            if (!CheckRequiredDetail("OpenArenaUI"))
            {
                return;
            }
            Panels toOpen = _targetpanel;
            if(PrevPanel != Panels.None)
            {
                toOpen = PrevPanel;
                PrevPanel = Panels.None;
            }
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            //metadata.Add("tourneyid", tourneyID);
            tourneyID = String.Empty;
            if (currencyMap != null)
                UserDataContainer.Instance.setUserMoney(currencyMap);
            UIPanelController.Instance.ShowPanel(toOpen, Panels.None, metadata, true);
        }

        public void PlayAfterPurchase (bool PurchaseSuccess = true, Dictionary<string, long> currencyMap = null)
        {
            if (!CheckRequiredDetail("OpenArenaUI"))
            {
                return;
            }
            Panels toOpen = Panels.TourneyPanel;
            if (PrevPanel != Panels.None)
            {
                toOpen = PrevPanel;
                PrevPanel = Panels.None;
            }
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            metadata.Add("tourneyid", tourneyID);
            tourneyID = String.Empty;
            if (currencyMap != null)
                UserDataContainer.Instance.setUserMoney(currencyMap);
            UIPanelController.Instance.ShowPanel(toOpen, Panels.None, metadata);
        }

        public void PlayAfterWatchAd(bool result)
        {
            if (!CheckRequiredDetail("PlayAfterWatchAd"))
            {
                return;
            }
            Panels toOpen = Panels.TourneyPanel;
            if (PrevPanel != Panels.None)
            {
                toOpen = PrevPanel;
                PrevPanel = Panels.None;
            }
            UserDataContainer.Instance.UpdateUserMoney(1, "adwatched", true);
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            if(result)
                metadata.Add("tourneyid", tourneyID);

            tourneyID = String.Empty;
            UIPanelController.Instance.ShowPanel(toOpen, Panels.None, metadata);
        }

        public void StartRecording(Dictionary<string, string> _gameData)
        {
            replayRecorder = new ReplayRecorder();
            replayRecorder.StartRecording(_gameData);
            StartCoroutine(replayRecorder.StartRecordingTimer());
        }

        public bool StopRecording(string _data = null, string _keyValue = null)
        {
            if (replayRecorder != null)
            {
                replayRecorder.StopRecording(_data, _keyValue);
                return true;
            }
            else {
                Debug.LogError("Replay Recorder is not initialized. Please call 'StartRecording' function ");
                return false;
            }
        }

        public bool AddNewData(string _data, string _keyValue = null)
        {
            if (replayRecorder != null)
            {
                replayRecorder.AddNewData(_data, _keyValue);
                return true;
            }
            else
            {
                Debug.LogError("Replay Recorder is not initialized. Please call 'StartRecording' function ");
                return false;
            } 
        }

        public ReplayData GetCurrentReplay()
        {
            if (replayRecorder != null)
            {
                return replayRecorder.GetCurrentReplay();
            }
            else
            {
                Debug.LogError("Replay Recorder was not initialized. No replay data has been recorded");
                return null;
            }
        }

        public void EnableDuelReplayTestMode(string _replayString, bool UseConsecutiveReplayData = false)
        {
            replayTestMode = true;
            useConsecutiveReplayData = UseConsecutiveReplayData;
            if (string.IsNullOrEmpty(_replayString))
            {
                if (UseConsecutiveReplayData)
                    UnityDebug.Debug.Log("Provided Test Replay Data is Empty! - First Replay data returned will be Null");
                else
                    UnityDebug.Debug.Log("Provided Test Replay Data is Empty! - Please provide a valid Test Data or Set \"UseConsecutiveReplayData\" to true");
                return;
            }
            replayTestData = _replayString.FromJson<APIReplayData>();
        }

        private IEnumerator WaitndSubmit(int Score, int subScore, string displayScore)
        {
            yield return new WaitForSeconds(0.2f);

            ReplayData data = GetCurrentReplay();

            if (PrevPanel == Panels.DuelPanel)
            {
                UIPanelController.Instance.SubmitDuelScore(tourneyID, Score, subScore, displayScore, data);
                if (useConsecutiveReplayData)
                {
                    string _replayString = data.ToJson();
                    UnityDebug.Debug.Log("Recorded Replay Data:\n" + _replayString);
                    replayTestData = _replayString.FromJson<APIReplayData>();
                }
            }
            else
                UIPanelController.Instance.SubmitScore(tourneyID, Score, subScore, displayScore, PrevPanel);
            isTourney = false;
            tourneyID = String.Empty;
        }

        private bool CheckRequiredDetail (string methodName)
        {
            if (String.IsNullOrEmpty(JamboxController.Instance.getMyuserId()))
            {
                if (!JamboxController.Instance.isGameIdSet())
                {
                    Debug.LogError("You are trying to call " + methodName  + " without being Set up SDK Variables ");
                    return false;
                }
                Debug.LogError("You are trying to call " + methodName + " without Starting the session ");
                return true;
            }
            return true;
        }

        public void InitializeRealtimeLeaderboard(List<leaderBoardData> data, bool scrollable, int NoOfRecordsToShow)
        {
            RealtimeLeaderboard.Instance.InitializeRealtimeLeaderboard(data, scrollable, NoOfRecordsToShow);
        }

        public void UpdatePlayerScoreOnRealtimeLB(long _score)
        {
            UnityDebug.Debug.Log("Update Player Score On RealtimeLB : Score : " + _score);
            RealtimeLeaderboard.Instance.UpdatePlayerScore(_score);
        }

        //******************* SDK API *******************//




        //******************* SDK Internal Functions *******************//

        public void FireOnStoreClick( int StoreMoney, string CurrKey, string TourneyId, Panels prev)
        {
            isTourney = false;
            tourneyID = TourneyId;
            PrevPanel = prev;

            if (OnPurchaseRequired != null)
            {
                OnPurchaseRequired(StoreMoney, CurrKey);
            }
        }

        public void FireOnUpdateUserMoney(long amount, string Currency, bool isIncrease = true)
        {
            if (UserMoneyUpdate != null)
            {
                UserMoneyUpdate((int)amount, Currency, isIncrease);
            }
        }

        public void FireOnPlayClick(Panels prevPanel, Match matchdata)
        {
#if GAME_FIREBASE_ENABLED
            Firebase.Analytics.Parameter[] eventarameters = {
                                new Firebase.Analytics.Parameter("event_type", matchdata.GetMatchTypeString()),
                                new Firebase.Analytics.Parameter("event_id",matchdata.tournamentID)
                                };
            Firebase.Analytics.FirebaseAnalytics.LogEvent("arena_event_attempt", eventarameters);
#endif

            PrevPanel = prevPanel;
            isTourney = true;
            tourneyID = matchdata.tournamentID;

            if (prevPanel == Panels.DuelPanel)
            {
                if (replayTestMode)
                {
                    UnityDebug.Debug.Log("Replacing Replay with Test Data");
                    matchdata.replayData = replayTestData;
                }
            }

            if (OnPlay != null)
            {
                OnPlay(matchdata);
            }
        }

        public void FireOnBackToLobby()
        {
            isTourney = false;

            if (OnBackToLobby != null)
            {
                OnBackToLobby();
            }
        }

        public void OnErrorFromServer(Dictionary<String, String> ErrorMsg)
        {
            Debug.Log("On Back to Lobby Hit >>>>");
            if (OnServerError != null)
            {
                OnServerError(ErrorMsg);
            }
        }

        public void FireOnWatchAD(String TourneyID, Panels previous)
        {
            PrevPanel = previous;
            tourneyID = TourneyID;
            if (OnWatchAdRequired != null)
            {
                OnWatchAdRequired();
            }
        }
        //******************* SDK Internal Functions *******************//
    }
}

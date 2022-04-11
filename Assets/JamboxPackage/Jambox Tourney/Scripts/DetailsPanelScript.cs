namespace Jambox.Tourney.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Jambox.Common;
    using Jambox.Common.Utility;
    using Jambox.Tourney.Connector;
    using Jambox.Tourney.Data;
    using UnityEngine;
    using UnityEngine.UI;

    public class DetailsPanelScript : MonoBehaviour
    {
        public Text HeaderText;
        public Text DescText;
        public Text ButtonName;
        public Button PlayJoinWatch;
        public GameObject watchAdText;
        public Text BestScore;
        public Text NoLBText;
        public Text UserMoney;
        public Text subtractMoneyText;
        public Image UserMoneyIcon;
        public GameObject Position;
        public Text RankDisplay;
        public GameObject EntryFee;
        public Text EntryAmount;
        private int entryFeeAmnt;

        public GameObject Attempts;
        public Text AttemptCount;
        public float attemptChangeTime;
        public float attemptsChangeScale;

        public GameObject Rewards;
        public Text RewardAmount;
        private string TourneyId;
        public Text EndTimeText;
        //public GameObject mainPanel;
        public RewarDetail rewardDetail;
        public GameObject rewardParent;
        public GameObject InvitePanel;
        public LeaderBoardListView theList;
        public LeaderboardHeaders LBHeader;
        public GameObject LbLoadingPanel;
        public Rotate TopLoadingPanel;
        public GameObject FullLoadingPanel;
        public Button LBrefreshButton;
        private List<leaderBoardData> leaderBoardList;

        public GameObject celebrations;
        public ClaimRewardPanel ClaimPanel;

        private Panels prevPanel;
        private bool friendlyCompleted = false;
        private RectTransform rectTransform;
        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                    rectTransform = GetComponent<RectTransform>();
                return rectTransform;
            }
        }

        #region Tablet_Checks
        private void Start()
        {
            TabletCheck();
        }

        public void TabletCheck()
        {
            if (TabletDetect.IsTablet())
            {
                this.gameObject.GetComponentInParent<CanvasScaleChange>().SetToTabletView();
            }
            else
            {
                this.gameObject.GetComponentInParent<CanvasScaleChange>().SetToDefault();
            }
        }
        #endregion

        public void SetTourneyId(string Id, Panels prevPanelDet, bool IsFriendly)
        {
            UnityDebug.Debug.Log("Tourney ID : " +  Id);
            TourneyId = Id;
            prevPanel = prevPanelDet;
            friendlyCompleted = IsFriendly;
            string screenName = "";
            if (prevPanel == Panels.TourneyPanel)
            {
                TourneyDetail tourneyDet = null;
                UserDataContainer.Instance.UpdatedTourneyData.TryGetValue(TourneyId, out tourneyDet);
                UpdateUI(tourneyDet);
                screenName = "tournament_live_details";
            }
            if(prevPanel == Panels.CompletedPanel)
            {
                CompletedTourneyDetail CompTourneyDet = null;
                foreach (var dataNew in UserDataContainer.Instance.CompletedTourney)
                {
                    if(TourneyId.Equals(dataNew.LeaderBoardID))
                    {
                        CompTourneyDet = dataNew;
                    }
                }
                UpdateCompletedUI(CompTourneyDet);
                screenName = "tournament_past_details";
            }
            if(prevPanel == Panels.DuelPanel)
            {
                TournamnetData DuelData = null;
                UserDataContainer.Instance.MyDuels.TryGetValue(Id, out DuelData);
                UpdateDuelDataOnUI(DuelData);
                screenName = "";
            }
            if(prevPanel == Panels.FriendlyPanel)
            {
                TourneyDetail tourneyDet = null;
                UserDataContainer.Instance.UpdatedFriendlyData.TryGetValue(TourneyId, out tourneyDet);
                UpdateUI(tourneyDet, prevPanel);
                screenName = "friendly_details";
            }

#if GAME_FIREBASE_ENABLED
            
            Firebase.Analytics.Parameter[] eventarameters = {
                                new Firebase.Analytics.Parameter("screen",screenName)
                                };
            Firebase.Analytics.FirebaseAnalytics.LogEvent("arena_screen", eventarameters);
#endif

        }

        public void OnBackbuttonClick()
        {
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            UIPanelController.Instance.ShowPanel(prevPanel, Panels.None, metadata, _friendlyCompleted: friendlyCompleted);
        }

        public void UpdateMoneyOnUI(bool _rewardClaimed = false)
        {
            UserMoney.text = UserDataContainer.Instance.GetDisplayCurrencyText();
            UserMoneyIcon.sprite = JamboxSDKParams.Instance.ArenaParameters.CoinBG;
            if (_rewardClaimed)
                ConfettiAnimation();
        }

        void ConfettiAnimation()
        {
            if (celebrations.activeInHierarchy == true)
            {
                celebrations.SetActive(false);
            }
            celebrations.SetActive(true);
        }

        private void UpdateCompletedUI (CompletedTourneyDetail CompTourneyDet)
        {
            //UserMoney.text = UserDataContainer.Instance.getUserMoney() + "";
            UpdateMoneyOnUI();
            HeaderText.text = CompTourneyDet.TourneyName;
            DescText.text = CompTourneyDet.TourneyName;
            Position.SetActive(true);
            EntryFee.SetActive(false);
            RankDisplay.text = CompTourneyDet.MyRank + " / " + CompTourneyDet.JoinedPlayer;
            Attempts.SetActive(false);
            Rewards.SetActive(true);
            RewardAmount.text = CompTourneyDet.RewardAmnt +"";
            if (CompTourneyDet.RewardList.RewardsDistribution.Count == 0)
            {
                rewardParent.SetActive(false);
            }
            else
            {
                String TID = CompTourneyDet.LeaderBoardID;
                rewardDetail.UpdateUi(TID, prevPanel, true);
            }
            if (CompTourneyDet.RewardAmnt <= 0)
            {
                PlayJoinWatch.gameObject.SetActive(false);
            }
            else
            {
                PlayJoinWatch.gameObject.SetActive(true);
                ButtonName.text = "CLAIM";
                if (CompTourneyDet.IsClaimed)
                {
                    ButtonName.text = "Claimed";
                    PlayJoinWatch.interactable = false;
                }
            }
            BestScore.text = "BEST : " + CompTourneyDet.Score;
            TimeSpan elapsed = (DateTime.UtcNow).Subtract(DateTime.Parse(CompTourneyDet.EndTime).ToUniversalTime());
            EndTimeText.text = "ENDED : " + EndTimeInFormat(elapsed) + "  AGO";
            LbLoadingPanel.gameObject.SetActive(true);
            _ = CommunicationController.Instance.GetLeaderBoard("", CompTourneyDet.LeaderBoardID, (dataN) => { OnLeaderBoardRcvd(dataN); }, this.gameObject);
            LeaderboardRefreshing(true);
        }

        private void UpdateDuelDataOnUI(TournamnetData DuelData)
        {
            //UserMoney.text = UserDataContainer.Instance.getUserMoney() + "";
            UpdateMoneyOnUI();
            HeaderText.text = DuelData.TourneyName;
            DescText.text = DuelData.Description;
            Position.SetActive(false);
            EntryFee.SetActive(true);
            EntryAmount.text = DuelData.EntryFee + "";
            Attempts.SetActive(false);
            Rewards.SetActive(true);
            RewardAmount.text = DuelData.RewardList.RewardsDistribution[0].WinAmount + " ";
            rewardDetail.UpdateUi(TourneyId, prevPanel);
            ButtonName.text = "JOIN";
            BestScore.gameObject.SetActive(false);
            TimeSpan elapsed = DateTime.Parse(DuelData.EndTime).ToUniversalTime().Subtract(DateTime.UtcNow);
            EndTimeText.text = "ENDS IN : " + EndTimeInFormat(elapsed);
            NoLBText.gameObject.SetActive(true);
        }

        private void UpdateUI(TourneyDetail tourneyDet, Panels prevPanel = Panels.TourneyPanel)
        {
            //UserMoney.text = UserDataContainer.Instance.getUserMoney() + "";
            UpdateMoneyOnUI();
            HeaderText.text = tourneyDet._tournament.TourneyName;
            DescText.text = tourneyDet._tournament.Description;
            //RankDisplay.text = tourneyDet._tournament.CurrentPlayers + " / " + tourneyDet._tournament.MaxPlayers;
            Position.SetActive(false);
            EntryFee.SetActive(true);
            string currName;
            UserDataContainer.Instance.currencyList.TryGetValue(tourneyDet._tournament.Currency, out currName);
            EntryAmount.text = tourneyDet._tournament.EntryFee + " " + currName;
            entryFeeAmnt = tourneyDet._tournament.EntryFee;
            if (prevPanel == Panels.FriendlyPanel)
            {
                rewardParent.SetActive(false);
                InvitePanel.SetActive(true);
                InvitePanel.GetComponent<InviteFriendly>().SetCode(tourneyDet._joinedTourneyData.JoinCode);
            }
            else
            {
                rewardDetail.UpdateUi(TourneyId, prevPanel);
            }
            Attempts.SetActive(true);
            Rewards.SetActive(false);
            AttemptCount.text =  "0 / " + tourneyDet._tournament.MaxEntry;
            TimeSpan elapsed = DateTime.Parse(tourneyDet._tournament.EndTime).ToUniversalTime().Subtract(DateTime.UtcNow);
            //2021-07-19T07:54:36.738535Z
            EndTimeText.text = "ENDS IN : " + EndTimeInFormat(elapsed);
            ButtonName.text = "Join";
            watchAdText.SetActive(false);
            UpdateJoinedStatus(tourneyDet);
        }

        string EndTimeInFormat(TimeSpan _elasped)
        {
            string timeString = "";

            if (_elasped.Days >= 2)
            {
                timeString = _elasped.Days + "D";
            }
            else if (_elasped.Days >= 1)
            {
                if (_elasped.Hours == 0)
                {
                    timeString = _elasped.Days + "D ";
                    return timeString;
                }
                timeString = _elasped.Days + "D " + _elasped.Hours + "H";
            }
            else if (_elasped.Hours >= 1)
            {
                if (_elasped.Minutes == 0)
                {
                    timeString = _elasped.Hours + "H ";
                    return timeString;
                }
                timeString = _elasped.Hours + "H " + _elasped.Minutes + "M";
            }
            else if (_elasped.Hours < 1 && _elasped.Minutes >= 1)
            {
                timeString = _elasped.Minutes + "M";
            }
            else if (_elasped.Minutes < 1)
            {
                timeString = "0M";
            }

            return timeString;
        }

        private void UpdateJoinedStatus(TourneyDetail tourneyDet)
        {
            if (tourneyDet.isJoined)
            {
                ButtonName.text = "Play";
                Attempts.SetActive(true);
                Rewards.SetActive(false);
                AttemptCount.text = tourneyDet._joinedTourneyData.CurrentAttempt + " / " + tourneyDet._tournament.MaxEntry;
                EntryFee.SetActive(false);
                Position.SetActive(true);
                RankDisplay.text = tourneyDet._joinedTourneyData.MyRank + " / " + tourneyDet._joinedTourneyData.CurrentPlayers;
                //LeaderBoard.gameObject.SetActive(true);
                BestScore.gameObject.SetActive(true);
                BestScore.text = "BEST: " + tourneyDet._joinedTourneyData.Score;
                LbLoadingPanel.gameObject.SetActive(true);
                _ = CommunicationController.Instance.GetLeaderBoard("", tourneyDet._joinedTourneyData.LeaderBoardID, (dataN) => { OnLeaderBoardRcvd(dataN); }, this.gameObject);
                LeaderboardRefreshing(true);
                updatePlayableStatus(tourneyDet);
            }
            else
            {
                BestScore.gameObject.SetActive(false);
                NoLBText.gameObject.SetActive(true);
            }
        }

        public void updatePlayableStatus(TourneyDetail tourneyDet)
        {
            if (tourneyDet._joinedTourneyData.CurrentAttempt >= tourneyDet._tournament.MaxEntry)
            {
                if (tourneyDet._tournament.PlayWithVideoAD)
                {
                    ButtonName.text = "";
                    watchAdText.SetActive(true);
                }
                else
                {
                    PlayJoinWatch.interactable = false;
                }
            }
        }

        private void OnLeaderBoardRcvd(IApiLeaderRecordList data)
        {
            if (LbLoadingPanel != null && LbLoadingPanel.activeInHierarchy)
                LbLoadingPanel.gameObject.SetActive(false);
            NoLBText.gameObject.SetActive(false);
            leaderBoardList = new List<leaderBoardData>();
            List<leaderBoardData> templist = new List<leaderBoardData>();

            leaderBoardData _myPlayerData = null;

            LBHeader.SetLeaderBoardHeader(data.LeaderBoardHeaders.ScoreText,
                data.LeaderBoardHeaders.NameText, data.LeaderBoardHeaders.RankText);
            foreach (var item in data.LeaderRecords)
            {

                if (item.PlayerId == JamboxController.Instance.getMyuserId())
                    _myPlayerData = new leaderBoardData(item);

                leaderBoardData itemData = new leaderBoardData(item);
                templist.Add(itemData);
            }
            leaderBoardList = templist.OrderBy(o => o.Rank).ToList();

            if (_myPlayerData != null)
            {
                RankDisplay.text = _myPlayerData.Rank + " / " + leaderBoardList.Count;
            }

            ShowLeaderBoard();
            LeaderboardRefreshing(false);
        }

        public void ShowLeaderBoard()
        {
            theList.gameObject.SetActive(true);
            theList.ItemCallback = PopulateItem;
            theList.RowCount = leaderBoardList.Count;
        }

        public void RefreshLeaderboard()
        {
            if (prevPanel == Panels.TourneyPanel)
            {
                TourneyDetail tourneyDetNew = null;
                UserDataContainer.Instance.UpdatedTourneyData.TryGetValue(TourneyId, out tourneyDetNew);
                if(tourneyDetNew.isJoined)
                {
                    theList.RowCount = 0;
                    LbLoadingPanel.gameObject.SetActive(true);
                    LeaderboardRefreshing(true);
                    _ = CommunicationController.Instance.GetLeaderBoard("", tourneyDetNew._joinedTourneyData.LeaderBoardID, (dataN) => { OnLeaderBoardRcvd(dataN); }, this.gameObject); 
                }
            }
            if (prevPanel == Panels.CompletedPanel)
            {
                CompletedTourneyDetail CompTourneyDet = null;
                foreach (var dataNew in UserDataContainer.Instance.CompletedTourney)
                {
                    if (TourneyId.Equals(dataNew.LeaderBoardID))
                    {
                        CompTourneyDet = dataNew;
                    }
                }
                theList.RowCount = 0;
                LbLoadingPanel.gameObject.SetActive(true);
                _ = CommunicationController.Instance.GetLeaderBoard("", CompTourneyDet.LeaderBoardID, (dataN) => { OnLeaderBoardRcvd(dataN); }, this.gameObject);
                LeaderboardRefreshing(true);
            }
            if (prevPanel == Panels.FriendlyPanel)
            {
                TourneyDetail tourneyDetNew = null;
                UserDataContainer.Instance.UpdatedFriendlyData.TryGetValue(TourneyId, out tourneyDetNew);
                theList.RowCount = 0;
                LbLoadingPanel.gameObject.SetActive(true);
                LeaderboardRefreshing(true);
                _ = CommunicationController.Instance.GetLeaderBoard("", tourneyDetNew._joinedTourneyData.LeaderBoardID, (dataN) => { OnLeaderBoardRcvd(dataN); }, this.gameObject);
            }
        }

        void LeaderboardRefreshing(bool value)
        {
            LBrefreshButton.interactable = (!value);
            LBrefreshButton.GetComponent<Image>().enabled = (!value);
            LBrefreshButton.transform.GetChild(0).gameObject.SetActive(value);
        }

        private void PopulateItem(LeaderBoardListViewItem item, int rowIndex)
        {
            var tDet = item as LeaderBoardItem;
            if (rowIndex < leaderBoardList.Count)
            {
                bool lastItem = false;
                if (rowIndex == leaderBoardList.Count - 1)
                    lastItem = true;
                tDet.FillItem(leaderBoardList[rowIndex], lastItem);
            }
        }

        public void OnPlayJoinClaimBtnCLick ()
        {
            if (prevPanel == Panels.DuelPanel)
            {
                TournamnetData DuelData = null;
                UserDataContainer.Instance.MyDuels.TryGetValue(TourneyId, out DuelData);
                UserDataContainer.Instance.UpdateUserMoney(DuelData.EntryFee, DuelData.Currency, false);
                Dictionary<string, string> metadata = new Dictionary<string, string>();
                metadata.Add("tourneyid", TourneyId);
                UIPanelController.Instance.ShowPanel(Panels.MatchMakingPanel, Panels.None, metadata);
            }

            if(prevPanel == Panels.TourneyPanel || prevPanel == Panels.FriendlyPanel)
            {
                TourneyDetail tourneyDet = null;
                if (prevPanel == Panels.FriendlyPanel)
                {
                    UserDataContainer.Instance.UpdatedFriendlyData.TryGetValue(TourneyId, out tourneyDet);
                }
                else
                {
                    UserDataContainer.Instance.UpdatedTourneyData.TryGetValue(TourneyId, out tourneyDet);
                }
                if (tourneyDet.isJoined)
                {
                    if (tourneyDet._joinedTourneyData.CurrentAttempt >= tourneyDet._tournament.MaxEntry)
                    {
                        if (tourneyDet._tournament.PlayWithVideoAD)
                        {
                            FullLoadingPanel.SetActive(true);
                            UIPanelController.Instance.OnWatchVideo(tourneyDet._joinedTourneyData.Tourneyid,
                                                                    prevPanel);
                        }
                        else
                        {
                            String msg = "YOU HAVE EXHAUSTED THE ATTEMPTS. PLEASE PLAY ANOTHER TOURNAMENT";
                            UIPanelController.Instance.SetTourneyScrollActive(false);
                            UIPanelController.Instance.SetTourneyNoMoneyDialogue(true, msg, "OKAY", true, false, null, 0, "", "",
                                                                    Panels.None, "OUT OF ATTEMPTS");
                        }
                    }
                    else
                    {
                        FullLoadingPanel.SetActive(true);
                        //_ = CommunicationController.Instance.PlayTourney("", TourneyId, "free", (data) => { PlayedSuccess(data); });
                        PlayAfterAttemptAnimation(tourneyDet);
                    }
                    UserDataContainer.Instance.tempRewardsCount = tourneyDet._tournament.RewardList.RewardsDistribution.Count;
                }
                else
                {
                    long Money = 0;
                    
                    //UserDataContainer.Instance.currencyList.TryGetValue(tourneyDet._tournament.Currency, out currKey);
                    UserDataContainer.Instance.getUserMoney().TryGetValue(tourneyDet._tournament.Currency, out Money);
                    //Money = 0;
                    if (Money < tourneyDet._tournament.EntryFee)
                    {
                        PlayJoinWatch.interactable = true;
                        UIPanelController.Instance.SetTourneyScrollActive(false);
                        String msg = "YOU DO NOT HAVE SUFFICIENT CHIPS TO PLAY THIS TOURNAMENT.PURCHASE SOME FROM OUR STORE AND ENJOY THE GAME";
                        String BtnTxt = "PURCHASE";
                        string Header = "OUT OF CHIPS";
                        //if (!String.IsNullOrEmpty(JamboxSDKParams.Instance.PurchaseText.ToString()))
                        //    msg = JamboxSDKParams.Instance.PurchaseText.ToString();
                        if (UIPanelController.Instance.EditableMessageData != null)
                        {
                            if (UIPanelController.Instance.EditableMessageData.ContainsKey("OutOfCurrencyMessage"))
                            {
                                string tempMsg = String.Empty;
                                UIPanelController.Instance.EditableMessageData.TryGetValue("OutOfCurrencyMessage", out tempMsg);
                                if(!String.IsNullOrEmpty(tempMsg))
                                {
                                    msg = tempMsg;
                                }
                            }
                            if (UIPanelController.Instance.EditableMessageData.ContainsKey("OutOfCurrencyTitle"))
                            {
                                string tempMsg2 = String.Empty;
                                UIPanelController.Instance.EditableMessageData.TryGetValue("OutOfCurrencyTitle", out tempMsg2);
                                if (!String.IsNullOrEmpty(tempMsg2))
                                {
                                    Header = tempMsg2;
                                }
                            }
                            if (UIPanelController.Instance.EditableMessageData.ContainsKey("OutOfCurrencyButton"))
                            {
                                string tempMsg3 = String.Empty;
                                UIPanelController.Instance.EditableMessageData.TryGetValue("OutOfCurrencyButton", out tempMsg3);
                                if (!String.IsNullOrEmpty(tempMsg3))
                                {
                                    BtnTxt = tempMsg3;
                                }
                            }
                        }
                        string CurrKey = tourneyDet._tournament.Currency;
                        UserDataContainer.Instance.currencyList.TryGetValue(tourneyDet._tournament.Currency, out CurrKey);
                        UIPanelController.Instance.SetTourneyNoMoneyDialogue(true, msg, BtnTxt, true, true, null,
                            (tourneyDet._tournament.EntryFee - (int)Money), CurrKey, tourneyDet._tournament.Tourneyid, prevPanel, Header);
                    }
                    else
                    {
                    #if !UNITY_EDITOR
                        Firebase.Analytics.FirebaseAnalytics.LogEvent("JoinTournament");
                    #endif
                        FullLoadingPanel.SetActive(true);
                        _ = CommunicationController.Instance.JoinTourney("", tourneyDet._tournament.Tourneyid, (data) => { JoinedSucess(data); });
                        UserDataContainer.Instance.UpdateUserMoney (tourneyDet._tournament.EntryFee,
                                                                    tourneyDet._tournament.Currency, false);
                        UIPanelController.Instance.UpdateMoneyOnTourneyPanel();
                    }
                }
            }

            if (prevPanel == Panels.CompletedPanel)
            {
                OnClaimBtnClick();
            }

        }

        void PlayAfterAttemptAnimation(TourneyDetail tourneyDet)
        {
#if !UNITY_EDITOR
            Firebase.Analytics.FirebaseAnalytics.LogEvent("PlayTournament");
#endif
            StartCoroutine(AttemptAnimation(tourneyDet, () =>
            {
                _ = CommunicationController.Instance.PlayTourney("", TourneyId, "free", (data) => { PlayedSuccess(data); });
            }));
        }

        IEnumerator AttemptAnimation(TourneyDetail tourneyDet, System.Action AnimationDone)
        {
            Vector3 tempScale = AttemptCount.transform.localScale;
            ScaleInAnimation _anim = UIAnimations.Instance.ScaleIn(AttemptCount.rectTransform, AttemptCount.transform.localScale, (AttemptCount.transform.localScale + new Vector3(attemptsChangeScale, attemptsChangeScale, attemptsChangeScale)), attemptChangeTime);
            while (!_anim.done)
            {
                yield return null;
            }
            AttemptCount.text = (tourneyDet._joinedTourneyData.CurrentAttempt + 1) + " / " + tourneyDet._tournament.MaxEntry;

            ScaleInAnimation _anim1 = UIAnimations.Instance.ScaleIn(AttemptCount.rectTransform, AttemptCount.transform.localScale, tempScale, attemptChangeTime);
            while (!_anim1.done)
            {
                yield return null;
            }

            AnimationDone();
        }

        public void OnClaimBtnClick()
        {
#if GAME_FIREBASE_ENABLED
            Firebase.Analytics.Parameter[] eventarameters = {
                                new Firebase.Analytics.Parameter("event_id",TourneyId)
                                };
            Firebase.Analytics.FirebaseAnalytics.LogEvent("claim_reward", eventarameters);
#endif

            CompletedTourneyDetail CompTourneyDet = null;
            CompTourneyDet = UserDataContainer.Instance.CompletedTourney.Find((x) => x.LeaderBoardID == TourneyId);

            PlayJoinWatch.interactable = false;
            Debug.LogError("OnClaimBtnClick 111111 >>>" + (CompTourneyDet == null));
            Debug.LogError("OnClaimBtnClick 222222 >>>" + (CompTourneyDet.LeaderBoardID));
            FullLoadingPanel.SetActive(true);
            _ = CommunicationController.Instance.GetClaim("", CompTourneyDet.LeaderBoardID, (data) => { OnClaimSuccess(data); });
        }

        private void OnClaimSuccess(IAPIClaimData dataRcvd)
        {
            FullLoadingPanel.SetActive(false);
            string UpdatedCurrencyKey = dataRcvd.RewardInfo.Virtual.Key;
            UserDataContainer.Instance.currencyList.TryGetValue(dataRcvd.RewardInfo.Virtual.Key, out UpdatedCurrencyKey);
            UIPanelController.Instance.ShowClaimSuccessPanel(dataRcvd.RewardInfo.Virtual.Value, UpdatedCurrencyKey);
            ButtonName.text = "Claimed";
            PlayJoinWatch.interactable = false;
            UserDataContainer.Instance.UpdateUserMoney(dataRcvd.RewardInfo.Virtual.Value,
                                                    dataRcvd.RewardInfo.Virtual.Key, true);
            UIPanelController.Instance.UpdateMoneyOnUI();

            //Updating info on UserDataContainer
            CompletedTourneyDetail CompTourneyDet = null;
            CompTourneyDet = UserDataContainer.Instance.CompletedTourney.Find((x) => x.LeaderBoardID == TourneyId);
            CompTourneyDet.IsClaimed = true;

            if (UserDataContainer.Instance.UnclaimedRewardsCount > 0)
            {
                UserDataContainer.Instance.UnclaimedRewardsCount--;
            }

        }

        public void ShowClaimSuccess(int reward, String Currency)
        {
            ClaimPanel.gameObject.SetActive(true);
            ClaimPanel.UpdateText(reward, Currency);
        }

        private void OnVideoWatched()
        {
            FullLoadingPanel.SetActive(true);
            //PlayJoinBtn.interactable = true;

            TourneyDetail tourneyDet = null;
            if (prevPanel == Panels.FriendlyPanel)
            {
                UserDataContainer.Instance.UpdatedFriendlyData.TryGetValue(TourneyId, out tourneyDet);
            }
            else
            {
                UserDataContainer.Instance.UpdatedTourneyData.TryGetValue(TourneyId, out tourneyDet);
            }
#if !UNITY_EDITOR
            Firebase.Analytics.FirebaseAnalytics.LogEvent("PlayTournament");
#endif
            StartCoroutine(AttemptAnimation(tourneyDet, () =>
            {
                _ = CommunicationController.Instance.PlayTourney("", TourneyId, "adv", (data) => { PlayedSuccess(data); });
            }));
        }

        private void JoinedSucess(IAPIJoinTourney _data)
        {
            FullLoadingPanel.SetActive(false);
            PlayJoinWatch.interactable = true;
            ButtonName.text = "Play";
            UserDataContainer.Instance.UpdateTourneyDataOnJoin(TourneyId, _data);
            TourneyDetail tourneyDet = null;
            UserDataContainer.Instance.UpdatedTourneyData.TryGetValue(TourneyId, out tourneyDet);
            UserMoney.text = UserDataContainer.Instance.GetDisplayCurrencyText();

            JoinMoneyAnimation();
            OnPlayJoinClaimBtnCLick();
        }

        void JoinMoneyAnimation()
        {
            subtractMoneyText.text = "-" + entryFeeAmnt;
            subtractMoneyText.gameObject.SetActive(true);
        }

        private void PlayedSuccess(IApiPlayTourney data)
        {
            FullLoadingPanel.SetActive(false);
            UnityDebug.Debug.Log("On played Success : Error : " + data.Error);
            PlayJoinWatch.interactable = true;
            if (!String.IsNullOrEmpty(data.Error))
                return;
            List<leaderBoardData> newleaderBoardList = new List<leaderBoardData>();
            List<leaderBoardData> templist = new List<leaderBoardData>();
            foreach (var item in data.LeaderRecords)
            {
                leaderBoardData itemData = new leaderBoardData(item);
                templist.Add(itemData);
            }
            newleaderBoardList = templist.OrderBy(o => o.Rank).ToList();
            if (prevPanel == Panels.TourneyPanel)
            {
                UserDataContainer.Instance.UpdateTourneyDataOnPlay(TourneyId, data);
            }
            TourneyDetail TourneyData;
            if (prevPanel == Panels.FriendlyPanel)
            {
                UserDataContainer.Instance.UpdatedFriendlyData.TryGetValue(TourneyId, out TourneyData);
            }
            else
            {
                UserDataContainer.Instance.UpdatedTourneyData.TryGetValue(TourneyId, out TourneyData);
            }
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            UIPanelController.Instance.ShowPanel(Panels.None, Panels.None, metadata);
            Match _matchData = new Match(TourneyId, data.LeaderBoardID, TourneyData._tournament.metadata,
                TourneyData._tournament.Category, null, newleaderBoardList, CommonUserData.Instance.MyAvatarURL,
                CommonUserData.Instance.userName, userAvatarSprite: CommonUserData.Instance.avatarSprite);
            ArenaSDKEvent.Instance.FireOnPlayClick(prevPanel, _matchData);
        }
    }
}
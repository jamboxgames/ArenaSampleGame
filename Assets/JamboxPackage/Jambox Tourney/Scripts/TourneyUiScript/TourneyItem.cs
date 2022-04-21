namespace Jambox.Tourney.UI
{
    using Jambox.Tourney.Connector;
    using Jambox.Tourney.Data;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Collections;
    using Jambox.Common;
    using Jambox.Common.Utility;


    /// <summary>
    /// UI refernces of Items which will be displayed on UI.
    /// </summary>
    public class TourneyItem : TourneyListViewItem
    {
        public Text TourneyName;
        public Text TourneyDesc;
        public Text PlayerCountDetail;
        public GameObject playerCountGameobejct;
        public GameObject Entry;
        public Text EntryFee;
        public GameObject Positions;
        public Text Position;
        public Text winAmt1;
        public Text winAmt2;

        public Text AttemptsDone;
        public GameObject attemptsObj;
        public float attemptChangeTime = 0.1f;
        public float attemptsChangeScale = 0.35f;

        public Text JoinBtnText;
        public Text EndTimeText;
        public Button PlayJoinBtn;
        public GameObject watchAdText;
        public Text BestScore;
        public Button LeaderBoard;

        //After Joined
        public GameObject WinAmountBeforeJoined;
        public GameObject WinAmountAfterJoined;

        //Duels
        public Boolean IsDuel = false;
        private TournamnetData duelData;
        private String _uniqueTId;
        private Panels previousPanel;

        private void Start()
        {
            if(JamboxSDKParams.Instance.ArenaParameters.cardBG != null)
                this.gameObject.GetComponent<Image>().sprite = JamboxSDKParams.Instance.ArenaParameters.cardBG;
        }
        public String UniqueTID
        {
            get { return _uniqueTId; }
            set
            {
                _uniqueTId = value;
                //UpdateUI();
            }
        }

        public void FillUIElements(TournamnetData data)
        {
            previousPanel = Panels.DuelPanel;
            UniqueTID = data.Tourneyid;
            TourneyName.text = data.TourneyName;
            TourneyDesc.text = data.Description;
            //PlayerCountDetail.text = data.CurrentPlayers + " / " + data.MaxPlayers;
            string Currency = data.Currency;
            UserDataContainer.Instance.currencyList.TryGetValue(data.Currency, out Currency);
            EntryFee.text = data.EntryFee + " " + Currency;
            if (winAmt1 != null)
                winAmt1.text = data.RewardList.RewardsDistribution[0].WinAmount + " " + UserDataContainer.Instance.GetCurrencyDisplayTextForKey(data.RewardList.RewardsDistribution[0].CurrencyType);
            if (winAmt2 != null)
                winAmt2.text = data.RewardList.RewardsDistribution[0].WinAmount + " " + UserDataContainer.Instance.GetCurrencyDisplayTextForKey(data.RewardList.RewardsDistribution[0].CurrencyType);
            IsDuel = true;
            duelData = data;
        }

        public void UpdateUI(String TourneyID)
        {
            UniqueTID = TourneyID;
            previousPanel = Panels.TourneyPanel;
            TourneyDetail tourneyDet = null;
            UserDataContainer.Instance.UpdatedTourneyData.TryGetValue(TourneyID, out tourneyDet);
            TourneyName.text = tourneyDet._tournament.TourneyName;
            TourneyDesc.text = tourneyDet._tournament.Description;
            PlayerCountDetail.text = tourneyDet._tournament.MaxPlayers + "";
            Entry.SetActive(true);
            playerCountGameobejct.SetActive(true);
            Positions.SetActive(false);
            attemptsObj.SetActive(false);
            string Currency = tourneyDet._tournament.Currency;
            UserDataContainer.Instance.currencyList.TryGetValue(tourneyDet._tournament.Currency, out Currency);
            EntryFee.text = tourneyDet._tournament.EntryFee + " " + Currency;
            if(winAmt1 != null)
                winAmt1.text = tourneyDet._tournament.RewardList.RewardsDistribution[0].WinAmount + " " + UserDataContainer.Instance.GetCurrencyDisplayTextForKey(tourneyDet._tournament.RewardList.RewardsDistribution[0].CurrencyType);
            if (winAmt2 != null)
                winAmt2.text = tourneyDet._tournament.RewardList.RewardsDistribution[0].WinAmount + " " + UserDataContainer.Instance.GetCurrencyDisplayTextForKey(tourneyDet._tournament.RewardList.RewardsDistribution[0].CurrencyType);
            AttemptsDone.text = "Attempts : " + "0 / " + tourneyDet._tournament.MaxEntry;
            TimeSpan elapsed = DateTime.Parse(tourneyDet._tournament.EndTime).ToUniversalTime().Subtract(DateTime.UtcNow);
            //2021-07-19T07:54:36.738535Z
            EndTimeText.text = "ENDS IN : " + EndTimeInFormat(elapsed);
            PlayJoinBtn.interactable = true;
            JoinBtnText.text = "Join";
            watchAdText.SetActive(false);
            UpdateJoinedStatus(tourneyDet);
        }

        string EndTimeInFormat(TimeSpan _elasped)
        {
            string timeString = "";

            if (_elasped.Days >= 2)
            {
                timeString = _elasped.Days + "D";
            }else if (_elasped.Days >= 1)
            {
                if (_elasped.Hours == 0)
                {
                    timeString = _elasped.Days + "D ";
                    return timeString;
                }
                timeString = _elasped.Days + "D " + _elasped.Hours + "H";
            }
            else if(_elasped.Hours >= 1)
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
            }else if (_elasped.Minutes < 1)
            {
                timeString = "0M";
            }

            return timeString;
        }

        private void UpdateJoinedStatus (TourneyDetail tourneyDet)
        {
            if(tourneyDet.isJoined)
            {
                JoinBtnText.text = "Play";
                if (attemptsObj != null)
                {
                    attemptsObj.SetActive(true);
                    AttemptsDone.text = tourneyDet._joinedTourneyData.CurrentAttempt + " / " + tourneyDet._tournament.MaxEntry;
                    PlayerCountDetail.text = tourneyDet._joinedTourneyData.CurrentPlayers + " / " + tourneyDet._tournament.MaxPlayers;
                }

                Entry.SetActive(false);
                Positions.SetActive(true);
                Position.text = tourneyDet._joinedTourneyData.MyRank + "/" + tourneyDet._joinedTourneyData.CurrentPlayers;
                updatePlayableStatus(tourneyDet);

                playerCountGameobejct.SetActive(false);
            }
            else
            {
                BestScore.gameObject.SetActive(false);
            }
        }

        public void updatePlayableStatus (TourneyDetail tourneyDet)
        {
            if(tourneyDet._joinedTourneyData.CurrentAttempt >= tourneyDet._tournament.MaxEntry)
            {
                if(tourneyDet._tournament.PlayWithVideoAD)
                {
                    JoinBtnText.text = "";
                    //JoinBtnText.gameObject.SetActive(false);
                    watchAdText.SetActive(true);
                }
                else
                {
                    PlayJoinBtn.interactable = false;
                }
            }
        }
        public void PlayAfterPurchase()
        {
            PlayJoinBtn.interactable = false;
            TourneyDetail tourneyDet = null;
            UserDataContainer.Instance.UpdatedTourneyData.TryGetValue(UniqueTID, out tourneyDet);
            long Money = 0;

            //UserDataContainer.Instance.currencyList.TryGetValue(tourneyDet._tournament.Currency, out CurrencyKey);
            UserDataContainer.Instance.getUserMoney().TryGetValue(tourneyDet._tournament.Currency, out Money);
            //Money = 0;
            if (Money < tourneyDet._tournament.EntryFee)
            {
                PlayJoinBtn.interactable = true;
            }
            else
            {
        #if !UNITY_EDITOR
                Firebase.Analytics.FirebaseAnalytics.LogEvent("JoinTournament");
        #endif
                UIPanelController.Instance.LoadingDialogue(true, false);
                _ = CommunicationController.Instance.JoinTourney( tourneyDet._tournament.Tourneyid, (data) => { JoinedSucess(data); }, (errorMsg) => { UIPanelController.Instance.ErrorFromServerRcvd(errorMsg); });
            }
        }
        public void OnVideoWatched ()
        {
            UIPanelController.Instance.LoadingDialogue(true, false);
            UIPanelController.Instance.SetTourneyScrollActive(true);
            //PlayJoinBtn.interactable = true;
            UnityDebug.Debug.Log("Watch Video >>>");

#if !UNITY_EDITOR
            Firebase.Analytics.FirebaseAnalytics.LogEvent("PlayTournament");
#endif

            TourneyDetail tourneyDet = null;
            UserDataContainer.Instance.UpdatedTourneyData.TryGetValue(UniqueTID, out tourneyDet);
            _ = CommunicationController.Instance.PlayTourney(UniqueTID, "adv", (data) => { PlayedSuccess(data); }, (errorMsg) => { UIPanelController.Instance.ErrorFromServerRcvd(errorMsg); });    
        }

        public void OnPlayDuelClicked(bool isPostPurchase = false)
        {
            long Money = 0;
            TournamnetData TourneyDet = null;
            UserDataContainer.Instance.MyDuels.TryGetValue(UniqueTID, out TourneyDet);
            UserDataContainer.Instance.getUserMoney().TryGetValue(TourneyDet.Currency, out Money);
                            
            if (Money < TourneyDet.EntryFee)
            {
                if (isPostPurchase)
                    return;
                //PlayJoinBtn.interactable = true;
                UIPanelController.Instance.SetTourneyScrollActive(false);
                String msg = "YOU DO NOT HAVE SUFFICIENT CHIPS TO PLAY THIS TOURNAMENT.PURCHASE SOME FROM OUR STORE AND ENJOY THE GAME";
                String BtnTxt = "PURCHASE";
                string Header = "OUT OF CHIPS";
                //if (!String.IsNullOrEmpty(JamboxSDKParams.Instance.PurchaseText.ToString()))
                    //msg = JamboxSDKParams.Instance.PurchaseText.ToString();
                if(UIPanelController.Instance.EditableMessageData != null)
                {
                    if (UIPanelController.Instance.EditableMessageData.ContainsKey("OutOfCurrencyMessage"))
                    {
                        string tempMsg = String.Empty;
                        UIPanelController.Instance.EditableMessageData.TryGetValue("OutOfCurrencyMessage", out tempMsg);
                        if (!String.IsNullOrEmpty(tempMsg))
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
                string CurrKey = TourneyDet.Currency;
                UserDataContainer.Instance.currencyList.TryGetValue(TourneyDet.Currency, out CurrKey);
                UIPanelController.Instance.SetTourneyNoMoneyDialogue(true, msg, BtnTxt, true, true, null,
                        (TourneyDet.EntryFee - (int)Money), CurrKey, TourneyDet.Tourneyid, Panels.DuelPanel, Header);
            }
            else
            {
                //UserDataContainer.Instance.UpdateUserMoney(TourneyDet.EntryFee, TourneyDet.Currency, false);
                Dictionary<string, string> metadata = new Dictionary<string, string>();
                metadata.Add("tourneyid", UniqueTID);
                UIPanelController.Instance.ShowPanel(Panels.MatchMakingPanel, Panels.DuelPanel, metadata);
            }
        }

        public void OnPlayBtnClick ()
        {
            if (IsDuel)
            {
                OnPlayDuelClicked();
                return;
            }
            TourneyDetail tourneyDet = null;
            UserDataContainer.Instance.UpdatedTourneyData.TryGetValue(UniqueTID, out tourneyDet);
            PlayJoinBtn.interactable = false;
            if (tourneyDet.isJoined)
            {
                if (tourneyDet._joinedTourneyData.CurrentAttempt >= tourneyDet._tournament.MaxEntry)
                {
                    if (tourneyDet._tournament.PlayWithVideoAD)
                    {
                        UIPanelController.Instance.SetTourneyScrollActive(false);
                        UIPanelController.Instance.OnWatchVideo(tourneyDet._tournament.Tourneyid, Panels.TourneyPanel);
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
                    //_ = CommunicationController.Instance.PlayTourney("", UniqueTID, "free", (data) => { PlayedSuccess(data); });
                    PlayAfterAttemptAnimation(tourneyDet);
                    UIPanelController.Instance.LoadingDialogue(true, false);
                }

                UserDataContainer.Instance.tempRewardsCount = tourneyDet._tournament.RewardList.RewardsDistribution.Count;

            }
            else
            {
                long Money = 0;
                
                //UserDataContainer.Instance.currencyList.TryGetValue(tourneyDet._tournament.Currency, out CurrencyKey);
                UserDataContainer.Instance.getUserMoney().TryGetValue(tourneyDet._tournament.Currency, out Money);
                //Money = 0;
                if (Money < tourneyDet._tournament.EntryFee)
                {
                    PlayJoinBtn.interactable = true;
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
                            if (!String.IsNullOrEmpty(tempMsg))
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
                        (tourneyDet._tournament.EntryFee - (int)Money), CurrKey, tourneyDet._tournament.Tourneyid, Panels.TourneyPanel, Header);
                }
                else
                {
                #if !UNITY_EDITOR
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("JoinTournament");
                #endif
                    UIPanelController.Instance.LoadingDialogue(true, false);
                    _ = CommunicationController.Instance.JoinTourney(tourneyDet._tournament.Tourneyid, (data) => { JoinedSucess(data); }, (errorMsg) => { UIPanelController.Instance.ErrorFromServerRcvd(errorMsg); });
                }
            }
        }



        void PlayAfterAttemptAnimation(TourneyDetail tourneyDet)
        {
#if !UNITY_EDITOR
            Firebase.Analytics.FirebaseAnalytics.LogEvent("JoinTournament");
#endif
            StartCoroutine(AttemptAnimation(tourneyDet, () =>
            {
                _ = CommunicationController.Instance.PlayTourney(UniqueTID, "adv", (data) => { PlayedSuccess(data); }, (errorMsg) => { UIPanelController.Instance.ErrorFromServerRcvd(errorMsg); });
            }));
        }

        IEnumerator AttemptAnimation(TourneyDetail tourneyDet, System.Action AnimationDone)
        {
            Vector3 tempScale = AttemptsDone.transform.localScale;
            ScaleInAnimation _anim = UIAnimations.Instance.ScaleIn(AttemptsDone.rectTransform, AttemptsDone.transform.localScale, (AttemptsDone.transform.localScale + new Vector3(attemptsChangeScale, attemptsChangeScale, attemptsChangeScale)), attemptChangeTime);
            while (!_anim.done)
            {
                yield return null;
            }
            AttemptsDone.text = (tourneyDet._joinedTourneyData.CurrentAttempt + 1) + " / " + tourneyDet._tournament.MaxEntry;

            ScaleInAnimation _anim1 = UIAnimations.Instance.ScaleIn(AttemptsDone.rectTransform, AttemptsDone.transform.localScale, tempScale, attemptChangeTime);
            while (!_anim1.done)
            {
                yield return null;
            }

            AnimationDone();
        }

        public void OnInfoButtonClick()
        {
            UIPanelController.Instance.ShowTourneyInfo(UniqueTID);
        }

        private void JoinedSucess (IAPIJoinTourney _data)
        {
            TourneyDetail tourneyDet = null;
            UserDataContainer.Instance.UpdatedTourneyData.TryGetValue(UniqueTID, out tourneyDet);
            UserDataContainer.Instance.UpdateUserMoney(tourneyDet._tournament.EntryFee, tourneyDet._tournament.Currency, false);
            UIPanelController.Instance.UpdateMoneyOnTourneyPanel();

            UIPanelController.Instance.LoadingDialogue(false);
            UnityDebug.Debug.Log("Joined Tournament Sucessfully >>>>>> ID : " + UniqueTID);
            PlayJoinBtn.interactable = true;
            JoinBtnText.text = "Play";
            UserDataContainer.Instance.UpdateTourneyDataOnJoin(UniqueTID , _data);

            UpdateJoinedStatus(tourneyDet);

            UIPanelController.Instance.SubtractMoneyAnimation(tourneyDet._tournament.EntryFee);
            OnPlayBtnClick();
#if GAME_FIREBASE_ENABLED
            string eventType = (tourneyDet._tournament.Category == 1) ? "tournament" : ((tourneyDet._tournament.Category == 2) ? "duel" : "friendly");
            Firebase.Analytics.Parameter[] eventarameters = {
                                new Firebase.Analytics.Parameter("event_type", eventType),
                                new Firebase.Analytics.Parameter("event_id",tourneyDet._tournament.Tourneyid)
                                };
            Firebase.Analytics.FirebaseAnalytics.LogEvent("arena_event_join", eventarameters);
#endif
        }

        private void PlayedSuccess (IApiPlayTourney data )
        {

            UIPanelController.Instance.LoadingDialogue(false);

            PlayJoinBtn.interactable = true;
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
            UserDataContainer.Instance.UpdateTourneyDataOnPlay(UniqueTID, data);
            TourneyDetail tourneyDet = null;
            UserDataContainer.Instance.UpdatedTourneyData.TryGetValue(UniqueTID, out tourneyDet);
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            UIPanelController.Instance.ShowPanel(Panels.None, Panels.None, metadata);

            /*
            Dictionary<string, string> tempMetaData = new Dictionary<string, string>();
            tempMetaData.Add("game_type", "timed");*/

            Match _matchData = new Match(UniqueTID, data.LeaderBoardID, tourneyDet._tournament.metadata,
                tourneyDet._tournament.Category, null, newleaderBoardList, CommonUserData.Instance.MyAvatarURL,
                CommonUserData.Instance.userName, userAvatarSprite: CommonUserData.Instance.avatarSprite);

            /*
            Match _matchData = new Match(UniqueTID, data.LeaderBoardID, tempMetaData,
                tourneyDet._tournament.Category, null, newleaderBoardList, UserDataContainer.Instance.MyAvatarURL,
                UserDataContainer.Instance.name);*/

            ArenaSDKEvent.Instance.FireOnPlayClick(Panels.TourneyPanel, _matchData);
        }

        public void OnMoreButtonClick ()
        {
            Panels thisPanel = Panels.TourneyPanel;
            if(IsDuel)
            {
                thisPanel = Panels.DuelPanel;
            }
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            metadata.Add("tourneyid", UniqueTID);
            UIPanelController.Instance.ShowPanel(Panels.DetailsPanel, thisPanel, metadata);
        }

        public void LeaderBoardBtnCLick()
        {
            TourneyDetail tourneyDet = null;
            UserDataContainer.Instance.UpdatedTourneyData.TryGetValue(UniqueTID, out tourneyDet);
            if (tourneyDet.isJoined)
            {
                UIPanelController.Instance.ShowLeaderBoard(tourneyDet._joinedTourneyData.LeaderBoardID);
            }
        }
    }
}

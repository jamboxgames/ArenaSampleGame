namespace Jambox.Tourney.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    //using System.Diagnostics.PerformanceData;
    using System.Linq;
    using Jambox.Common;
    using Jambox.Common.Utility;
    using Jambox.Tourney.Connector;
    using Jambox.Tourney.Data;
    using UnityEngine;
    using UnityEngine.UI;

    [System.Serializable]
    public class leaderBoardData
    {
        public int AttemptCount;
        public string PlayerId;
        public int Rank;
        public int Score;
        public String DisplayScore;
        public string Username;
        public string PlayerImageURL;

        public leaderBoardData (IApiLeaderRecord dataNew)
        {
            AttemptCount = dataNew.AttemptCount;
            PlayerId = dataNew.PlayerId;
            Rank = dataNew.Rank;
            Score = dataNew.Score;
            DisplayScore = dataNew.DisplayScore;
            Username = dataNew.Username;
            PlayerImageURL = dataNew.AvatarUrl;
        }
        //For Realtime leaderboard
        public leaderBoardData(string _username,int _score)
        {
            AttemptCount = 0;
            PlayerId = null;
            Rank = 0;
            Score = _score;
            Username = _username;
        }
    }

    public class SubmitScore : MonoBehaviour
    {
        public Text currencyText;
        public Image CurrencyIcon;
        public Text SubmitScoreText;
        public Text EndTimeText;
        public Button SubmitButton;
        private long _Score;
        private string _displayScore;
        private string TourneyId;
        private string LeaderBoardID;
        //public GameObject mainPanel;
        public GameObject ScoreDetail;
        public RewarDetail rewardDetail;
        public LeaderBoardListView theList;
        public GameObject LbLoadingPanel;
        public LeaderboardHeaders LBHeader;
        public Button LBrefreshButton;
        public Rotate TopLoadingPanel;
        public GameObject FullLoadingPanel;
        private List<leaderBoardData> leaderBoardList;

        private bool showConfetti = false;
        public GameObject celebrations;

        private Panels prevPanel;
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

        private void OnEnable()
        {
            UpdateCurrency();
        }

        public void UpdateSubmitScoreData(Panels prevPanelNew)
        {
            TourneyId = UIPanelController.Instance.tempScore.TourneyId;
            TourneyDetail tourneyDet = null;
            prevPanel = prevPanelNew;
            if (prevPanel == Panels.FriendlyPanel)
            {
                UserDataContainer.Instance.UpdatedFriendlyData.TryGetValue(TourneyId, out tourneyDet);
                LeaderBoardID = tourneyDet._joinedTourneyData.LeaderBoardID;
            }
            else
            {
                UserDataContainer.Instance.UpdatedTourneyData.TryGetValue(TourneyId, out tourneyDet);
                LeaderBoardID = tourneyDet._joinedTourneyData.LeaderBoardID;
            }
            _Score = UIPanelController.Instance.tempScore.Score;
            _displayScore = UIPanelController.Instance.tempScore.displayScore;
            UIPanelController.Instance.tempScore = null;
            //rewardDetail.UpdateUi(TourneyId);
            TimeSpan elapsed = DateTime.Parse(tourneyDet._tournament.EndTime).ToUniversalTime().Subtract(DateTime.UtcNow);
            EndTimeText.text = "ENDS IN : " + EndTimeInFormat(elapsed);

            LbLoadingPanel.gameObject.SetActive(true);
            TopLoadingPanel.gameObject.SetActive(true);
            ScoreDetail.SetActive(false);
            SubmitButtonClick(prevPanel);
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

        public void UpdateCurrency()
        {
            currencyText.text = UserDataContainer.Instance.GetDisplayCurrencyText();
            CurrencyIcon.sprite = JamboxSDKParams.Instance.ArenaParameters.CoinBG;
        }

        public void SubmitButtonClick(Panels prevPanel)
        {
            RealtimeLeaderboard.Instance.DisableRealtimeLbGameobject();
            LeaderboardRefreshing(true);
            _ = CommunicationController.Instance.SubmitScore(LeaderBoardID, _Score, _displayScore, (data) => { ScoreSubmitted(data, prevPanel); }, (errorMsg) => { UIPanelController.Instance.ErrorFromServerRcvd(errorMsg); });
        }

        public void OnBackbuttonClick()
        {
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            UIPanelController.Instance.ShowPanel(prevPanel, Panels.None, metadata);
        }

        public void LoadingDialog(bool isShow, bool stillInteractable = true)
        {
            if (isShow)
            {
                FullLoadingPanel.SetActive(true);
                if (stillInteractable) {
                    FullLoadingPanel.GetComponent<Image>().enabled = false;
                }
            }
            else
            {
                FullLoadingPanel.SetActive(false);
            }
        }

        private void ScoreSubmitted(IApiSubmitScore data, Panels prevPanel)
        {
            UnityDebug.Debug.Log("ScoreSubmitted Successfully >>>>>>");
            if (prevPanel == Panels.FriendlyPanel)
                UserDataContainer.Instance.UpdateFriendlyDataOnScore(TourneyId, data);
            else
                UserDataContainer.Instance.UpdateTourneyDataOnScore(TourneyId, data);
            TopLoadingPanel.gameObject.SetActive(false);
            ScoreDetail.SetActive(true);
            ScoreDetail.GetComponent<ScoreDisplay>().ShowResult(data, TourneyId, prevPanel, ((dataNew) =>
            {
                showConfetti = dataNew;
            }));
            _ = CommunicationController.Instance.GetLeaderBoard(data.LeaderBoardID, (dataN) => { OnLeaderBoardRcvd(dataN);}, (errorMsg) => { UIPanelController.Instance.ErrorFromServerRcvd(errorMsg); }, this.gameObject);
        }

        public void RefreshLeaderBoard ()
        {
            theList.RowCount = 0;
            LbLoadingPanel.gameObject.SetActive(true);
            LeaderboardRefreshing(true);
            _ = CommunicationController.Instance.GetLeaderBoard(LeaderBoardID, (dataN) => {
                OnLeaderBoardRcvd(dataN);
            }, (errorMsg) => { UIPanelController.Instance.ErrorFromServerRcvd(errorMsg); }, this.gameObject);
        }
        private void OnLeaderBoardRcvd (IApiLeaderRecordList data)
        {
            if(LbLoadingPanel != null && LbLoadingPanel.activeInHierarchy)
                LbLoadingPanel.gameObject.SetActive(false);
            LeaderboardRefreshing(false);
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
                ScoreDetail.GetComponent<ScoreDisplay>().UpdateRankOnLBrefresh(_myPlayerData.Rank, leaderBoardList.Count);
            }

            ShowLeaderBoard();
            CheckForCelebration(leaderBoardList);
        }

        void LeaderboardRefreshing(bool value)
        {
            LBrefreshButton.interactable = (!value);
            LBrefreshButton.GetComponent<Image>().enabled = (!value);
            LBrefreshButton.transform.GetChild(0).gameObject.SetActive(value);
        }

        void CheckForCelebration(List<leaderBoardData> lbList)
        {
            int _myRank;
            leaderBoardData _data = lbList.Find((x) => x.PlayerId == JamboxController.Instance.getMyuserId());
            if (_data != null)
            {
                _myRank = _data.Rank;
                if (_myRank <= UserDataContainer.Instance.tempRewardsCount)
                {
                    if (showConfetti)
                    {
                        celebrations.SetActive(true);
                        showConfetti = false;
                    }    
                }
            }
        }

        public void EnableMainPanel()
        {
            this.gameObject.SetActive(false);
        }

        public void ShowLeaderBoard()
        {
            theList.gameObject.SetActive(true);
            theList.ItemCallback = PopulateItem;
            theList.RowCount = leaderBoardList.Count;
        }

        private void PopulateItem(LeaderBoardListViewItem item, int rowIndex)
        {
            StartCoroutine(InitItem(item, rowIndex));
        }
        private IEnumerator InitItem (LeaderBoardListViewItem item, int rowIndex)
        {
            yield return new WaitForSeconds(0.05f);
            var tDet = item as LeaderBoardItem;
            if (rowIndex < leaderBoardList.Count)
            {
                bool lastItem = false;
                if (rowIndex == leaderBoardList.Count - 1)
                    lastItem = true;
                tDet.FillItem(leaderBoardList[rowIndex], lastItem);
            }
        }
    }
}

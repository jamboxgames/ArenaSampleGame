namespace Jambox.Tourney.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using Jambox.Tourney.UI;
    using System.Linq;
    using UnityEngine.UI;
    using Jambox.Common.Utility;
    using Jambox.Tourney.Connector;
    using Jambox.Common;

    public class RealtimeLeaderboard : MonoSingleton<RealtimeLeaderboard>
    {

        private GameObject InGameJamboxCanvas;

        public List<leaderBoardData> leaderboardDataList;
        private bool highScoreIsTopRank;

        private int myPlayerIndex;
        private RealtimeLbListView thelist;
        private CanvasGroup lbObject;
        private RectTransform lbRect;
        private GameObject fullLb;
        public GameObject onlyTextLb;
        public Text onlyTexttext;

        private bool disapperaingLB = false;
        private bool fullLeaderboard;

        public float lbShowTime = 3f;
        private float timer = 0f;
        public float fadeSpeed = 15f;

        public float tweenSpeed = 10f;
        public float xOffset = 550f;

        private Vector2 targetPos;
        private Vector2 orgPos;
        private Vector2 hidePos;

        private float targetAlpha = 0f;

        private bool IsLeaderboardShown = false;

        private void Update()
        {
            if (disapperaingLB)
            {
                if (lbObject != null)
                {

                    if (IsLeaderboardShown)
                    {
                        if (timer <= 0f)
                        {
                            ShowRealtimeLB(false);
                        }
                        else
                        {
                            timer -= Time.deltaTime;
                        }
                    }

                    if (lbObject.alpha != targetAlpha)
                    {
                        float _alpha = Mathf.Lerp(lbObject.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
                        lbObject.alpha = _alpha;
                    }

                    if (lbRect.anchoredPosition != targetPos)
                    {
                        lbRect.anchoredPosition = Vector2.Lerp(lbRect.anchoredPosition, targetPos, tweenSpeed * Time.deltaTime);
                    }
                }
            }
        }

        public void ShowRealtimeLB(bool value)
        {
            if (value)
            {
                targetPos = orgPos;
                targetAlpha = 1f;
                timer = lbShowTime;
                IsLeaderboardShown = true;
            }
            else
            {
                targetPos = hidePos;
                targetAlpha = 0f;
                IsLeaderboardShown = false;
            }
        }

        public void InitializeRealtimeLeaderboard(List<leaderBoardData> data, bool scrollable, int noOfRecordsToShow, bool disappearing, bool fullLeaderboard, ESortOrder tourneySortOrder)
        {
            this.highScoreIsTopRank = (tourneySortOrder == ESortOrder.ESortOrderDescending);
            this.disapperaingLB = disappearing;
            this.fullLeaderboard = fullLeaderboard;

            if (InGameJamboxCanvas != null)
            {
                Destroy(InGameJamboxCanvas);
                InGameJamboxCanvas = null;
                //leaderboard = null;
            }

            if (UIPanelController.Instance.IsLandScape())
                InGameJamboxCanvas = (GameObject)Instantiate(Resources.Load("JamboxInGameCanvas_Landscape"));
            else
                InGameJamboxCanvas = (GameObject)Instantiate(Resources.Load("JamboxInGameCanvas"));
            thelist = FindObjectOfType<RealtimeLbListView>();
            lbObject = InGameJamboxCanvas.transform.GetChild(0).GetChild(0).GetComponent<CanvasGroup>();
            lbRect = InGameJamboxCanvas.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();

            fullLb = lbRect.GetChild(0).gameObject;
            onlyTextLb = lbRect.GetChild(1).gameObject;
            onlyTexttext = onlyTextLb.transform.GetChild(0).GetComponent<Text>();
            if (fullLeaderboard)
            {
                fullLb.SetActive(true);
                onlyTextLb.SetActive(false);
            }
            else
            {
                fullLb.SetActive(false);
                onlyTextLb.SetActive(true);
            }

            //Initialize Pos Animation Values
            orgPos = lbRect.anchoredPosition;
            hidePos = new Vector3(lbRect.anchoredPosition.x + xOffset, lbRect.anchoredPosition.y);

            //Doing Pos Animation
            if (disapperaingLB)
            {
                lbRect.anchoredPosition = hidePos;
                ShowRealtimeLB(true);
            }

            thelist.ToggleScrolling(scrollable);
            thelist.SetNoRecordsToShow(noOfRecordsToShow);

            leaderboardDataList = new List<leaderBoardData>();

            leaderBoardData myPlayerData = null;
            foreach (var v in data)
            {
                if (v.PlayerId != CommunicationController.Instance.getMyuserId())
                {
                    leaderboardDataList.Add(v);
                }
            }

            //Adding my player
            int tempRankForOrdering = 0;
            if (highScoreIsTopRank)
                tempRankForOrdering = data.Count;
            myPlayerData = new leaderBoardData(CommunicationController.Instance.getMyuserId(), tempRankForOrdering, CommonUserData.Instance.userName, 0);
            leaderboardDataList.Add(myPlayerData);

            //Ordering the list
            if (highScoreIsTopRank)
            {
                leaderboardDataList = leaderboardDataList.OrderByDescending(s => s.Score).ThenBy(r => r.Rank).ToList();
            }
            else
            {
                leaderboardDataList = leaderboardDataList.OrderBy(s => s.Score).ThenBy(r => r.Rank).ToList();
            }

            //Redefining the ranks
            RedefineRanks(0, leaderboardDataList.Count - 1);
            onlyTexttext.text = "YOU HAVE REACHED RANK " + (myPlayerData.Rank);

            myPlayerIndex = leaderboardDataList.IndexOf(myPlayerData);
            thelist.UpdatePlayerIndex(myPlayerIndex);

            thelist.ItemCallback = PopulateItem;
            thelist.RowCount = leaderboardDataList.Count;

        }

        public void DisableRealtimeLbGameobject()
        {
            if (InGameJamboxCanvas != null)
            {
                Destroy(InGameJamboxCanvas);
                InGameJamboxCanvas = null;
            }
        }

        private void PopulateItem(RealtimeLbListViewItem item, int rowIndex)
        {
            UnityDebug.Debug.LogInfo("Inside PopulateItem : LeaderBoard Count : " + leaderboardDataList.Count + "  rowIndex : " + rowIndex);
            var tDet = item as RealtimeLBitem;
            UnityDebug.Debug.LogInfo("Inside PopulateItem 222222: LeaderBoard Count : " + leaderboardDataList.Count + "  rowIndex : " + rowIndex);
            if (rowIndex < leaderboardDataList.Count)
            {
                UnityDebug.Debug.LogInfo("Inside PopulateItem 333333: LeaderBoard Count : " + leaderboardDataList.Count + "  rowIndex : " + rowIndex);
                leaderBoardData r = leaderboardDataList[rowIndex];
                tDet.SetDetails(r.PlayerId, r.Rank, r.Username, r.Score);
            }
        }

        public void UpdatePlayerScore(long _score)
        {
            leaderBoardData myPlayer = leaderboardDataList[myPlayerIndex];
            myPlayer.Score = (int)_score;

            if (highScoreIsTopRank)
            {
                leaderboardDataList = leaderboardDataList.OrderByDescending(s => s.Score).ThenBy(r => r.Rank).ToList();
            }
            else
            {
                leaderboardDataList = leaderboardDataList.OrderBy(s => s.Score).ThenBy(r => r.Rank).ToList();
            }

            int tempPrevIndex = myPlayerIndex;
            myPlayerIndex = leaderboardDataList.IndexOf(myPlayer);

            if (myPlayerIndex != tempPrevIndex)
            {
                int startIndex = (myPlayerIndex < tempPrevIndex) ? myPlayerIndex : tempPrevIndex;
                int endtIndex = (myPlayerIndex > tempPrevIndex) ? myPlayerIndex : tempPrevIndex;
                RedefineRanks(startIndex, endtIndex);

                if (disapperaingLB)
                {
                    ShowRealtimeLB(true);
                }
            }
            onlyTexttext.text = "YOU HAVE REACHED RANK " + (myPlayer.Rank);

            if (fullLeaderboard)
            {
                thelist.UpdatePlayerScore(_score, myPlayerIndex);
            }
        }

        void RedefineRanks(int startIndex, int endIndex)
        {
            for(int i = startIndex; i <= endIndex; i++)
            {
                leaderboardDataList[i].Rank = i + 1;
            }
        }

    }

    [Serializable]
    public class RealtimeLBclass
    {
        public string username;
        public long score;
        public Sprite avatar;

        public RealtimeLBclass(string _username, long _score, Sprite _avatar)
        {
            username = _username;
            score = _score;
            avatar = _avatar;
        }
    }
}


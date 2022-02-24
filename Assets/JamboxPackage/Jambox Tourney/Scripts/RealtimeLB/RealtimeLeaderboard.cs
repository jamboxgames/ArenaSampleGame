namespace Jambox.Tourney.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Jambox.Common.Utility;
    using System;
    using Jambox.Tourney.UI;
    using System.Linq;
    using UnityEngine.UI;
    using Jambox.Common;

    public class RealtimeLeaderboard : MonoSingleton<RealtimeLeaderboard>
    {

        private GameObject InGameJamboxCanvas;
        //private RealtimeLBlist leaderboard;
        public List<leaderBoardData> leaderboardDataList;

        private int myPlayerIndex;
        private RealtimeLbListView thelist;

        public void InitializeRealtimeLeaderboard(List<leaderBoardData> data, bool scrollable, int _noOfRecordsToShow)
        {

            leaderboardDataList = new List<leaderBoardData>();

            if (InGameJamboxCanvas != null)
            {
                Destroy(InGameJamboxCanvas);
                InGameJamboxCanvas = null;
                //leaderboard = null;
            }

            InGameJamboxCanvas  = (GameObject) Instantiate(Resources.Load("JamboxInGameCanvas"));
            thelist = FindObjectOfType<RealtimeLbListView>();

            thelist.ToggleScrolling(scrollable);
            thelist.SetNoRecordsToShow(_noOfRecordsToShow);

            List<leaderBoardData> sortedList = data.OrderBy(p => p.Rank).ToList();
            //leaderboardDataList = sortedList;

            leaderBoardData myPlayerData = null;
            foreach(var v in sortedList)
            {
                if (v.PlayerId == JamboxController.Instance.getMyuserId())
                {
                    myPlayerData = v;
                }
                else
                {
                    leaderboardDataList.Add(v);
                }
            }

            //Adding my player
            myPlayerIndex = leaderboardDataList.Count;
            leaderBoardData myPlayerDataNew = new leaderBoardData(CommonUserData.Instance.userName, 0);
            myPlayerDataNew.PlayerId = JamboxController.Instance.getMyuserId();
            leaderboardDataList.Add(myPlayerDataNew);

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
            var tDet = item as RealtimeLBitem;
            if (rowIndex < leaderboardDataList.Count)
            {
                leaderBoardData r = leaderboardDataList[rowIndex];
                tDet.SetDetails(r.PlayerId, rowIndex + 1, r.Username, r.Score);
            }
        }

        public void UpdatePlayerScore(long _score)
        {
            leaderBoardData tempPlayer = leaderboardDataList[myPlayerIndex];
            leaderboardDataList[myPlayerIndex].Score = (int)_score;
            leaderboardDataList = leaderboardDataList.OrderByDescending(p => p.Score).ToList();

            myPlayerIndex = leaderboardDataList.IndexOf(tempPlayer);

            thelist.UpdatePlayerScore(_score, myPlayerIndex);
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


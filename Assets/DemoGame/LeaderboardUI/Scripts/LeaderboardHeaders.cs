namespace Jambox.Leaderboard.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class LeaderboardHeaders : MonoBehaviour
    {
        public Text RankHeader;
        public Text ScoreHeader;
        public Text NameHeader;
        public GameObject LBItemHeader;

        public void SetLeaderBoardHeader(string score, string name = "", string rank = "")
        {
            if (!string.IsNullOrEmpty(score))
            {
                ScoreHeader.text = score;
            }
            if (!string.IsNullOrEmpty(name))
            {
                NameHeader.text = name;
            }
            if (!string.IsNullOrEmpty(rank))
            {
                RankHeader.text = rank;
            }
            LBItemHeader.SetActive(true);
        }
    }
}

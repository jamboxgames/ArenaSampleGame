namespace Jambox.Leaderboard.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using JBX.Leaderboard.Controller;
    using Jambox.Tourney.Connector;
    using UnityEngine;
    using UnityEngine.UI;

    public class RewardItem : RewardListViewItem
    {
        public Text position;
        public Text reward;

        public void FillItem(LeaderBoardRewardData _data)
        {
            if (_data.StartRank == _data.EndRank)
                position.text = _data.StartRank + "";
            else
                position.text = _data.StartRank + " - " + _data.EndRank;
            foreach (var rewData in _data.VirtualRewards)
            {
                reward.text = rewData.Value + "";
            }
        }
    }
}

namespace Jambox.Leaderboard.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using Jambox.Common;
    using JBX.Leaderboard.Controller;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIPendingRewardItem : UIPendingRewardListViewItem
    {
        public Text Name;
        public Text Rank;
        public Text Score;
        public Text rewardText;
        public Image rewardImage;
        public Button claimBtn;
        public Text claimBtnText;

        private string leaderboardId;
        private UIPendingRewardPanel pendingRewardsPanel;

        public void FillItem(UIPendingRewardData data, Sprite rewardSprite, UIPendingRewardPanel _pendingRewardsPanel)
        {
            leaderboardId = data.LeaderboardId;
            pendingRewardsPanel = _pendingRewardsPanel;

            Name.text =  data.LeaderboardName;
            Rank.text = "Rank : " + data.Rank;
            Score.text = "Score : " + data.Score;
            foreach (var rewData in data.VirtualRewards)
            {
                rewardText.text = "" + rewData.Value;
            }
            rewardImage.sprite = rewardSprite;
            RewardClaimed(data.IsClaimed);
        }

        void RewardClaimed(bool value)
        {
            if (value)
            {
                claimBtn.interactable = false;
                claimBtnText.text = "Claimed";
            }
            else
            {
                claimBtn.interactable = true;
                claimBtnText.text = "Claim";
            }
        }

        public void OnClaimBtnClicked()
        {
            pendingRewardsPanel.ClaimReward(leaderboardId);
            RewardClaimed(true);
        }

    }
}

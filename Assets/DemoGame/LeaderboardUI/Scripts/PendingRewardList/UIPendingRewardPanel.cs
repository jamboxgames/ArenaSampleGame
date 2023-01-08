namespace Jambox.Leaderboard.UI
{
    using JBX.Leaderboard.Controller;
    using JBX.Leaderboard.Data;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIPendingRewardPanel : MonoBehaviour
    {

        public GameObject loadingPanel;

        public List<UIPendingRewardData> pendingRewardsList;
        public UIPendingRewardListView pendingRewardsListView;

        public GameObject pendingContent;
        public GameObject claimSuccesDialogue;

        public GameObject claimAllBtn;
        public Text claimSuccessText;

        private Sprite rewardIconSprite;

        public void SetMetadata(IPendingReward data, Sprite rewardIconSprite)
        {
            this.rewardIconSprite = rewardIconSprite;

            pendingRewardsList = new List<UIPendingRewardData>();
            
            foreach (var tempo in data.PendingRewardList)
            {
                Debug.Log("SetMetadata : " + tempo.LeaderboardID);
                UIPendingRewardData newData = new UIPendingRewardData(tempo);
                pendingRewardsList.Add(newData);
            }
            ShowPendingRewards();
        }

        public void ShowPendingRewards()
        {
            pendingRewardsListView.gameObject.SetActive(true);
            pendingRewardsListView.ItemCallback = PopulateItem;
            pendingRewardsListView.RowCount = pendingRewardsList.Count;
        }

        private void PopulateItem(UIPendingRewardListViewItem item, int rowIndex)
        {
            var tDet = item as UIPendingRewardItem;
            if (rowIndex < pendingRewardsList.Count)
            {
                UIPendingRewardData lbData = pendingRewardsList[rowIndex];
                tDet.FillItem(lbData, rewardIconSprite, this);
            }
        }

        public void ClaimReward(string lbId, bool all = false)
        {
            loadingPanel.SetActive(true);
            _ = JBXLeaderboardCommunicator.Instance.ClaimLBReward(lbId, all, (data) => { OnLBRewardClaimed(data, all); });
        }

        void OnLBRewardClaimed(IClaimReward data, bool all)
        {
            loadingPanel.SetActive(false);

            pendingContent.SetActive(false);
            claimSuccesDialogue.SetActive(true);

            int totalReward = 0;
            String LeaderboardName = "";
            String RewardCoinName = "";
            LeaderboardName = data.ClaimRewardList.LeaderBoardName;
            UIPendingRewardData _reward = pendingRewardsList.Find(x => x.LeaderboardId.Equals(data.ClaimRewardList.LeaderboardID.ToString(), System.StringComparison.OrdinalIgnoreCase));
            if (_reward != null)
            {
                _reward.IsClaimed = true;
            }
            foreach (var rewData in data.ClaimRewardList.FullRewardData)
            {
                    
                if ((rewData.RewardData != null) && !String.IsNullOrEmpty(rewData.RewardData.VirtualRewards.Key))
                {
                    totalReward += rewData.RewardData.VirtualRewards.Value;
                    RewardCoinName = rewData.RewardData.VirtualRewards.Key;
                }
            }
            claimSuccessText.text = string.Format("You have successfully claimed {0} {1} for {2}", totalReward , RewardCoinName , LeaderboardName);

            if (all)
            {
                pendingRewardsListView.Refresh();
                if (claimAllBtn != null)
                    claimAllBtn.SetActive(false);
            }
            else
            {
                UIPendingRewardData _rewardNew = pendingRewardsList.Find(x => x.IsClaimed == false);
                if(_rewardNew == null)
                {
                    if (claimAllBtn != null)
                        claimAllBtn.SetActive(false);
                }
            }
                
        }

        public void OnClaimAllBtnClicked()
        {
            ClaimReward("", true);
        }

        public void OnCloseBtnClicked()
        {
            TourneyManager.Instance.OpenMainPanel();
            Destroy(this.gameObject);
        }

        public void OnCloseClaimSuccessBtnClicked()
        {
            pendingContent.SetActive(true);
            claimSuccesDialogue.SetActive(false);
        }

    }

}

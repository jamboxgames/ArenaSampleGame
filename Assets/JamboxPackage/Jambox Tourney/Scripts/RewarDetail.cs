using System.Collections;
using System.Collections.Generic;
using Jambox.Tourney.Connector;
using Jambox.Tourney.UI;
using UnityEngine;
using UnityEngine.UI;

public class RewarDetail : MonoBehaviour
{
    public RewardListView theList;
    public List<RewardDistribution> rewardsList;

    public void UpdateUi(string TourneyID, Panels prevPanel = Panels.TourneyPanel, bool isCompleted = false)
    {
        TourneyDetail tourneyDet = null;
        if (isCompleted)
        {
            foreach(var dataNewTempo in UserDataContainer.Instance.CompletedTourney)
            {
                if(TourneyID.Equals(dataNewTempo.LeaderBoardID))
                {
                    rewardsList = dataNewTempo.RewardList.RewardsDistribution;
                }
            }
        }
        else
        {
            UserDataContainer.Instance.UpdatedTourneyData.TryGetValue(TourneyID, out tourneyDet);
            rewardsList = tourneyDet._tournament.RewardList.RewardsDistribution;
        }
        ShowRewards();
    }

    public void ShowRewards()
    {
        theList.ItemCallback = PopulateItem;
        theList.RowCount = rewardsList.Count;
    }
    private void PopulateItem(RewardListViewItem item, int rowIndex)
    {
        var tDet = item as RewardItem;
        if (rowIndex < rewardsList.Count)
        {
            bool lastItem = false;
            if (rowIndex == rewardsList.Count - 1)
                lastItem = true;
            tDet.FillItem(rewardsList[rowIndex], lastItem);
        }
    }

    public void OnClosebuttonClick()
    {
        this.gameObject.SetActive(false);
    }
}
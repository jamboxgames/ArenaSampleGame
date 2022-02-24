namespace Jambox.Tourney.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using Jambox.Tourney.Connector;
    using UnityEngine;
    using UnityEngine.UI;

    public class RewardItem : RewardListViewItem
    {
        public Text position;
        public Text reward;
        public GameObject divider;

        public void FillItem(RewardDistribution _data, bool last)
        {
            position.text = "Rank  " + _data.Start_Rank + " - " + _data.End_Rank;
            reward.text = _data.WinAmount + " " + UserDataContainer.Instance.GetCurrencyDisplayTextForKey(_data.CurrencyType);
            if (last)
                divider.SetActive(false);
        }
    }
}

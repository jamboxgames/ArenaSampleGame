namespace Jambox.Tourney.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using Jambox.Common;
    using Jambox.Tourney.Connector;
    using UnityEngine;
    using UnityEngine.UI;

    public class LeaderBoardItem : LeaderBoardListViewItem
    {
        public Text Rank;
        public Text Name;
        public Text Score;
        //public Text Attempt;
        public Sprite myBG;
        public GameObject divider;

        public void FillItem(leaderBoardData data_, bool last)
        {
            Rank.text =  ""+ data_.Rank ;
            Name.text =  data_.Username;
            Debug.Log("DisplayScore : " + data_.DisplayScore);
            if(!string.IsNullOrEmpty(data_.DisplayScore))
                Score.text = data_.DisplayScore;
            else
                Score.text = "" + data_.Score;
            if (JamboxController.Instance.getMyuserId().Equals(data_.PlayerId))
            {
                Name.text = "You";
                this.gameObject.GetComponent<Image>().enabled = true;

            }
            else
            {
                this.gameObject.GetComponent<Image>().enabled = false;
            }

            if (divider != null)
            {
                if (last)
                    divider.SetActive(false);
                else
                    divider.SetActive(true);
            }
        }
    }
}

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
            Score.text = "" + data_.Score;
            //Attempt.text = " Attempt : " + data_.AttemptCount + " / " + _totalAttempt;
            if (JamboxController.Instance.getMyuserId().Equals(data_.PlayerId))
            {
                Name.text = "You";
                this.gameObject.GetComponent<Image>().enabled = true;
                //this.gameObject.GetComponent<Image>().sprite = myBG;
                //img.color = UnityEngine.Color.magenta;
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

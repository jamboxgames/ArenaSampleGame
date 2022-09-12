namespace Jambox.Leaderboard.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using Jambox.Common;
    using JBX.Leaderboard.Controller;
    //using Jambox.Tourney.Connector;
    using UnityEngine;
    using UnityEngine.UI;

    public class LeaderBoardItem : LeaderBoardListViewItem
    {
        public Text Rank;
        public Text Name;
        public Text Score;
        public Image avatarContainer;
        public Image avatar;
        public Image rewardImage;

        public Sprite normalsprite;
        public Sprite myPlayerSprite;

        private string playerID;

        public void FillItem(LeaderBoardData data, Sprite rewardSprite)
        {

            avatarContainer.gameObject.SetActive(false);

            playerID = data.PlayerId;
            Rank.text = "" + data.Rank;
            Name.text = data.Username;
            Score.text = "" + data.Score;
            if (rewardImage != null)
                rewardImage.sprite = rewardSprite;

            //if (data.PlayerId == 4 + "")
            if (data.PlayerId == JamboxController.Instance.getMyuserId())
            {
                GetComponent<Image>().sprite = myPlayerSprite;
            }
            else
            {
                GetComponent<Image>().sprite = normalsprite;
            }

            UpdateAvatar(data.avatarSprite);
        }

        public void UpdateAvatar(Sprite _sprite)
        {
            if (_sprite != null)
            {
                avatarContainer.gameObject.SetActive(true);
                avatar.sprite = _sprite;
            }
        }

        public string GetCurrentPlayerID()
        {
            return playerID;
        }

    }
}

namespace Jambox.Tourney.UI
{
    using Jambox.Common;
    using Jambox.Tourney.Connector;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    [System.Serializable]
    public class RealtimeLBitem : RealtimeLbListViewItem
    {

        public int rank;
        public Text rankText;

        public Text username;

        public Text scoreText;
        public long score;

        public Image avatar;

        public Sprite normalBG;
        public Sprite myPlayerBG;

        public void SetDetails(string _playerId, int _rank, string _username, long _score, Sprite _avatar)
        {
            rank = _rank;
            rankText.text = _rank + "";

            username.text = _username;

            scoreText.text = _score + "";
            score = _score;

            if (avatar != null)
            {
                avatar.sprite = _avatar;
            }

            GetComponent<Image>().sprite = normalBG;
            if (_playerId == JamboxController.Instance.getMyuserId())
            {
                GetComponent<Image>().sprite = myPlayerBG;
            }
        }

        public void SetDetails(string _playerId, int _rank, string _username, long _score)
        {
            rank = _rank;
            rankText.text = _rank + "";

            username.text = _username;

            scoreText.text = _score + "";
            score = _score;

            GetComponent<Image>().sprite = normalBG;

            if (_playerId == JamboxController.Instance.getMyuserId())
            {
                GetComponent<Image>().sprite = myPlayerBG;
            }
        }

        public void UpdateScore(long _score)
        {
            if (username.ToString() == CommonUserData.Instance.userName)
            {
                score = _score;
                scoreText.text = _score + "";
            }
        }

        public void DecreaseRank()
        {
            rank--;
            rankText.text = rank + "";
        }

        public void IncreaseRank()
        {
            rank++;
            rankText.text = rank + "";
        }

        public void SetRank(int _rank)
        {
            rank = _rank;
            rankText.text = rank + "";
        }
    }
}

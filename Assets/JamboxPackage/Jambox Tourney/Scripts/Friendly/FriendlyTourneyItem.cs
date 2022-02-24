namespace Jambox.Tourney.UI
{
    using Jambox.Tourney.Connector;
    using Jambox.Tourney.Data;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Net.Http.Headers;
    using System.Collections.Generic;
    using System.Linq;
    using Jambox.Common;


    /// <summary>
    /// UI refernces of Items which will be displayed on UI.
    /// </summary>
    public class FriendlyTourneyItem : FriendlylistViewItem
    {
        public Text TourneyName;
        public Text TourneyDesc;
        public Text PlayerCountDetail;
        public GameObject Positions;
        public Text Position;
        public Text AttemptsDone;
        //public Text JoinBtnText;
        public Text EndTimeText;
        public Button PlayBtn;
        public Text BestScore;
        public Button LeaderBoard;
        private TourneyDetail friendlyTourneyDet;

        private void Start()
        {

        }
        public TourneyDetail FriendlyTourneyDet
        {
            get { return friendlyTourneyDet; }
            set
            {
                friendlyTourneyDet = value;
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            TourneyName.text = friendlyTourneyDet._tournament.TourneyName;
            TourneyDesc.text = friendlyTourneyDet._tournament.Description;
            //PlayerCountDetail.text = friendlyTourneyDet.CurrentPlayers + " / " + friendlyTourneyDet.MaxPlayers;
            Positions.SetActive(false);
            AttemptsDone.text = "Attempts : " + friendlyTourneyDet._joinedTourneyData.CurrentAttempt + "/ "
                                        + friendlyTourneyDet._tournament.MaxEntry;
            TimeSpan elapsed = DateTime.Parse(friendlyTourneyDet._tournament.EndTime).ToUniversalTime().Subtract(DateTime.UtcNow);
            //2021-07-19T07:54:36.738535Z
            EndTimeText.text = "ENDS IN : " + EndTimeInFormat(elapsed);
            //JoinBtnText.text = "Join";
        }

        string EndTimeInFormat(TimeSpan _elasped)
        {
            string timeString = "";

            if (_elasped.Days >= 2)
            {
                timeString = _elasped.Days + "D";
            }
            else if (_elasped.Days >= 1)
            {
                if (_elasped.Hours == 0)
                {
                    timeString = _elasped.Days + "D ";
                    return timeString;
                }
                timeString = _elasped.Days + "D " + _elasped.Hours + "H";
            }
            else if (_elasped.Hours >= 1)
            {
                if (_elasped.Minutes == 0)
                {
                    timeString = _elasped.Hours + "H ";
                    return timeString;
                }
                timeString = _elasped.Hours + "H " + _elasped.Minutes + "M";
            }
            else if (_elasped.Hours < 1 && _elasped.Minutes >= 1)
            {
                timeString = _elasped.Minutes + "M";
            }
            else if (_elasped.Minutes < 1)
            {
                timeString = "0M";
            }

            return timeString;
        }

        public void OnInviteBtnClicked()
        {
            GetComponentInParent<FriendlyPanel>().OnInviteBtnClicked(FriendlyTourneyDet._joinedTourneyData.JoinCode);
        }

        public void OnPlayBtnClick()
        {
            UserDataContainer.Instance.UpdatedFriendlyData.TryGetValue(friendlyTourneyDet._tournament.Tourneyid, out friendlyTourneyDet);
            PlayBtn.interactable = false;
            if (friendlyTourneyDet._joinedTourneyData.CurrentAttempt < friendlyTourneyDet._tournament.MaxEntry)
            {
                //UIPanelController.Instance.SetTourneyScrollActive(false);
                //_ = CommunicationController.Instance.PlayFriendlyTourney("", friendlyTourneyDet._tournament.Tourneyid, (data) => { PlayedSuccess(data); });
                _ = CommunicationController.Instance.PlayTourney("", friendlyTourneyDet._tournament.Tourneyid, "free", (data) => { PlayedSuccess(data); });

                UIPanelController.Instance.LoadingDialogue(true, false);
            }
        }

        private void PlayedSuccess(IApiPlayTourney data)
        {

            UIPanelController.Instance.LoadingDialogue(false);
            PlayBtn.interactable = true;
            if (!String.IsNullOrEmpty(data.Error))
                return;

            List<leaderBoardData> newleaderBoardList = new List<leaderBoardData>();
            List<leaderBoardData> templist = new List<leaderBoardData>();
            foreach (var item in data.LeaderRecords)
            {
                leaderBoardData itemData = new leaderBoardData(item);
                templist.Add(itemData);
            }

            newleaderBoardList = templist.OrderBy(o => o.Rank).ToList();

            //UserDataContainer.Instance.UpdateFriendlyTourneyDataOnPlay(friendlyTourneyDet._tournament.Tourneyid, data);
            UserDataContainer.Instance.UpdatedFriendlyData.TryGetValue(friendlyTourneyDet._tournament.Tourneyid, out friendlyTourneyDet);
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            UIPanelController.Instance.ShowPanel(Panels.None, Panels.None, metadata);
            Match _matchData = new Match(friendlyTourneyDet._tournament.Tourneyid, data.LeaderBoardID,
                friendlyTourneyDet._tournament.metadata, friendlyTourneyDet._tournament.Category, null, newleaderBoardList,
                CommonUserData.Instance.MyAvatarURL, CommonUserData.Instance.userName, userAvatarSprite: CommonUserData.Instance.avatarSprite);
            ArenaSDKEvent.Instance.FireOnPlayClick(Panels.FriendlyPanel, _matchData);
        }

        public void OnInfoButtonClick()
        {
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            metadata.Add("tourneyid", friendlyTourneyDet._tournament.Tourneyid);
            UIPanelController.Instance.ShowPanel(Panels.DetailsPanel, Panels.FriendlyPanel, metadata);
        }

        /*
        public void updatePlayableStatus()
        {
            if (friendlyTourneyDet._joinedTourneyData.CurrentAttempt >= friendlyTourneyDet._tournament.MaxEntry)
            {
                if (friendlyTourneyDet._tournament.PlayWithVideoAD)
                {
                    JoinBtnText.text = "WATCH";
                }
                else
                {
                    PlayJoinBtn.interactable = false;
                }
            }
        }

        public void OnMoreButtonClick()
        {
            UnityDebug.Debug.Log("OnMoreButtonClick : TourneyId : " + friendlyTourneyDet._tournament.Tourneyid);
            UIPanelController.Instance.ShowPanel(Panels.DetailsPanel, Panels.TourneyPanel, friendlyTourneyDet._tournament.Tourneyid);
        }

        public void LeaderBoardBtnCLick()
        {
            UnityDebug.Debug.Log("LeaderBoardBtnCLick Hit >>>>>");
            UserDataContainer.Instance.UpdatedTourneyData.TryGetValue(friendlyTourneyDet._tournament.Tourneyid, out friendlyTourneyDet);
            if (friendlyTourneyDet.isJoined)
            {
                UIPanelController.Instance.ShowLeaderBoard(friendlyTourneyDet._joinedTourneyData.LeaderBoardID);
            }
        }*/
    }
}

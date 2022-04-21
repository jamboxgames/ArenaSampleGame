namespace Jambox.Tourney.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Jambox.Common;
    using Jambox.Tourney.Connector;
    using Jambox.Tourney.Data;
    using UnityEngine;
    using UnityEngine.UI;

    public class ScoreDisplay : MonoBehaviour
    {
        //public Text EndTimeText;
        public Text Rank;
        //public Text PlayersCount;
        public Text BestScore;
        public Text CurrentScore;

        public Text Attempts;
        public float attemptChangeTime;
        public float attemptsChangeScale;

        //public Button GetLeaderBoard;
        public Button PlayAgainBtn;
        public GameObject PlayText;
        public GameObject watchAdText;
        public Text tourneyName;
        public Text Desc;
        private TourneyDetail TourneyData;
        public SubmitScore submitScorePanel;
        private Panels PreviousPanel = Panels.None;

        public void ShowResult(IApiSubmitScore ScoreData, string TourneyId, Panels prevPanel, Action<bool> ShowConfetti)
        {
            TourneyData = null;
            PreviousPanel = prevPanel;
            if (prevPanel == Panels.TourneyPanel)
                UserDataContainer.Instance.UpdatedTourneyData.TryGetValue(TourneyId, out TourneyData);
            if(prevPanel == Panels.FriendlyPanel)
                UserDataContainer.Instance.UpdatedFriendlyData.TryGetValue(TourneyId, out TourneyData);
            tourneyName.text = TourneyData._tournament.TourneyName;
            Desc.text = TourneyData._tournament.Description;
            Rank.text = ScoreData.MyRank + "/" + ScoreData.CurrentPlayer;
            CurrentScore.text = "Current Score :    " + ScoreData.CurrentScore;
            BestScore.text =    "Best Score :       " + ScoreData.BestScore;
            Attempts.text = TourneyData._joinedTourneyData.CurrentAttempt + " / " + TourneyData._tournament.MaxEntry;

            PlayText.SetActive(true);
            watchAdText.SetActive(false);
            PlayAgainBtn.gameObject.SetActive(true);

            if (TourneyData._joinedTourneyData.CurrentAttempt >= TourneyData._tournament.MaxEntry)
            {
                if(TourneyData._tournament.PlayWithVideoAD)
                {
                    PlayText.SetActive(false);
                    watchAdText.SetActive(true);
                }
                else
                {
                    PlayAgainBtn.gameObject.SetActive(false);
                }
            }

            if(ScoreData.CurrentScore< ScoreData.BestScore)
            {
                ShowConfetti(false);
            }
            else
            {
                ShowConfetti(true);
            }
        }

        private void OnVideoWatched()
        {
            submitScorePanel.LoadingDialog(true, false);
            //PlayJoinBtn.interactable = true;

            TourneyDetail tourneyDet = null;
            if (PreviousPanel == Panels.FriendlyPanel)
            {
                UserDataContainer.Instance.UpdatedFriendlyData.TryGetValue(TourneyData._tournament.Tourneyid, out tourneyDet);
            }
            else
            {
                UserDataContainer.Instance.UpdatedTourneyData.TryGetValue(TourneyData._tournament.Tourneyid, out tourneyDet);
            }
#if !UNITY_EDITOR
            Firebase.Analytics.FirebaseAnalytics.LogEvent("PlayTournament");
#endif
            StartCoroutine(AttemptAnimation(() =>
            {
                _ = CommunicationController.Instance.PlayTourney("", TourneyData._tournament.Tourneyid, "adv", (data) => { PlayedSuccess(data); }, (errorMsg) => { UIPanelController.Instance.ErrorFromServerRcvd(errorMsg); });
            }));
        }

        public void UpdateRankOnLBrefresh(int _rank, int _player)
        {
            Rank.text = _rank + "/" + _player;
        }

        public void PlayAgainClicked()
        {
            PlayAgainBtn.interactable = false;
            submitScorePanel.LoadingDialog(true, false);

            if (TourneyData._joinedTourneyData.CurrentAttempt >= TourneyData._tournament.MaxEntry)
            {
                if (TourneyData._tournament.PlayWithVideoAD)
                {
                    UIPanelController.Instance.OnWatchVideo(TourneyData._joinedTourneyData.Tourneyid,
                                                            Panels.TourneyPanel);
                }
                else
                {
                    String msg = "YOU HAVE EXHAUSTED THE ATTEMPTS. PLEASE PLAY ANOTHER TOURNAMENT";
                    UIPanelController.Instance.SetTourneyScrollActive(false);
                    UIPanelController.Instance.SetTourneyNoMoneyDialogue(true, msg, "OKAY", true, false, null, 0, "", "",
                                                            Panels.None, "OUT OF ATTEMPTS");
                }
            }
            else
            {
                PlayAfterAttemptAnimation();
            }
        }

        void PlayAfterAttemptAnimation()
        {
#if !UNITY_EDITOR
            Firebase.Analytics.FirebaseAnalytics.LogEvent("PlayTournament");
#endif
            StartCoroutine(AttemptAnimation(() =>
            {
                _ = CommunicationController.Instance.PlayTourney("", TourneyData._tournament.Tourneyid, "free", (data) => {
                    PlayedSuccess(data);
                }, (errorMsg) => { UIPanelController.Instance.ErrorFromServerRcvd(errorMsg); });
            }));
        }

        IEnumerator AttemptAnimation(System.Action AnimationDone)
        {
            Vector3 tempScale = Attempts.transform.localScale;
            ScaleInAnimation _anim = UIAnimations.Instance.ScaleIn(Attempts.rectTransform, Attempts.transform.localScale, (Attempts.transform.localScale + new Vector3(attemptsChangeScale, attemptsChangeScale, attemptsChangeScale)), attemptChangeTime);
            while (!_anim.done)
            {
                yield return null;
            }
            Attempts.text = (TourneyData._joinedTourneyData.CurrentAttempt + 1) + " / " + TourneyData._tournament.MaxEntry;

            ScaleInAnimation _anim1 = UIAnimations.Instance.ScaleIn(Attempts.rectTransform, Attempts.transform.localScale, tempScale, attemptChangeTime);
            while (!_anim1.done)
            {
                yield return null;
            }

            AnimationDone();
        }

        private void PlayedSuccess(IApiPlayTourney data)
        {
            submitScorePanel.LoadingDialog(false);
            PlayAgainBtn.interactable = true;
            List<leaderBoardData> newleaderBoardList = new List<leaderBoardData>();
            List<leaderBoardData> templist = new List<leaderBoardData>();
            foreach (var item in data.LeaderRecords)
            {
                leaderBoardData itemData = new leaderBoardData(item);
                templist.Add(itemData);
            }
            newleaderBoardList = templist.OrderBy(o => o.Rank).ToList();

            if (TourneyData._tournament.Category == 3)
            {
                UserDataContainer.Instance.UpdatedFriendlyData.TryGetValue(TourneyData._tournament.Tourneyid, out TourneyData);
            }
            else
            {
                UserDataContainer.Instance.UpdateTourneyDataOnPlay(TourneyData._tournament.Tourneyid, data);
                UserDataContainer.Instance.UpdatedTourneyData.TryGetValue(TourneyData._tournament.Tourneyid, out TourneyData);
            }
            
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            UIPanelController.Instance.ShowPanel(Panels.None, Panels.None, metadata);
            Match _matchData = new Match(TourneyData._tournament.Tourneyid, data.LeaderBoardID, TourneyData._tournament.metadata,
                TourneyData._tournament.Category, null, newleaderBoardList, CommonUserData.Instance.MyAvatarURL,
                CommonUserData.Instance.userName, userAvatarSprite: CommonUserData.Instance.avatarSprite);
            ArenaSDKEvent.Instance.FireOnPlayClick(PreviousPanel, _matchData);
        }
    }
}

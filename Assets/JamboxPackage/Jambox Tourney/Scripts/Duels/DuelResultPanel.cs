namespace Jambox.Tourney.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using Jambox.Tourney.Connector;
    using Jambox.Tourney.Data;
    using Jambox.Tourney.UI;
    using Jambox.Common.TinyJson;
    using Jambox.Common;
    using Jambox.Common.Utility;

    public class DuelResultPanel : MonoBehaviour
    {

        [SerializeField] private PlayerDetailsContainer myPlayer;
        [SerializeField] private PlayerDetailsContainer opponentPlayer;

        [SerializeField] private Text title;
        [SerializeField] private GameObject resultContainer;
        [SerializeField] private Text resultText;
        [SerializeField] private Text WinAmount;
        [SerializeField] private GameObject WinAmt;

        public float rewardChangeDuration;

        //Play Again And Home Button reference
        public Button PlayAgain;
        public Button Back;

        public GameObject WaitingDialog;
        public Text WaitingText;

        public GameObject RewardContainer;
        public Text RewardText;
        public Image RewardImage;

        public UIParticleSystem CoinsWon;
        public UIParticleSystem CoinsWonOpponent;
        public GameObject celebrations;

        [Header("Temporary")]
        public Sprite[] randomProfilePictures;

        public float canvasCustomValue;

        private string TourneyID;

        private RectTransform rectTransform;
        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                    rectTransform = GetComponent<RectTransform>();
                return rectTransform;
            }
        }

        #region Tablet_Checks
        private void Start()
        {
            TabletCheck();
#if GAME_FIREBASE_ENABLED

            Firebase.Analytics.Parameter[] eventarameters = {
                                new Firebase.Analytics.Parameter("screen","duel_result")
                                };
            Firebase.Analytics.FirebaseAnalytics.LogEvent("arena_screen", eventarameters);
#endif

        }

        public void TabletCheck()
        {
            if (TabletDetect.IsTablet())
            {
                this.gameObject.GetComponentInParent<CanvasScaleChange>().SetToTabletView();
            }
            else
            {
                this.gameObject.GetComponentInParent<CanvasScaleChange>().SetToDefault();
            }
        }
        #endregion

        private void OnEnable()
        {
            WinAmount.color = new Color(WinAmount.color.r, WinAmount.color.g, WinAmount.color.b, 0f);
            WinAmt.SetActive(false);
            RewardContainer.SetActive(false);
        }

        public void SetTournEyID(Dictionary<string, string> metadata)
        {
            UpdateWaitingDialogue(true, "Waiting for result");
            metadata.TryGetValue("tourneyid", out TourneyID);
            TournamnetData DuelData = null;
            UserDataContainer.Instance.MyDuels.TryGetValue(TourneyID, out DuelData);
            title.text = DuelData.TourneyName;
            string LeaderBoardID = DuelData.LeaderBoardID;
            long _Score = UIPanelController.Instance.tempScore.Score;
            ReplayData _replayData = UIPanelController.Instance.tempScore.replayData;
            UIPanelController.Instance.tempScore = null;
            SetMyPlayerDetails(CommonUserData.Instance.userName, CommonUserData.Instance.avatarSprite, _Score, false);

            _ = CommunicationController.Instance.SubmitDuelScore("", LeaderBoardID, _Score, (data) => { ScoreSubmitted(data); }, _replayData);
        }

        public void UpdateWaitingDialogue(bool status, string DisplayText)
        {
            WaitingText.text = DisplayText;
            WaitingDialog.gameObject.SetActive(status);
        }

        void ScoreSubmitted(IApiSubmitDuelScore data)
        {
            StartCoroutine(ResultPanelShow(data));
        }

        public void EnableButtons()
        {
            PlayAgain.gameObject.SetActive(true);
            Back.gameObject.SetActive(true);
        }

        IEnumerator ResultPanelShow(IApiSubmitDuelScore data)
        {

            if (data.delayInResultDisplay > 0)
            {
                foreach (var playerData in data.LeaderRecords)
                {
                    if (JamboxController.Instance.getMyuserId().Equals(playerData.PlayerId))
                    {
                        //SetMyPlayerDetails(playerData.Username, UserDataContainer.Instance.MyAvatarURL, playerData.Score, won);
                    }
                    else
                    {
                        SetOpponentPlayerDetails(playerData.Username, UserDataContainer.Instance.tempDuelOpponentSprite, -1, false);
                    }
                }
                UpdateWaitingDialogue(true, "Waiting for opponent to finish");
                yield return new WaitForSeconds(data.delayInResultDisplay);
            }
            UpdateWaitingDialogue(false, "");
            //EnableButtons();
            UnityDebug.Debug.Log("Result : " + data.result.resultDisplay.ToString());
            bool won = false;
            bool lose = false;
            if (data.result.resultDisplay.Equals("lose", System.StringComparison.CurrentCultureIgnoreCase))
            {
                resultText.text = "YOU LOOSE";
                resultContainer.SetActive(true);
                lose = true;
            }
            if (data.result.resultDisplay.Equals("won", System.StringComparison.CurrentCultureIgnoreCase))
            {
                resultText.text = "YOU WIN";
                resultContainer.SetActive(true);
                won = true;

                UserDataContainer.Instance.UpdateUserMoney(data.result.rewardList.Virtual.Value,
                                                        data.result.rewardList.Virtual.Key, true);

                celebrations.SetActive(true);
            }
            if (data.result.resultDisplay.Equals("draw", System.StringComparison.CurrentCultureIgnoreCase))
            {
                resultText.text = "DRAW";
                resultContainer.SetActive(true);
                //won = true;
                UserDataContainer.Instance.UpdateUserMoney(data.result.rewardList.Virtual.Value,
                                                        data.result.rewardList.Virtual.Key, true);

                celebrations.SetActive(true);
            }
            foreach (var playerData in data.LeaderRecords)
            {
                if (JamboxController.Instance.getMyuserId().Equals(playerData.PlayerId))
                {
                    bool _wonFrame = true;
                    if (!won && lose) _wonFrame = false;
                    SetMyPlayerDetails(playerData.Username, CommonUserData.Instance.avatarSprite, playerData.Score, _wonFrame);
                }
                else
                {
                    bool _wonFrame = false;
                    if (!won && lose) _wonFrame = true;
                    SetOpponentPlayerDetails(playerData.Username, UserDataContainer.Instance.tempDuelOpponentSprite, playerData.Score, _wonFrame);
                    UserDataContainer.Instance.tempDuelOpponentSprite = null;
                }
            }

            bool _scoreShown = false;
            while (!_scoreShown)
            {
                if (myPlayer.informationFilled && opponentPlayer.informationFilled)
                {
                    myPlayer.ShowScore();
                    opponentPlayer.ShowScore();
                    _scoreShown = true;
                }
                yield return null;
            }

            int TotalMoney = 0;
            CoinsWon.Particle = JamboxSDKParams.Instance.CoinBG;
            CoinsWonOpponent.Particle = JamboxSDKParams.Instance.CoinBG;
            if (won)
            {
                TotalMoney = data.result.rewardList.Virtual.Value;
                RewardText.text = TotalMoney.ToString();
                RewardContainer.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                myPlayer.IncreaseRewards(data.result.rewardList.Virtual.Value);
                opponentPlayer.IncreaseRewards(0);
                CoinsWon.Play();
            }
            else if (lose)
            {
                foreach (var playerData in data.LeaderRecords)
                {
                    if (!JamboxController.Instance.getMyuserId().Equals(playerData.PlayerId))
                    {
                        TotalMoney = playerData.rewardList.Virtual.Value;
                        RewardText.text = TotalMoney.ToString();
                        RewardContainer.SetActive(true);
                        yield return new WaitForSeconds(0.5f);
                        myPlayer.IncreaseRewards(0);
                        opponentPlayer.IncreaseRewards(playerData.rewardList.Virtual.Value);
                        break;
                    }
                }
                CoinsWonOpponent.Play();
            }
            else
            {
                TotalMoney = (data.result.rewardList.Virtual.Value * 2);
                RewardText.text = TotalMoney.ToString();
                RewardContainer.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                myPlayer.IncreaseRewards(data.result.rewardList.Virtual.Value);
                opponentPlayer.IncreaseRewards(data.result.rewardList.Virtual.Value);
                CoinsWon.Play();
                CoinsWonOpponent.Play();
            }

            float rewardMoney = TotalMoney;
            RewardImage.sprite = JamboxSDKParams.Instance.CoinBG;
            SetCurrencyPreferredSize(rewardMoney.ToString());
            UpdateTextAnimation _textAnim = UIAnimations.Instance.ChangeTextOverTime(RewardText, (int)rewardMoney, 0, rewardChangeDuration, false);
            while (!_textAnim.done)
            {

                //Color
                float _alphaPercentage = _textAnim.doneRatio;
                Color c = RewardText.color;
                RewardText.color = new Color(c.r, c.g, c.b, (1 - _alphaPercentage));

                yield return null;
            }

            CoinsWon.Stop();
            CoinsWonOpponent.Stop();

            RewardContainer.SetActive(false);
            EnableButtons();
        }

        void SetCurrencyPreferredSize(string _text)
        {
            string _temp = RewardText.text;
            RewardText.text = _text;
            RewardText.rectTransform.sizeDelta = new Vector2(RewardText.preferredWidth, RewardText.rectTransform.sizeDelta.y);
            RewardText.text = _temp;
        }

        public void SetDetailsOfResultPanel(string _title, int _coins, bool _won)
        {
            title.text = _title;
            if (_won)
            {
                resultText.text = "YOU WON!\n" + _coins + " Coins";
            }
            else
            {
                resultText.text = "YOU LOST!";
            }
        }

        public void SetMyPlayerDetails(string _name, Sprite avatarSprite, long score, bool won)
        {
            myPlayer.SetPlayerDetails(_name, avatarSprite, -1, score, won);
        }

        public void SetOpponentPlayerDetails(string _name, Sprite avatarSprite, long score, bool won)
        {
            opponentPlayer.SetPlayerDetails(_name, avatarSprite, -1, score, won);
        }

        public void PlayAgainClick()
        {
            TournamnetData DuelData = null;
            UserDataContainer.Instance.MyDuels.TryGetValue(TourneyID, out DuelData);
            if (UserDataContainer.Instance.getUserMoney()[DuelData.Currency] < DuelData.EntryFee)
            {
                ReturnHomeOnNoMoney();
            }
            else
            {
                //UserDataContainer.Instance.UpdateUserMoney(DuelData.EntryFee, DuelData.Currency, false);
                Dictionary<string, string> metadata = new Dictionary<string, string>();
                metadata.Add("tourneyid", TourneyID);
                UIPanelController.Instance.ShowPanel(Panels.MatchMakingPanel, Panels.DuelPanel, metadata);
                Destroy(this.gameObject);
            }
        }

        public void BackButtonClick()
        {
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            UIPanelController.Instance.ShowPanel(Panels.DuelPanel, Panels.None, metadata);
            Destroy(this.gameObject);
        }

        public void ReturnHomeOnNoMoney()
        {
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            metadata.Add("NoMoney", "NoMoney");
            UIPanelController.Instance.ShowPanel(Panels.DialoguePanel, Panels.None, metadata);
            Destroy(this.gameObject);
        }
    }
}

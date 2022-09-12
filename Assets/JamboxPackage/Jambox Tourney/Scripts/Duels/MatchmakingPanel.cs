namespace Jambox.Tourney.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using Jambox.Tourney.Connector;
    using Jambox.Tourney.Data;
    using Jambox.Tourney.UI;
    using System.Linq;
    using Jambox.Common.TinyJson;
    using Jambox.Common;
    using Jambox.Common.Utility;

    public class MatchmakingPanel : MonoBehaviour
    {

        [SerializeField] private PlayerDetailsContainer myPlayer;
        [SerializeField] private PlayerDetailsContainer opponentPlayer;

        [SerializeField] private GameObject loadingImagePrefab;

        [SerializeField] private Text opponentFindingText;

        [SerializeField] private Text title;
        [SerializeField] private Text coins;
        [SerializeField] private GameObject cancelButton;

        [Header("Temporary")]
        public Sprite[] randomProfilePictures;
        public float findingOpponentTime;
        private bool OpponentRcvd = false;
        private bool ShowOpponent = false;

        //Animations
        public Transform findingOpponentRollingAnimation;
        public float rollSpeed;
        public float rollSlowDownSpeed;

        public float updateRewardDuration;
        public float rewardScaleAnimationRate;

        //VsText Effect
        public RectTransform vsText;
        public float VsStartScale;
        public float VsEndScale;
        public float VsAnimTime;

        public GameObject TotalReward;
        //public GameObject RewardDet;
        public Text RewardText;
        public Image RewardTImage;
        public Text RewardAmt;
        public Text IncreasingDOT;

        public string tempJson;

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

        private string UniqueTID;

        #region Tablet_Checks
        private void Start()
        {
            TabletCheck();
        }

        public void TabletCheck()
        {
            if (TabletDetect.IsTablet())
            {
                if (UIPanelController.Instance.IsLandScape())
                {
                    this.gameObject.GetComponentInParent<CanvasScaleChange>().SetToTabletView(1.6f);
                }
                else
                {
                    this.gameObject.GetComponentInParent<CanvasScaleChange>().SetToTabletView();
                }
            }
            else
            {
                this.gameObject.GetComponentInParent<CanvasScaleChange>().SetToDefault();
            }
        }
        #endregion

        private void OnEnable()
        {
            SetDetailsOfMatchmakingPanel("July Bonaza", 1050);
            StartCoroutine(ProfileSearchAnimations());
            vsText.localScale = new Vector3(VsStartScale, VsStartScale, VsStartScale);
            Vector3 _endScale = new Vector3(VsEndScale, VsEndScale, VsEndScale);
            UIAnimations.Instance.ScaleIn(vsText, vsText.localScale, _endScale, VsAnimTime);
        }

        public void SetData(Dictionary<string, string> metadata)
        {
            metadata.TryGetValue("tourneyid", out UniqueTID);
            TournamnetData DuelData = null;
            UserDataContainer.Instance.MyDuels.TryGetValue(UniqueTID, out DuelData);
            SetDetailsOfMatchmakingPanel(DuelData.TourneyName, DuelData.RewardList.RewardsDistribution[0].WinAmount);
            SetMyPlayerDetails("YOU", CommonUserData.Instance.avatarSprite, -1);
            //RewardText.text = DuelData.RewardText.ToString();
            RewardAmt.text = DuelData.RewardList.RewardsDistribution[0].WinAmount.ToString();
            findingOpponentTime = 5.0f;
            StartCoroutine(FindOpponent());
            _ = CommunicationController.Instance.JoinDuel(UniqueTID, (data) =>
            {
                OpponentFound(data);
            }, (errorCode, errorMsg) => { UIPanelController.Instance.ErrorFromServerRcvd(errorCode, errorMsg); });

        }
        GameObject _loading;
        IEnumerator FindOpponent()
        {
            UnityDebug.Debug.LogInfo("Finding Opponent!");
            opponentFindingText.text = "FINDING OPPONENT";
            cancelButton.SetActive(true);
            //_loading = Instantiate(loadingImagePrefab, opponentPlayer.GetProfileImage().gameObject.transform.position, Quaternion.identity, opponentPlayer.gameObject.transform);
            yield return new WaitForSeconds(findingOpponentTime);
            while (!OpponentRcvd)
            {
                yield return null;
            }
            OpponentRcvd = false;
            ShowOpponent = true;
        }

        public void OpponentFound(IAPIJoinDuel data)
        {
            OpponentRcvd = true;
            UnityDebug.Debug.LogInfo("OpponentFound >>>>>>");
            //cancelButton.GetComponent<Button>().interactable = false;
            StartCoroutine(ShowOppUser(data));
        }
        IEnumerator ShowOppUser(IAPIJoinDuel data)
        {
            while (!ShowOpponent)
            {
                yield return null;
            }

            //Getting Opponent Avatar Ready before showing
            UnityDebug.Debug.LogInfo("GetTextureRequest HIt URL : " + data.OpponenetData.PlayerURL);
            if (string.IsNullOrEmpty(data.OpponenetData.PlayerURL))
            {
                UnityDebug.Debug.LogWarning("url String is Empty In GetTextureRequest >>>>>>>>");
            }
            while (data.OpponenetData.PlayerURL == null)
            {
                yield return new WaitForEndOfFrame();
            }
            WWW www = new WWW(data.OpponenetData.PlayerURL);
            yield return www;
            UserDataContainer.Instance.tempDuelOpponentSprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));

            cancelButton.GetComponent<Button>().interactable = false;
            ShowOpponent = false;
            TournamnetData DuelData = null;
            UserDataContainer.Instance.MyDuels.TryGetValue(UniqueTID, out DuelData);
            opponentFindingText.text = "OPPONENT FOUND";
            Destroy(_loading);
            cancelButton.SetActive(false);

            if (!DuelData.JoinWithVideoAD)
                UserDataContainer.Instance.UpdateUserMoney(DuelData.EntryFee, DuelData.Currency, false);

            DuelData.LeaderBoardID = data.LeaderBoardID;
            if (UserDataContainer.Instance.MyDuels.ContainsKey(UniqueTID))
            {
                UserDataContainer.Instance.MyDuels.Remove(UniqueTID);
                UserDataContainer.Instance.MyDuels.Add(UniqueTID, DuelData);
            }
            myPlayer.enableEntryFee(DuelData.EntryFee);
            SetOpponentPlayerDetails(data.OpponenetData.PlayerName, UserDataContainer.Instance.tempDuelOpponentSprite, DuelData.EntryFee);

            OpponentFoundAnimations();

            StartCoroutine(SetCollectMoney(findingOpponentTime));
            yield return new WaitForSeconds(findingOpponentTime);
            UIPanelController.Instance.ShowPanel(Panels.None, Panels.None, null);

            /*
            Dictionary<string, string> tempMetaData = new Dictionary<string, string>();
            tempMetaData.Add("game_type", "timed");*/

            Jambox.Match _matchData = new Jambox.Match(UniqueTID, data.LeaderBoardID, DuelData.metadata, DuelData.Category, DuelData.SortOrder, DuelData.ScoringMode,
                 data.OpponenetData.ReplayData, null, CommonUserData.Instance.MyAvatarURL, CommonUserData.Instance.userName,
                 data.OpponenetData.PlayerURL, data.OpponenetData.PlayerName, CommonUserData.Instance.avatarSprite, UserDataContainer.Instance.tempDuelOpponentSprite);

            /*
            Jambox.Match _matchData = new Jambox.Match(UniqueTID, data.LeaderBoardID, tempMetaData, DuelData.Category,
                 data.OpponenetData.ReplayData, null, UserDataContainer.Instance.MyAvatarURL, UserDataContainer.Instance.name,
                 data.OpponenetData.PlayerURL, data.OpponenetData.PlayerName);*/

            ArenaSDKEvent.Instance.FireOnPlayClick(Panels.DuelPanel, _matchData);
        }

        IEnumerator SetCollectMoney(float _waitTime)
        {

            TournamnetData DuelData = null;
            UserDataContainer.Instance.MyDuels.TryGetValue(UniqueTID, out DuelData);

            float totalAmount = DuelData.RewardList.RewardsDistribution[0].WinAmount;

            TotalReward.gameObject.SetActive(true);
            RewardTImage.sprite = JamboxSDKParams.Instance.ArenaParameters.CoinBG;
            SetCurrencyPreferredSize(totalAmount.ToString());
            yield return new WaitForSeconds(_waitTime / 3.0f);

            myPlayer.HideMoneyText();
            opponentPlayer.HideMoneyText();

            //TotalAmount Increase Effect
            int _minFontSize = RewardText.fontSize;
            int _maxFontSize = RewardText.fontSize + 10;
            float _rate = rewardScaleAnimationRate;
            //RewardTImage.sprite = JamboxSDKParams.Instance.CoinBG;

            UpdateTextAnimation _textAnim = UIAnimations.Instance.ChangeTextOverTime(RewardText, 0, (int)totalAmount, updateRewardDuration, true);
            while (!_textAnim.done)
            {

                //Text Blink Animation
                RewardText.fontSize += (int)(_rate * Time.deltaTime);

                if (RewardText.fontSize >= _maxFontSize || RewardText.fontSize <= _minFontSize)
                {
                    _rate = _rate * -1;
                }

                yield return null;
            }

            //Resetting text scale and value
            RewardText.fontSize = _minFontSize;
            RewardText.text = DuelData.RewardList.RewardsDistribution[0].WinAmount + "";

        }

        void SetCurrencyPreferredSize(string _text)
        {
            string _temp = RewardText.text;
            RewardText.text = _text;
            RewardText.rectTransform.sizeDelta = new Vector2(RewardText.preferredWidth, RewardText.rectTransform.sizeDelta.y);
            RewardText.text = _temp;
        }

        public void SetDetailsOfMatchmakingPanel(string _title, int _coins)
        {
            title.text = _title;
            coins.text = "Prize\n" + _coins + " Coins";
        }

        public void SetMyPlayerDetails(string _name, Sprite avatarSprite, long entryFee = 0)
        {
            myPlayer.SetPlayerDetails(_name, avatarSprite, entryFee);
        }

        public void SetOpponentPlayerDetails(string _name, Sprite avatarSprite, long entryFee = 0)
        {
            opponentPlayer.SetPlayerDetails(_name, avatarSprite, entryFee);
        }

        public void ShowEntryFeeAmount()
        {
            myPlayer.ShowEntryAmount();
            opponentPlayer.ShowEntryAmount();
        }

        public void CancelSearchingOpponent()
        {
            UnityDebug.Debug.LogInfo("Cancel Search!!");
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            UIPanelController.Instance.ShowPanel(Panels.DuelPanel, Panels.None, metadata);
            Destroy(this.gameObject);
        }

        void OpponentFoundAnimations()
        {
            StopCoroutine(ProfileSearchAnimations());
            findingOpponentRollingAnimation.gameObject.SetActive(false);
            ShowEntryFeeAmount();
        }

        IEnumerator ProfileSearchAnimations()
        {
            float _width = findingOpponentRollingAnimation.GetChild(0).GetComponent<RectTransform>().rect.width;
            float _yPos = _width;
            float intialRollSpeed = rollSpeed;
            for(int i = 0; i < findingOpponentRollingAnimation.childCount; i++)
            {
                findingOpponentRollingAnimation.GetChild(i).GetComponent<Image>().sprite = randomProfilePictures[Random.Range(0, randomProfilePictures.Length)];
            }
            while (true)
            {
                findingOpponentRollingAnimation.localPosition += new Vector3(0f, rollSpeed * Time.deltaTime, 0f);
                if (findingOpponentRollingAnimation.localPosition.y >= _yPos)
                {
                    findingOpponentRollingAnimation.GetChild(0).GetComponent<Image>().sprite = randomProfilePictures[Random.Range(0, randomProfilePictures.Length)];
                    findingOpponentRollingAnimation.GetChild(0).localPosition -= new Vector3(0, _width * findingOpponentRollingAnimation.childCount - 1, 0);
                    findingOpponentRollingAnimation.GetChild(0).SetAsLastSibling();
                    _yPos += _width;
                }

                if (rollSpeed >= (intialRollSpeed / 9.5))
                {
                    rollSpeed -= rollSlowDownSpeed * Time.deltaTime;
                }
                yield return null;
            }

        }

        private void OnDisable()
        {
            opponentPlayer.GetProfileImage().gameObject.SetActive(false);
        }
    }
}

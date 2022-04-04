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
    using Jambox.Common.Utility;
    using Random = UnityEngine.Random;
    using System.Threading.Tasks;
    using UnityEngine;

    /// <summary>
    /// UI Panel script responsible to populate the data on UI.
    /// </summary>
    public class TourneyPanel : MonoBehaviour
    {
        public Text playerNameText;
        public Image profilePicture;
        public TourneyListView theList;
        public Panels prevPanel;
        private bool InFriendlyCompleted = false;
        public RewarDetail RewardPanel;
        public Text CurrencyText;
        public Text CurrencySubtractAnimText;
        public Text nameText;
        public NoMoneyPopUp NoMoneyDialogue;
        public GameObject ComingSoon;
        public Text ComingSoonBody;
        public GameObject LoadingDialogue;
        public ClaimRewardPanel ClaimPanel;
        public GameObject FriendlyPanel;
        public GameObject TourneyDetail;
        public UpdatePlayerDetails ChangeName;
        public DialoguePanel DialogueUI;
        public Image BGimage;
        public GameObject celebrations;
        public Image CoinImage;

        //Bottom Menu Texts

        public GameObject TourneySelected;
        public GameObject DuelSelected;
        public GameObject FriendlySelected;
        public Text tourneyButtonText;
        public Text duelsButtonText;
        public Text friendlyButtonText;
        public Color TabSelectedColor;
        public Color TabNotSelectedColor;

        //Top Navgation Menu
        [Header("Navigation Menu")]
        public GameObject singleTabMenu;
        public GameObject doubleTabMenu;

        public Button liveButton;
        public Button PastButton;

        public GameObject liveActiveGameobject;
        public GameObject PastActiveGameobject;

        public GameObject claimSymbolContainer;
        public GameObject claimSymbol;

        public Text topText;
        public GameObject leftArrow;
        public GameObject rightArrow;

        public Text singleTabText;
        public Text[] doubleLiveText;
        public Text[] doublePastText;
        private Coroutine updateUser;
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
        private Panels CurrentPanelEnable = Panels.None;

        private void Start()
        {
            BGimage.sprite = UIPanelController.Instance.bgSprite;
            SetToDefaultView();
            if(JamboxSDKParams.Instance.CoinBG != null)
            {
                CoinImage.sprite = JamboxSDKParams.Instance.CoinBG;
            }
        }

        public void SetToDefaultView()
        {
            GetComponentInParent<CanvasScaleChange>().SetToDefault();
        }

        public void OnEnable()
        {
            LoadingDialog(true, false);
            FriendlyPanel.SetActive(false);
            TourneyDetail.SetActive(true);
            updateUser = StartCoroutine(UpdateUserBasicData());
            CheckForUnclaimedRewardsExclamation();
            JamboxController.Instance.UserProfileUpdated += OnProfileUpdate;
            JamboxController.Instance.ErrorFromServer += OnErrorFromServer;
        }

        private void OnErrorFromServer(string ErrorMsg = "")
        {
            Debug.Log("OnErrorFromServer Inside Tpourneypanel Hit >>>>"  + ErrorMsg);
            UIPanelController.Instance.ErrorFromServerRcvd(ErrorMsg);
        }

        private void OnProfileUpdate()
        {
            LoadingDialog(true, false);
            Dictionary<string, string> metaData = new Dictionary<string, string>();
            if (CurrentPanelEnable == Panels.None)
                CurrentPanelEnable = Panels.TourneyPanel;
            _ = GetDetailsAfterNameUpdate(CurrentPanelEnable, metaData);
        }

        private void OnDestroy()
        {
            if (updateUser != null)
                StopCoroutine(updateUser);
        }

        private void OnDisable()
        {
            JamboxController.Instance.UserProfileUpdated -= OnProfileUpdate;
            JamboxController.Instance.ErrorFromServer -= OnErrorFromServer;
            if (updateUser != null)
                StopCoroutine(updateUser);
        }
        private IEnumerator UpdateUserBasicData()
        {
            while (String.IsNullOrEmpty(JamboxController.Instance.getMyuserId()))
            {
                yield return null;
            }
            if (!String.IsNullOrEmpty(CommonUserData.Instance.userName))
            {
                playerNameText.text = CommonUserData.Instance.userName;
                playerNameText.gameObject.SetActive(true);
            }
            if (!String.IsNullOrEmpty(CommonUserData.Instance.MyAvatarURL))
            {
                WWW www = new WWW(CommonUserData.Instance.MyAvatarURL);
                yield return www;
                profilePicture.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
                profilePicture.gameObject.SetActive(true);
                CommonUserData.Instance.avatarSprite = profilePicture.sprite;
            }
        }
        public void OnCurrencyDataRcvd(IAPICurrencyList data)
        {
            UserDataContainer.Instance.UpdateCurrencyList(data);
            UpdateCurrency();
        }

        public void ShowClaimSuccess(int reward, String Currency)
        {
            ClaimPanel.gameObject.SetActive(true);
            ClaimPanel.UpdateText(reward, Currency);
        }

        public void UpdateCurrency(bool _rewardClaimed = false)
        {
            //Debug.Log("CurrencyText.text : " + (CurrencyText.text != null));
            Debug.Log("UserDataContainer.Instance : " + (UserDataContainer.Instance != null));
            if(CurrencyText != null && CurrencyText.text != null)
                CurrencyText.text = UserDataContainer.Instance.GetDisplayCurrencyText();

            if (_rewardClaimed)
                ConfettiAnimation();
        }

        void ConfettiAnimation()
        {
            if (celebrations.activeInHierarchy == true)
            {
                celebrations.SetActive(false);
            }
            celebrations.SetActive(true);
        }

        public void SubtractMoneyAnimation(int _amount)
        {
            CurrencySubtractAnimText.text = "-" + _amount;
            CurrencySubtractAnimText.gameObject.SetActive(true);
        }

        private Dictionary<String, String> metaData = null;
        private void TourneyDataRcvd(IApiTourneyList data, Panels CurrentPanel, Dictionary<string, string> _metaData)
        {
            LoadingDialog(false);
            metaData = _metaData;
            UserDataContainer.Instance.UpdateTourneyData(data);
            if (CurrentPanel == Panels.TourneyPanel)
            {
                TourneyBtnProcess(metaData);
            }
            if (CurrentPanel == Panels.CompletedPanel)
            {
                CompletdListProcess(metaData);
            }
            if (CurrentPanel == Panels.DuelPanel)
            {
                DuelBtnProcess(metaData);
            }
            if (CurrentPanel == Panels.FriendlyPanel)
            {
                FriendlyBtnProcess(metaData);
            }

            if (metaData.ContainsKey("AllRewardsClaimed") && metaData.ContainsKey("RewardAmount") && metaData.ContainsKey("RewardKey"))
            {
                UIPanelController.Instance.ShowClaimSuccessPanel(int.Parse(metaData["RewardAmount"]), metaData["RewardKey"]);
                ConfettiAnimation();
            }
        }

        public void SetScrollViewStatus(bool Status)
        {
            theList.gameObject.SetActive(Status);
        }

        public void SetFriendlyScrollView(bool Status)
        {
            FriendlyPanel.GetComponent<FriendlyPanel>().FriendlyItems(Status);
        }

        public void SetNoMoneyDialogue(bool Status, string msg, string BtnTxt, bool isBuyBtn,bool isClosebtn,
                Action OnBtnClick, int StoreAmt, String CurrKey, string _tID, Panels prev, string headerMsg)
        {
            NoMoneyDialogue.gameObject.SetActive(Status);
            if (Status)
            {
                NoMoneyDialogue.SetUIData(msg, BtnTxt, isBuyBtn, isClosebtn, OnBtnClick, StoreAmt, CurrKey, _tID, prev, headerMsg);
            }
        }

        public void LoadingDialog(bool isShow, bool stillInteractable = true)
        {
            if (isShow)
            {
                //Show The loading Dialogue
                if (LoadingDialogue != null && !LoadingDialogue.gameObject.activeInHierarchy)
                    LoadingDialogue.SetActive(true);

                if (stillInteractable)
                    LoadingDialogue.GetComponent<Image>().enabled = false;
                else
                    LoadingDialogue.GetComponent<Image>().enabled = true;

            }
            else
            {
                //Hide The Loading Dialogue
                if (LoadingDialogue != null && LoadingDialogue.gameObject.activeInHierarchy)
                    LoadingDialogue.SetActive(false);
            }
        }
        public async Task ShowTourneyItem(Panels prevPanelDet, Panels Currentpanel,
                        Dictionary<string, string> metaData, bool ShowDialogue = false, bool ShowFriendlyCompleted = false)
        {
            //UserDataContainer.Instance.userName = String.Empty;
            if (ShowDialogue)
            {
                ShowDialogueOnUI(metaData);
            }
            else if (!JamboxController.Instance.CheckForNetwork())
            {
                return;
            }
            else if (!JamboxController.Instance.ChechkForSession())
            {
                await JamboxController.Instance.RefreshSession();
                //_ = JamboxController.Instance.CreateSession(() => {
                    ShowTourneyItem(prevPanelDet, Currentpanel, metaData, ShowDialogue);
                //});
            }
            else if (String.IsNullOrEmpty(CommonUserData.Instance.userName))
            {
                //ChangeName.gameObject.SetActive(true);
                //ChangeName.SetMetaData(prevPanelDet, Currentpanel, metaData, "", profilePicture.sprite.texture);
                LoadingDialog(false);
                CurrentPanelEnable = Currentpanel;
                StartCoroutine(ShowChangeNamePanel(prevPanelDet, Currentpanel, metaData));
            }
            else
            {
                //_ = CommunicationController.Instance.GetCurrencyData("", (data) => { OnCurrencyDataRcvd(data); });

                ShowTourneyItemNew( Currentpanel, metaData, ShowFriendlyCompleted);                
            }
        }

        public void CheckForUnclaimedRewards()
        {
            _ = CommunicationController.Instance.UnclaimedRewards("", (data) => { OnUnclaimedRewardsRcvd(data); });
        }

        void OnUnclaimedRewardsRcvd(IAPIUnclaimedRewards data)
        {
            if (data.UnclaimedRewards.Count() > 0)
            {
                Dictionary<string, string> _dict = new Dictionary<string, string>();
                _dict.Add("Unclaimed", "");
                _dict.Add("Header", "Unclaimed Rewards");
                _dict.Add("DialogBody", "There are rewards pending for your previous Tournament Entry. Claim your rewards!");
                //_dict.Add("Btn1Name", "Cancel");
                //_dict.Add("Btn2Name", "Claim");
                _dict.Add("Btn1Name", "Claim");
                _dict.Add("Btn2Name", "Collect All");
                ShowDialogueOnUI(_dict);
            }

            UserDataContainer.Instance.UnclaimedRewardsCount = data.UnclaimedRewards.Count();
            CheckForUnclaimedRewardsExclamation();
        }

        //Check to determine if exclamation mark should be show
        public void CheckForUnclaimedRewardsExclamation()
        {
            if (claimSymbol != null)
            {
                if (UserDataContainer.Instance.UnclaimedRewardsCount > 0)
                    claimSymbol.SetActive(true);
                else
                    claimSymbol.SetActive(false);
            }
        }

        IEnumerator ShowChangeNamePanel(Panels prevPanel, Panels currentPanel, Dictionary<string,string> metadata)
        {
            if (UIPanelController.Instance.IsLandScape())
                ChangeName = Instantiate(Resources.Load("UpdatePlayer_Landscape") as GameObject).GetComponent<UpdatePlayerDetails>();
            else
                ChangeName = Instantiate(Resources.Load("UpdatePlayer") as GameObject).GetComponent<UpdatePlayerDetails>();
            ChangeName.RectTransform.SetParent(JamboxSDKParams.Instance.gameObject.GetComponent<RectTransform>(), false);
            RectTransform m_Viewport = ChangeName.GetComponent<RectTransform>();
            UIPanelController.Instance.SetPanelUI(m_Viewport);
            ChangeName.gameObject.SetActive(true);
            if (!String.IsNullOrEmpty(CommonUserData.Instance.MyAvatarURL))
            {
                WWW www = new WWW(CommonUserData.Instance.MyAvatarURL);
                yield return www;
                ChangeName.SetMetaData( "", www.texture);
            }
        }

        private void ShowDialogueOnUI(Dictionary<string, string> metaData)
        {
            LoadingDialog(false);
            if (metaData.ContainsKey("NoMoney"))
            {
                UpdateCurrency();
                UIPanelController.Instance.SetTourneyScrollActive(false);
                String msg = "YOU DO NOT HAVE SUFFICIENT CHIPS TO PLAY THIS TOURNAMENT.PURCHASE SOME FROM OUR STORE AND ENJOY THE GAME";
                //if (!String.IsNullOrEmpty(JamboxSDKParams.Instance.PurchaseText.ToString()))
                //    msg = JamboxSDKParams.Instance.PurchaseText.ToString();
                String BtnTxt = "PURCHASE";
                string Header = "OUT OF CHIPS";
                if (UIPanelController.Instance.EditableMessageData != null)
                {
                    if (UIPanelController.Instance.EditableMessageData.ContainsKey("OutOfCurrencyMessage"))
                    {
                        string tempMsg = String.Empty;
                        UIPanelController.Instance.EditableMessageData.TryGetValue("OutOfCurrencyMessage", out tempMsg);
                        if (!String.IsNullOrEmpty(tempMsg))
                        {
                            msg = tempMsg;
                        }
                    }
                    if (UIPanelController.Instance.EditableMessageData.ContainsKey("OutOfCurrencyTitle"))
                    {
                        string tempMsg2 = String.Empty;
                        UIPanelController.Instance.EditableMessageData.TryGetValue("OutOfCurrencyTitle", out tempMsg2);
                        if (!String.IsNullOrEmpty(tempMsg2))
                        {
                            Header = tempMsg2;
                        }
                    }
                    if (UIPanelController.Instance.EditableMessageData.ContainsKey("OutOfCurrencyButton"))
                    {
                        string tempMsg3 = String.Empty;
                        UIPanelController.Instance.EditableMessageData.TryGetValue("OutOfCurrencyButton", out tempMsg3);
                        if (!String.IsNullOrEmpty(tempMsg3))
                        {
                            BtnTxt = tempMsg3;
                        }
                    }
                }
                UIPanelController.Instance.SetTourneyNoMoneyDialogue(true, msg, BtnTxt, true, true, null,0, "","", Panels.None, Header);
            }
            else if (metaData.ContainsKey("Unclaimed"))
            {
                DialogueUI.gameObject.SetActive(true);
                String DialogBody = "Something went wrong. Please try again in sometime.";
                String Header = "ALERT";
                String Btn1Name = "Retry";
                String Btn2Name = "Home";
                UnityDebug.Debug.Log("Does metadata Contains : " + metaData.Keys.Contains("DialogBody"));
                UnityDebug.Debug.Log("Is data Corrupt >>>> " + !String.IsNullOrEmpty(metaData["DialogBody"]));
                if (metaData.Keys.Contains("DialogBody") && !String.IsNullOrEmpty(metaData["DialogBody"]))
                {
                    DialogBody = metaData["DialogBody"];
                }
                if (metaData.Keys.Contains("Header") && !String.IsNullOrEmpty(metaData["Header"]))
                {
                    Header = metaData["Header"];
                }
                if (metaData.Keys.Contains("Btn1Name") && !String.IsNullOrEmpty(metaData["Btn1Name"]))
                {
                    Btn1Name = metaData["Btn1Name"];
                }
                if (metaData.Keys.Contains("Btn2Name") && !String.IsNullOrEmpty(metaData["Btn2Name"]))
                {
                    Btn2Name = metaData["Btn2Name"];
                }
                //DialogueUI.ShowDialogue(Header, DialogBody, Btn1Name, Btn2Name, () => { RetryBtnClick(); },
                //                           () => { HomeBtnCLick(); });OnRightArrowClicked

                //DialogueUI.ShowDialogue(Header, DialogBody, Btn1Name, Btn2Name, OnBtn1Click: () => { CloseDialogueUI(); }, OnBtn2Click: () => { OnRightArrowClicked(); });
                DialogueUI.ShowDialogue(Header, DialogBody, Btn1Name, Btn2Name, OnBtn1Click: () => { OnRightArrowClicked(); }, OnBtn2Click: () => { ClaimAllUnclaimedRewards(); }, true);
            }
            else
            {
                DialogueUI.gameObject.SetActive(true);
                String DialogBody = "Something went wrong. Please try again in sometime.";
                String Header = "ALERT";
                String Btn1Name = "Retry";
                String Btn2Name = "Home";
                if (metaData.Keys.Contains("DialogBody") && !String.IsNullOrEmpty(metaData["DialogBody"]))
                {
                    DialogBody = metaData["DialogBody"];
                }
                if (metaData.Keys.Contains("Header") && !String.IsNullOrEmpty(metaData["Header"]))
                {
                    Header = metaData["Header"];
                }
                if (metaData.Keys.Contains("Btn1Name") && !String.IsNullOrEmpty(metaData["Btn1Name"]))
                {
                    Btn1Name = metaData["Btn1Name"];
                }
                if (metaData.Keys.Contains("Btn2Name") && !String.IsNullOrEmpty(metaData["Btn2Name"]))
                {
                    Btn2Name = metaData["Btn2Name"];
                }
                //DialogueUI.ShowDialogue(Header, DialogBody, Btn1Name, Btn2Name, () => { RetryBtnClick(); },
                //                           () => { HomeBtnCLick(); });

                DialogueUI.ShowDialogue(Header, DialogBody, Btn1Name, null, OnBtn1Click: () => { HomeBtnCLick(); });
            }
        }

        public void CloseDialogueUI()
        {
            DialogueUI.gameObject.SetActive(false);
        }

        void ClaimAllUnclaimedRewards()
        {
            LoadingDialog(true, false);
            _ = CommunicationController.Instance.GetClaim("", "all", (data) => { OnUnclaimedRewardsClaimed(data); });
        }

        void OnUnclaimedRewardsClaimed(IAPIClaimData data)
        {
            LoadingDialog(false);
            string UpdatedCurrencyKey = data.RewardInfo.Virtual.Key;
            UserDataContainer.Instance.currencyList.TryGetValue(data.RewardInfo.Virtual.Key, out UpdatedCurrencyKey);
            UserDataContainer.Instance.UpdateUserMoney(data.RewardInfo.Virtual.Value,
                                                    data.RewardInfo.Virtual.Key, true);
            UIPanelController.Instance.UpdateMoneyOnUI(false);

            Dictionary<string, string> _metadata = new Dictionary<string, string>();
            _metadata.Add("AllRewardsClaimed", " ");
            _metadata.Add("RewardAmount", data.RewardInfo.Virtual.Value.ToString());
            _metadata.Add("RewardKey", UpdatedCurrencyKey);
            ShowTourneyItemNew(Panels.TourneyPanel, _metadata);

            UserDataContainer.Instance.UnclaimedRewardsCount = 0;
            CheckForUnclaimedRewardsExclamation();
        }

        public void ShowUpdateDetailsPanel()
        {
            if (UIPanelController.Instance.IsLandScape())
                ChangeName = Instantiate(Resources.Load("UpdatePlayer_Landscape") as GameObject).GetComponent<UpdatePlayerDetails>();
            else
                ChangeName = Instantiate(Resources.Load("UpdatePlayer") as GameObject).GetComponent<UpdatePlayerDetails>();
            ChangeName.RectTransform.SetParent(JamboxSDKParams.Instance.gameObject.GetComponent<RectTransform>(), false);
            RectTransform m_Viewport = ChangeName.GetComponent<RectTransform>();
            UIPanelController.Instance.SetPanelUI(m_Viewport);
            ChangeName.gameObject.SetActive(true);
            ChangeName.SetMetaData(CommonUserData.Instance.userName, profilePicture.sprite.texture);
        }

        private void RetryBtnClick()
        {
            UIPanelController.Instance.ShowPanel(Panels.TourneyPanel, Panels.None);
        }

        private void HomeBtnCLick()
        {
            OnBackBtnClick();
        }

        public void UpdateUserDetails(String name, int avatarId, string avatarGroup, Sprite _imageSprite, Panels Currentpanel, Dictionary<string, string> metaData)
        {
            theList.gameObject.SetActive(false);
            profilePicture.sprite = _imageSprite;
            playerNameText.text = name;
            playerNameText.gameObject.SetActive(true);

            LoadingDialog(true, false);
            _ = CommunicationController.Instance.UpdateUserDetails("", name, avatarId, avatarGroup, (data) => { OnNameUpdateSuccess(data, Currentpanel, metaData); });
        }

        private void OnNameUpdateSuccess(IAPIUpdateUserData Data, Panels Currentpanel, Dictionary<string, string> metaData)
        {
            _ = GetDetailsAfterNameUpdate( Currentpanel, metaData);
        }

        public async Task GetDetailsAfterNameUpdate(Panels Currentpanel, Dictionary<string, string> metaData)
        {
            await JamboxController.Instance.RefreshSession();
            updateUser = StartCoroutine(UpdateUserBasicData());
            ShowTourneyItemNew(Currentpanel, metaData);
        }

        public void ShowTourneyItemNew(Panels Currentpanel, Dictionary<string, string> metaData, bool ShowFriendlyCompleted = false)
        {
            LoadingDialog(true, false);
            if (Currentpanel == Panels.CompletedPanel)
            {
                if (ShowFriendlyCompleted)
                    CurrentPanelEnable = Panels.FriendlyPanel;
                else
                    CurrentPanelEnable = Panels.TourneyPanel;
                theList.UpdateItem(false);
            }
                
            string authToken = "";
            //_ = JamboxController.Instance.StartSession(null, null);
            _ = CommunicationController.Instance.GetCurrencyData("", (data) => { OnCurrencyDataRcvd(data); });
            _ = CommunicationController.Instance.GetTourneydetail(authToken, (data) => { TourneyDataRcvd(data, Currentpanel, metaData); });
        }

        private void PopulateItem()
        {
            theList.ItemCallback = PopulateItem;
            theList.RowCount = UserDataContainer.Instance.UpdatedTourneyData.Values.Count;
            if (UserDataContainer.Instance.UpdatedTourneyData.Values.Count == 0)
            {
                ComingSoon.SetActive(true);
                ComingSoonBody.text = "Duels are not Available Currently. Please Try Tournamentss.";
            }
        }

        private void PopulateItem(TourneyListViewItem item, int rowIndex)
        {
            var tDet = item as TourneyItem;
            if (rowIndex < UserDataContainer.Instance.UpdatedTourneyData.Values.Count)
            {
                string _tID = UserDataContainer.Instance.UpdatedTourneyData.Values.ToList()[rowIndex]._tournament.Tourneyid;
                tDet.UpdateUI(_tID);
                if (_tID.Equals(tourneyId))
                {
                    
                    TourneyDetail _TourneyData = null;
                    UserDataContainer.Instance.UpdatedTourneyData.TryGetValue(tourneyId, out _TourneyData);
                    tourneyId = string.Empty;
                    if (_TourneyData != null && _TourneyData.isJoined)
                        tDet.OnVideoWatched();
                    if (_TourneyData != null && !_TourneyData.isJoined)
                        tDet.PlayAfterPurchase();
                }
            }
        }

        private void PopulateItemDuel()
        {
            theList.RowCount = 0;
            theList.ItemCallback = PopulateItemDuel;
            theList.RowCount = UserDataContainer.Instance.MyDuels.Values.Count;
            if (UserDataContainer.Instance.MyDuels.Values.Count == 0)
            {
                ComingSoon.SetActive(true);
                ComingSoonBody.text = "Duels are not Available Currently. Please Try Tournamentss.";
            }
        }

        private void PopulateItemDuel(TourneyListViewItem item, int rowIndex)
        {
            var tDet = item as TourneyItem;
            if (rowIndex < UserDataContainer.Instance.MyDuels.Values.Count)
            {
                tDet.FillUIElements(UserDataContainer.Instance.MyDuels.Values.ToList()[rowIndex]);
                string _tID = UserDataContainer.Instance.MyDuels.Values.ToList()[rowIndex].Tourneyid;
                if (_tID.Equals(tourneyId))
                {
                    tourneyId = string.Empty;
                    tDet.OnPlayDuelClicked(true);
                }
            }
        }

        private void PopulateItemFriendly()
        {
            FriendlyPanel.GetComponent<FriendlyPanel>().theList.RowCount = 0;
            FriendlyPanel.GetComponent<FriendlyPanel>().theList.ItemCallback = PopulateItemFriendly;
            FriendlyPanel.GetComponent<FriendlyPanel>().theList.RowCount = UserDataContainer.Instance.UpdatedFriendlyData.Values.Count;
            if (UserDataContainer.Instance.UpdatedFriendlyData.Values.Count == 0)
            {
                ComingSoon.SetActive(true);
                ComingSoonBody.text = "You have not joined any Friendly Tournament. Please Try Duels or Tournaments.";
            }
            LoadingDialog(false);
        }
        private void PopulateItemFriendly(FriendlylistViewItem item, int rowIndex)
        {
            var tDet = item as FriendlyTourneyItem;
            if (rowIndex < UserDataContainer.Instance.UpdatedFriendlyData.Values.Count)
            {
                tDet.FriendlyTourneyDet = (UserDataContainer.Instance.UpdatedFriendlyData.Values.ToList()[rowIndex]);
                string _tID = UserDataContainer.Instance.UpdatedFriendlyData.Values.ToList()[rowIndex]._tournament.Tourneyid;
                if (_tID.Equals(tourneyId))
                {
                    tourneyId = string.Empty;
                    //tDet.OnVideoWatched();
                }
            }
        }
        public void OnBackBtnClick()
        {
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            UIPanelController.Instance.ShowPanel(Panels.None, Panels.None, metadata);
            ArenaSDKEvent.Instance.FireOnBackToLobby();
        }

        public void ShowTourneyInfo(String TourneyID)
        {
            RewardPanel.gameObject.SetActive(true);
            RewardPanel.UpdateUi(TourneyID);
        }
        private string tourneyId = String.Empty;

        public void OpenTourneyBtnClick()
        {
            TourneyBtnProcess();
#if GAME_FIREBASE_ENABLED
            Firebase.Analytics.Parameter[] eventarameters = {
                                new Firebase.Analytics.Parameter("screen","tournament_live")
                                };
            Firebase.Analytics.FirebaseAnalytics.LogEvent("arena_screen", eventarameters);
#endif
        }

        public void TourneyBtnProcess(Dictionary<string, string> metaData = null)
        {
            try
            {
                if (metaData != null)
                {
                    metaData.TryGetValue("tourneyid", out tourneyId);
                }
                DisableAllSelector();
                CurrentPanelEnable = Panels.TourneyPanel;
                SetScrollViewStatus(true);
                theList.UpdateItem(true);
                theList.RowCount = 0;

                if (topText == null)
                {
                    SetDoubleTabTexts("Live", "Past");
                    ToggleNavigationUI(true);
                }
                else
                {
                    leftArrow.SetActive(false);
                    rightArrow.SetActive(true);
                    topText.text = "Live";
                }
                //nameText.text = "Live";

                LoadingDialog(false);
                PopulateItem();
                if (UserDataContainer.Instance.UpdatedTourneyData.Values.Count == 0)
                {
                    ComingSoon.SetActive(true);
                    ComingSoonBody.text = "Tournaments are not Available Currently. Please Try Duels.";
                }

                //Updating Bottom Menu
                if(TourneySelected != null)
                    TourneySelected.SetActive(true);
                if (tourneyButtonText != null && TabSelectedColor != null)
                    tourneyButtonText.color = TabSelectedColor;
            }
            catch (Exception e)
            {
                UnityDebug.Debug.Log("Processing TourneyBtnProcess has exception >>>> " + e.ToString());
            }

        }

        public void OpenDuelBtnClick()
        {
            DuelBtnProcess();
        }

        public void DuelBtnProcess(Dictionary<string, string> metaData = null)
        {
#if GAME_FIREBASE_ENABLED
            
            Firebase.Analytics.Parameter[] eventarameters = {
                                new Firebase.Analytics.Parameter("screen","duel")
                                };
            Firebase.Analytics.FirebaseAnalytics.LogEvent("arena_screen", eventarameters);
#endif

            if (metaData != null)
                metaData.TryGetValue("tourneyid", out tourneyId);
            DisableAllSelector();
            CurrentPanelEnable = Panels.DuelPanel;
            theList.UpdateItem(false, true);

            if (topText == null)
            {
                SetSingleTabTexts("Duels");
            }
            else
            {
                leftArrow.SetActive(false);
                rightArrow.SetActive(false);
                topText.text = "Duels";
            }
            

            string authToken = string.Empty;
            //nameText.text = "Duels";
            LoadingDialog(false);
            PopulateItemDuel();

            //Updating Bottom Menu
            duelsButtonText.color = TabSelectedColor;
            DuelSelected.SetActive(true);

        }

        public void OpenFriendlyBtnClick()
        {
            FriendlyBtnProcess();
#if GAME_FIREBASE_ENABLED
            
            Firebase.Analytics.Parameter[] eventarameters = {
                                new Firebase.Analytics.Parameter("screen","friendly_live")
                                };
            Firebase.Analytics.FirebaseAnalytics.LogEvent("arena_screen", eventarameters);
#endif
        }

        public void FriendlyBtnProcess(Dictionary<string, string> metaData = null)
        {

            if (metaData != null)
                metaData.TryGetValue("tourneyid", out tourneyId);
            DisableAllSelector();
            CurrentPanelEnable = Panels.FriendlyPanel;
            FriendlyPanel.SetActive(true);
            TourneyDetail.SetActive(false);
            FriendlyPanel.GetComponent<FriendlyPanel>().theList.gameObject.SetActive(true);

            if (topText == null)
            {
                SetDoubleTabTexts("Live", "Past");
                ToggleNavigationUI(true);
            }
            else
            {
                leftArrow.SetActive(false);
                rightArrow.SetActive(true);
                topText.text = "Live";
            }
            //nameText.text = "Live";

            LoadingDialog(true, false);
            PopulateItemFriendly();

            //Updating Bottom Menu
            friendlyButtonText.color = TabSelectedColor;
            FriendlySelected.SetActive(true);

        }
        private void DisableAllSelector()
        {
            if(TourneySelected != null)
                TourneySelected.SetActive(false);
            if (DuelSelected != null)
                DuelSelected.SetActive(false);
            if (FriendlySelected != null)
                FriendlySelected.SetActive(false);

            if (tourneyButtonText != null && TabNotSelectedColor != null)
                tourneyButtonText.color = TabNotSelectedColor;
            if (duelsButtonText != null && TabNotSelectedColor != null)
                duelsButtonText.color = TabNotSelectedColor;
            if (friendlyButtonText != null && TabNotSelectedColor != null)
                friendlyButtonText.color = TabNotSelectedColor;

            if (ComingSoon != null && ComingSoon.activeInHierarchy)
            {
                ComingSoon.SetActive(false);
            }
            theList.RowCount = 0;
            if (FriendlyPanel != null)
                FriendlyPanel.SetActive(false);
            if (TourneyDetail != null)
                TourneyDetail.SetActive(true);
        }

        public void OnLeftArrowClicked()
        {

            if (topText == null)
            {
                ToggleNavigationUI(true);
            }
            else
            {
                leftArrow.SetActive(true);
                rightArrow.SetActive(false);
            }

            if (CurrentPanelEnable == Panels.TourneyPanel)
            {
                OpenTourneyBtnClick();
            }
            if (CurrentPanelEnable == Panels.DuelPanel)
            {
                OpenDuelBtnClick();
            }
            if (CurrentPanelEnable == Panels.FriendlyPanel)
            {
                OpenFriendlyBtnClick();
            }

        }
        public void OnRightArrowClicked()
        {

            if (topText == null)
            {
                ToggleNavigationUI(false);
            }
            else
            {
                leftArrow.SetActive(true);
                rightArrow.SetActive(false);
            }

            OpenCompletdList();
        }

        public void OpenCompletdList()
        {
            CompletdListProcess();
        }

        public void CompletdListProcess(Dictionary<string, string> metaData = null)
        {
            FriendlyPanel.SetActive(false);
            TourneyDetail.SetActive(true);
            ComingSoon.SetActive(false);
            LoadingDialog(true, false);
            theList.UpdateItem(false);

            if (topText == null)
            {
                ToggleNavigationUI(false);
            }
            else
            {
                leftArrow.SetActive(true);
                rightArrow.SetActive(false);

                topText.text = "Past";
            }

            theList.RowCount = 0;
            //nameText.text = "Past";
            string authToken = string.Empty;
            String Category = "1";
            string screenName = "";
            if (CurrentPanelEnable == Panels.TourneyPanel)
            {
                Category = "1";
                screenName = "tournament_past";
            }
            if (CurrentPanelEnable == Panels.DuelPanel)
            {
                Category = "2";
                screenName = "duel_past";
            }
            if (CurrentPanelEnable == Panels.FriendlyPanel)
            {
                Category = "3";
                screenName = "friendly_past";
                InFriendlyCompleted = true;
            }
            else
            {
                InFriendlyCompleted = false;
            }

#if GAME_FIREBASE_ENABLED
            
            Firebase.Analytics.Parameter[] eventarameters = {
                                new Firebase.Analytics.Parameter("screen",screenName)
                                };
            Firebase.Analytics.FirebaseAnalytics.LogEvent("arena_screen", eventarameters);
#endif

            //Updating Bottom Menu
            if (CurrentPanelEnable == Panels.TourneyPanel)
            {
                tourneyButtonText.color = TabSelectedColor;
                TourneySelected.SetActive(true);
            }
            else if (CurrentPanelEnable == Panels.FriendlyPanel)
            {
                friendlyButtonText.color = TabSelectedColor;
                FriendlySelected.SetActive(true);
            }

            _ = CommunicationController.Instance.GetCompletedTourneyData(authToken, Category, (data) => { CompletedTDataRcvd(data); });
        }

        private void CompletedTDataRcvd(IAPICompTourneyList data)
        {
            LoadingDialog(false);
            UserDataContainer.Instance.UpdateCompTourneyData(data);
            PopulateItemCompleted();
        }

        void ToggleNavigationUI(bool inLive)
        {
            if (inLive)
            {
                liveButton.gameObject.SetActive(false);
                PastButton.gameObject.SetActive(true);

                liveActiveGameobject.SetActive(true);
                PastActiveGameobject.SetActive(false);
            }
            else
            {
                liveButton.gameObject.SetActive(true);
                PastButton.gameObject.SetActive(false);

                liveActiveGameobject.SetActive(false);
                PastActiveGameobject.SetActive(true);
            }

            if (claimSymbolContainer != null)
                claimSymbolContainer.SetActive(inLive && CurrentPanelEnable == Panels.TourneyPanel);
        }

        void SetSingleTabTexts(string _name)
        {
            singleTabMenu.SetActive(true);
            doubleTabMenu.SetActive(false);

            singleTabText.text = _name;
        }

        void SetDoubleTabTexts(string _menu1, string _menu2)
        {
            singleTabMenu.SetActive(false);
            doubleTabMenu.SetActive(true);

            foreach(Text t in doubleLiveText)
            {
                t.text = _menu1;
            }
            foreach (Text t in doublePastText)
            {
                t.text = _menu2;
            }
        }

        public void OnExitBtnClick()
        {

        }

        private void PopulateItemCompleted()
        {
            theList.ItemCallback = PopulateItemCompleted;
            theList.RowCount = UserDataContainer.Instance.CompletedTourney.Count;
            if (UserDataContainer.Instance.CompletedTourney.Count == 0)
            {
                ComingSoon.SetActive(true);
                ComingSoonBody.text = "You have not completed any  Tournament. Please complete some.";
            }
        }

        private void PopulateItemCompleted(TourneyListViewItem item, int rowIndex)
        {
            var tDet = item as CompTourneyItem;
            if (rowIndex < UserDataContainer.Instance.CompletedTourney.Count)
            {
                tDet.CompTourneyDet = UserDataContainer.Instance.CompletedTourney[rowIndex];
                if (InFriendlyCompleted)
                    tDet.IsFriendlyCompleted(true);
                else
                    tDet.IsFriendlyCompleted(false);
            }
        }

        public void DisableFriendlyPanel(bool status)
        {
            FriendlyPanel.GetComponent<FriendlyPanel>().theList.gameObject.SetActive(status);
            ComingSoon.SetActive(status);
        }
    }
}

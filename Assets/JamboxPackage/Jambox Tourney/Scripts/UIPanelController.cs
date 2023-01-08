namespace Jambox.Tourney.UI
{
    using System;
    using System.IO;
    using System.Collections;
    using System.Collections.Generic;
    using Jambox.Tourney.Connector;
    using Jambox.Server;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Runtime.Serialization;
    using System.Security.Cryptography;
    using UnityEngine.Scripting;
    using Jambox.Common.TinyJson;
    using Jambox.Common.Utility;
    using UnityEditor;

    public enum Panels
    {
        TourneyPanel,
        DuelPanel,
        FriendlyPanel,
        LeaderBoardPanel,
        ResultPanel,
        CompletedPanel,
        MatchMakingPanel,
        DuelResultPanel,
        DetailsPanel,
        DialoguePanel,
        None
    }

    public class ScoreData
    {
        public string TourneyId;
        public long Score;
        public long subScore;
        public string displayScore;
        public ReplayData replayData;

        public ScoreData(string _tourneyId, long _score, string _displayScore, long _subScore = 0, ReplayData _replayData = null)
        {
            TourneyId = _tourneyId;
            Score = _score;
            subScore = _subScore;
            displayScore = _displayScore;
            replayData = _replayData;
        }
    }
    [Serializable]
    public class MsgData
    {
        public string MessageKey;
        public string MessageBody;

    }
    public class UIPanelController : MonoSingleton<UIPanelController>
    {
        //private static UIPanelController m_Instance = null;
        public ScoreData tempScore;
        private LeaderBoardPanel LBPanelLoaded;
        private SubmitScore ResultPanelLoaded;
        private TourneyPanel TourneylistLoaded;
        private CompletedTourney CompletedPanelLoaded;
        private MatchmakingPanel MatchMakingLoaded;
        private DuelResultPanel DuelResultLoaded;
        private DetailsPanelScript DetailsPanelLoaded;

        private GameObject ParentPanel;
        public Sprite bgSprite;
        public GameObject jamboxCamera;
        public Dictionary<string, string> EditableMessageData;

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
        private void Awake()
        {
            EditableMessageData = new Dictionary<string, string>();
            LoadMessageAsset();
            if(JamboxSDKParams.Instance.ArenaParameters != null &&
                JamboxSDKParams.Instance.ArenaParameters.bgSprite != null)
                bgSprite = JamboxSDKParams.Instance.ArenaParameters.bgSprite;
        }

        private void CheckFilesInResource()
        {
            string path2 = Application.dataPath + "/Resources";
            string pathN = Path.Combine(path2, "Theme2", "Portrait");
            //FileInfo[] fileInfo = levelDirectoryPath.GetFiles("*.*", SearchOption.AllDirectories);

        }

        private void LoadMessageAsset ()
        {
            string filePath = "Assets/Resources/ConfigMessage.json";
            if(!File.Exists(filePath))
            {
                UnityDebug.Debug.LogWarning("The Data File Does not exist.");
                return;
            }
            TextAsset MessageData = Resources.Load<TextAsset>("ConfigMessage");
            string textData = MessageData.text;
            MsgData[] Editablemessages = CustomJsonHelper.FromJson<MsgData>(textData);
            foreach(var data in Editablemessages)
            {
                EditableMessageData.Add(data.MessageKey, data.MessageBody);
            }
        }

        public void ErrorFromServerRcvd(int errorcode, string errorString)
        {
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            metadata.Add("Header", "Server Error");
            if (string.IsNullOrEmpty(errorString))
                errorString = "Our servers are not responding. Please try after sometime.";
            metadata.Add("DialogBody", errorString);
            metadata.Add("Btn1Name", "Home");
            ShowPanel(Panels.DialoguePanel, Panels.None, metadata);
        }

        public void SetParentPanel (GameObject parent)
        {
            ParentPanel = parent;

            //Camera Setup
            //Camera _cam = Instantiate(jamboxCamera).GetComponent<Camera>();
            //ParentPanel.GetComponentInParent<Canvas>().worldCamera = _cam;
        }
        public void UpdateMoneyOnUI (bool _showConfetti = true)
        {
            if(TourneylistLoaded != null && TourneylistLoaded.isActiveAndEnabled)
            {
                TourneylistLoaded.UpdateCurrency(_showConfetti);
            }
            if(DetailsPanelLoaded != null && DetailsPanelLoaded.isActiveAndEnabled)
            {
                DetailsPanelLoaded.UpdateMoneyOnUI(_showConfetti);
            }
        }
        
        public bool IsLandScape ()
        {
#if !UNITY_EDITOR
            if(Screen.orientation == ScreenOrientation.Landscape)
            {
                //UnityDebug.Debug.Log("This game is in Landscapee >>>>>>>>");
                return true;
            }
            if(Screen.orientation == ScreenOrientation.Portrait)
            {
                //UnityDebug.Debug.Log("This game is in Portrait >>>>>>>>");
            }
            return false;
#elif UNITY_EDITOR
            if (Screen.height > Screen.width)
            {
                return false;
            }
            else
            {
                return true;
            }
#endif
        }

        public void SetTourneyScrollActive (bool Active)
        {
            if(TourneylistLoaded != null && TourneylistLoaded.gameObject.activeInHierarchy)
            {
                TourneylistLoaded.SetScrollViewStatus(Active);
            }
        }

        public void LoadingDialogue(bool isShow, bool stillInteractable = true)
        {
            if (TourneylistLoaded != null && TourneylistLoaded.gameObject.activeInHierarchy)
            {
                TourneylistLoaded.LoadingDialog(isShow, stillInteractable);
            }
        }

        public void SetPrivateTourneyActive(bool Active)
        {
            if (TourneylistLoaded != null && TourneylistLoaded.gameObject.activeInHierarchy)
            {
                TourneylistLoaded.SetFriendlyScrollView(Active);
            }
        }
        public void UpdateMoneyOnTourneyPanel ()
        {
            if(TourneylistLoaded != null)
                TourneylistLoaded.UpdateCurrency();
        }

        public void SubtractMoneyAnimation(int _amnt)
        {
            TourneylistLoaded.SubtractMoneyAnimation(_amnt);
        }

        public void UpdateMoneyOnTCompPanel()
        {
            CompletedPanelLoaded.UpdateCurrency();
        }
        public void OnWatchVideo (string TourneyID, Panels previous, ActionRequiredFromUser actionRequired)
        {
            ArenaSDKEvent.Instance.FireOnWatchAD(TourneyID, previous, actionRequired);
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            ShowPanel(Panels.None, Panels.None, metadata);
        }

        public void SetTourneyNoMoneyDialogue(bool Status, string msg, string BtnTxt, bool isBuyBtn, bool isClosebtn,
            Action OnBtnClick = null, int money = 0, string CurrKey = "", string _tID = "", Panels prev = Panels.None, string head = "", bool fromTourneyPanel = true, bool outOfAttempts = false)
        {

            if (!fromTourneyPanel)
            {
                ShowPanel(Panels.DialoguePanel);
            }

            if (TourneylistLoaded != null && TourneylistLoaded.gameObject.activeInHierarchy)
            {
                TourneylistLoaded.SetNoMoneyDialogue(Status, msg, BtnTxt, isBuyBtn, isClosebtn, OnBtnClick, money, CurrKey, _tID, prev, head, outOfAttempts);
            }
        }
        
        internal void ShowLeaderBoard (string _tId, Panels CurrPanel = Panels.TourneyPanel)
        {
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            metadata.Add("lbid", _tId);
            ShowPanel(Panels.LeaderBoardPanel, CurrPanel, metadata);
        }

        public void SubmitScore (string tourneyId, long Score, int subScore, string displayScore, Panels Previous)
        {
            tempScore = new ScoreData(tourneyId, Score, displayScore);
            Panels prevPanel = Previous;
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            ShowPanel(Panels.ResultPanel, prevPanel, metadata);
        }

        public void SubmitDuelScore(string tourneyId, long Score, int subScore, string displayScore, ReplayData replayData)
        {
            tempScore = new ScoreData(tourneyId, Score, displayScore, subScore, replayData);
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            metadata.Add("tourneyid", tourneyId);
            ShowPanel(Panels.DuelResultPanel, Panels.TourneyPanel, metadata);
        }

        public void ShowTourneyInfo (String TourneyID)
        {
            if(TourneylistLoaded != null)
            {
                TourneylistLoaded.ShowTourneyInfo(TourneyID);
            }
        }

        public void DisableFriendlyPanelView(bool status)
        {
            if (TourneylistLoaded != null)
            {
                TourneylistLoaded.DisableFriendlyPanel(status);
            }
        }

        public void ShowClaimSuccessPanel (int reward, string currency)
        {
            if (TourneylistLoaded != null && TourneylistLoaded.isActiveAndEnabled)
            {
                TourneylistLoaded.ShowClaimSuccess(reward, currency);
            }
            if (DetailsPanelLoaded != null && DetailsPanelLoaded.isActiveAndEnabled)
            {
                DetailsPanelLoaded.ShowClaimSuccess(reward, currency);
            }
        }

        public void ShowStore (int StoreMoney, String Currency, String TourneyId, Panels prev)
        {
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            ShowPanel(Panels.None, Panels.None, metadata);
            string CurrencyKey = Currency;
            UserDataContainer.Instance.currencyList.TryGetValue(Currency, out CurrencyKey);
            ArenaSDKEvent.Instance.FireOnStoreClick(StoreMoney, Currency, TourneyId, prev);
        }

        public String ThemePath(String basePrefab)
        {
            string newPath = "";
            if (JamboxSDKParams.Instance.ActiveTheme == ThemeType.CustomTheme)
            {
                newPath = "JamboxArenaUI/" + basePrefab;
                return newPath;
            }
            else {
                if (IsLandScape())
                {
                    newPath = "Theme/Landscape/" + basePrefab;
                    return newPath;
                }
                else
                {
                    newPath = "Theme/Portrait/" + basePrefab;
                    return newPath;
                }
            }
        }

        public void ShowPanel(Panels panelName, Panels prevPanel = Panels.None, Dictionary<string, string> metaData = null, bool checkUnclaimed = false, bool _friendlyCompleted = false)
        {
            if (ParentPanel == null)
            {
                ParentPanel = JamboxSDKParams.Instance.gameObject;
            }
            DestroyAll();
            RectTransform m_Viewport;
            String resourcePath = "";
            if (panelName == Panels.TourneyPanel || panelName == Panels.CompletedPanel ||
                panelName == Panels.DuelPanel || panelName == Panels.FriendlyPanel)
            {
                CheckFilesInResource();
                resourcePath = ThemePath("TourneyPanel");
                Debug.Log("resourcePath : " + resourcePath);
                TourneylistLoaded = Instantiate(Resources.Load(resourcePath) as GameObject).GetComponent<TourneyPanel>();
                TourneylistLoaded.RectTransform.SetParent(ParentPanel.GetComponent<RectTransform>(), false);
                m_Viewport  = TourneylistLoaded.GetComponent<RectTransform>();
                SetPanelUI(m_Viewport);
                TourneylistLoaded.gameObject.SetActive(true);
                TourneylistLoaded.ShowTourneyItem(prevPanel, panelName, metaData, ShowFriendlyCompleted: _friendlyCompleted);

                if (checkUnclaimed)
                    if (panelName == Panels.TourneyPanel)
                        TourneylistLoaded.CheckForUnclaimedRewards();

            }
            if(panelName == Panels.ResultPanel)
            {
                resourcePath = ThemePath("SubmitScore");
                ResultPanelLoaded = Instantiate(Resources.Load(resourcePath) as GameObject).GetComponent<SubmitScore>();
                ResultPanelLoaded.RectTransform.SetParent(ParentPanel.GetComponent<RectTransform>(), false);
                m_Viewport = ResultPanelLoaded.GetComponent<RectTransform>();
                SetPanelUI(m_Viewport);
                ResultPanelLoaded.gameObject.SetActive(true);
                ResultPanelLoaded.UpdateSubmitScoreData(prevPanel);
            }
            if (panelName == Panels.DetailsPanel)
            {
                resourcePath = ThemePath("DetailsScreen");
                DetailsPanelLoaded = Instantiate(Resources.Load(resourcePath) as GameObject).GetComponent<DetailsPanelScript>();
                DetailsPanelLoaded.RectTransform.SetParent(ParentPanel.GetComponent<RectTransform>(), false);
                m_Viewport = DetailsPanelLoaded.GetComponent<RectTransform>();
                SetPanelUI(m_Viewport);
                DetailsPanelLoaded.gameObject.SetActive(true);
                string tid = "";
                if (prevPanel == Panels.CompletedPanel)
                {
                    metaData.TryGetValue("lbid", out tid);
                }
                else
                {
                    metaData.TryGetValue("tourneyid", out tid);
                }
                DetailsPanelLoaded.SetTourneyId(tid, prevPanel, _friendlyCompleted);
            }
            if (panelName == Panels.MatchMakingPanel)
            {
                resourcePath = ThemePath("MatchmakingPanel");
                MatchMakingLoaded = Instantiate(Resources.Load(resourcePath) as GameObject).GetComponent<MatchmakingPanel>();
                MatchMakingLoaded.RectTransform.SetParent(ParentPanel.GetComponent<RectTransform>(), false);
                m_Viewport = MatchMakingLoaded.GetComponent<RectTransform>();
                SetPanelUI(m_Viewport);
                MatchMakingLoaded.gameObject.SetActive(true);
                MatchMakingLoaded.SetData(metaData);
            }
            if (panelName == Panels.DuelResultPanel)
            {
                resourcePath = ThemePath("ResultPanel");
                DuelResultLoaded = Instantiate(Resources.Load(resourcePath) as GameObject).GetComponent<DuelResultPanel>();
                DuelResultLoaded.RectTransform.SetParent(ParentPanel.GetComponent<RectTransform>(), false);
                m_Viewport = DuelResultLoaded.GetComponent<RectTransform>();
                SetPanelUI(m_Viewport);
                DuelResultLoaded.gameObject.SetActive(true);
                DuelResultLoaded.SetTournEyID(metaData);
            }
            if(panelName == Panels.DialoguePanel)
            {
                resourcePath = ThemePath("TourneyPanel");
                TourneylistLoaded = Instantiate(Resources.Load(resourcePath) as GameObject).GetComponent<TourneyPanel>();
                UnityDebug.Debug.LogInfo("Inside ShowPanel  222 ParentPanel is null : " + (ParentPanel == null));
                TourneylistLoaded.RectTransform.SetParent(ParentPanel.GetComponent<RectTransform>(), false);
                m_Viewport = TourneylistLoaded.GetComponent<RectTransform>();
                SetPanelUI(m_Viewport);
                TourneylistLoaded.gameObject.SetActive(true);
                TourneylistLoaded.ShowTourneyItem(prevPanel, panelName, metaData, true);
            }

        }

        public void CheckForUnclaimedExclamation()
        {
            if (TourneylistLoaded != null)
                TourneylistLoaded.CheckForUnclaimedRewardsExclamation();
        }

        public void UpdateUserDetails(String name, int avatarId, string avatarGroup, Sprite _imageSprite, Panels Currentpanel, Dictionary<string, string> metaData)
        {
            if (TourneylistLoaded != null)
                TourneylistLoaded.UpdateUserDetails(name, avatarId, avatarGroup, _imageSprite, Currentpanel, metaData);
        }

        public void SetPanelUI (RectTransform m_Viewport)
        {
            m_Viewport.anchorMin = Vector3.zero;
            m_Viewport.anchorMax = Vector3.one;
            m_Viewport.anchoredPosition = new Vector2(0f, 0f);
            m_Viewport.sizeDelta = new Vector2(0, 0);
            m_Viewport.localScale = new Vector3(1, 1, 1);
        }

        private void DestroyAll()
        {
            if (TourneylistLoaded != null)
            {
                Destroy(TourneylistLoaded.gameObject);
            }
            if (ResultPanelLoaded != null)
            {
                Destroy(ResultPanelLoaded.gameObject);
            }
            if (LBPanelLoaded != null)
            {
                Destroy(LBPanelLoaded.gameObject);
            }
            if (CompletedPanelLoaded != null)
            {
                Destroy(CompletedPanelLoaded.gameObject);
            }
            if(DetailsPanelLoaded != null)
            {
                Destroy(DetailsPanelLoaded.gameObject);
            }
            if (MatchMakingLoaded != null)
            {
                Destroy(MatchMakingLoaded.gameObject);
            }
            if(DuelResultLoaded != null)
            {
                Destroy(DuelResultLoaded.gameObject);
            }
        }
    }
}

public enum ActionRequiredFromUser
{
    AD_JOIN_TOURNEY,
    AD_PLAY_TOURNEY,
    AD_PLAY_DUEL,
    PURCHASE_JOIN_TOURNEY,
    NONE
}

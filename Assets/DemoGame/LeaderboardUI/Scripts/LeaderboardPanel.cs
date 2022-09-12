namespace Jambox.Leaderboard.UI
{
    using Jambox.Common;
    using Jambox.Common.Utility;
    using JBX.Leaderboard.Controller;
    using JBX.Leaderboard.Data;
    using Jambox.Tourney.Connector;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Threading.Tasks;

    public class LeaderboardPanel : MonoBehaviour
    {
        public bool IsFullLB = false;
        public GameObject content;
        public GameObject LoadingPanel;
        public TabMenuDetails tabMenuDetails;

        public Text moneyText;
        public Image moneyIconImage;
        private Sprite rewardIconSprite;

        public Text leaderNameText;
        public Text endTimeText;
        public Text PartitionText;
        public Image leaderboardImage;
        public Button lastWeekResultBtn;

        private List<LeaderBoardRewardData> rewardsList;
        public RectTransform rewardsContainer;
        public RewardListView rewardListView;

        private List<LeaderBoardData> leaderboardList;
        public RectTransform leaderboardRect;
        public LeaderBoardListView leaderboardListView;

        public UIPendingRewardPanel pendingRewardsPanel;
        public PreviousResultPanel previousResultPanel;

        public LeaderBoardItem myPlayerInfoAtTop;
        public LeaderBoardItem myPlayerInfoAtBottom;

        private Queue<ImageRequestData> imageRequestQueue = new Queue<ImageRequestData>();
        private Dictionary<string, LeaderBoardListViewItem> activeLBItemsWaitingForImage = new Dictionary<string, LeaderBoardListViewItem>();

        private Coroutine imageDownloadCoroutine = null;

        public Text NameHeader;
        public Text ScoreHeader;
        public Text RankHeader;

        private Action OnCloseAction;

        private Action OnContinueAction;
        private string OnContinueText;
        public Text OnContinueTextObj;

        private void OnEnable()
        {
            UnityDebug.Debug.Log("OnEnable of LeaderboardPanel Hit >>>");
            content.gameObject.SetActive(false);
            ToggleLoadingPanel(true);
        }

        public void ToggleLoadingPanel(bool value)
        {
            LoadingPanel.SetActive(value);
        }

        public void RequestLeaderboardData(String LBID, String PartititonKey = "", bool showLastWeekResult = true, int money = 0)
        {
            UnityDebug.Debug.Log("RequestLeaderboardData of LeaderboardPanel Hit >>>");
            _ = JBXLeaderboardCommunicator.Instance.GetLeaderboardRecord(LBID, PartititonKey,
                (data) => { OnLeaderboardDetailsRcvd(LBID, PartititonKey, data); });
            rewardIconSprite = JamboxSDKParams.Instance.ArenaParameters.CoinBG;
            if (IsFullLB)
                lastWeekResultBtn.gameObject.SetActive(showLastWeekResult);
        }

        void OnLeaderboardDetailsRcvd(String LBID, String PartititonKey, IApiLeaderRecordList dataNew)
        {
            content.gameObject.SetActive(true);
            ToggleLoadingPanel(false);
            if (IsFullLB)
                tabMenuDetails.InitializeTabs();

            NameHeader.text = dataNew.LeaderBoardHeaders.NameText;
            ScoreHeader.text = dataNew.LeaderBoardHeaders.ScoreText;
            RankHeader.text = dataNew.LeaderBoardHeaders.RankText;

            LeaderBoardMasterData lbMasterData = LeaderboardDataContain.Instance.GetMasterLBDatawithLBID(LBID);
            if (IsFullLB)
                ShowRewards(lbMasterData.LBRewardList);

            //Top Details
            leaderNameText.text = lbMasterData.Name;
            if(!String.IsNullOrEmpty(PartititonKey)&& PartitionText != null)
            {
                PartitionText.text = PartititonKey;
                PartitionText.gameObject.SetActive(true);
            }
            else if(PartitionText != null)
            {
                PartitionText.gameObject.SetActive(false);
            }
            if (endTimeText != null && !String.IsNullOrEmpty(lbMasterData.ResetTime))
            {
                try
                {
                    TimeSpan elapsed = DateTime.Parse(lbMasterData.ResetTime).ToUniversalTime().Subtract(DateTime.UtcNow);
                    endTimeText.text = "ENDS IN : " + EndTimeInFormat(elapsed);
                }
                catch
                {
                    endTimeText.gameObject.SetActive(false);
                }
            }
            else if (endTimeText != null)
                endTimeText.gameObject.SetActive(false);

            Debug.Log("Is LBContainer Instance Null : " + (LeaderboardDataContain.Instance == null));

            //Leader board
            leaderboardList = new List<LeaderBoardData>();
            List<LeaderBoardData> templist = new List<LeaderBoardData>();

            foreach(var tempo in dataNew.LeaderRecords)
            {
                LeaderBoardData newData = new LeaderBoardData(tempo);
                leaderboardList.Add(newData);
            }
            LeaderBoardData _myPlayerData = null;
            int _myPlayerIndex = 0;
            for(int i = 0; i < leaderboardList.Count; i++)
            {
                //if (tempLeaderboardData[i].PlayerId == 4 + "")
                if (leaderboardList[i].PlayerId == JamboxController.Instance.getMyuserId())
                {
                    _myPlayerData = leaderboardList[i];
                    _myPlayerIndex = i;
                }
                LeaderBoardData itemData = leaderboardList[i];
                templist.Add(itemData);
            }
            leaderboardList = templist.OrderBy(o => o.Rank).ToList();
            ShowLeaderBoard();

            if (_myPlayerData != null)
            {
                myPlayerInfoAtTop.FillItem(_myPlayerData, JamboxSDKParams.Instance.ArenaParameters.CoinBG);
                myPlayerInfoAtBottom.FillItem(_myPlayerData, JamboxSDKParams.Instance.ArenaParameters.CoinBG);

                StartCoroutine(DownloadAvatarForMyPlayerInfo_Temp(_myPlayerData));
                leaderboardListView.UpdateMyPlayerRequiredDetails(_myPlayerIndex);
            }
            //leaderboardListView.UpdateMyPlayerRequiredDetails(_myPlayerIndex);
        }

        IEnumerator DownloadAvatarForMyPlayerInfo_Temp(LeaderBoardData data)
        {
            if (String.IsNullOrEmpty(data.AvatarUrl))
                data.AvatarUrl = "https://s3.ap-southeast-1.amazonaws.com/dl.gamenova/users/Avatars_6.png";
            WWW www = new WWW(data.AvatarUrl);
            yield return www;
            Sprite avatarSprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));

            myPlayerInfoAtTop.UpdateAvatar(avatarSprite);
            myPlayerInfoAtBottom.UpdateAvatar(avatarSprite);
        }

        public void  ShowMyPlayerInfo(bool value, bool topPosition = true)
        {
            if (value)
            {
                myPlayerInfoAtTop.gameObject.SetActive(topPosition);
                myPlayerInfoAtBottom.gameObject.SetActive(!topPosition);
            }
            else
            {
                myPlayerInfoAtTop.gameObject.SetActive(false);
                myPlayerInfoAtBottom.gameObject.SetActive(false);
            }
        }

        public void OnBackBtnClicked()
        {
            if (OnCloseAction != null)
            {
                OnCloseAction();
            }
            Destroy(this.gameObject);
        }

        public void SetActions(Action OnClose, Action OnContinue = null, string OnContinueText = "")
        {
            OnContinueAction = OnContinue;
            if (!string.IsNullOrEmpty(OnContinueText))
                OnContinueTextObj.text = OnContinueText;

            OnCloseAction = OnClose;
        }

        public void OnContinueBtnClicked()
        {
            if (OnContinueAction != null)
            {
                OnContinueAction();
            }
            Destroy(this.gameObject);
        }

        public void ShowLeaderBoard()
        {
            leaderboardListView.gameObject.SetActive(true);
            leaderboardListView.ItemCallback = PopulateItem;
            leaderboardListView.RowCount = leaderboardList.Count;
        }

        private void PopulateItem(LeaderBoardListViewItem item, int rowIndex)
        {
            var tDet = item as LeaderBoardItem;
            if (rowIndex < leaderboardList.Count)
            {
                LeaderBoardData lbData = leaderboardList[rowIndex];
                tDet.FillItem(lbData, rewardIconSprite);
                if (lbData.avatarSprite == null)
                {
                    if (activeLBItemsWaitingForImage.ContainsKey(lbData.PlayerId))
                        return;
                    RequestImageDownload(rowIndex, lbData.AvatarUrl);
                    activeLBItemsWaitingForImage.Add(lbData.PlayerId, item);
                }
            }
        }

        #region FULL_LB_METHODS
        public void ShowRewards(List<LeaderBoardRewardData> LBRewardList)
        {
            if (LBRewardList.Count > 0)
            {
                rewardsList = LBRewardList;
                rewardListView.ItemCallback = PopulateItem;
                rewardListView.RowCount = LBRewardList.Count;
            }
            else
            {
                NoRewards();
            }
        }

        void NoRewards()
        {
            rewardsContainer.gameObject.SetActive(false);
            leaderboardRect.offsetMax = new Vector2(leaderboardRect.offsetMax.x, leaderboardRect.offsetMax.y + rewardsContainer.sizeDelta.y);
        }

        private void PopulateItem(RewardListViewItem item, int rowIndex)
        {
            var tDet = item as RewardItem;
            if (rowIndex < rewardsList.Count)
            {
                tDet.FillItem(rewardsList[rowIndex]);
            }
        }

        public void OnPreviousResultBtnClicked()
        {
            previousResultPanel.gameObject.SetActive(true);
            //previousResultPanel.SetMetadata(TourneyManager.GlobalLeaderboardID);
        }

        void SetLeaderboardImage(Sprite sprite)
        {
            if (leaderboardImage == null)
                return;

            if (sprite != null)
            {
                leaderboardImage.sprite = sprite;
                leaderboardImage.gameObject.SetActive(true);
            }
        }
        #endregion

        private void Update()
        {
            if (imageDownloadCoroutine == null)
            {
                if (imageRequestQueue.Count > 0)
                {
                    imageDownloadCoroutine = StartCoroutine(ImageDownloadCoroutine());
                }
            }
        }

        IEnumerator ImageDownloadCoroutine()
        {
            while (imageRequestQueue.Count > 0)
            {
                ImageRequestData data = imageRequestQueue.Dequeue();
                if (String.IsNullOrEmpty(data.avatarURL))
                    data.avatarURL = "https://s3.ap-southeast-1.amazonaws.com/dl.gamenova/users/Avatars_6.png";
                WWW www = new WWW(data.avatarURL);
                yield return www;
                Sprite avatarSprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
                ImageDownloadComplete(data.index, avatarSprite);
            }
            imageDownloadCoroutine = null;
        }

        void RequestImageDownload(int index, string url)
        {
            ImageRequestData data = new ImageRequestData(index, url);
            imageRequestQueue.Enqueue(data);
        }

        void ImageDownloadComplete(int index, Sprite avatarSprite)
        {
            leaderboardList[index].avatarSprite = avatarSprite;

            LeaderBoardData lbData = leaderboardList[index];
            if (activeLBItemsWaitingForImage.ContainsKey(lbData.PlayerId))
            {
                LeaderBoardItem item = activeLBItemsWaitingForImage[lbData.PlayerId] as LeaderBoardItem;
                if (item.gameObject.activeInHierarchy)
                {
                    if(item.GetCurrentPlayerID() == lbData.PlayerId)
                    {
                        item.UpdateAvatar(lbData.avatarSprite);
                    }
                }
                activeLBItemsWaitingForImage.Remove(lbData.PlayerId);
            }
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

    }
    struct ImageRequestData
    {
        public int index;
        public string avatarURL;

        public ImageRequestData(int index, string url)
        {
            this.index = index;
            this.avatarURL = url;
        }
    }
}
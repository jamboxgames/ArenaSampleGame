namespace Jambox.Leaderboard.UI
{
    using Jambox.Common;
    using Jambox.Common.Utility;
    using JBX.Leaderboard.Controller;
    using JBX.Leaderboard.Data;
    using Jambox.Tourney.Connector;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;

    public class PreviousResultPanel : MonoBehaviour
    {
        public Text leaderNameText;
        public Text NoLBText;
        public Image leaderboardImage;
        public GameObject lbLoadingPanel;
        public GameObject content;

        private List<LeaderBoardData> leaderboardList;
        public LeaderBoardListView leaderboardListView;

        public LeaderBoardItem myPlayerInfoAtTop;
        public LeaderBoardItem myPlayerInfoAtBottom;

        private Queue<ImageRequestData> imageRequestQueue = new Queue<ImageRequestData>();
        private Dictionary<string, LeaderBoardListViewItem> activeLBItemsWaitingForImage = new Dictionary<string, LeaderBoardListViewItem>();

        private Sprite rewardIconSprite;
        private Coroutine imageDownloadCoroutine = null;

        public Text NameHeader;
        public Text ScoreHeader;
        public Text RankHeader;

        public void SetMetadata(string LBID, string PartitionKey = "")
        {
            lbLoadingPanel.SetActive(true);
            content.SetActive(false);
            NoLBText.gameObject.SetActive(false);
            leaderboardImage.gameObject.SetActive(false);
            leaderboardListView.Clear();

            _ = JBXLeaderboardCommunicator.Instance.GetPreviousLeaderBoard(LBID,
                (data) => { OnLeaderboardDetailsRcvd(LBID, data); });
        }

        void SetLeaderboardImage(Sprite sprite)
        {
            if (sprite != null)
            {
                leaderboardImage.sprite = sprite;
                leaderboardImage.gameObject.SetActive(true);
            }
        }

        private void OnDisable()
        {
            if (imageDownloadCoroutine != null)
            {
                StopCoroutine(imageDownloadCoroutine);
                imageDownloadCoroutine = null;
            }

            leaderboardList.Clear();
            imageRequestQueue.Clear();
            activeLBItemsWaitingForImage.Clear();
        }

        void OnLeaderboardDetailsRcvd(string LBID, IApiLeaderRecordList dataNew)
        {
            lbLoadingPanel.SetActive(false);
            content.SetActive(true);

            LeaderBoardMasterData lbMasterData = LeaderboardDataContain.Instance.GetMasterLBDatawithLBID(LBID);
            leaderNameText.text = dataNew.leaderboardName;

            NameHeader.text = dataNew.LeaderBoardHeaders.NameText;
            ScoreHeader.text = dataNew.LeaderBoardHeaders.ScoreText;
            RankHeader.text = dataNew.LeaderBoardHeaders.RankText;

            //Leader board
            leaderboardList = new List<LeaderBoardData>();
            List<LeaderBoardData> templist = new List<LeaderBoardData>();

            foreach (var tempo in dataNew.LeaderRecords)
            {
                LeaderBoardData newData = new LeaderBoardData(tempo);
                leaderboardList.Add(newData);
            }
            LeaderBoardData _myPlayerData = null;
            int _myPlayerIndex = 0;
            for (int i = 0; i < leaderboardList.Count; i++)
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
        }

        public void ShowLeaderBoard()
        {
            if (leaderboardList.Count > 0)
            {
                NoLBText.gameObject.SetActive(false);
                leaderboardListView.gameObject.SetActive(true);
                leaderboardListView.ItemCallback = PopulateItem;
                leaderboardListView.RowCount = leaderboardList.Count;
            }
            else
            {
                NoLBText.gameObject.SetActive(true);
            }
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

        public void ShowMyPlayerInfo(bool value, bool topPosition = true)
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
                if (string.IsNullOrEmpty(data.avatarURL))
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
                    if (item.GetCurrentPlayerID() == lbData.PlayerId)
                    {
                        item.UpdateAvatar(lbData.avatarSprite);
                    }
                }
                activeLBItemsWaitingForImage.Remove(lbData.PlayerId);
            }
        }

        IEnumerator DownloadAvatarForMyPlayerInfo_Temp(LeaderBoardData data)
        {
            if (string.IsNullOrEmpty(data.AvatarUrl))
                data.AvatarUrl = "https://s3.ap-southeast-1.amazonaws.com/dl.gamenova/users/Avatars_6.png";
            WWW www = new WWW(data.AvatarUrl);
            yield return www;
            Sprite avatarSprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));

            myPlayerInfoAtTop.UpdateAvatar(avatarSprite);
            myPlayerInfoAtBottom.UpdateAvatar(avatarSprite);
        }

        public void OnCloseBtnClicked()
        {
            gameObject.SetActive(false);
        }

    }
}
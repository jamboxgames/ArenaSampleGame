namespace Jambox.Tourney.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Jambox.Tourney.Connector;
    using Jambox.Tourney.Data;
    using UnityEngine;
    using UnityEngine.UI;

    public class LeaderBoardPanel : MonoBehaviour
    {
        public LeaderBoardListView theList;
        public Button CloseBtn;
        public Text LeaderBoardText;
        public List<leaderBoardData> leaderBoardList;
        public string LeaderBoardID;
        private TourneyDetail tourneyDet;
        private Panels prevPanel;

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
        private void OnEnable()
        {
            if (string.IsNullOrEmpty(LeaderBoardID))
            {
                StartCoroutine(WaitAndGet());
            }
            else
            {
                GetLeaderBoard();
            }
        }
        private void GetLeaderBoard()
        {
            _ = CommunicationController.Instance.GetLeaderBoard(LeaderBoardID, (data) => { LeaderBoardSuccess(data); },
                (errorMsg) => { UIPanelController.Instance.ErrorFromServerRcvd(errorMsg); }, this.gameObject);
        }
        private IEnumerator WaitAndGet()
        {
            while(string.IsNullOrEmpty(LeaderBoardID))
            {
                yield return null;
            }
            GetLeaderBoard();
        }
        public void SetPrevpanel (Panels prevPanelDet)
        {
            prevPanel = prevPanelDet;
        }
        private void LeaderBoardSuccess(IApiLeaderRecordList leaderBoard)
        {
            if (prevPanel == Panels.TourneyPanel)
            {
                foreach(var dataTempo in UserDataContainer.Instance.UpdatedTourneyData)
                {
                    if(dataTempo.Value.isJoined && dataTempo.Value._joinedTourneyData.LeaderBoardID.Equals(LeaderBoardID))
                    {
                        tourneyDet = dataTempo.Value;
                    }
                }
                string TDes = tourneyDet._tournament.TourneyName + " : " + tourneyDet._tournament.Description;
                StartLeaderBoard(leaderBoard, tourneyDet._tournament.Tourneyid, TDes);
            }
            if (prevPanel == Panels.CompletedPanel)
            {
                CompletedTourneyDetail CompData;
                string TDes = "";
                foreach (var DataNewTempo in UserDataContainer.Instance.CompletedTourney)
                {
                    if(DataNewTempo.LeaderBoardID.Equals(LeaderBoardID))
                    {
                        CompData = DataNewTempo;
                        TDes = CompData.TourneyName;
                    }
                    StartLeaderBoard(leaderBoard, LeaderBoardID, TDes);
                }
            }
        }
        public void StartLeaderBoard (IApiLeaderRecordList data, string TId, string TDes)
        {
            LeaderBoardID = data.leaderboardId;
            LeaderBoardText.text = TDes;
            leaderBoardList = new List<leaderBoardData>();
            List<leaderBoardData> templist = new List<leaderBoardData>();
            foreach (var item in data.LeaderRecords)
            {
                leaderBoardData itemData = new leaderBoardData(item);
                templist.Add(itemData);
            }
            leaderBoardList = templist.OrderBy(o => o.Rank).ToList();
            ShowLeaderBoard();

        }
        public void ShowLeaderBoard()
        {
            theList.gameObject.SetActive(true);
            theList.ItemCallback = PopulateItem;
            theList.RowCount = leaderBoardList.Count;
        }

        public void onCloseClicked()
        {
            this.gameObject.SetActive(false);
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            UIPanelController.Instance.ShowPanel(prevPanel, Panels.None, metadata);
        }

        private void PopulateItem(LeaderBoardListViewItem item, int rowIndex)
        {
            var tDet = item as LeaderBoardItem;
            if (rowIndex < leaderBoardList.Count)
            {
                bool lastItem = false;
                if (rowIndex == leaderBoardList.Count - 1)
                    lastItem = true;
                if (prevPanel == Panels.TourneyPanel)
                {
                    tDet.FillItem(leaderBoardList[rowIndex], lastItem);
                }
                else
                {
                    tDet.FillItem(leaderBoardList[rowIndex], lastItem);
                }
            }
        }
    }
}

namespace Jambox.Tourney.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Jambox.Tourney.Connector;
    using Jambox.Tourney.Data;
    using UnityEngine;
    using UnityEngine.UI;
    using Random = UnityEngine.Random;

    /// <summary>
    /// UI Panel script responsible to populate the data on UI.
    /// </summary>
    public class CompletedTourney : MonoBehaviour
    {
        public TourneyListView theList;
        public Panels prevPanel;
        public Text NoDataDialogue;
        public ClaimRewardPanel RewardPanel;
        public Text Currency;
        public Text NameDes;
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
        public void OnEnable()
        {
            LoadingDialog(true);
            UpdateCurrency();
            _ = CommunicationController.Instance.GetCompletedTourneyData("1", (data) => { CompletedTDataRcvd(data); }, (errorMsg) => { UIPanelController.Instance.ErrorFromServerRcvd(errorMsg); });
        }

        public void UpdateCurrency()
        {
            Currency.text = "Coins : " + UserDataContainer.Instance.getUserMoney();
        }

        private void CompletedTDataRcvd(IAPICompTourneyList data)
        {
            LoadingDialog(false);
            UserDataContainer.Instance.UpdateCompTourneyData(data);
            PopulateItem();
        }

        private void LoadingDialog(bool isShow)
        {
            if (isShow)
            {
                //Show The loading Dialogue
                UnityDebug.Debug.Log("Enabling Loading Dialogue Of TourneyPanel.");
            }
            else
            {
                //Hide The Loading Dialogue
                UnityDebug.Debug.Log("Disabling Loading Dialogue Of TourneyPanel.");
            }
        }
        public void ShowTourneyItem(Panels prevPanelDet)
        {
            prevPanel = prevPanelDet;
        }
        private void PopulateItem()
        {
            theList.ItemCallback = PopulateItem;
            theList.RowCount = UserDataContainer.Instance.CompletedTourney.Count;
            if(UserDataContainer.Instance.CompletedTourney.Count == 0)
            {
                UnityDebug.Debug.Log("No Completed Tourney Data From Server >>>>>>");
                NoDataDialogue.gameObject.SetActive(true);
            }
            else
            {
                if(NoDataDialogue.gameObject.activeInHierarchy)
                    NoDataDialogue.gameObject.SetActive(false);
            }
        }

        private void PopulateItem(TourneyListViewItem item, int rowIndex)
        {
            var tDet = item as CompTourneyItem;
            if (rowIndex < UserDataContainer.Instance.CompletedTourney.Count)
            {
                tDet.CompTourneyDet = UserDataContainer.Instance.CompletedTourney[rowIndex];
            }
        }

        public void OnBackBtnClick()
        {
            if(RewardPanel.gameObject.activeInHierarchy)
            {
                RewardPanel.OnClosebuttonClick();
            }
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            UIPanelController.Instance.ShowPanel(Panels.None, Panels.None, metadata);
            ArenaSDKEvent.Instance.FireOnBackToLobby();
        }

        public void ShowClaimSuccess (int reward, String Currency)
        {
            RewardPanel.gameObject.SetActive(true);
            RewardPanel.UpdateText(reward, Currency);
        }
    }
}

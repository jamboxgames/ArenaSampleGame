namespace Jambox.Tourney.UI
{
    using Jambox.Tourney.Connector;
    using Jambox.Tourney.Data;
    using Jambox.Tourney.UI;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;
    using System;

    public class FriendlyPanel : MonoBehaviour
    {

        public CreateFriendly createPanel;
        public JoinFriendly joinPanel;
        public InviteFriendly invitePanel;

        public FriendlyListView theList;

        public GameObject FriendlyDetailsPanel;

        private void OnEnable()
        {
        }

        public void ShowTourneyInfo(FriendlyTournamentData data)
        {

        }

        public void OnCreateBtnClicked()
        {
            theList.gameObject.SetActive(false);
            invitePanel.gameObject.SetActive(false);
            joinPanel.gameObject.SetActive(false);
            UIPanelController.Instance.DisableFriendlyPanelView(false);
            createPanel.gameObject.SetActive(true);
        }
        public void FriendlyItems(bool status)
        {
            theList.gameObject.SetActive(status);
        }
        public void OnJoinBtnClicked()
        {
            theList.gameObject.SetActive(false);
            invitePanel.gameObject.SetActive(false);
            createPanel.gameObject.SetActive(false);
            UIPanelController.Instance.DisableFriendlyPanelView(false);
            joinPanel.gameObject.SetActive(true);
        }

        public void OnInviteBtnClicked(String InviteCode)
        {
            theList.gameObject.SetActive(false);
            invitePanel.gameObject.SetActive(true);
            invitePanel.GetComponent<InviteFriendly>().SetCode(InviteCode);
        }
    }
}

namespace Jambox.Tourney.UI
{
    using Jambox.Tourney.Connector;
    using Jambox.Tourney.Data;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class FriendlyDetailsPanel : MonoBehaviour
    {

        public Text coinsText;
        public Text tourneyName;
        public Text description;
        public Text attempts;
        public Text position;
        public Text playBtnText;
        private string tourneyId;

        private void OnEnable()
        {
            coinsText.text = "Coins : " + UserDataContainer.Instance.getUserMoney();
        }

        public void UpdateUI(TourneyDetail data)
        {
            tourneyName.text = data._tournament.TourneyName;
            description.text = data._tournament.Description;
            attempts.text = data._joinedTourneyData.CurrentAttempt + "/" + data._tournament.MaxEntry;
            playBtnText.text = "PLAY (" + (data._tournament.MaxEntry - data._joinedTourneyData.CurrentAttempt) + " LEFT)";

            tourneyId = data._tournament.Tourneyid;
        }

        public void OnBacktBtnClicked()
        {
            Destroy(this.gameObject);
        }

        public void OnPlayBtnClicked()
        {
            _ = CommunicationController.Instance.PlayFriendlyTourney("", tourneyId, (data) => { OnPlayTourneySuccess(data); });
        }

        void OnPlayTourneySuccess(IApiPlayFriendlyTourney data)
        {
            UnityDebug.Debug.Log("PLAY GAME RECEIVED");
        }
    }
}

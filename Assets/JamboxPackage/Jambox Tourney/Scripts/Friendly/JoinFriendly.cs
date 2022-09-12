namespace Jambox.Tourney.UI
{
    using Jambox.Tourney.Connector;
    using Jambox.Tourney.Data;
    using Jambox.Tourney.UI;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class JoinFriendly : MonoBehaviour
    {

        public InputField codeInput;

        public void OnJoinBtnClicked()
        {
            if (string.IsNullOrWhiteSpace(codeInput.text))
            {
                UnityDebug.Debug.LogError("Enter Valid Code¸");
                return;
            }
            UIPanelController.Instance.LoadingDialogue(true, false);
            _ = CommunicationController.Instance.JoinFriendly(codeInput.text, (data) => { OnJoinFriendlySuccess(data); },
                (errorCode, errorMsg) => { UIPanelController.Instance.ErrorFromServerRcvd(errorCode, errorMsg); });

        }

        private void OnJoinFriendlySuccess(IAPIJoinFriendly data)
        {
#if GAME_FIREBASE_ENABLED
            //Todo : Change Friendly Struct to have tourney details
            Firebase.Analytics.FirebaseAnalytics.LogEvent("join_friendly");
#endif

            CloseJoinPanel();
        }

        public void CloseJoinPanel()
        {
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            UIPanelController.Instance.ShowPanel(Panels.FriendlyPanel, Panels.None, metadata);
            gameObject.SetActive(false);
        }
    }
}

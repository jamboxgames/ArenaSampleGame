namespace Jambox.Tourney.UI
{
    using Jambox.Tourney.Connector;
    using Jambox.Tourney.Data;
    using Jambox.Tourney.UI;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class CreateFriendly : MonoBehaviour
    {

        public InputField nameInput;
        public Slider attemptsInput;
        public Slider durationInput;

        public Text attemptsValueText;
        public Text durationValueText;
        public Button CreateButton;
        public Button CloseButton;

        public int[] durationInHrs;

        private void OnEnable()
        {
            OnAttemptValueChnaged();
            OnDurationValueChanged();
            OnNameChanged();
        }

        public void OnCreateBtnClicked()
        {
            DisableAll();
            CreateButton.interactable = false;
            CloseButton.interactable = false;

            string _name = nameInput.text;
            int _attempts = (int)attemptsInput.value;
            int _duration = durationInHrs[(int)durationInput.value - 1];

            UnityDebug.Debug.Log("Friendly Tournament Created!\nName : " + _name + "  Attempts : " + _attempts + " Duration : " + _duration);
            _ = CommunicationController.Instance.CreateFriendly(_name, _attempts, _duration, (data) => { OnCreateTournamentSuccesful(data); }, (errorMsg) => { UIPanelController.Instance.ErrorFromServerRcvd(errorMsg); });
            UIPanelController.Instance.LoadingDialogue(true, false);
        }

        void OnCreateTournamentSuccesful(IAPICreateFriendly data)
        {
#if GAME_FIREBASE_ENABLED
            Firebase.Analytics.Parameter[] eventarameters = {
                                new Firebase.Analytics.Parameter("event_id",data.Tourneyid)
                                };
            Firebase.Analytics.FirebaseAnalytics.LogEvent("create_friendly", eventarameters);
#endif

            //Join with the received data
            CloseCreatePanel();
        }

        public void OnNameChanged()
        {
            if (string.IsNullOrWhiteSpace(nameInput.text))
            {
                CreateButton.interactable = false;
                return;
            }
            else
            {
                CreateButton.interactable = true;
            }
        }

        public void OnAttemptValueChnaged()
        {
            attemptsValueText.text = attemptsInput.value.ToString();
        }

        public void OnDurationValueChanged()
        {
            int _inDays = durationInHrs[(int)durationInput.value - 1] / 24;
            if (_inDays <= 0)
            {
                durationValueText.text = durationInHrs[(int)(durationInput.value) - 1] + " Hour";
            }
            else
            {
                if (_inDays == 1)
                {
                    durationValueText.text = _inDays + " Day";
                }
                else
                {
                    durationValueText.text = _inDays + " Days";
                }
            }

        }

        public void CloseCreatePanel()
        {
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            UIPanelController.Instance.ShowPanel(Panels.FriendlyPanel, Panels.None, metadata);
            gameObject.SetActive(false);
        }
        public void DisableAll()
        {
            UIPanelController.Instance.DisableFriendlyPanelView(false);
        }
    }
}

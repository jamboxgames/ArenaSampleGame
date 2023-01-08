namespace Jambox.Tourney.UI
{
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    public class ClaimRewardPanel : MonoBehaviour
    {
        public Text rewardText;
        public Text HeaderText;
        public Text ButtonText;
        public Image BGimage;

        private void Start()
        {
            BGimage.sprite = UIPanelController.Instance.bgSprite;
        }

        public void OnClosebuttonClick()
        {
            this.gameObject.SetActive(false);
        }

        public void UpdateText(int reward, string currency)
        {
            string rewardMsg = "Your Claim has been Successful. You Claimed ";
            string Header = "ALERT";
            string Btntext = "OKAY";
            if (UIPanelController.Instance.EditableMessageData != null)
            {
                if (UIPanelController.Instance.EditableMessageData.ContainsKey("ClaimMessageBody"))
                {
                    string tempMsg = string.Empty;
                    UIPanelController.Instance.EditableMessageData.TryGetValue("ClaimMessageBody", out tempMsg);
                    if (!string.IsNullOrEmpty(tempMsg))
                    {
                        rewardMsg = tempMsg;
                    }
                }
                if (UIPanelController.Instance.EditableMessageData.ContainsKey("ClaimMessageTitle"))
                {
                    string tempMsg2 = string.Empty;
                    UIPanelController.Instance.EditableMessageData.TryGetValue("ClaimMessageTitle", out tempMsg2);
                    if (!string.IsNullOrEmpty(tempMsg2))
                    {
                        Header = tempMsg2;
                    }
                }
                if (UIPanelController.Instance.EditableMessageData.ContainsKey("ClaimMessageButton"))
                {
                    string tempMsg3 = string.Empty;
                    UIPanelController.Instance.EditableMessageData.TryGetValue("ClaimMessageButton", out tempMsg3);
                    if (!string.IsNullOrEmpty(tempMsg3))
                    {
                        Btntext = tempMsg3;
                    }
                }
            }
            rewardText.text = rewardMsg + reward + " " + currency;
            HeaderText.text = Header;
            ButtonText.text = Btntext;
        }
    }
}

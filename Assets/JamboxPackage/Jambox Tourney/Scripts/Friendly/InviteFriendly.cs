namespace Jambox.Tourney.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using Jambox.Tourney.UI;
    using UnityEngine;
    using UnityEngine.UI;

    public class InviteFriendly : MonoBehaviour
    {

        public Text codeText;

        private string ShareCode;

        public void OnShareBtnClicked()
        {
            Sharing.Instance.ShareCode(ShareCode);
        }

        public void OnCopyBtnClicked()
        {
            GUIUtility.systemCopyBuffer = ShareCode;
            StartCoroutine(DisplayCopiedText());
        }

        private IEnumerator DisplayCopiedText()
        {
            codeText.text = "Copy Success";
            yield return new WaitForSeconds(3.0f);
            codeText.text = ShareCode;
        }

        public void SetCode(string code)
        {
            ShareCode = code;
            codeText.text = code;
        }

        public void CloseInvitePanel()
        {
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            UIPanelController.Instance.ShowPanel(Panels.FriendlyPanel, Panels.None, metadata);
            gameObject.SetActive(false);
        }
    }
}

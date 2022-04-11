using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jambox.Tourney.UI;
using Jambox.Common.Utility;

public class Sharing : MonoSingleton <Sharing>
{

    public string subject = "Jambox Friendly Invite";
    public string body = "Your friend has invited you to a Friendly Tournament. \n Use Code: ";
    public string link = "";

    public void ShareCode(string code)
    {
        if (UIPanelController.Instance.EditableMessageData != null)
        {
#if UNITY_ANDROID
            if(UIPanelController.Instance.EditableMessageData.ContainsKey("InviteMessageAndroid"))
            {
                UIPanelController.Instance.EditableMessageData.TryGetValue("InviteMessageAndroid", out body);
            }
#elif UNITY_IPHONE
            if(UIPanelController.Instance.EditableMessageData.ContainsKey("InviteMessageIOS"))
            {
                UIPanelController.Instance.EditableMessageData.TryGetValue("InviteMessageIOS", out body);
            }
#endif
        }
        new NativeShare().SetSubject(subject).SetText(body + code).SetUrl(link)
        .SetCallback((result, shareTarget) => UnityDebug.Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
        .Share();
    }

}

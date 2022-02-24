using System.Collections;
using System.Collections.Generic;
using Jambox.Tourney.UI;
using UnityEngine;
using UnityEngine.UI;

public class UpdateNameUI : MonoBehaviour
{   
    public InputField nameInput;
    public TourneyPanel tourneyPanel;
    Panels _prevPanelDet;
    Panels _currentpanel;
    Dictionary<string, string> _metaData = new Dictionary<string, string>();

    public void OnUpdateClick()
    {
        if (string.IsNullOrWhiteSpace(nameInput.text) || (nameInput.text).Equals("Your name")
            || (nameInput.text).Equals("Enter Valid Name"))
        {
            nameInput.text = "Enter Valid Name";
            UnityDebug.Debug.Log("Enter Valid Name >>>> ");
            return;
        }
        UnityDebug.Debug.Log("Updated Name : " + nameInput.text);
        //tourneyPanel.UpdateUserName(nameInput.text, _prevPanelDet, _currentpanel, _metaData);
        this.gameObject.SetActive(false);
    }

    public void SetMetaData (Panels prevPanelDet, Panels Currentpanel, Dictionary<string, string> metaData)
    {
        _prevPanelDet = prevPanelDet;
        _currentpanel = Currentpanel;
        _metaData = metaData;
    }
}

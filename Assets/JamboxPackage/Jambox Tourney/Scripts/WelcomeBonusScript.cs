using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class WelcomeBonusScript : MonoBehaviour
{
    public Text WelcomeText;
    public Button CollectBtn;
    public Text ButtonText;
    public GameObject StartInst;
    Action<int> OnButtonClick;

    private int WelcomeMoney = 0;

    public void UpdateWelcomeText (String Message, string Buttontex , Action<int> OnCollected, int welcomeMoney = 0)
    {
        WelcomeMoney = welcomeMoney;
        OnButtonClick = OnCollected;
        WelcomeText.text = Message;
        ButtonText.text = Buttontex;
    }

    public void OnCollectBtnClick()
    {
        EnableStartButtons();
        //StartCoroutine(WaitAndSend(1.0f));

        if (OnButtonClick != null)
            OnButtonClick(WelcomeMoney);
        this.gameObject.SetActive(false);
    }

    //private IEnumerator WaitAndSend(float timeToWait)
    //{
    //    yield return new WaitForSeconds(timeToWait);
    //    if (OnButtonClick != null)
    //        OnButtonClick(WelcomeMoney);
    //    this.gameObject.SetActive(false);
    //}

    public void EnableStartButtons()
    {
        StartInst.SetActive(true);
    }

}

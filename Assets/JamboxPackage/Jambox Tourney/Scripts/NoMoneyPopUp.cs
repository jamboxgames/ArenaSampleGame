using System;
using System.Collections;
using System.Collections.Generic;
using Jambox.Tourney.UI;
using UnityEngine;
using UnityEngine.UI;

public class NoMoneyPopUp : MonoBehaviour
{
    public Text NoMoneyText;
    public Text HeaderText;
    public Button BuyBtn;
    public Text BuyText;
    public Button CloseBtn;
    Action OnButtonClick;
    private int StoreMoney = 0;
    private string CurrencyKey;
    private string TourneyId = String.Empty;
    private Panels PreviousPanel = Panels.None;

    public void OnCloseBtnClick()
    {
        //UIPanelController.Instance.SetTourneyScrollActive(true);
        UIPanelController.Instance.ShowPanel(PreviousPanel);
        this.gameObject.SetActive(false);
    }

    public void OnBuyBtnClick ()
    {
        UIPanelController.Instance.SetTourneyScrollActive(true);
        if (OnButtonClick != null)
            OnButtonClick();
        else
            UIPanelController.Instance.ShowStore(StoreMoney, CurrencyKey, TourneyId, PreviousPanel);
        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        NoMoneyText.text = "YOU DO NOT HAVE SUFFICIENT CHIPS TO PLAY THIS TOURNAMENT. PURCHASE SOME FROM OUR STORE AND ENJOY THE GAME.";
        BuyBtn.gameObject.SetActive(true);
        BuyText.text = "PURCHASE";
        CloseBtn.gameObject.SetActive(true);
    }

    public void SetUIData(string msg, string BtnTxt, bool isBuyBtn, bool isClosebtn, Action OnBtnClick, int StoreAmt, String currKey, string _tID, Panels prev, String headerMsg)
    {
        OnButtonClick = OnBtnClick;
        NoMoneyText.text = msg;
        BuyBtn.gameObject.SetActive(isBuyBtn);
        BuyText.text = BtnTxt;
        CloseBtn.gameObject.SetActive(isClosebtn);
        StoreMoney = StoreAmt;
        CurrencyKey = currKey;
        TourneyId = _tID;
        PreviousPanel = prev;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePanel : MonoBehaviour
{
    public GameObject HeaderParent;
    public Text Header;
    public Text ButtonOneName;
    public Button ButtonOne;
    public Text ButtonTwoName;
    public Button ButtonTwo;
    public Text Description;
    public Button CentreButton;
    public Text CentreButtonName;
    public GameObject closeButton;

    public Action OnButtonOneClick;
    public Action OnButtonTwoClick;
    public Action OnButtonCentreClick;

    public void OnFirstButtonClick ()
    {
        Debug.LogError("OnFirstButtonClick >>>>>>>>");
        if (OnButtonOneClick != null)
            OnButtonOneClick();
        this.gameObject.SetActive(false);
    }

    public void OnSecondButtonClick ()
    {
        Debug.LogError("OnSecondButtonClick >>>>>>>>");
        if (OnButtonTwoClick != null)
            OnButtonTwoClick();
        this.gameObject.SetActive(false);
    }

    public void OnCentreButtonClick()
    {
        Debug.LogError("OnCentreButtonClick >>>>>>>>");
        if (OnButtonCentreClick != null)
            OnButtonCentreClick();
        this.gameObject.SetActive(false);
    }

    public void ShowDialogue (string header, string body, string Btn1Name, string Btn2name,
        Action OnBtn1Click = null, Action OnBtn2Click = null, bool closeBtn = false)
    {
        Header.text = header;
        Description.text = body;

        if (closeButton != null)
        {
            if (closeBtn)
                closeButton.SetActive(true);
            else
                closeButton.SetActive(false);
        }

        if (Btn2name == null)
        {
            TwoButtonDialogue(false);

            CentreButtonName.text = Btn1Name;
            OnButtonCentreClick = OnBtn1Click;
        }
        else
        {
            TwoButtonDialogue(true);

            ButtonOneName.text = Btn1Name;
            ButtonTwoName.text = Btn2name;
            OnButtonOneClick = OnBtn1Click;
            OnButtonTwoClick = OnBtn2Click;
        }
        
    }

    void TwoButtonDialogue(bool _value)
    {
        if (_value)
        {
            if (ButtonOne != null)
                ButtonOne.gameObject.SetActive(true);
            if (ButtonTwo != null)
                ButtonTwo.gameObject.SetActive(true);
            CentreButton.gameObject.SetActive(false);
        }
        else
        {
            if (ButtonOne != null)
                ButtonOne.gameObject.SetActive(false);
            if (ButtonTwo != null)
                ButtonTwo.gameObject.SetActive(false);
            CentreButton.gameObject.SetActive(true);
        }
    }

    public void OnCloseBtnClicked()
    {
        gameObject.SetActive(false);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    public int coins = 0;
    public Text coinsText;
    public Transform coinsContainer;

    public GameObject MainMenuPanel;
    public GameObject mainButtons;
    public GameObject shop;

    private int requiredTotalAmnt = 0;

    private void Awake()
    {
        MainMenuPanel.SetActive(false);
    }

    private void Start()
    {
        if (!TourneyManager.Instance.inTourney)
            MainMenuPanel.SetActive(true);

        coins = PlayerPrefs.GetInt("coins", 0);
        //PlayerPrefs.SetInt("coins", 0);
        coinsText.text = "Coins : " + coins.ToString();

        CheckOrientation();
    }

    public void CheckOrientation()
    {
        if (myScreenOrientation == ScreenOrientation.Portrait)
        {
            SetUIForPortrait();
        }
        else
        {
            SetUIForLandscape();
        }
    }

    public ScreenOrientation myScreenOrientation
    {
        get
        {
#if !UNITY_EDITOR
            return Screen.orientation;
#elif UNITY_EDITOR
            if (Screen.height > Screen.width)
            {
                return ScreenOrientation.Portrait;
            }
            else
            {
                return ScreenOrientation.Landscape;
            }
#endif
        }
    }

    public void SetUIForLandscape()
    {
        MainMenuPanel.GetComponent<CanvasScaler>().matchWidthOrHeight = 1f;

        mainButtons.transform.localScale = Vector3.one * 1.25f;
        mainButtons.transform.localPosition = new Vector3(0f, -100, 0f);

        coinsContainer.localScale = Vector3.one * 1.5f;
        shop.transform.localScale = Vector3.one;
    }

    public void SetUIForPortrait()
    {
        MainMenuPanel.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.5f;

        mainButtons.transform.localScale = Vector3.one;
        mainButtons.transform.localPosition = Vector3.zero;

        coinsContainer.localScale = Vector3.one;
        shop.transform.localScale = Vector3.one * 0.8f;
    }

    public void PlayBtnClicked()
    {
        SceneManager.LoadScene(1);
    }

    public void TourneyBtnClicked()
    {
        TourneyManager.Instance.OpenArena();
        MainMenuPanel.SetActive(false);
    }

    public void OnPurchaseRequired(int _amnt, string _currency)
    {
        BackToMainMenu();
        OnShopClicked();
        requiredTotalAmnt = coins + _amnt;
    }

    public void BackToMainMenu()
    {
        MainMenuPanel.SetActive(true);
    }

    public void AddCoins(int _amnt)
    {
        coins += _amnt;
        coinsText.text = "Coins : " + coins.ToString();
        PlayerPrefs.SetInt("coins", coins);
        if (TourneyManager.Instance.tourneyPurchaseRequired)
        {
            if (coins >= requiredTotalAmnt)
            {
                TourneyManager.Instance.OpenArena(true);
                MainMenuPanel.SetActive(false);
            }
        }
    }

    public void UpdateCoins(int _amnt, string _curreny, bool _increase)
    {
        if (_increase)
        {
            coins += _amnt;
        }
        else
        {
            coins -= _amnt;
        }
        coinsText.text = "Coins : " + coins.ToString();
        PlayerPrefs.SetInt("coins", coins);
    }

    public void OnShopClicked()
    {
        mainButtons.SetActive(false);
        shop.SetActive(true);
    }

    public void CloseShop()
    {
        mainButtons.SetActive(true);
        shop.SetActive(false);
        if (TourneyManager.Instance.tourneyPurchaseRequired)
            TourneyManager.Instance.tourneyPurchaseRequired = false;
    }

}

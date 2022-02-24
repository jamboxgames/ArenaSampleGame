using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    public int coins = 0;
    public Text coinsText;

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

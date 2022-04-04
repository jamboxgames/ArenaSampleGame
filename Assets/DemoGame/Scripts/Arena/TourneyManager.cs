using Jambox.Tourney.Connector;
using Jambox.Tourney.Data;
using Jambox.Common.TinyJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jambox;
using Jambox.Tourney.UI;
using UnityEngine.SceneManagement;

public class TourneyManager : MonoBehaviour
{

    public bool inTourney = false;
    public EMatchType matchType;
    public bool tourneyPurchaseRequired = false;

    public int realtimeLBcount;
    public bool scrollableRealtimeLB;

    public static TourneyManager Instance;

    #region SINGLETON
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    #endregion

    #region INITIALIZATION
    private void Start()
    {
        InitializeArena();
        RegisterEvents();
    }

    void RegisterEvents()
    {
        ArenaSDKEvent.Instance.OnPlay += OnPlayHit;
        ArenaSDKEvent.Instance.OnBackToLobby += OnBackToLobbyHit;
        ArenaSDKEvent.Instance.OnPurchaseRequired += OnPurchaseRequired;
        ArenaSDKEvent.Instance.UserMoneyUpdate += OnUpdateMoney;
        ArenaSDKEvent.Instance.OnWatchAdRequired += WatchVideoClicked;
    }

    public void InitializeArena()
    {
        ArenaSDKEvent.Instance.InitializeArenaSdk();
    }
    #endregion

    public void OpenArena(bool isPurchaseSuccess = false)
    {
        Dictionary<string, long> MoneyDetail = new Dictionary<string, long>();
        int coins = PlayerPrefs.GetInt("coins", 0);
        MoneyDetail.Add("key_gems", coins);

        if (isPurchaseSuccess)
            ArenaSDKEvent.Instance.PlayAfterPurchase(true, MoneyDetail);
        else
            ArenaSDKEvent.Instance.OpenArenaUI(MoneyDetail);

        if (tourneyPurchaseRequired)
        {
            tourneyPurchaseRequired = false;
        }
    }

    private void OnPlayHit(Match matchData)
    {

        UnityDebug.Debug.Log("OnPlayHit : " + matchData.matchType);
        inTourney = true;

        IAPIReplayData _replayData = null;

        //It means its duels
        matchType = matchData.matchType;
        if (matchData.matchType == EMatchType.EMatchTypeDuel && matchData.replayData != null)
        {
            _replayData = matchData.replayData;
            TimeReplayPlayer.Instance.SetOpponentReplayData(_replayData);
        }
        else if ((matchData.matchType == EMatchType.EMatchTypeTourney || matchData.matchType == EMatchType.EMatchTypeFriendly) && matchData.Leaderboard != null)
        {
            StartCoroutine(StartRealtimeLB(matchData));
        }
        PlayGame();
    }

    IEnumerator StartRealtimeLB(Match matchData)
    {
        while (SceneManager.GetActiveScene().buildIndex != 1)
            yield return null;
        ArenaSDKEvent.Instance.InitializeRealtimeLeaderboard(matchData.Leaderboard, scrollableRealtimeLB, realtimeLBcount);
    }

    public void PlayGame()
    {
        FindObjectOfType<MenuManager>().PlayBtnClicked();
    }

    private void OnBackToLobbyHit()
    {
        inTourney = false;
        OpenMainPanel();
    }

    private void OnPurchaseRequired(int coinsRequired, string currency)
    {
        UnityDebug.Debug.Log("Requied : " + coinsRequired + " in : " + currency);
        FindObjectOfType<MenuManager>().OnPurchaseRequired(coinsRequired, currency);
        tourneyPurchaseRequired = true;
    }

    private void OnUpdateMoney(int userMoney, string currency, bool isIncrease)
    {
        UpdateMoney(userMoney, currency, isIncrease);
    }

    void UpdateMoney(int userMoney, string currency, bool isIncrease)
    {
        if (isIncrease)
            FindObjectOfType<MenuManager>().UpdateCoins(userMoney, currency, true);
        else
            FindObjectOfType<MenuManager>().UpdateCoins(userMoney, currency, false);
    }

    private void WatchVideoClicked()
    {
        StartCoroutine(WatchVideoSuccessWithDelay());
    }

    IEnumerator WatchVideoSuccessWithDelay() {
        yield return new WaitForSeconds(0.5f);
        OnFinishedRewardedAd(true);
    }

    void OnFinishedRewardedAd(bool data)
    {
        ArenaSDKEvent.Instance.PlayAfterWatchAd(data);
    }

    public void SubmitScore(int _score)
    {
        ArenaSDKEvent.Instance.SubmitScore(_score, 0);
    }

    public void OpenMainPanel()
    {
        FindObjectOfType<MenuManager>().BackToMainMenu();
    }

}

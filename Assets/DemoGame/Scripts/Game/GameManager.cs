using Jambox.Tourney.Connector;
using JBX.Leaderboard.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public float totalTime;
    private float timer = 0f;
    private float timeToIncrease = 0f;
    public float reduceTime;

    public GameObject normalGameTitle;
    public GameObject tourneyTitle;

    public CanvasScaler gameCanvas;
    public GameObject gameContent;

    public Text scoreText;
    public Image timerProgress;

    public int minIncreaseAmnt;
    public int maxInscreasAmnt;
    private int increaseAmnt;

    public float minIncreaseRate;
    public float maxInscreasRate;
    private float increaseRate;

    private int score = 0;

    public Text endScreenScoreText;
    public GameObject endScreenContainer;
    public GameObject endScreenContent;
    public GameObject restartBtn;

    private bool gameOver = false;

    private void Start()
    {
        endScreenContainer.SetActive(false);

        SetGameTitle();

        increaseAmnt = Random.Range(minIncreaseAmnt, maxInscreasAmnt);
        increaseRate = Random.Range(minIncreaseRate, maxInscreasRate);

        scoreText.text = "" + score;

        StartCoroutine(UpdateScore());
        CheckOrientation();
    }

    private void Update()
    {
        if (!gameOver)
        {
            timer += Time.deltaTime;
            timerProgress.fillAmount = timer / totalTime;
        }

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
        gameCanvas.matchWidthOrHeight = 1f;

        gameContent.transform.localScale = Vector3.one * 1.5f;
        tourneyTitle.transform.localScale = Vector3.one * 1.75f;

        endScreenContent.transform.localScale = Vector3.one;
    }

    public void SetUIForPortrait()
    {
        gameCanvas.matchWidthOrHeight = 0.5f;

        gameContent.transform.localScale = Vector3.one;
        tourneyTitle.transform.localScale = Vector3.one;

        endScreenContent.transform.localScale = Vector3.one * 0.85f;
    }

    void SetGameTitle()
    {
        if ((TourneyManager.Instance.inTourney && TourneyManager.Instance.matchType != Jambox.EMatchType.EMatchTypeDuel) || myScreenOrientation == ScreenOrientation.Landscape)
        {
            tourneyTitle.SetActive(true);
            normalGameTitle.SetActive(false);
        }
        else
        {
            tourneyTitle.SetActive(false);
            normalGameTitle.SetActive(true);
        }
    }

    IEnumerator UpdateScore()
    {
        yield return new WaitForSeconds(increaseRate);
        while (timer < totalTime)
        {
            IncreaseScore();
            yield return new WaitForSeconds(increaseRate);
        }
        gameOver = true;
        SubmitScore();
    }

    public void IncreaseScore()
    {
        score += increaseAmnt;
        scoreText.text = "" + score;
        if (TourneyManager.Instance.inTourney)
        {
            if (TourneyManager.Instance.matchType != Jambox.EMatchType.EMatchTypeDuel)
                ArenaSDKEvent.Instance.UpdatePlayerScoreOnRealtimeLB(score);
        }
    }

    public void ReduceTiming()
    {
        if (timer > 0f)
            timer -= reduceTime;
    }

    public void SubmitScore()
    {
        EndGame();
    }

    void EndGame()
    {
        _ = JBXLeaderboardCommunicator.Instance.SubmitLBRecord(TourneyManager.GlobalLeaderboardID, "", score, score.ToString(), (data) => {});
        endScreenScoreText.text = "" + score;
        endScreenContainer.SetActive(true);

        if (TourneyManager.Instance.inTourney)
            restartBtn.SetActive(false);
    }

    public void HomeBtn()
    {
        if (TourneyManager.Instance.inTourney)
        {
            SceneManager.LoadScene(0);
            TourneyManager.Instance.SubmitScore(score);
            
            
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

    public void RestartBtn()
    {
        SceneManager.LoadScene(1);
    }

}

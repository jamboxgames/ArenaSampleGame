using Jambox.Tourney.Connector;
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
    public GameObject endScreenMenu;
    public GameObject restartBtn;

    private bool gameOver = false;

    private void Start()
    {
        endScreenMenu.SetActive(false);

        if (TourneyManager.Instance.inTourney && TourneyManager.Instance.matchType != Jambox.EMatchType.EMatchTypeDuel)
        {
            tourneyTitle.SetActive(true);
            normalGameTitle.SetActive(false);
        }
        else
        {
            tourneyTitle.SetActive(false);
            normalGameTitle.SetActive(true);
        }

        increaseAmnt = Random.Range(minIncreaseAmnt, maxInscreasAmnt);
        increaseRate = Random.Range(minIncreaseRate, maxInscreasRate);

        scoreText.text = "" + score;

        StartCoroutine(UpdateScore());
    }

    private void Update()
    {
        if (!gameOver)
        {
            timer += Time.deltaTime;
            timerProgress.fillAmount = timer / totalTime;
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
        endScreenScoreText.text = "" + score;
        endScreenMenu.SetActive(true);

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

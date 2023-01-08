using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Jambox;

public class DuelWidget : MonoBehaviour
{

    public RectTransform container;

    [Header("Player Details")]
    public GameObject playerUI;
    public Text playerName;
    public Image playerProfile;
    public Text playerScoreText;

    [Header("Opponent Details")]
    public GameObject opponentUI;
    public Text opponentName;
    public Image opponentProfile;
    public Text opponentScoreText;
    public int opponentScore = 0;
    public GameObject gameOverText;

    private float targetInterval = 0f;
    private float interval = 0f;

    public float _lerpValue = 0f;

    public void SetReplayData(Match matchData, float _yPos)
    {
        container.anchoredPosition = new Vector2(container.anchoredPosition.x, -_yPos);

        playerName.text = matchData.UserName;
        playerProfile.sprite = matchData.UserAvatarSprite;
        playerScoreText.text = "0";

        opponentName.text = matchData.OpponentName;
        opponentProfile.sprite = matchData.OpponentAvatarSprite;
        opponentScoreText.text = opponentScore.ToString();
    }

    public void SetPlayerScore(int _score)
    {
        playerScoreText.text = _score.ToString();
    }

    public void SetOpponentScore(int _score)
    {
        opponentScore = _score;
        opponentScoreText.text = opponentScore.ToString();
    }

    public void SetNextScore(float _interval, int _targetScore)
    {
        interval = 0f;
        targetInterval = _interval;
        StopAllCoroutines();
        StartCoroutine(ScoreAnim(_targetScore));
    }

    IEnumerator ScoreAnim(int targetScore)
    {
        _lerpValue = 0f;
        int _startScore = opponentScore;
        while (interval < targetInterval){
            interval += Time.deltaTime;
            _lerpValue = interval / targetInterval;
            opponentScore = (int)Mathf.Lerp(_startScore, targetScore, _lerpValue);
            opponentScoreText.text = opponentScore + "";
            yield return null;
        }
    }

    public void GameOver()
    {
        gameOverText.SetActive(true);
    }

}

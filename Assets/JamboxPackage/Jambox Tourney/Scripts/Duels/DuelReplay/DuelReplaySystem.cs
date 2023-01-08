using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jambox.Tourney.Connector;
using Jambox.Common.TinyJson;
using Jambox.Common.Utility;
using Jambox;

public class DuelReplaySystem : MonoSingleton<DuelReplaySystem>
{

    private DuelWidget duelWidget = null;
    private bool gradualScoreIncrease;

    public void Start()
    {
        TimeReplayPlayer.Instance.ExecuteDataString += OnPerfomFunction;
        TimeReplayPlayer.Instance.OnReplayStopped += OnReplayStoppedPlaying;
    }

    private void OnReplayStoppedPlaying()
    {
        if (duelWidget != null)
        {
            duelWidget.GameOver();
        }
    }

    private void OnPerfomFunction(string str)
    {
        int _score = int.Parse(str);
        if (duelWidget != null)
        {
            duelWidget.SetOpponentScore(_score);
        }
    }

    public void StartRecordingReplay(Dictionary<string, string> gameData)
    {
        UnityDebug.Debug.LogInfo("Game Started Recording!");
        ArenaSDKEvent.Instance.StartRecording(gameData);
    }

    public void AddScoreToReplayRecorder(int _score)
    {
        string json = _score.ToJson();
        ArenaSDKEvent.Instance.AddNewData(json);
    }

    public void StopRecordingReplay()
    {
        UnityDebug.Debug.LogInfo("Stopped Recording!");
        ArenaSDKEvent.Instance.StopRecording();
    }

    public void PlayOpponentReplay(Match matchData, bool useWidget, bool _gradualScoreIncrease, float _widgetOffsetFromTop)
    {
        if (useWidget)
        {
            if (duelWidget == null)
            {
                duelWidget = ((GameObject)Instantiate(Resources.Load("DuelWidget"))).GetComponent<DuelWidget>();
            }
            duelWidget.SetReplayData(matchData, _widgetOffsetFromTop);
        }

        gradualScoreIncrease = _gradualScoreIncrease;
        if (useWidget && _gradualScoreIncrease)
            StartCoroutine(StartGradualScoreIncreaseCoroutine());
        else
            TimeReplayPlayer.Instance.PlayOpponentReplay();
    }

    public void SetPlayerScoreOnWidget(int _score)
    {
        if (duelWidget != null)
        {
            duelWidget.SetPlayerScore(_score);
        }
    }

    IEnumerator StartGradualScoreIncreaseCoroutine()
    {
        ReplayData opponentReplayData = TimeReplayPlayer.Instance.GetOpponentReplayData();
        bool _timeInMilliseconds = opponentReplayData.timeInMilliseconds;

        int index = 0;
        IntervalData _previousIntervalData = null;
        while (index < opponentReplayData.intervalData.Count - 1)
        {
            IntervalData _data = opponentReplayData.intervalData[index];
            float _interval = float.Parse(_data.i);
            if (index != 0)
            {
                float _previousInterval = float.Parse(_previousIntervalData.i);
                _interval -= _previousInterval;
            }

            if (_timeInMilliseconds)
                _interval = _interval / 1000f;

            int _score = int.Parse(_data.d);
            duelWidget.SetNextScore(_interval, _score);

            yield return new WaitForSeconds(_interval);

            _previousIntervalData = _data;
            index++;
        }

        duelWidget.GameOver();
    }

    public void StopReplayPlaying()
    {
        TimeReplayPlayer.Instance.StopPlayingReplay();
    }

    public void DestroyDuelReplayWidget()
    {
        if (duelWidget != null)
        {
            Destroy(duelWidget.gameObject);
        }
        Destroy(this.gameObject);
    }
}
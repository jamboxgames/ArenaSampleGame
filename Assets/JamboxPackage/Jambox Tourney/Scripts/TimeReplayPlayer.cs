using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Jambox.Tourney.Data;
using Jambox.Common.Utility;
using System.Globalization;

namespace Jambox.Tourney.Connector
{
    public class TimeReplayPlayer : MonoSingleton<TimeReplayPlayer>
    {
        protected bool playingReplay = false;
        protected ReplayData opponentReplayData = null;
        protected float playingTimer = 0f;

        public event Action<string> ExecuteDataString;
        public event Action OnReplayStopped;

        /// <summary>
        /// Store the information of the opponents Replay
        /// </summary>
        /// <param name="_data">Opponent Replay Data</param>
        public void SetOpponentReplayData(IAPIReplayData _data)
        {
            opponentReplayData = new ReplayData(_data);
        }
        private Coroutine replayCoroutine;
        /// <summary>
        /// Start playing Opponent Replay. It will return false if there is no replay data set
        /// </summary>
        public bool PlayOpponentReplay()
        {
            if (opponentReplayData == null)
                return false;

            if (opponentReplayData.intervalData.Count <= 0)
                return false;

            playingTimer = 0f;
            playingReplay = true;
            replayCoroutine = StartCoroutine(StartReplay());

            return true;
        }

        public ReplayData GetOpponentReplayData()
        {
            return opponentReplayData;
        }

        private void OnDisable()
        {
            if (replayCoroutine != null)
                StopCoroutine(replayCoroutine);
        }

        IEnumerator StartReplay()
        {
            bool _timeInMilliseconds = opponentReplayData.timeInMilliseconds;
            List<IntervalData> data = opponentReplayData.intervalData;
            UnityDebug.Debug.LogInfo("Starting Opponent Replay");
            int totalIntervalCount = 0;
            if (data != null)
                totalIntervalCount = data.Count;
            int playedIntervalCount = 0;
            IntervalData _interval = null;
            if (data != null)
                _interval = data[playedIntervalCount];

            //-1 because the last interval is dummy
            while (playedIntervalCount < totalIntervalCount - 1)
            {
                //change comma to dot for converting it in to invariant style
                string strInterval = _interval.i.Replace(',', '.');
                strInterval = strInterval.Replace('/', '.');

                float _time = float.Parse(strInterval, CultureInfo.InvariantCulture);
                if (_timeInMilliseconds)
                    _time = _time / 1000f;

                if (playingTimer >= _time)
                {
                    ExecuteDataString(_interval.d);
                    playedIntervalCount++;
                    _interval = data[playedIntervalCount];

                }
                yield return new WaitForEndOfFrame();
            }

            //Check for the end of replay
            string strIntervalLast = _interval.i.Replace(',', '.');
            strIntervalLast = strIntervalLast.Replace('/', '.');

            float _timeLast = float.Parse(strIntervalLast, CultureInfo.InvariantCulture);
            if (_timeInMilliseconds)
                _timeLast = _timeLast / 1000f;

            while (_interval != null && playingTimer <= _timeLast)
            {
                yield return null;
            }

            StopPlayingReplay(true);
        }

        /// <summary>
        /// Called to stop playing the replay
        /// </summary>
        /// <param name="manual"> If stopped manually, the "OnReplayStopped" will not be called </param>
        public void StopPlayingReplay(bool callOnStoppedEvent = false)
        {
            UnityDebug.Debug.LogInfo("End Of Opponent Replay");
            playingReplay = false;
            opponentReplayData = null;
            playingTimer = 0f;
            //StopAllCoroutines();
            if (callOnStoppedEvent)
                OnReplayStopped();
        }

        private void Update()
        {
            if (playingReplay && Time.timeScale > 0f)
                playingTimer += Time.deltaTime;
        }

    }
}
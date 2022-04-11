using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Jambox.Tourney.Data;
using Jambox.Common.Utility;

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

        private void OnDisable()
        {
            if (replayCoroutine != null)
                StopCoroutine(replayCoroutine);
        }

        IEnumerator StartReplay()
        {
            List<IntervalData> data = opponentReplayData.intervalData;
            UnityDebug.Debug.Log("Starting Opponent Replay");
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
                string strInterval = _interval.i.Replace(',', '.');
                strInterval = strInterval.Replace('/', '.');
                if (playingTimer >= float.Parse(strInterval))
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
            while (_interval != null && playingTimer <= float.Parse(strIntervalLast))
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
            UnityDebug.Debug.Log("End Of Opponent Replay");
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
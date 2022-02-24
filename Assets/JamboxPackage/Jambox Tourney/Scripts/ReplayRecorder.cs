namespace Jambox.Tourney.Connector
{
    using Jambox.Tourney.Data;
    using Jambox.Common.TinyJson;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using UnityEngine;
    using UnityEngine.Scripting;

    public class ReplayRecorder
    {

        /// <summary>
        /// List of recordings that contain information at specific time
        /// </summary>
        public ReplayData replayData = null;

        private bool IsRecording = false;
        private float recordingTimer = 0f;

        /*
        private void Update()
        {
            if (IsRecording && Time.timeScale > 0f)
                recordingTimer += Time.deltaTime;
        }*/

        /// <summary>
        /// Starts a new Replay Recording
        /// </summary>
        public void StartRecording(Dictionary<string, string> _gameData)
        {
            replayData = new ReplayData(_gameData);
            IsRecording = true;
        }

        public IEnumerator StartRecordingTimer()
        {
            while (IsRecording)
            {
                if (Time.timeScale > 0f)
                {
                    recordingTimer += Time.deltaTime;
                }
                yield return null;
            }
        }

        /// <summary>
        /// Stops current Replay Recording
        /// </summary>
        public void StopRecording(string _data = null, string _keyValue = null)
        {
            //Creating an interval to find the end time
            
            AddNewData(_data);

            IsRecording = false;
            recordingTimer = 0f;
        }

        /// <summary>
        /// Adds new data to the dictionary with the key as the time, it is added
        /// </summary>
        /// <param name="_data"> Data that new to be stored converted to json string </param> 
        /// <param name="_keyValue"> If keyValue is null, the timer will be used as the key, else the given key will used </param> 
        public void AddNewData(string _data, string _keyValue = null)
        {
            string keyValueAsString;
            if (_keyValue != null)
            {
                keyValueAsString = _keyValue.ToString();
            }
            else
            {
                keyValueAsString = recordingTimer.ToString();
            }
            IntervalData newInterval = new IntervalData(keyValueAsString, _data);
            replayData.intervalData.Add(newInterval);
        }


        /// <summary>
        /// Gets the dictionary of the lastest replay recording
        /// </summary>
        /// <returns></returns>
        public ReplayData GetCurrentReplay()
        {
            return replayData;
        }

    }

    /// <summary>
    /// Complete Replay with all information
    /// </summary>
    public class ReplayData
    {
        [DataMember(Name = "game_data"), Preserve]
        public Dictionary<string, string> gameData;

        /// <summary>
        /// Last interval is dummy, to find the end tie
        /// </summary>
        [DataMember(Name = "interval_data"), Preserve]
        public List<IntervalData> intervalData;

        public ReplayData(Dictionary<string, string> _data)
        {
            intervalData = new List<IntervalData>();
            gameData = _data;
        }

        public ReplayData(IAPIReplayData _data)
        {
            gameData = new Dictionary<string, string>();
            foreach (var v in _data.GameData)
            {
                gameData.Add(v.Key, v.Value);
            }
            intervalData = new List<IntervalData>();
            foreach (var _interval in _data.IntervalData)
            {
                IntervalData _i = new IntervalData(_interval.interval, _interval.data);
                intervalData.Add(_i);
            }
        }
    }

    /// <summary>
    /// Class which contains a specific time frame along with the information at that time;
    /// </summary>
    public class IntervalData
    {
        public string i;
        public string d;

        public IntervalData(string _interval, string _data)
        {
            i = _interval;
            d = _data;
        }
    }

}


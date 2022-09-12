namespace JBX.Leaderboard.Controller
{
    using Jambox.Common.Utility;
    using JBX.Leaderboard.Data;
    using System.Collections.Generic;
    using System;
    using UnityEngine;

    public class LeaderBoardData
    {
        public string PlayerId;
        public long Rank;
        public long Score;
        public string Username;
        public string AvatarUrl;
        public string MetaData;
        public Sprite avatarSprite;

        public LeaderBoardData (IApiLeaderRecord _leaderData)
        {
            PlayerId = _leaderData.PlayerId;
            Rank = _leaderData.Rank;
            Score = _leaderData.Score;
            Username = _leaderData.Username;
            AvatarUrl = _leaderData.AvatarUrl;
            MetaData = _leaderData.MetaData;
        }
    }

    public class LeaderBoardMasterData
    {
        public string LeaderboardID;
        public string Name;
        public string Description;
        public bool isActive;
        public string ResetTime;
        public bool TestOnly;
        public int MaximumUser;
        public int SortingOrder;
        public int Operator;
        public List<LeaderBoardRewardData> LBRewardList;

        public LeaderBoardMasterData(ILeaderboardElement MasterData)
        {
            LeaderboardID = MasterData.LeaderboardID.ToString();
            Name = MasterData.Name;
            Description = MasterData.Description;
            isActive = MasterData.isActive;
            ResetTime = MasterData.ResetTime;
            TestOnly = MasterData.TestOnly;
            MaximumUser = MasterData.MaximumUser;
            SortingOrder = MasterData.SortingOrder;
            Operator = MasterData.Operator;
            LBRewardList = new List<LeaderBoardRewardData>();
            foreach (var dataTempo in MasterData.RewardList.RewardsDistribution)
            {
                LeaderBoardRewardData tempoNew = new LeaderBoardRewardData(dataTempo);
                LBRewardList.Add(tempoNew);
            }
        }
    }

    public class LeaderBoardRewardData
    {
        public int StartRank;
        public int EndRank;
        public Dictionary<String, int> VirtualRewards;
        public Dictionary<String, int> InGameRewards;

        public LeaderBoardRewardData (IApiRewardDistribution dataServ)
        {
            StartRank = dataServ.StartRank;
            EndRank = dataServ.EndRank;
            VirtualRewards = new Dictionary<string, int>();
            InGameRewards = new Dictionary<string, int>();
            if(!String.IsNullOrEmpty(dataServ.VirtualRewards.Key))
                VirtualRewards.Add(dataServ.VirtualRewards.Key, dataServ.VirtualRewards.Value);
            if (!String.IsNullOrEmpty(dataServ.InGameRewards.Key))
                InGameRewards.Add(dataServ.InGameRewards.Key, dataServ.InGameRewards.Value);
        }
    }

    public class UIPendingRewardData
    {
        public string LeaderboardId;
        public string LeaderboardName;
        public int Score;
        public string PartitionKey;
        public string DisplayScore;
        public int Rank;
        public bool IsClaimed;
        public string CreateTime;
        public Dictionary<String, int> VirtualRewards;
        public Dictionary<String, int> InGameRewards;

        public UIPendingRewardData(IRewardData data)
        {
            LeaderboardId = data.LeaderboardID.ToString();
            LeaderboardName = data.LeaderBoardName;
            foreach (var dataNew in data.FullRewardData)
            {
                Score = (int)dataNew.score;
                PartitionKey = dataNew.PartitionKey;
                DisplayScore = dataNew.DisplayScore;
                Rank = (int)dataNew.Rank;
                IsClaimed = dataNew.IsClaimed;
                CreateTime = dataNew.CreateTime;
                VirtualRewards = new Dictionary<string, int>();
                InGameRewards = new Dictionary<string, int>();
                Debug.Log("FullRewardData : " + dataNew.ToString());
                Debug.Log("Reward Data Null : " + (dataNew.RewardData != null));
                Debug.Log("Reward Data empty : " + String.IsNullOrEmpty(dataNew.RewardData.VirtualRewards.Key));
                if ((dataNew.RewardData != null) && !String.IsNullOrEmpty(dataNew.RewardData.VirtualRewards.Key))
                {
                    Debug.Log("Virtual Key : " + dataNew.RewardData.VirtualRewards.Key);
                    Debug.Log("Virtual value : " + dataNew.RewardData.VirtualRewards.Value);
                    VirtualRewards.Add(dataNew.RewardData.VirtualRewards.Key, dataNew.RewardData.VirtualRewards.Value);
                }
                if ((dataNew.RewardData != null) && !String.IsNullOrEmpty(dataNew.RewardData.InGameRewards.Key))
                    InGameRewards.Add(dataNew.RewardData.InGameRewards.Key, dataNew.RewardData.InGameRewards.Value);
            }
        }
    }

    public class LeaderboardDataContain : MonoSingleton<LeaderboardDataContain>
    {
        public List<LeaderBoardMasterData> LBMasterData;

        public void UpdateLBMasterData (ILeaderboardList _Master)
        {
            UnityDebug.Debug.Log("UpdateLBMasterData Hit >>>>>>");
            LBMasterData = new List<LeaderBoardMasterData>();
            foreach(var tempo in _Master.LeaderBoardList)
            {
                Debug.Log("Updating master LbData Loop Hit >>>>>" + tempo.Name);
                LeaderBoardMasterData newData = new LeaderBoardMasterData(tempo);
                LBMasterData.Add(newData);
            }
        }

        public LeaderBoardMasterData GetMasterLBDatawithLBID (string LBID)
        {
            foreach (var dataNew in LBMasterData)
            {
                if(dataNew.LeaderboardID.Equals(LBID))
                {
                    return dataNew;
                }
            }
            return null;
        }
    }
}

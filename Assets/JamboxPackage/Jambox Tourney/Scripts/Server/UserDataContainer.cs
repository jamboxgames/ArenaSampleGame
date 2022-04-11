namespace Jambox.Tourney.Connector
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Jambox.Tourney.Data;
    using UnityEngine;
    using Jambox.Common.Utility;

    #region REWARD_CLASS
    public class RewardList
    {
        public List<RewardDistribution> RewardsDistribution = new List<RewardDistribution>();
    }

    public class RewardDistribution
    {
        public int Start_Rank;
        public int End_Rank;

        //Single Currency
        public string CurrencyType;
        public int WinAmount;

        //Single Currency
        public string InGameType;
        public int InGameAmount;


        public RewardDistribution(IApiRewardDistribution Data)
        {
            Start_Rank = Data.StartRank;
            End_Rank = Data.EndRank;

            CurrencyType = Data.VirtualRewards.Key;
            WinAmount = Data.VirtualRewards.Value;

            InGameType = Data.InGameRewards.Key;
            InGameAmount = Data.InGameRewards.Value;

        }
    }
    #endregion

    public class CompletedTourneyDetail
    {
        public string Tourneyid;
        public int MyRank;
        public int JoinedPlayer;
        public int Score;
        public int SubScore;
        public string LeaderBoardID;
        public string TourneyName;
        public int RewardAmnt;
        public string RewardCurrency;
        public int IngameItemCount;
        public string IngameItemType;
        public bool IsClaimed;
        public string EndTime;
        public RewardList RewardList;

        public CompletedTourneyDetail(IAPICompletedTourney data_)
        {
            Tourneyid = data_.TourneyID;
            MyRank = data_.Rank;
            JoinedPlayer = data_.JoinedPlayer;
            TourneyName = data_.TourneyName;
            Score = data_.Score;
            SubScore = data_.SubScore;
            LeaderBoardID = data_.LeaderBoardID;
            RewardAmnt = data_.RewardAmnt;
            RewardCurrency = data_.RewardCurrency;
            IngameItemType = data_.RewardInfo.InGame.Key;
            IngameItemCount = data_.RewardInfo.InGame.Value;
            IsClaimed = data_.isClaimed;
            EndTime = data_.EndTime;
            RewardList = new RewardList();
            foreach (var RewardData in data_.RewardList.RewardsDistribution)
            {
                RewardList.RewardsDistribution.Add(new RewardDistribution(RewardData));
            }
        }
    }

    public class JoinedTourneyDetail
    {
        public string Tourneyid;
        public int MyRank;
        public int CurrentPlayers;
        public int Score;
        public int SubScore;
        public int CurrentAttempt;
        public string LeaderBoardID;
        public string JoinCode;

        public JoinedTourneyDetail (string tourneyid, int myRank, int currPlayer, int score, int subScore, int currAttempt, string lbID)
        {
            Tourneyid = tourneyid;
            MyRank = myRank;
            CurrentPlayers = currPlayer;
            CurrentAttempt = currAttempt;
            Score = score;
            SubScore = subScore;
            LeaderBoardID = lbID;
        }
        public JoinedTourneyDetail(IApiJoinedTourneyDetail data_)
        {
            Tourneyid = data_.Tourneyid;
            MyRank = data_.MyRank;
            CurrentPlayers = data_.CurrentPlayers;
            CurrentAttempt = data_.CurrentAttempt;
            Score = data_.Score;
            SubScore = data_.SubScore;
            LeaderBoardID = data_.LeaderBoardID;
            JoinCode = data_.JoinCode; 
    }
    public JoinedTourneyDetail(string lbID)
        {
            LeaderBoardID = lbID;
        }
    }

    public class TournamnetData 
    {
        public string Tourneyid;
        public string TourneyName;
        public int MaxPlayers;
        public int EntryFee;
        public string Currency;
        public string StartTime;
        public string EndTime;
        public string Status;
        public int MaxEntry;
        public string Description;
        public int CurrentPlayers;
        public int Category;
        public bool PlayWithVideoAD;
        //Adding this for Duels So that we don't have to save multiple data.
        public string LeaderBoardID;
        public Dictionary<string, string> metadata;

        public RewardList RewardList;

        public TournamnetData(IApiTournamentData data_)
        {
            Tourneyid = data_.Tourneyid;
            TourneyName = data_.TourneyName;
            MaxPlayers = data_.MaxPlayers;
            EntryFee = data_.EntryFee;
            Currency = data_.Currency;
            StartTime = data_.StartTime;
            EndTime = data_.EndTime;
            Status = data_.Status;
            MaxEntry = data_.MaxEntry;
            Description = data_.Description;
            CurrentPlayers = data_.CurrentPlayers;
            Category = data_.Category;
            PlayWithVideoAD = data_.PlayWithAd;
            //metadata = data_.MetaData;
            if (!string.IsNullOrEmpty(data_.MetaData))
            {
                metadata = Jambox.Common.TinyJson.JsonParser.FromJson<Dictionary<string, string>>(data_.MetaData);
            }
            RewardList = new RewardList();
            foreach (var RewardData in data_.RewardList.RewardsDistribution)
            {
                RewardList.RewardsDistribution.Add(new RewardDistribution(RewardData));
            }
        }
    }

    /// <summary>
    /// Combned Data of All Tourney and user Joined Tourney.
    /// </summary>
    public class TourneyDetail
    {
        public TournamnetData _tournament;
        public JoinedTourneyDetail _joinedTourneyData;
        public bool isJoined;

        public TourneyDetail(TournamnetData data1, JoinedTourneyDetail data2 = null, bool isJoin = false)
        {
            _tournament = data1;
            _joinedTourneyData = data2;
            isJoined = isJoin;
        }
    }

    public class FriendlyTournamentData
    {
        public string Tourneyid;
        public string TourneyName;
        public int MaxPlayers;
        public int EntryFee;
        public string Currency;
        public string StartTime;
        public string EndTime;
        public string Status;
        public int MaxEntry;
        public string Description;
        public int CurrentPlayers;
        public string Category;
        public bool PlayWithVideoAD;
        public int CurrentAttempt;
        public string LeaderboardID;

        public FriendlyTournamentData(IAPIFriendlyTournamentData data_)
        {
            /*
            Tourneyid = data_.Tourneyid;
            TourneyName = data_.TourneyName;
            MaxPlayers = data_.MaxPlayers;
            RewardText = data_.RewardText;
            Currency = data_.Currency;
            StartTime = data_.StartTime;
            EndTime = data_.EndTime;
            Status = data_.Status;
            MaxEntry = data_.MaxEntry;
            Description = data_.Description;
            CurrentPlayers = data_.CurrentPlayers;
            Category = data_.Category;
            PlayWithVideoAD = data_.PlayWithAd;
            RewardList = new List<RewardPerTourney>();
            */
        }
    }

    public class FriendlyTourneyDetail
    {
        public FriendlyTournamentData _tournament;
        //public JoinedTourneyDetail _joinedTourneyData;
        //public bool isJoined;

        public FriendlyTourneyDetail(FriendlyTournamentData data1)
        {
            _tournament = data1;
        }
    }

    /// <summary>
    /// This class will contain all the user data. 
    /// </summary>
    public class UserDataContainer : MonoSingleton<UserDataContainer>
    {
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, TournamnetData> MyTotalTourney;
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, JoinedTourneyDetail> MyJoinedTourney;
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, TourneyDetail> UpdatedTourneyData;

        public Dictionary<string, TournamnetData> MyDuels;

        public Dictionary<string, TourneyDetail> UpdatedFriendlyData;

        public List<CompletedTourneyDetail> CompletedTourney;

        public Dictionary<string, string> currencyList = new Dictionary<string, string>();

        private Dictionary<string, long> userMoney;

        private int unclaimedRewardsCount;
        public int UnclaimedRewardsCount
        {
            set { unclaimedRewardsCount = value; }
            get { return unclaimedRewardsCount; }
        }

        private void Awake()
        {
            userMoney = new Dictionary<string, long>();
        }
        public Dictionary<string, long> getUserMoney()
        {
            return userMoney;
        }

        public string GetDisplayCurrencyText()
        {
            //Get first currency
            string displayString = "";
            foreach (var kvp in userMoney)
            {
                string currencyDsiplayString = "";
                bool bCurrencyFound = currencyList.TryGetValue(kvp.Key, out currencyDsiplayString);

                if (bCurrencyFound)
                    displayString += kvp.Value.ToString();
                else
                    Debug.LogError("Currency key is not setup for the Game : " + kvp.Key);
            }
            return displayString;
        }

        public string GetCurrencyDisplayTextForKey(string currencyKey)
        {
            string currDisplayText = "";
            currencyList.TryGetValue(currencyKey, out currDisplayText);
            return currDisplayText;
        }

        public void UpdateUserMoney(long val, string CurrencyKey, bool isIncrease)
        {
            string UpdatedCurrencyKey = CurrencyKey;
            currencyList.TryGetValue(CurrencyKey, out UpdatedCurrencyKey);
            long money = 0;
            userMoney.TryGetValue(CurrencyKey, out money);
            if(isIncrease)
            {
                money += val;
            }
            else
            {
                money -= val;
            }
            if(userMoney.ContainsKey(CurrencyKey))
            {
                userMoney.Remove(CurrencyKey);
                userMoney.Add(CurrencyKey, money);
            }
            UnityDebug.Debug.Log("Updated User Money :\nKey : " + CurrencyKey + "  Value : " + money);
            ArenaSDKEvent.Instance.FireOnUpdateUserMoney(val, CurrencyKey, isIncrease);
        }
        public void setUserMoney(Dictionary<string, long> MoneyDet)
        {
            foreach (var data in MoneyDet)
            {
                if (userMoney.ContainsKey(data.Key))
                {
                    userMoney.Remove(data.Key);
                }
                else {
                    //Debug.LogError("The Cyrrency with key :" + data.Key + " is not available on server .");
                }
                userMoney.Add(data.Key, data.Value);
            }
        }

        //Duel Opponent Sprite Temporary
        public Sprite tempDuelOpponentSprite;

        public int tempRewardsCount;

        //Friendly
        public Dictionary<string, FriendlyTournamentData> myFriendlyTournaments;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tourneys"></param>
        public void UpdateTourneyData(IApiTourneyList tourneys)
        {
            MyTotalTourney = new Dictionary<string, TournamnetData>();   
            MyJoinedTourney = new Dictionary<string, JoinedTourneyDetail>();
            UpdatedTourneyData = new Dictionary<string, TourneyDetail>();
            UpdatedFriendlyData = new Dictionary<string, TourneyDetail>();
            MyDuels = new Dictionary<string, TournamnetData>();
            
            foreach (var dataJoined in tourneys.Tournaments)
            {
                if (MyJoinedTourney.ContainsKey(dataJoined.JoinedData.Tourneyid))
                    MyJoinedTourney.Remove(dataJoined.JoinedData.Tourneyid);
                MyJoinedTourney.Add(dataJoined.JoinedData.Tourneyid, new JoinedTourneyDetail (dataJoined.JoinedData));
            }
            foreach (var dataTourney in tourneys.Tournaments)
            {
                if (MyTotalTourney.ContainsKey(dataTourney.TourneyDetail.Tourneyid))
                    MyTotalTourney.Remove(dataTourney.TourneyDetail.Tourneyid);
                MyTotalTourney.Add(dataTourney.TourneyDetail.Tourneyid, new TournamnetData(dataTourney.TourneyDetail));
                JoinedTourneyDetail dataJoin = null;
                bool isJoined = false;
                if (MyJoinedTourney.Keys.Count > 0 && MyJoinedTourney.ContainsKey(dataTourney.TourneyDetail.Tourneyid))
                {
                    isJoined = true;
                    MyJoinedTourney.TryGetValue(dataTourney.TourneyDetail.Tourneyid, out dataJoin);
                }

                if (UpdatedTourneyData.ContainsKey(dataTourney.TourneyDetail.Tourneyid))
                    UpdatedTourneyData.Remove(dataTourney.TourneyDetail.Tourneyid);
                if(DateTime.Parse(dataTourney.TourneyDetail.EndTime).ToUniversalTime() > DateTime.UtcNow)
                    UpdatedTourneyData.Add(dataTourney.TourneyDetail.Tourneyid, new TourneyDetail(new TournamnetData(dataTourney.TourneyDetail), dataJoin, isJoined));
            }

            foreach (var dataDuels in tourneys.Duels)
            {
                if (MyDuels.ContainsKey(dataDuels.Tourneyid))
                    MyDuels.Remove(dataDuels.Tourneyid);
                MyDuels.Add(dataDuels.Tourneyid, new TournamnetData(dataDuels));
            }
            foreach (var dataPrivate in tourneys.PrivateTourney)
            {
                bool isJoined = !string.IsNullOrEmpty(dataPrivate.JoinedData.LeaderBoardID);
                UpdatedFriendlyData.Add(dataPrivate.TourneyDetail.Tourneyid, new TourneyDetail
                          (new TournamnetData(dataPrivate.TourneyDetail), new JoinedTourneyDetail(dataPrivate.JoinedData), true));
            }
        }

        public void UpdateTourneyDataOnJoin(string tourneyID, IAPIJoinTourney JoinedData)
        {
            TourneyDetail TourneyData;
            UpdatedTourneyData.TryGetValue(tourneyID, out TourneyData);
            TourneyData.isJoined = true;
            TourneyData._joinedTourneyData = new JoinedTourneyDetail(tourneyID, 0, TourneyData._tournament.CurrentPlayers, 0, 0, 0,
                                                                        JoinedData.LeaderBoardID);
            UpdatedTourneyData.Remove(tourneyID);
            UpdatedTourneyData.Add(tourneyID, TourneyData);
        }

        public void UpdateTourneyDataOnPlay(string tourneyID, IApiPlayTourney PlayData)
        {
            TourneyDetail TourneyData;
            UpdatedTourneyData.TryGetValue(tourneyID, out TourneyData);
            TourneyData._joinedTourneyData.CurrentAttempt = TourneyData._joinedTourneyData.CurrentAttempt + 1;
            TourneyData._joinedTourneyData.LeaderBoardID = PlayData.LeaderBoardID;
            UpdatedTourneyData.Remove(tourneyID);
            UpdatedTourneyData.Add(tourneyID, TourneyData);
        }

        public void UpdateTourneyDataOnScore(string tourneyId, IApiSubmitScore data)
        {
            TourneyDetail TourneyData;
            UpdatedTourneyData.TryGetValue(tourneyId, out TourneyData);
            TourneyData._joinedTourneyData.MyRank = data.MyRank;
            TourneyData._joinedTourneyData.Score = data.BestScore;
            TourneyData._joinedTourneyData.CurrentPlayers = data.CurrentPlayer;
            TourneyData._joinedTourneyData.LeaderBoardID = data.LeaderBoardID;
            TourneyData._joinedTourneyData.CurrentAttempt = data.EntryCount;
            UpdatedTourneyData.Remove(tourneyId);
            UpdatedTourneyData.Add(tourneyId, TourneyData);
        }

        public void UpdateFriendlyDataOnScore(string tourneyId, IApiSubmitScore data)
        {
            TourneyDetail TourneyData;
            UpdatedFriendlyData.TryGetValue(tourneyId, out TourneyData);
            TourneyData._joinedTourneyData.MyRank = data.MyRank;
            TourneyData._joinedTourneyData.Score = data.BestScore;
            TourneyData._joinedTourneyData.CurrentPlayers = data.CurrentPlayer;
            TourneyData._joinedTourneyData.LeaderBoardID = data.LeaderBoardID;
            TourneyData._joinedTourneyData.CurrentAttempt = data.EntryCount;
            UpdatedFriendlyData.Remove(tourneyId);
            UpdatedFriendlyData.Add(tourneyId, TourneyData);
        }
        public void UpdateCompTourneyData (IAPICompTourneyList dataRcvd)
        {
            CompletedTourney = new List<CompletedTourneyDetail>();
            foreach (var dataComp in dataRcvd.CompTournaments)
            {
                CompletedTourney.Add(new CompletedTourneyDetail (dataComp));
            }
        }

        #region FRIENDLY_TOURNAMENTS
        public void UpdateFriendlyDetails(IAPIFriendlyTourneyList tourneys)
        {
            /*
            MyTotalTourney = new Dictionary<string, TournamnetData>();
            MyJoinedTourney = new Dictionary<string, JoinedTourneyDetail>();
            UpdatedTourneyData = new Dictionary<string, TourneyDetail>();

            UnityDebug.Debug.Log("UpdateTourneyData Hit 1111111>>>>> ");

            foreach (var data in tourneys.JoinedTournaments)
            {
                if (MyJoinedTourney.ContainsKey(data.Tourneyid))
                    MyJoinedTourney.Remove(data.Tourneyid);
                MyJoinedTourney.Add(data.Tourneyid, new JoinedTourneyDetail(data));
            }
            foreach (var data in tourneys.Tournaments)
            {
                if (MyTotalTourney.ContainsKey(data.Tourneyid))
                    MyTotalTourney.Remove(data.Tourneyid);
                MyTotalTourney.Add(data.Tourneyid, new TournamnetData(data));
                JoinedTourneyDetail dataJoin = null;
                bool isJoined = false;
                if (MyJoinedTourney.Keys.Count > 0 && MyJoinedTourney.ContainsKey(data.Tourneyid))
                {
                    isJoined = true;
                    MyJoinedTourney.TryGetValue(data.Tourneyid, out dataJoin);
                }

                if (UpdatedTourneyData.ContainsKey(data.Tourneyid))
                    UpdatedTourneyData.Remove(data.Tourneyid);
                UpdatedTourneyData.Add(data.Tourneyid, new TourneyDetail(new TournamnetData(data), dataJoin, isJoined));
            }
            UnityDebug.Debug.Log("UpdateTourneyData Hit 1111111>>>>> " + UpdatedTourneyData.Values.Count);*/
        }

        public void UpdateFriendlyTourneyDataOnPlay(string tourneyID, IApiPlayFriendlyTourney PlayData)
        {
            FriendlyTournamentData TourneyData;
            myFriendlyTournaments.TryGetValue(tourneyID, out TourneyData);
            TourneyData.CurrentAttempt = TourneyData.CurrentAttempt + 1;
            TourneyData.LeaderboardID = PlayData.LeaderBoardID;
            myFriendlyTournaments.Remove(tourneyID);
            myFriendlyTournaments.Add(tourneyID, TourneyData);
        }
        #endregion

        public void UpdateCurrencyList(IAPICurrencyList dataRcvd)
        {
            currencyList.Clear();
            foreach (var data in dataRcvd.CurrencyList)
            {
                currencyList.Add(data.CurrentKey, data.CurrentName); ;
            }
        }

        public string GetCurrencyName (string CurrKey)
        {
            string CurrencyName = "";
            currencyList.TryGetValue(CurrKey, out CurrencyName);
            return CurrencyName;
        }

        public void UpdateRewardsClaimed(CompletedTourneyDetail data)
        {
            foreach (var _data in CompletedTourney)
            {
                if (data == _data)
                {
                    _data.IsClaimed = true;
                }
            }
        }

    }
}
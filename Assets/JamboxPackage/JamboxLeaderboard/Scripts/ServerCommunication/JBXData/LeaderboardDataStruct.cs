namespace JBX.Leaderboard.Data
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Security.Cryptography;
    using UnityEngine.Scripting;

    public interface ILeaderboardList
    {
        IEnumerable<ILeaderboardElement> LeaderBoardList { get; }
    }

    internal class ApiLeaderboardList : ILeaderboardList
    {
        /// <inheritdoc />
        [DataMember(Name = "leaderboard_list"), Preserve]
        public List<APILeaderboardElement> _LeaderBoardList { get; set; }

        public IEnumerable<ILeaderboardElement> LeaderBoardList => _LeaderBoardList ?? new List<APILeaderboardElement>(0);

    }

    public interface ILeaderboardElement
    {
        /// <summary>
		/// 
		/// </summary>
        string LeaderboardID { get; }

        /// <summary>
		/// 
		/// </summary>
        string Name { get; }

        /// <summary>
		/// 
		/// </summary>
        string Description { get; }

        /// <summary>
		/// 
		/// </summary>
        Boolean isActive { get; }

        /// <summary>
		/// 
		/// </summary>
        string ResetTime { get; }

        bool TestOnly { get; }

        int MaximumUser { get; }

        int SortingOrder { get; }

        int Operator { get; }

        IApiLeaderboardReward RewardList { get; }
    }

    internal class APILeaderboardElement : ILeaderboardElement
    {
        /// <inheritdoc />
        [DataMember(Name = "leaderboard_id"), Preserve]
        public string LeaderboardID { set; get; }

        /// <inheritdoc />
        [DataMember(Name = "name"), Preserve]
        public string Name { set; get; }

        /// <inheritdoc />
        [DataMember(Name = "description"), Preserve]
        public string Description { set; get; }

        /// <inheritdoc />
        [DataMember(Name = "is_active"), Preserve]
        public bool isActive { set; get; }

        /// <inheritdoc />
        [DataMember(Name = "reset_time"), Preserve]
        public string ResetTime { set; get; }

        /// <inheritdoc />
        [DataMember(Name = "test_only"), Preserve]
        public bool TestOnly { set; get; }

        /// <inheritdoc />
        [DataMember(Name = "max_user_count"), Preserve]
        public int MaximumUser { set; get; }

        /// <inheritdoc />
        [DataMember(Name = "sort_order"), Preserve]
        public int SortingOrder { set; get; }

        /// <inheritdoc />
        [DataMember(Name = "operator"), Preserve]
        public int Operator { set; get; }

        /// <inheritdoc />
        [DataMember(Name = "rewardlist"), Preserve]
        public ApiLeaderboardReward _rewardList { set; get; }

        public IApiLeaderboardReward RewardList => _rewardList ?? new ApiLeaderboardReward();
    }

    public interface IApiLeaderboardReward
    {
        IEnumerable<IApiRewardDistribution> RewardsDistribution { get; }
    }

    internal class ApiLeaderboardReward : IApiLeaderboardReward
    {
        /// <inheritdoc />
        [DataMember(Name = "distribution"), Preserve]
        public List<ApiRewardDistribution> _rewardsDistribution { get; set; }

        public IEnumerable<IApiRewardDistribution> RewardsDistribution => _rewardsDistribution ?? new List<ApiRewardDistribution>(0);
    }

    public interface IApiRewardDistribution
    {
        /// <summary>
        /// 
        /// </summary>
        int StartRank { get; }
        /// <summary>
        /// 
        /// </summary>
        int EndRank { get; }

        IApiReward VirtualRewards { get; }

        IApiReward InGameRewards { get; }

    }

    internal class ApiRewardDistribution : IApiRewardDistribution
    {
        /// <inheritdoc />
        [DataMember(Name = "start"), Preserve]
        public int StartRank { get; set; }
        /// <inheritdoc />
        [DataMember(Name = "end"), Preserve]
        public int EndRank { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "virtual"), Preserve]
        public ApiReward _virtualRewards { get; set; }

        public IApiReward VirtualRewards => _virtualRewards ?? new ApiReward();

        /// <inheritdoc />
        [DataMember(Name = "ingame"), Preserve]
        public ApiReward _inGameRewards { get; set; }

        public IApiReward InGameRewards => _inGameRewards ?? new ApiReward();
    }

    public interface IApiReward
    {

        string Key { get; }

        int Value { get; }
    }

    /// <inheritdoc />
    internal class ApiReward : IApiReward
    {
        /// <inheritdoc />
        [DataMember(Name = "key"), Preserve]
        public string Key { set; get; }

        /// <inheritdoc />
        [DataMember(Name = "value"), Preserve]
        public int Value { set; get; }
    }

    #region LEADERBOARD_ELEMENT

    /// <summary>
    /// Represents a complete leaderboard record with all scores and associated metadata.
    /// </summary>
    public interface IApiLeaderRecord
    {
        /// <summary>
        /// The ID of the score owner, usually a user.
        /// </summary>
        string PlayerId { get; }

        /// <summary>
        /// The rank of this record.
        /// </summary>
        long Rank { get; }

        /// <summary>
        /// The score value.
        /// </summary>
        long Score { get; }

        /// <summary>
        /// The username of the score owner, if the owner is a user.
        /// </summary>
        string Username { get; }

        /// <summary>
        /// The Avatar url of the score owner, if the owner is a user.
        /// </summary>
        string AvatarUrl { get; }

        /// <summary>
        /// MetaData is the data send by client, it will be returned by server as it is.
        /// </summary>
        string MetaData { get; }

        IApiLeaderboardReward rewardList { get; }
    }

    /// <inheritdoc />
    internal class ApiLeaderRecord : IApiLeaderRecord
    {
        /// <inheritdoc />
        [DataMember(Name = "player_id"), Preserve]
        public string PlayerId { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "rank"), Preserve]
        public long Rank { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "score"), Preserve]
        public long Score { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "player_name"), Preserve]
        public string Username { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "player_url"), Preserve]
        public string AvatarUrl { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "metadata"), Preserve]
        public string MetaData { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "reward_info"), Preserve]
        public ApiLeaderboardReward _rewardList { set; get; }

        public IApiLeaderboardReward rewardList => _rewardList ?? new ApiLeaderboardReward();

        public override string ToString()
        {
            var output = "";
            output = string.Concat(output, "OwnerId: ", PlayerId, ", ");
            output = string.Concat(output, "Rank: ", Rank, ", ");
            output = string.Concat(output, "Score: ", Score, ", ");
            output = string.Concat(output, "Username: ", Username, ", ");
            output = string.Concat(output, "AvatarUrl: ", AvatarUrl, ", ");
            return output;
        }
    }

    public interface IAPILeaderBoardheaders
    {
        /// <summary>
        /// The Score header text
        /// </summary>
        string ScoreText { get; }

        /// <summary>
        /// The Rank header text
        /// </summary>
        string RankText { get; }

        /// <summary>
        /// The name header Text
        /// </summary>
        string NameText { get; }
    }

    internal class APILeaderBoardheaders : IAPILeaderBoardheaders
    {
        /// <inheritdoc />
        [DataMember(Name = "score"), Preserve]
        public string ScoreText { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "rank"), Preserve]
        public string RankText { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "name"), Preserve]
        public string NameText { get; set; }
    }

    /// <summary>
    /// A set of leaderboard records, may be part of a leaderboard records page or a batch of individual records.
    /// </summary>
    public interface IApiLeaderRecordList
    {
        /// <summary>
        /// The UNIX time when the leaderboard record Refresh.
        /// </summary>
        string refreshTime { get; }

        /// <summary>
        /// Headers for leaderboard columns.
        /// </summary>
        IAPILeaderBoardheaders LeaderBoardHeaders { get; }

        /// <summary>
        /// The unique Id which is associated with every leaderboard.
        /// </summary>
        string leaderboardId { get; }

        string leaderboardName { get; }

        /// <summary>
        /// A batched set of leaderboard records belonging to specified owners.
        /// </summary>
        IEnumerable<IApiLeaderRecord> LeaderRecords { get; }
    }

    /// <inheritdoc />
    internal class ApiLeaderRecordList : IApiLeaderRecordList
    {
        /// <inheritdoc />
        [DataMember(Name = "refresh_time"), Preserve]
        public string refreshTime { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "leaderboard_id"), Preserve]
        public string leaderboardId { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "name"), Preserve]
        public string leaderboardName { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "headers"), Preserve]
        public APILeaderBoardheaders LBHeaders { get; set; }

        public IAPILeaderBoardheaders LeaderBoardHeaders => LBHeaders ?? new APILeaderBoardheaders();

        /// <inheritdoc />
        [DataMember(Name = "user_records"), Preserve]
        public List<ApiLeaderRecord> _leaderboardRecords { get; set; }

        public IEnumerable<IApiLeaderRecord> LeaderRecords => _leaderboardRecords ?? new List<ApiLeaderRecord>(0);

        public override string ToString()
        {
            var output = "";
            output = string.Concat(output, "refreshTime: ", refreshTime, ", ");
            output = string.Concat(output, "leaderboardId: ", leaderboardId, ", ");
            output = string.Concat(output, "OwnerRecords: [", string.Join(", ", LeaderRecords), "], ");
            return output;
        }
    }


    /// <summary>
    /// A set of leaderboard records, may be part of a leaderboard records page or a batch of individual records.
    /// </summary>
    public interface IApiLeaderAroundMe
    {
        /// <summary>
        /// Headers for leaderboard columns.
        /// </summary>
        IAPILeaderBoardheaders LeaderBoardHeaders { get; }

        /// <summary>
        /// The unique Id which is associated with every leaderboard.
        /// </summary>
        string leaderboardId { get; }

        /// <summary>
        /// A batched set of leaderboard records belonging to specified owners.
        /// </summary>
        IEnumerable<IApiLeaderRecord> LeaderRecords { get; }
    }

    /// <inheritdoc />
    internal class ApiLeaderAroundMe : IApiLeaderAroundMe
    {
        /// <inheritdoc />
        [DataMember(Name = "leaderboard_id"), Preserve]
        public string leaderboardId { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "headers"), Preserve]
        public APILeaderBoardheaders LBHeaders { get; set; }

        public IAPILeaderBoardheaders LeaderBoardHeaders => LBHeaders ?? new APILeaderBoardheaders();

        /// <inheritdoc />
        [DataMember(Name = "records"), Preserve]
        public List<ApiLeaderRecord> _leaderboardRecords { get; set; }

        public IEnumerable<IApiLeaderRecord> LeaderRecords => _leaderboardRecords ?? new List<ApiLeaderRecord>(0);

        public override string ToString()
        {
            var output = "";
            output = string.Concat(output, "leaderboardId: ", leaderboardId, ", ");
            output = string.Concat(output, "OwnerRecords: [", string.Join(", ", LeaderRecords), "], ");
            return output;
        }
    }

    #endregion

    #region SUBMIT_SCORE

    public interface ISubmitScore
    {
        long BestScore { get; }

        int CurrentPlayers { get; }

        string LBID { get; }

        int Operator { get; }

        long Rank { get; }
    }

    internal class ApiSubmitScore : ISubmitScore
    {
        /// <inheritdoc />
        [DataMember(Name = "best_score"), Preserve]
        public long BestScore { set; get; }

        /// <inheritdoc />
        [DataMember(Name = "curr_players"), Preserve]
        public int CurrentPlayers { set; get; }

        /// <inheritdoc />
        [DataMember(Name = "lb_master_id"), Preserve]
        public string LBID { set; get; }

        /// <inheritdoc />
        [DataMember(Name = "operator"), Preserve]
        public int Operator { set; get; }

        /// <inheritdoc />
        [DataMember(Name = "rank"), Preserve]
        public long Rank { set; get; }
    }
    #endregion

    #region PENDING_REWARD && CLAIM_REWARD

    public interface IPendingReward
    {
        IEnumerable<IRewardData> PendingRewardList { get; }
    }

    internal class ApiPendingReward : IPendingReward
    {
        /// <inheritdoc />
        [DataMember(Name = "unclaimed_records"), Preserve]
        public List<APIRewardData> _PendingData { set; get; }

        public IEnumerable<IRewardData> PendingRewardList => _PendingData ?? new List<APIRewardData>(0);
    }

    public interface IRewardData
    {
        string LeaderboardID { get; }

        string LeaderBoardName { get; }

        IEnumerable<IFullReward> FullRewardData { get; }
    }

    internal class APIRewardData : IRewardData
    {
        /// <inheritdoc />
        [DataMember(Name = "leaderboard_id"), Preserve]
        public string LeaderboardID { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "name"), Preserve]
        public string LeaderBoardName { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "rewards"), Preserve]
        public List<APIFullReward> _fullRewardData { get; set; }

        /// <inheritdoc />
        public IEnumerable<IFullReward> FullRewardData => _fullRewardData ?? new List<APIFullReward>(0);

        public override string ToString()
        {
            var output = "";
            output = string.Concat(output, "LeaderboardID: ", LeaderboardID, ", ");
            return output;
        }
    }

    public interface IFullReward
    {
        string MetaData { get; }

        IReward RewardData { get; }

        long score { get; }

        string PartitionKey { get; }

        string DisplayScore { get; }

        long Rank { get; }

        bool IsClaimed { get; }

        string CreateTime { get; }
    }

    internal class APIFullReward : IFullReward
    {
        /// <inheritdoc />
        [DataMember(Name = "metadata"), Preserve]
        public string MetaData { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "reward_info"), Preserve]
        public APIRewardDet _rewardData { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "score"), Preserve]
        public long score { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "partition_key"), Preserve]
        public string PartitionKey { get; set; }

        /// <inheritdoc/>
        [DataMember(Name = "display_score"), Preserve]
        public string DisplayScore { get; set; }

        /// <inheritdoc/>
        [DataMember(Name = "rank"), Preserve]
        public long Rank { get; set; }

        /// <inheritdoc/>
        [DataMember(Name = "is_claimed"), Preserve]
        public bool IsClaimed { get; set; }

        /// <inheritdoc/>
        [DataMember(Name = "create_time"), Preserve]
        public string CreateTime { get; set; }

        public IReward RewardData => _rewardData ?? new APIRewardDet();

        public override string ToString()
        {
            var output = "";
            output = string.Concat(output, "score: ", score, ", ");
            output = string.Concat(output, "PartitionKey: ", PartitionKey, ", ");
            output = string.Concat(output, "DisplayScore: ", DisplayScore, ", ");
            output = string.Concat(output, "Rank: ", Rank, ", ");
            output = string.Concat(output, "IsClaimed: ", IsClaimed, ", ");
            output = string.Concat(output, "CreateTime: ", CreateTime, ", ");
            return output;
        }
    }

    public interface IReward
    {
        IApiReward VirtualRewards { get; }

        IApiReward InGameRewards { get; }
    }

    internal class APIRewardDet : IReward
    {
        public IApiReward VirtualRewards => _virtualRewards ?? new ApiReward();
        /// <inheritdoc />
        [DataMember(Name = "virtual"), Preserve]
        public ApiReward _virtualRewards { get; set; }

        public IApiReward InGameRewards => _inGameRewards ?? new ApiReward();
        /// <inheritdoc />
        [DataMember(Name = "ingame"), Preserve]
        public ApiReward _inGameRewards { get; set; }
    }

    public interface IClaimReward
    {
        IRewardData ClaimRewardList { get; }
    }

    internal class ApiClaimReward : IClaimReward
    {
        /// <inheritdoc />
        [DataMember(Name = "claimed_rewards"), Preserve]
        public APIRewardData ClaimData { set; get; }

        public IRewardData ClaimRewardList => ClaimData ?? new APIRewardData();
    }

    #endregion
}

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using UnityEngine.Scripting;

namespace Jambox.Tourney.Data 
{
    /// <summary>
    /// Send a device to the server. Used with authenticate/link/unlink and user.
    /// </summary>
    public interface IApiAccountDevice
    {

        /// <summary>
        /// A device identifier. Should be obtained by a platform-specific device API.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Extra information that will be bundled in the session token.
        /// </summary>
        IDictionary<string, string> Vars { get; }
    }

    /// <inheritdoc />
    internal class ApiAccountDevice : IApiAccountDevice
    {

        /// <inheritdoc />
        [DataMember(Name = "id"), Preserve]
        public string Id { get; set; }

        /// <inheritdoc />
        public IDictionary<string, string> Vars => _vars ?? new Dictionary<string, string>();
        [DataMember(Name = "vars"), Preserve]
        public Dictionary<string, string> _vars { get; set; }

        public override string ToString()
        {
            var output = "";
            output = string.Concat(output, "Id: ", Id, ", ");

            var mapString = "";
            foreach (var kvp in Vars)
            {
                mapString = string.Concat(mapString, "{" + kvp.Key + "=" + kvp.Value + "}");
            }
            output = string.Concat(output, "Vars: [" + mapString + "]");
            return output;
        }
    }

    /// <summary>
    /// Authenticate against the server with a refresh token.
    /// </summary>
    public interface IApiSessionRefreshRequest
    {

        /// <summary>
        /// Refresh token.
        /// </summary>
        string Token { get; }

        /// <summary>
        /// Extra information that will be bundled in the session token.
        /// </summary>
        IDictionary<string, string> Vars { get; }
    }

    /// <inheritdoc />
    internal class ApiSessionRefreshRequest : IApiSessionRefreshRequest
    {

        /// <inheritdoc />
        [DataMember(Name = "token"), Preserve]
        public string Token { get; set; }

        /// <inheritdoc />
        public IDictionary<string, string> Vars => _vars ?? new Dictionary<string, string>();
        [DataMember(Name = "vars"), Preserve]
        public Dictionary<string, string> _vars { get; set; }

        public override string ToString()
        {
            var output = "";
            output = string.Concat(output, "Token: ", Token, ", ");

            var mapString = "";
            foreach (var kvp in Vars)
            {
                mapString = string.Concat(mapString, "{" + kvp.Key + "=" + kvp.Value + "}");
            }
            output = string.Concat(output, "Vars: [" + mapString + "]");
            return output;
        }
    }

    /// <summary>
    /// Log out a session, invalidate a refresh token, or log out all sessions/refresh tokens for a user.
    /// </summary>
    public interface IApiSessionLogoutRequest
    {

        /// <summary>
        /// Refresh token to invalidate.
        /// </summary>
        string RefreshToken { get; }

        /// <summary>
        /// Session token to log out.
        /// </summary>
        string Token { get; }
    }

    /// <inheritdoc />
    internal class ApiSessionLogoutRequest : IApiSessionLogoutRequest
    {

        /// <inheritdoc />
        [DataMember(Name = "refresh_token"), Preserve]
        public string RefreshToken { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "token"), Preserve]
        public string Token { get; set; }

        public override string ToString()
        {
            var output = "";
            output = string.Concat(output, "RefreshToken: ", RefreshToken, ", ");
            output = string.Concat(output, "Token: ", Token, ", ");
            return output;
        }
    }

    /// <summary>
    /// A user in the server.
    /// </summary>
    public interface IApiUser
    {

        /// <summary>
        /// The Apple Sign In ID in the user's account.
        /// </summary>
        string AppleId { get; }

        /// <summary>
        /// A URL for an avatar image.
        /// </summary>
        string AvatarUrl { get; }

        /// <summary>
        /// The UNIX time when the user was created.
        /// </summary>
        string CreateTime { get; }

        /// <summary>
        /// The display name of the user.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Number of related edges to this user.
        /// </summary>
        int EdgeCount { get; }

        /// <summary>
        /// The Facebook id in the user's account.
        /// </summary>
        string FacebookId { get; }

        /// <summary>
        /// The Facebook Instant Game ID in the user's account.
        /// </summary>
        string FacebookInstantGameId { get; }

        /// <summary>
        /// The Apple Game Center in of the user's account.
        /// </summary>
        string GamecenterId { get; }

        /// <summary>
        /// The Google id in the user's account.
        /// </summary>
        string GoogleId { get; }

        /// <summary>
        /// The id of the user's account.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The language expected to be a tag which follows the BCP-47 spec.
        /// </summary>
        string LangTag { get; }

        /// <summary>
        /// The location set by the user.
        /// </summary>
        string Location { get; }

        /// <summary>
        /// Additional information stored as a JSON object.
        /// </summary>
        string Metadata { get; }

        /// <summary>
        /// Indicates whether the user is currently online.
        /// </summary>
        bool Online { get; }

        /// <summary>
        /// The Steam id in the user's account.
        /// </summary>
        string SteamId { get; }

        /// <summary>
        /// The timezone set by the user.
        /// </summary>
        string Timezone { get; }

        /// <summary>
        /// The UNIX time when the user was last updated.
        /// </summary>
        string UpdateTime { get; }

        /// <summary>
        /// The username of the user's account.
        /// </summary>
        string Username { get; }
    }

    /// <inheritdoc />
    internal class ApiUser : IApiUser
    {

        /// <inheritdoc />
        [DataMember(Name = "apple_id"), Preserve]
        public string AppleId { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "avatar_url"), Preserve]
        public string AvatarUrl { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "create_time"), Preserve]
        public string CreateTime { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "display_name"), Preserve]
        public string DisplayName { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "edge_count"), Preserve]
        public int EdgeCount { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "facebook_id"), Preserve]
        public string FacebookId { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "facebook_instant_game_id"), Preserve]
        public string FacebookInstantGameId { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "gamecenter_id"), Preserve]
        public string GamecenterId { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "google_id"), Preserve]
        public string GoogleId { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "id"), Preserve]
        public string Id { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "lang_tag"), Preserve]
        public string LangTag { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "location"), Preserve]
        public string Location { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "metadata"), Preserve]
        public string Metadata { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "online"), Preserve]
        public bool Online { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "steam_id"), Preserve]
        public string SteamId { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "timezone"), Preserve]
        public string Timezone { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "update_time"), Preserve]
        public string UpdateTime { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "username"), Preserve]
        public string Username { get; set; }

        public override string ToString()
        {
            var output = "";
            output = string.Concat(output, "AppleId: ", AppleId, ", ");
            output = string.Concat(output, "AvatarUrl: ", AvatarUrl, ", ");
            output = string.Concat(output, "CreateTime: ", CreateTime, ", ");
            output = string.Concat(output, "DisplayName: ", DisplayName, ", ");
            output = string.Concat(output, "EdgeCount: ", EdgeCount, ", ");
            output = string.Concat(output, "FacebookId: ", FacebookId, ", ");
            output = string.Concat(output, "FacebookInstantGameId: ", FacebookInstantGameId, ", ");
            output = string.Concat(output, "GamecenterId: ", GamecenterId, ", ");
            output = string.Concat(output, "GoogleId: ", GoogleId, ", ");
            output = string.Concat(output, "Id: ", Id, ", ");
            output = string.Concat(output, "LangTag: ", LangTag, ", ");
            output = string.Concat(output, "Location: ", Location, ", ");
            output = string.Concat(output, "Metadata: ", Metadata, ", ");
            output = string.Concat(output, "Online: ", Online, ", ");
            output = string.Concat(output, "SteamId: ", SteamId, ", ");
            output = string.Concat(output, "Timezone: ", Timezone, ", ");
            output = string.Concat(output, "UpdateTime: ", UpdateTime, ", ");
            output = string.Concat(output, "Username: ", Username, ", ");
            return output;
        }
    }


    /// <summary>
    /// A user with additional account details. Always the current user.
    /// </summary>
    public interface IApiAccount
    {

        /// <summary>
        /// The custom id in the user's account.
        /// </summary>
        string CustomId { get; }

        /// <summary>
        /// The devices which belong to the user's account.
        /// </summary>
        IEnumerable<IApiAccountDevice> Devices { get; }

        /// <summary>
        /// The UNIX time when the user's account was disabled/banned.
        /// </summary>
        string DisableTime { get; }

        /// <summary>
        /// The email address of the user.
        /// </summary>
        string Email { get; }

        /// <summary>
        /// The user object.
        /// </summary>
        IApiUser User { get; }

        /// <summary>
        /// The UNIX time when the user's email was verified.
        /// </summary>
        string VerifyTime { get; }

        /// <summary>
        /// The user's wallet data.
        /// </summary>
        string Wallet { get; }
    }

    /// <inheritdoc />
    internal class ApiAccount : IApiAccount
    {

        /// <inheritdoc />
        [DataMember(Name = "custom_id"), Preserve]
        public string CustomId { get; set; }

        /// <inheritdoc />
        public IEnumerable<IApiAccountDevice> Devices => _devices ?? new List<ApiAccountDevice>(0);
        [DataMember(Name = "devices"), Preserve]
        public List<ApiAccountDevice> _devices { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "disable_time"), Preserve]
        public string DisableTime { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "email"), Preserve]
        public string Email { get; set; }

        /// <inheritdoc />
        public IApiUser User => _user;
        [DataMember(Name = "user"), Preserve]
        public ApiUser _user { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "verify_time"), Preserve]
        public string VerifyTime { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "wallet"), Preserve]
        public string Wallet { get; set; }

        public override string ToString()
        {
            var output = "";
            output = string.Concat(output, "CustomId: ", CustomId, ", ");
            output = string.Concat(output, "Devices: [", string.Join(", ", Devices), "], ");
            output = string.Concat(output, "DisableTime: ", DisableTime, ", ");
            output = string.Concat(output, "Email: ", Email, ", ");
            output = string.Concat(output, "User: ", User, ", ");
            output = string.Concat(output, "VerifyTime: ", VerifyTime, ", ");
            output = string.Concat(output, "Wallet: ", Wallet, ", ");
            return output;
        }
    }

    /// <summary>
    /// A user's session used to authenticate messages.
    /// </summary>
    //public interface IApiSession
    //{
    //    /// <summary>
    //    /// True if the corresponding account was just created, false otherwise.
    //    /// </summary>
    //    bool Created { get; }

    //    /// <summary>
    //    /// Refresh token that can be used for session token renewal.
    //    /// </summary>
    //    string RefreshToken { get; }

    //    /// <summary>
    //    /// The Id of the player who have sent this authentication
    //    /// </summary>
    //    string MyID { get; }

    //    /// <summary>
    //    /// The Avatar Image URL of the player who have sent this authentication
    //    /// </summary>
    //    string MyAvatar { get; }

    //    string AvatarType { get; }

    //    int AvatarIndex { get; }

    //    /// <summary>
    //    /// Authentication credentials.
    //    /// </summary>
    //    string Token { get; }

    //    /// <summary>
    //    /// User Name data.
    //    /// </summary>
    //    string Name { get; }
    //}

    ///// <inheritdoc />
    //internal class ApiSession : IApiSession
    //{
    //    /// <inheritdoc />
    //    [DataMember(Name = "created"), Preserve]
    //    public bool Created { get; set; }

    //    /// <inheritdoc />
    //    [DataMember(Name = "refresh_token"), Preserve]
    //    public string RefreshToken { get; set; }

    //    /// <inheritdoc />
    //    [DataMember(Name = "player_id"), Preserve]
    //    public string MyID { get; set; }

    //    /// <inheritdoc />
    //    [DataMember(Name = "avatar_url"), Preserve]
    //    public string MyAvatar { get; set; }

    //    /// <inheritdoc />
    //    [DataMember(Name = "avatar_group_id"), Preserve]
    //    public string AvatarType { get; set; }

    //    /// <inheritdoc />
    //    [DataMember(Name = "avatar_index_id"), Preserve]
    //    public int AvatarIndex { get; set; }

    //    /// <inheritdoc />
    //    [DataMember(Name = "token"), Preserve]
    //    public string Token { get; set; }

    //    /// <inheritdoc />
    //    [DataMember(Name = "userName"), Preserve]
    //    public string Name { get; set; }

    //    public override string ToString()
    //    {
    //        var output = "";
    //        output = string.Concat(output, "Created: ", Created, ", ");
    //        output = string.Concat(output, "RefreshToken: ", RefreshToken, ", ");
    //        output = string.Concat(output, "Token: ", Token, ", ");
    //        output = string.Concat(output, "MyID: ", MyID, ", ");
    //        output = string.Concat(output, "Name: ", Name, ", ");
    //        return output;
    //    }
    //}

    #region TourneyData

    #region GetTournamentDataAPI

    public interface IApiFullTourneyData
    {
        IApiTournamentData TourneyDetail { get; }

        IApiJoinedTourneyDetail JoinedData { get; }
    }

    internal class ApiFullTourneyData : IApiFullTourneyData
    {
        /// <inheritdoc />
        [DataMember(Name = "detail"), Preserve]
        public ApiTournamnetData _TourneyDetail { get; set; }

        public IApiTournamentData TourneyDetail => _TourneyDetail ?? new ApiTournamnetData();

        /// <inheritdoc />
        [DataMember(Name = "joined"), Preserve]
        public ApiJoinedTourneyDetail _JoinedData { get; set; }

        public IApiJoinedTourneyDetail JoinedData => _JoinedData ?? new ApiJoinedTourneyDetail();

    }

    /// <summary>
    /// Represents a complete tournament detail with player count and associated metadata.
    /// </summary>
    public interface IApiTournamentData
    {
        /// <summary>
        /// The Unique Id of every Tournament.
        /// </summary>
        string Tourneyid { get; }

        /// <summary>
        /// The Name of the Tournament
        /// </summary>
        string TourneyName { get; }

        /// <summary>
        /// The Count of Players which can join the tournament.
        /// </summary>
        int MaxPlayers { get; }

        /// <summary>
        /// The entry fee of joining this tournament.
        /// </summary>
        int EntryFee { get; }

        /// <summary>
        /// The Currency of entry Fee.
        /// </summary>
        string Currency { get; }

        /// <summary>
        /// The UNIX time when the Tournament Will Start.
        /// </summary>
        string StartTime { get; }

        /// <summary>
        /// The UNIX time when the Tournament will end.
        /// </summary>
        string EndTime { get; }

        /// <summary>
        /// The Status of tournament.
        /// </summary>
        string Status { get; }

        /// <summary>
        /// No. of attempts an user is allowed to improve his score.
        /// </summary>
        int MaxEntry { get; }

        /// <summary>
        /// Description of tournament.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Total Players Joined the tournament.
        /// </summary>
        int CurrentPlayers { get; }

        /// <summary>
        /// Category of Tournament will be defined here.
        /// </summary>
        int Category { get; }

        bool PlayWithAd { get; }

        /// <summary>
        /// MetaData is the data send by client, it will be returned by server as it is.
        /// </summary>
        //Dictionary<String, String> MetaData { get; }
        String MetaData { get; }

        /// <summary>
        /// 
        /// </summary>
        IApiTournamentReward RewardList { get; }
    }

    /// <inheritdoc />
    internal class ApiTournamnetData : IApiTournamentData
    {
        /// <inheritdoc />
        [DataMember(Name = "tourney_id"), Preserve]
        public string Tourneyid { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "name"), Preserve]
        public string TourneyName { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "max_players"), Preserve]
        public int MaxPlayers { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "entry_fee"), Preserve]
        public int EntryFee { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "currency"), Preserve]
        public string Currency { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "start_time"), Preserve]
        public string StartTime { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "end_time"), Preserve]
        public string EndTime { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "status"), Preserve]
        public string Status { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "max_entry_count"), Preserve]
        public int MaxEntry { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "description"), Preserve]
        public string Description { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "current_players"), Preserve]
        public int CurrentPlayers { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "category"), Preserve]
        public int Category { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "play_with_ads"), Preserve]
        public bool PlayWithAd { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "metadata"), Preserve]
        public String MetaData { get; set; }
        //public Dictionary<String, String> MetaData { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "rewardlist"), Preserve]
        public ApiTournamentReward _rewardList { get; set; }

        public IApiTournamentReward RewardList => _rewardList ?? new ApiTournamentReward();

        public override string ToString()
        {
            var output = "";
            output = string.Concat(output, "Tourneyid : ", Tourneyid, ", ");
            output = string.Concat(output, "TourneyName : ", TourneyName, ", ");
            output = string.Concat(output, "MaxPlayers : ", MaxPlayers, ", ");
            output = string.Concat(output, "RewardText : ", MaxEntry, ", ");
            output = string.Concat(output, "Currency : ", Currency, ", ");
            output = string.Concat(output, "StartTime : ", StartTime, ", ");
            output = string.Concat(output, "EndTime : ", EndTime, ", ");
            output = string.Concat(output, "Status : ", Status, ", ");
            output = string.Concat(output, "MaxEntry : ", EntryFee, ", ");
            output = string.Concat(output, "Description : ", Description, ", ");
            output = string.Concat(output, "CurrentPlayers : ", CurrentPlayers, ", ");
            output = string.Concat(output, "Category : ", Category, ", ");
            output = string.Concat(output, "MetaData : ", MetaData, ", ");
            return output;
        }
    }

    public interface IApiTournamentReward
    {
        IEnumerable<IApiRewardDistribution> RewardsDistribution { get; }
    }

    internal class ApiTournamentReward : IApiTournamentReward
    {
        /// <inheritdoc />
        public IEnumerable<IApiRewardDistribution> RewardsDistribution => _rewardsDistribution ?? new List<ApiRewardDistribution>(0);
        [DataMember(Name = "distribution"), Preserve]
        public List<ApiRewardDistribution> _rewardsDistribution { get; set; }
    }

    #region REWARD_API

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

        public IApiReward VirtualRewards => _virtualRewards ?? new ApiReward();
        /// <inheritdoc />
        [DataMember(Name = "virtual"), Preserve]
        public ApiReward _virtualRewards { get; set; }

        public IApiReward InGameRewards => _inGameRewards ?? new ApiReward();
        /// <inheritdoc />
        [DataMember(Name = "ingame"), Preserve]
        public ApiReward _inGameRewards { get; set; }

    }

    public interface IApiRewardObject
    {

        IApiReward Virtual { get; }

        IApiReward InGame { get; }

    }

    internal class ApiRewardObject : IApiRewardObject
    {

        public IApiReward Virtual => _virtual ?? new ApiReward();
        /// <inheritdoc />
        [DataMember(Name = "virtual"), Preserve]
        public ApiReward _virtual { get; set; }

        public IApiReward InGame => _ingame ?? new ApiReward();
        /// <inheritdoc />
        [DataMember(Name = "ingame"), Preserve]
        public ApiReward _ingame { get; set; }

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

    #endregion

    //public interface IApiReward
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    int StartRank { get; }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    int EndRank { get; }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    int Amount { get; }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    string Currency { get; }
    //}

    //internal class ApiReward : IApiReward
    //{
    //    /// <inheritdoc />
    //    [DataMember(Name = "start_rank"), Preserve]
    //    public int StartRank { get; set; }
    //    /// <inheritdoc />
    //    [DataMember(Name = "end_rank"), Preserve]
    //    public int EndRank { get; set; }
    //    /// <inheritdoc />
    //    [DataMember(Name = "amount"), Preserve]
    //    public int Amount { get; set; }
    //    /// <inheritdoc />
    //    [DataMember(Name = "currency"), Preserve]
    //    public string Currency { get; set; }
    //}

    public interface IApiJoinedTourneyDetail
    {
        /// <summary>
        /// The Unique Id of every Tournament.
        /// </summary>
        string Tourneyid { set;  get; }

        /// <summary>
        /// My Current rank in particular Joined tournamemnt.
        /// </summary>
        int MyRank { set; get; }

        /// <summary>
        /// Total Players Joined the tournament.
        /// </summary>
        int CurrentPlayers { set; get; }

        /// <summary>
        /// User's Best score in all the submissions.
        /// </summary>
        int Score { set; get; }

        /// <summary>
        /// Subscores is basically based on Skills of theuser.
        /// </summary>
        int SubScore { set; get; }

        /// <summary>
        /// Total no. of attempts user has done in particular tournament.
        /// </summary>
        int CurrentAttempt { get; }

        /// <summary>
        /// The LeaderBoardId to get leaderboard of that particular tournament.
        /// </summary>
        string LeaderBoardID { get; }

        String JoinCode { get; }

    }

    /// <inheritdoc />
    internal class ApiJoinedTourneyDetail : IApiJoinedTourneyDetail
    {
        /// <inheritdoc />
        [DataMember(Name = "tourney_id"), Preserve]
        public string Tourneyid { set; get; }

        /// <inheritdoc />
        [DataMember(Name = "rank"), Preserve]
        public int MyRank { set; get; }

        /// <inheritdoc />
        [DataMember(Name = "curr_players"), Preserve]
        public int CurrentPlayers { set; get; }

        /// <inheritdoc />
        [DataMember(Name = "score"), Preserve]
        public int Score { set; get; }

        /// <inheritdoc />
        [DataMember(Name = "subscore"), Preserve]
        public int SubScore { set; get; }

        /// <inheritdoc />
        [DataMember(Name = "curr_entry_count"), Preserve]
        public int CurrentAttempt { set; get; }

        /// <inheritdoc />
        [DataMember(Name = "leaderboard_id"), Preserve]
        public string LeaderBoardID { set; get; }

        /// <inheritdoc />
        [DataMember(Name = "join_code"), Preserve]
        public string JoinCode { set; get; }
    }

    /// <summary>
    /// A list of tournaments.
    /// </summary>
    public interface IApiTourneyList
    {
        /// <summary>
        /// The list of tournaments returned.
        /// </summary>
        IEnumerable<IApiFullTourneyData> Tournaments { get; }

        /// <summary>
        /// The list of tournaments returned.
        /// </summary>
        IEnumerable<IApiTournamentData> Duels { get; }

        /// <summary>
        /// The List of private tournaments either created or joined by user.
        /// </summary>
        IEnumerable<IApiFullTourneyData> PrivateTourney { get; }

    }

    /// <inheritdoc />
    internal class ApiTourneyList : IApiTourneyList
    {
        /// <inheritdoc />
        public IEnumerable<IApiFullTourneyData> Tournaments => _tournaments ?? new List<ApiFullTourneyData>(0);

        /// <inheritdoc />
        public IEnumerable<IApiTournamentData> Duels => _duels ?? new List<ApiTournamnetData>(0);

        /// <inheritdoc />
        public IEnumerable<IApiFullTourneyData> PrivateTourney => _privateList ?? new List<ApiFullTourneyData>(0);

        /// <inheritdoc />
        [DataMember(Name = "tourneylist"), Preserve]
        public List<ApiFullTourneyData> _tournaments { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "duellist"), Preserve]
        public List<ApiTournamnetData> _duels { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "privatelist"), Preserve]
        public List<ApiFullTourneyData> _privateList { get; set; }

        public override string ToString()
        {
            var output = "";
            output = string.Concat(output, "Tournaments: [", string.Join(", ", Tournaments), "], ");
            output = string.Concat(output, "Duels: [", string.Join(", ", Duels), "], ");
            output = string.Concat(output, "Private Tourney : [", string.Join(", ", PrivateTourney), "], ");
            return output;
        }
    }

    #endregion

    #region CompletedTournamentAPI

    public interface IAPICompletedTourney
    {
        String TourneyID { get; }

        String TourneyName { get; }

        String LeaderBoardID { get; }

        Int32 Rank { get; }

        Int32 JoinedPlayer { get; }

        Int32 Score { get; }

        Int32 SubScore { get; }

        Int32 RewardAmnt { get; }

        String RewardCurrency { get; }

        IApiRewardObject RewardInfo { get; }

        Boolean isClaimed { get; }

        String EndTime { get; }

        IApiTournamentReward RewardList { get; }
    }

    internal class APICompletedTourney : IAPICompletedTourney
    {
        /// <inheritdoc />
        [DataMember(Name = "tourney_id"), Preserve]
        public string TourneyID { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "tourney_name"), Preserve]
        public string TourneyName { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "leaderboard_id"), Preserve]
        public string LeaderBoardID { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "rank"), Preserve]
        public int Rank { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "joined_player_count"), Preserve]
        public int JoinedPlayer { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "score"), Preserve]
        public int Score { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "subscore"), Preserve]
        public int SubScore { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "value"), Preserve]
        public int RewardAmnt { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "key"), Preserve]
        public String RewardCurrency { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "reward_info"), Preserve]
        public ApiRewardObject _rewardInfo { get; set; }
        public IApiRewardObject RewardInfo => _rewardInfo ?? new ApiRewardObject();

        /// <inheritdoc />
        [DataMember(Name = "is_claimed"), Preserve]
        public bool isClaimed { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "end_time"), Preserve]
        public string EndTime { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "rewardlist"), Preserve]
        public ApiTournamentReward _rewardList { get; set; }

        public IApiTournamentReward RewardList => _rewardList ?? new ApiTournamentReward();
    }

    public interface IAPICompTourneyList
    {
        IEnumerable<IAPICompletedTourney> CompTournaments { get; }
    }

    internal class APICompTourneyList : IAPICompTourneyList
    {
        /// <inheritdoc />
        public IEnumerable<IAPICompletedTourney> CompTournaments => _tournaments ?? new List<APICompletedTourney>(0);

        /// <inheritdoc />
        [DataMember(Name = "tourneylist"), Preserve]
        public List<APICompletedTourney> _tournaments { get; set; }
    }

    #endregion

    #region JoinTournamentAPI

    public interface IAPIJoinTourney
    {
        /// <summary>
        /// The TourneyId In Which User Wants to join
        /// </summary>
        String TourneyId { get;  }

        /// <summary>
        /// The Newly Generated Leaderboard Id through which the user can directly ask for Specific LeaderBoard. 
        /// </summary>
        String LeaderBoardID { get; }

        /// <summary>
        /// My Rank After Joining the tournament. this will be changing over the time.
        /// </summary>
        String MyRank { get; }

        /// <summary>
        /// Total No. of players available in this tournament.
        /// </summary>
        String TotalPlayer { get; }

    }

    internal class APIJoinTourney : IAPIJoinTourney
    {
        /// <inheritdoc />
        [DataMember(Name = "tourney_id"), Preserve]
        public string TourneyId { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "lb_id"), Preserve]
        public string LeaderBoardID { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "my_rank"), Preserve]
        public string MyRank { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "totol_player"), Preserve]
        public string TotalPlayer { get; set; }
    }

    #endregion


    #region JoinDuelAPI

    /// <summary>
    /// IApiDuelOpponenet
    /// </summary>
    public interface IApiDuelOpponenet
    {
        /// <summary>
        /// The Player ID of the opponent
        /// </summary>
        string PlayerId { get; }

        /// <summary>
        /// The Name of the opponent
        /// </summary>
        string PlayerName { get; }

        /// <summary>
        /// The Player ID of the opponent
        /// </summary>
        string PlayerURL { get; }

        /// <summary>
        /// The Name of the opponent
        /// </summary>
        string Metadata { get; }

        IAPIReplayData ReplayData { get; }
    }

    /// <inheritdoc />
    internal class ApiDuelOpponenet : IApiDuelOpponenet
    {
        /// <inheritdoc />
        [DataMember(Name = "player_id"), Preserve]
        public string PlayerId { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "player_name"), Preserve]
        public string PlayerName { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "avatar_url"), Preserve]
        public string PlayerURL { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "metadata"), Preserve]
        public string Metadata { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "replay_data"), Preserve]
        public APIReplayData _replayData { get; set; }

        public IAPIReplayData ReplayData => _replayData ?? new APIReplayData();

        public override string ToString()
        {
            var output = "";
            output = string.Concat(output, "OwnerId: ", PlayerId, ", ");
            output = string.Concat(output, "PlayerURL: ", PlayerURL, ", ");            
            output = string.Concat(output, "Username: ", PlayerName, ", ");
            output = string.Concat(output, "Metadata: ", Metadata, ", ");
            return output;
        }
    }

    public interface IAPIJoinDuel
    {
        ///// <summary>
        ///// The DuelId In Which User Wants to join
        ///// </summary>
        //String DuelId { get; }

        /// <summary>
        /// The Newly Generated Leaderboard Id through which the user can directly ask for Specific LeaderBoard. 
        /// </summary>
        String LeaderBoardID { get; }

        /// <summary>
        /// Opponenet data with which Duel is matchmaked
        /// </summary>
        IApiDuelOpponenet OpponenetData { get; }

    }

    internal class APIJoinDuel : IAPIJoinDuel
    {
        ///// <inheritdoc />
        //[DataMember(Name = "duel_id"), Preserve]
        //public string DuelId { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "leaderboard_id"), Preserve]
        public string LeaderBoardID { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "opponent"), Preserve]
        public ApiDuelOpponenet duelOpponenet { get; set; }

        public IApiDuelOpponenet OpponenetData => duelOpponenet ?? new ApiDuelOpponenet();

    }

    #endregion

    #region SubmitScore

    public interface IApiSubmitScore
    {
        /// <summary>
        /// The Score user has scored in current game Completed
        /// </summary>
        int CurrentScore { get; }

        /// <summary>
        /// Best Score By the user in this current tournament
        /// </summary>
        int BestScore { get; }

        /// <summary>
        /// Rank in this particular Tournament.
        /// </summary>
        int MyRank { get; }

        /// <summary>
        /// Total Player playing this tournament in User's league.
        /// </summary>
        int CurrentPlayer { get; }

        /// <summary>
        /// The Uniqu Id of LeaderBoard that will fetch LeaderBoard of any tournament.
        /// </summary>
        string LeaderBoardID { get; }

        /// <summary>
        /// 
        /// </summary>
        int EntryCount { get; }

        IAPIIntervalData replayData { get; }
    }

    internal class ApiSubmitScore : IApiSubmitScore
    {
        /// <inheritdoc />
        [DataMember(Name = "curr_score"), Preserve]
        public int CurrentScore { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "best_score"), Preserve]
        public int BestScore { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "rank"), Preserve]
        public int MyRank { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "curr_players"), Preserve]
        public int CurrentPlayer { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "leaderboard_id"), Preserve]
        public string LeaderBoardID { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "entry_count"), Preserve]
        public int EntryCount { get; set; }

        // <inheritdoc />
        [DataMember(Name = "replay_data"), Preserve]
        public APIIntervalData _replayData { get; set; }
        public IAPIIntervalData replayData => _replayData ?? new APIIntervalData();
    }

    #endregion

    #region SubmitDuelScore

    public interface IAPIDuelResult
    {
        string resultDisplay { get; }

        IApiRewardObject rewardList { get; }

        int Rank { get; }      
    }

    internal class APIDuelResult : IAPIDuelResult
    {
        [DataMember(Name = "result"), Preserve]
        public string resultDisplay { get; set; }

        // <inheritdoc />
        [DataMember(Name = "reward_info"), Preserve]
        public ApiRewardObject _rewardList { get; set; }
        public IApiRewardObject rewardList => _rewardList ?? new ApiRewardObject();

        [DataMember(Name = "rank"), Preserve]
        public int Rank { get; set; }
        
        /*
        public override string ToString()
        {
            var output = "";
            output = string.Concat(output, " resultDisplay : ", resultDisplay, ", ");
            output = string.Concat(output, " wonAmount : ", wonAmount, ", ");
            output = string.Concat(output, " WonCurrency : ", WonCurrency, ", ");
            output = string.Concat(output, " Rank : ", Rank, ", ");
            return output;
        }*/
    }
    /// <summary>
    /// A set of leaderboard records, may be part of a leaderboard records page or a batch of individual records.
    /// </summary>
    public interface IApiSubmitDuelScore
    {
        /// <summary>
        /// A batched set of leaderboard records belonging to specified owners.
        /// </summary>
        IEnumerable<IApiLeaderRecord> LeaderRecords { get; }

        /// <summary>
        /// The result of the duel
        /// </summary>
        IAPIDuelResult result { get; }

        int delayInResultDisplay { get; }
    }

    /// <inheritdoc />
    internal class ApiSubmitDuelScore : IApiSubmitDuelScore
    {
        /// <inheritdoc />
        public IEnumerable<IApiLeaderRecord> LeaderRecords => _leaderboardRecords ?? new List<ApiLeaderRecord>(0);
        [DataMember(Name = "leaderboard"), Preserve]
        public List<ApiLeaderRecord> _leaderboardRecords { get; set; }

        [DataMember(Name = "delay"), Preserve]
        public int delayInResultDisplay { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "game_result"), Preserve]
        public APIDuelResult resultData { get; set; }

        public IAPIDuelResult result => resultData ?? new APIDuelResult();

        public override string ToString()
        {
            var output = "";
            //output = string.Concat(output, "result: ", result, ", ");
            output = string.Concat(output, "LeaderRecords: [", string.Join(", ", LeaderRecords), "], ");
            return output;
        }
    }

    #endregion

    #region ReplayAPI

    public interface IAPIReplayData
    {
        IDictionary<string, string> GameData { get; }

        IEnumerable<IAPIIntervalData> IntervalData { get; }
    }

    internal class APIReplayData : IAPIReplayData
    {

        /// <inheritdoc />
        public IDictionary<string, string> GameData => _gameData ?? new Dictionary<string, string>();
        [DataMember(Name = "game_data"), Preserve]
        public Dictionary<string, string> _gameData { get; set; }

        /// <inheritdoc />
        public IEnumerable<IAPIIntervalData> IntervalData => _intervalData ?? new List<APIIntervalData>(0);
        [DataMember(Name = "interval_data"), Preserve]
        public List<APIIntervalData> _intervalData { get; set; }

    }

    /*
    public interface IAPIGameData
    {
        string level { get; }

        string car { get; }

        string difficulty { get; }
    }

    internal class APIGameData : IAPIGameData
    {
        [DataMember(Name = "level"), Preserve]
        public string level { get; set; }

        [DataMember(Name = "car"), Preserve]
        public string car { get; set; }

        [DataMember(Name = "difficulty"), Preserve]
        public string difficulty { get; set; }

    }*/

    public interface IAPIIntervalData
    {
        string interval { get; }

        string data { get; }
    }

    internal class APIIntervalData : IAPIIntervalData
    {
        [DataMember(Name = "i"), Preserve]
        public string interval { get; set; }

        [DataMember(Name = "d"), Preserve]
        public string data { get; set; }

    }

    #endregion


    #region PlayTournament

    public interface IApiPlayTourney
    {
        /// <summary>
        /// The id of the tournament User going to play.
        /// </summary>
        string LeaderBoardID { get; }

        /// <summary>
        /// The type of attempt it was by the user, it can be free, paid, videoAd.
        /// </summary>
        string AttemptType { get; }

        /// <summary>
        /// The number of free attempt played by user.
        /// </summary>
        string AttemptDone { get; }

        /// <summary>
        /// The total number of free ateempts available.
        /// </summary>
        string TotalAttempt { get; }

        IEnumerable<IApiLeaderRecord> LeaderRecords { get; }

        string Error { get; }
    }

    internal class ApiPlayTourney : IApiPlayTourney
    {
        /// <inheritdoc />
        [DataMember(Name = "leaderboard_id"), Preserve]
        public string LeaderBoardID { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "attempt_type"), Preserve]
        public string AttemptType { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "attempt_done"), Preserve]
        public string AttemptDone { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "total_attempt"), Preserve]
        public string TotalAttempt { get; set; }

        public IEnumerable<IApiLeaderRecord> LeaderRecords => _leaderboardRecords ?? new List<ApiLeaderRecord>(0);
        [DataMember(Name = "leaderboard"), Preserve]
        public List<ApiLeaderRecord> _leaderboardRecords { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "error"), Preserve]
        public string Error { get; set; }
    }
    #endregion

    #endregion
    
    #region LeaderboardData

    /// <summary>
    /// Represents a complete leaderboard record with all scores and associated metadata.
    /// </summary>
    public interface IApiLeaderRecord
    {
        /// <summary>
        /// The number of score updates owner has availed.
        /// </summary>
        int AttemptCount { get; }

        /// <summary>
        /// The ID of the score owner, usually a user.
        /// </summary>
        string PlayerId { get; }

        /// <summary>
        /// The rank of this record.
        /// </summary>
        int Rank { get; }

        /// <summary>
        /// The score value.
        /// </summary>
        int Score { get; }

        /// <summary>
        /// The score which will be displayed in leaderboard.
        /// </summary>
        string DisplayScore { get; }

        /// <summary>
        /// The username of the score owner, if the owner is a user.
        /// </summary>
        string Username { get; }

        /// <summary>
        /// The Avatar url of the score owner, if the owner is a user.
        /// </summary>
        string AvatarUrl { get; }

        IApiRewardObject rewardList { get; }

    }

    /// <inheritdoc />
    internal class ApiLeaderRecord : IApiLeaderRecord
    {
        /// <inheritdoc />
        [DataMember(Name = "score_entry_count"), Preserve]
        public int AttemptCount { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "player_id"), Preserve]
        public string PlayerId { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "rank"), Preserve]
        public int Rank { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "score"), Preserve]
        public int Score { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "display_score"), Preserve]
        public string DisplayScore { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "player_name"), Preserve]
        public string Username { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "plauer_url"), Preserve]
        public string AvatarUrl { get; set; }

        // <inheritdoc />
        [DataMember(Name = "reward_info"), Preserve]
        public ApiRewardObject _rewardList { get; set; }
        public IApiRewardObject rewardList => _rewardList ?? new ApiRewardObject();


        public override string ToString()
        {
            var output = "";
            output = string.Concat(output, "AttemptCount: ", AttemptCount, ", ");
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
        [DataMember(Name = "headers"), Preserve]
        public APILeaderBoardheaders LBHeaders { get; set; }

        public IAPILeaderBoardheaders LeaderBoardHeaders => LBHeaders ?? new APILeaderBoardheaders();

        /// <inheritdoc />
        public IEnumerable<IApiLeaderRecord> LeaderRecords => _leaderboardRecords ?? new List<ApiLeaderRecord>(0);
        [DataMember(Name = "leaderboard"), Preserve]
        public List<ApiLeaderRecord> _leaderboardRecords { get; set; }

        public override string ToString()
        {
            var output = "";
            output = string.Concat(output, "refreshTime: ", refreshTime, ", ");
            output = string.Concat(output, "leaderboardId: ", leaderboardId, ", ");
            output = string.Concat(output, "OwnerRecords: [", string.Join(", ", LeaderRecords), "], ");
            return output;
        }
    }

    #endregion

    #region CLAIM_TOURNAMENT

    public interface IAPIClaimData
    {
        IApiRewardObject RewardInfo { get; }
    }

    internal class APIClaimData : IAPIClaimData
    {
        // <inheritdoc />
        [DataMember(Name = "reward_info"), Preserve]
        public ApiRewardObject _rewardInfo { get; set; }
        public IApiRewardObject RewardInfo => _rewardInfo ?? new ApiRewardObject();
    }
    #endregion

    #region CREATE_FRIENDLY
    public interface IAPICreateFriendly
    {
        /// <summary>
        /// The Unique Id of every Tournament.
        /// </summary>
        string Tourneyid { get; }

        /// <summary>
        /// The Name of the Tournament
        /// </summary>
        string TourneyName { get; }

        /// <summary>
        /// The Count of Players which can join the tournament.
        /// </summary>
        int MaxPlayers { get; }

        /// <summary>
        /// The entry fee of joining this tournament.
        /// </summary>
        int EntryFee { get; }

        /// <summary>
        /// The Currency of entry Fee.
        /// </summary>
        string Currency { get; }

        /// <summary>
        /// The UNIX time when the Tournament Will Start.
        /// </summary>
        string StartTime { get; }

        /// <summary>
        /// The UNIX time when the Tournament will end.
        /// </summary>
        string EndTime { get; }

        /// <summary>
        /// The Status of tournament.
        /// </summary>
        string Status { get; }

        /// <summary>
        /// No. of attempts an user is allowed to improve his score.
        /// </summary>
        int MaxEntry { get; }

        /// <summary>
        /// Description of tournament.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Total Players Joined the tournament.
        /// </summary>
        int CurrentPlayers { get; }

        /// <summary>
        /// Category of Tournament will be defined here.
        /// </summary>
        int Category { get; }

        bool PlayWithAd { get; }

        /// <summary>
        /// MetaData is the data send by client, it will be returned by server as it is.
        /// </summary>
        string MetaData { get; }

        /// <summary>
        /// 
        /// </summary>
        IEnumerable<IApiReward> Rewards { get; }
    }

    internal class APICreateFriendly : IAPICreateFriendly
    {
        /// <inheritdoc />
        [DataMember(Name = "tourney_id"), Preserve]
        public string Tourneyid { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "name"), Preserve]
        public string TourneyName { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "max_players"), Preserve]
        public int MaxPlayers { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "entry_fee"), Preserve]
        public int EntryFee { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "currency"), Preserve]
        public string Currency { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "start_time"), Preserve]
        public string StartTime { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "end_time"), Preserve]
        public string EndTime { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "status"), Preserve]
        public string Status { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "max_entry_count"), Preserve]
        public int MaxEntry { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "description"), Preserve]
        public string Description { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "current_players"), Preserve]
        public int CurrentPlayers { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "category"), Preserve]
        public int Category { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "play_with_ads"), Preserve]
        public bool PlayWithAd { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "metadata"), Preserve]
        public string MetaData { get; set; }

        /// <inheritdoc />
        public IEnumerable<IApiReward> Rewards => _rewards ?? new List<ApiReward>(0);

        /// <inheritdoc />
        [DataMember(Name = "rewardlist"), Preserve]
        public List<ApiReward> _rewards { get; set; }
    }
    #endregion

    #region JOIN_FRIENDLY
    public interface IAPIJoinFriendly
    {
        IEnumerable<IAPIFriendlyTournamentData> friendlyTournamentDetails { get; }
    }

    internal class APIJoinFriendly : IAPIJoinFriendly
    {
        public IEnumerable<IAPIFriendlyTournamentData> friendlyTournamentDetails => _friendlyTournamentDetails ?? new List<APIFriendlyTournamentData>(0);
        [DataMember(Name = "leaderboard"), Preserve]
        public List<APIFriendlyTournamentData> _friendlyTournamentDetails { get; set; }
    }
    #endregion

    #region FRIENDLY_TOURNEY_LIST
    public interface IAPIFriendlyTourneyList
    {
        IEnumerable<IAPIFriendlyTournamentData> friendlyTournaments { get; }
    }

    internal class APIFriendlyTourneyList : IAPIFriendlyTourneyList
    {
        public IEnumerable<IAPIFriendlyTournamentData> friendlyTournaments => _friendlyTournaments ?? new List<APIFriendlyTournamentData>(0);
        [DataMember(Name = "friendlyTournaments"), Preserve]
        public List<APIFriendlyTournamentData> _friendlyTournaments { get; set; }
    }
    #endregion

    #region FRIENDLY_TOURANAMENT_DATA
    public interface IAPIFriendlyTournamentData
    {

        string TournamentName { get; }

        string Attempts { get; }

        string EndTime { get; }
    }

    internal class APIFriendlyTournamentData : IAPIFriendlyTournamentData
    {
        /// <inheritdoc />
        [DataMember(Name = "reward_amt"), Preserve]
        public string TournamentName { set; get; }

        /// <inheritdoc />
        [DataMember(Name = "reward_currency"), Preserve]
        public string Attempts { set; get; }

        /// <inheritdoc />
        [DataMember(Name = "reward_currency"), Preserve]
        public string EndTime { set; get; }
    }

    public interface IApiPlayFriendlyTourney
    {
        /// <summary>
        /// The id of the tournament User going to play.
        /// </summary>
        string LeaderBoardID { get; }

        /// <summary>
        /// The number of free attempt played by user.
        /// </summary>
        string AttemptDone { get; }

        /// <summary>
        /// The total number of free ateempts available.
        /// </summary>
        string TotalAttempt { get; }

        string Error { get; }
    }

    internal class ApiPlayFriendlyTourney : IApiPlayFriendlyTourney
    {
        /// <inheritdoc />
        [DataMember(Name = "leaderboard_id"), Preserve]
        public string LeaderBoardID { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "attempt_done"), Preserve]
        public string AttemptDone { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "total_attempt"), Preserve]
        public string TotalAttempt { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "error"), Preserve]
        public string Error { get; set; }
    }
    #endregion

    #region CURRENCY_DETAILS
    public interface IAPICurrencyList
    {
        IEnumerable<IAPICurrencyData> CurrencyList { get; }
    }

    internal class APICurrencyList : IAPICurrencyList
    {
        public IEnumerable<IAPICurrencyData> CurrencyList => _currencyList ?? new List<APICurrencyData>(0);
        [DataMember(Name = "currencylist"), Preserve]
        public List<APICurrencyData> _currencyList { get; set; }
    }

    public interface IAPICurrencyData
    {

        string CurrentName { get; }

        string CurrentKey { get; }

    }

    internal class APICurrencyData : IAPICurrencyData
    {
        /// <inheritdoc />
        [DataMember(Name = "name"), Preserve]
        public string CurrentName { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "key"), Preserve]
        public string CurrentKey { get; set; }
    }

    #endregion

    public interface IAPIUnclaimedRewards 
    {

        IEnumerable<IAPIUnclaimedRewardObject> UnclaimedRewards { get; }
    }

    internal class APIUnclaimedRewards : IAPIUnclaimedRewards
    {
        /// <inheritdoc />
        public IEnumerable<IAPIUnclaimedRewardObject> UnclaimedRewards => _unclaimedRewards ?? new List<APIUnclaimedRewardObject>(0);
        /// <inheritdoc />
        [DataMember(Name = "unclaimed"), Preserve]
        public List<APIUnclaimedRewardObject> _unclaimedRewards { get; set; }
    }

    public interface IAPIUnclaimedRewardObject
    {

        String TourneyName { get; }

        String LeaderBoardID { get; }

        String EndTime { get; }

        IApiRewardObject RewardInfo { get; }
    }

    internal class APIUnclaimedRewardObject : IAPIUnclaimedRewardObject
    {

        /// <inheritdoc />
        [DataMember(Name = "name"), Preserve]
        public string TourneyName { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "leaderboard_id"), Preserve]
        public string LeaderBoardID { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "end_time"), Preserve]
        public string EndTime { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "reward_info"), Preserve]
        public ApiRewardObject _rewardInfo { get; set; }
        public IApiRewardObject RewardInfo => _rewardInfo ?? new ApiRewardObject();
    }

}

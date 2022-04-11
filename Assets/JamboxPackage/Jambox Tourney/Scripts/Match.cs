using System;
using System.Collections.Generic;
using Jambox.Tourney.Connector;
using Jambox.Tourney.Data;
using Jambox.Tourney.UI;
using UnityEngine;

namespace Jambox
{
    public enum EMatchType {
        EMatchTypeTourney = 1,
        EMatchTypeDuel = 2,
        EMatchTypeFriendly = 3,
        NONE = 4
    }

    public class Match
    {
        /// <summary>
        /// Match Identifier. To Check MatchType and enable corresponding Match UI. Use this variable.
        /// </summary>
        public readonly EMatchType matchType;

        public readonly string tournamentID;
        public readonly string matchID;
        public readonly Dictionary<string, string> metadata;

        /// <summary>
        /// This is competitive Leaderboard Which can be shown on GamePlayUI.
        /// To show the user real time rank update in particular tournament.
        /// </summary>
        public readonly List<leaderBoardData> Leaderboard;

        /// <summary>
        /// Following these two variables are Current user's name
        /// and their Avatar ID URL as per our Tourney Screen.
        /// </summary>
        public readonly string UserImage;
        public readonly string UserName;
        public readonly Sprite UserAvatarSprite;

        /// <summary>
        /// Following these two variables are used in case of Duel match.
        /// Here OpponentImage will contain URL of opponent Image.
        /// while OpponentName is name of opponent.
        /// </summary>
        public readonly string OpponentImage;
        public readonly string OpponentName;
        public readonly Sprite OpponentAvatarSprite;

        /// <summary>
        /// Opponent Replay data to be shown in case of Showing opponent play data in duel play UI.
        /// </summary>
        public IAPIReplayData replayData;

        public Match(string _tourneyID, string _matchID, Dictionary<string, string> _metadata, int _category,
            IAPIReplayData _duelReplayData = null, List<leaderBoardData> dataLB = null, string _userImage = "",
            string _userName = "", string OppImage = "", string OppName = "", Sprite userAvatarSprite = null, Sprite oppAvatarSprite = null)
        {
            matchID = _matchID;
            tournamentID = _tourneyID;
            metadata = _metadata;
            matchType = (EMatchType)_category;
            UserImage = _userImage;
            UserName = _userName;
            OpponentImage = OppImage;
            OpponentName = OppName;
            replayData = _duelReplayData;
            Leaderboard = dataLB;
            UserAvatarSprite = userAvatarSprite;
            OpponentAvatarSprite = oppAvatarSprite;
        } 
        public string GetMatchTypeString() {
            string retValue = "";
            switch (matchType)
            {
                case EMatchType.EMatchTypeTourney:
                    retValue = "tournament";
                    break;
                case EMatchType.EMatchTypeDuel:
                    retValue = "duel";
                    break;
                case EMatchType.EMatchTypeFriendly:
                    retValue = "friendly";
                    break;
                case EMatchType.NONE:
                    retValue = "None";
                    break;
                default:
                    retValue = "None";
                    break;
            }

            return retValue;
        }
    }
}
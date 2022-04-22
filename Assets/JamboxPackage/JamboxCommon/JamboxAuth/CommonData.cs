namespace Jambox.Common
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// A user's session used to authenticate messages.
    /// </summary>
    public interface IApiSession
    {
        /// <summary>
        /// True if the corresponding account was just created, false otherwise.
        /// </summary>
        bool Created { get; }

        /// <summary>
        /// Refresh token that can be used for session token renewal.
        /// </summary>
        string RefreshToken { get; }

        /// <summary>
        /// The Id of the player who have sent this authentication
        /// </summary>
        string MyID { get; }

        /// <summary>
        /// The Avatar Image URL of the player who have sent this authentication
        /// </summary>
        string MyAvatar { get; }

        string AvatarType { get; }

        int AvatarIndex { get; }

        /// <summary>
        /// Authentication credentials.
        /// </summary>
        string Token { get; set;  }

        /// <summary>
        /// User Name data.
        /// </summary>
        string Name { get; }
    }

    /// <inheritdoc />
    internal class ApiSession : IApiSession
    {
        /// <inheritdoc />
        [DataMember(Name = "created"), Preserve]
        public bool Created { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "refresh_token"), Preserve]
        public string RefreshToken { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "player_id"), Preserve]
        public string MyID { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "avatar_url"), Preserve]
        public string MyAvatar { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "avatar_group_id"), Preserve]
        public string AvatarType { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "avatar_index_id"), Preserve]
        public int AvatarIndex { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "token"), Preserve]
        public string Token { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "userName"), Preserve]
        public string Name { get; set; }

        public override string ToString()
        {
            var output = "";
            output = string.Concat(output, "Created: ", Created, ", ");
            output = string.Concat(output, "RefreshToken: ", RefreshToken, ", ");
            output = string.Concat(output, "Token: ", Token, ", ");
            output = string.Concat(output, "MyID: ", MyID, ", ");
            output = string.Concat(output, "Name: ", Name, ", ");
            return output;
        }
    }

    #region DETAILS_UPDATE

    public interface IAPIUpdateUserData
    {
        /// <summary>
        /// The updated name of the user.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The updated URL of the user.
        /// </summary>
        string URL { get; }

        /// <summary>
        /// 
        /// </summary>
        string Status { get; }
    }

    internal class APIUpdateUserData : IAPIUpdateUserData
    {
        /// <inheritdoc />
        [DataMember(Name = "name"), Preserve]
        public string Name { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "url"), Preserve]
        public string URL { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "status"), Preserve]
        public string Status { get; set; }
    }
    #endregion
}

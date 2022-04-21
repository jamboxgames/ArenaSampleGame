namespace Jambox.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface CommonIClient
    {
        /// <summary>
        /// True if the session should be refreshed with an active refresh token.
        /// </summary>
        bool AutoRefreshSession { get; }

        /// <summary>
        /// The host address of the server. Defaults to "127.0.0.1".
        /// </summary>
        string Host { get; }

        /// <summary>
        /// The port number of the server. Defaults to 7350.
        /// </summary>
        int Port { get; }

        /// <summary>
        /// The protocol scheme used to connect with the server. Must be either "http" or "https".
        /// </summary>
        string Scheme { get; }

        /// <summary>
        /// The key used to authenticate with the server without a session. Defaults to "defaultkey".
        /// </summary>
        string ServerKey { get; }

        /// <summary>
        /// Set the timeout in seconds on requests sent to the server.
        /// </summary>
        int Timeout { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="GameID"></param>
        /// <param name="UserID"></param>
        /// <param name="UserName"></param>
        /// <param name="AppSecret"></param>
        /// <returns></returns>
        Task<IApiSession> AuthenticateUser(String GameID, String UserID, String UserName, String AppSecret);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="name"></param>
        /// <param name="avatarId"></param>
        /// <param name="avatarGroup"></param>
        /// <returns></returns>
        Task<IAPIUpdateUserData> UpdateUserDetails(string authToken, string name, int avatarId, string avatarGroup);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="name"></param>
        /// <param name="avatarUrl"></param>
        /// <returns></returns>
        Task<IAPIUpdateUserData> UpdateUserDetails(string authToken, string name, string avatarUrl);
    }
}

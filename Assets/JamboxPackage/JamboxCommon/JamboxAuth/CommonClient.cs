namespace Jambox.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Jambox.Server;

    /// <inheritdoc cref="CommonIClient"/>
    public class CommonClient : CommonIClient
    {
        /// <summary>
        /// The default host address of the server.
        /// </summary>
        public const string DefaultHost = "18.220.70.244";

        /// <summary>
        /// The default protocol scheme for the socket connection.
        /// </summary>
        public const string DefaultScheme = "http";

        /// <summary>
        /// The default port number of the server.
        /// </summary>
        public const int DefaultPort = 8100;

        /// <summary>
        /// The default expired timespan used to check session lifetime.
        /// </summary>
        public static TimeSpan DefaultExpiredTimeSpan = TimeSpan.FromMinutes(5);

        /// <inheritdoc cref="CommonIClient.AutoRefreshSession"/>
        public bool AutoRefreshSession { get; }

        /// <inheritdoc cref="CommonIClient.Host"/>
        public string Host { get; }

        /// <summary>
        /// The logger to use with the client.
        /// </summary>
        public Jambox.Server.ILogger Logger
        {
            get => _logger;
            set
            {
                if(_comApiClient != null && _comApiClient.HttpAdapter != null)
                    _comApiClient.HttpAdapter.Logger = value;
                _logger = value;
            }
        }

        /// <inheritdoc cref="CommonIClient.Port"/>
        public int Port { get; }

        /// <inheritdoc cref="CommonIClient.Scheme"/>
        public string Scheme { get; }

        /// <inheritdoc cref="CommonIClient.ServerKey"/>
        public string ServerKey { get; }

        /// <inheritdoc cref="CommonIClient.Timeout"/>
        public int Timeout
        {
            get => _comApiClient.Timeout;
            set => _comApiClient.Timeout = value;
        }

        public CommonAPIClient _comApiClient;
        private Jambox.Server.ILogger _logger;

        public int DefaultTimeout = 15;

        //public CommonClient ()
        //{
        //    _comApiClient = new CommonAPIClient();
        //}

        public CommonClient(string serverKey, bool autoRefreshSession = true) : this(serverKey, HttpRequestAdapter.WithGzip(),
            autoRefreshSession)
        {
        }

        public CommonClient(string serverKey, IHttpAdapter adapter, bool autoRefreshSession = true) : this(DefaultScheme,
            DefaultHost, DefaultPort, serverKey,
            adapter, autoRefreshSession)
        {
        }

        public CommonClient(string scheme, string host, int port, string serverKey, bool autoRefreshSession = true) : this(
            scheme, host, port, serverKey,
            HttpRequestAdapter.WithGzip(), autoRefreshSession)
        {
        }

        public CommonClient(string scheme, string host, int port, string serverKey, IHttpAdapter adapter,
            bool autoRefreshSession = true)
        {
            AutoRefreshSession = autoRefreshSession;
            Host = host;
            Port = port;
            Scheme = scheme;
            ServerKey = serverKey;
            _comApiClient = new CommonAPIClient(new UriBuilder(scheme, host).Uri, adapter, DefaultTimeout);
            Logger = NullLogger.Instance; // must set logger last.
        }

        public override string ToString()
        {
            return $"Client(Host='{Host}', Port={Port}, Scheme='{Scheme}', ServerKey='{ServerKey}', Timeout={Timeout})";
        }

        public async Task<IApiSession> AuthenticateUser(String GameID, String UserID, String UserName, String AppSecret)
        {
            return await _comApiClient.AuthenticateUser(GameID, UserID, UserName, AppSecret);
        }

        public async Task<IAPIUpdateUserData> UpdateUserDetails(string authToken, string name, int avatarId, string avatarGroup)
        {
            return await _comApiClient.updateUserDetailsOnServer(authToken, name, avatarId, avatarGroup);
        }
    }
}

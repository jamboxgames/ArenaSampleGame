namespace Jambox.Common
{
    using System;
    using System.Threading.Tasks;
    using Jambox.Common.Utility;
    using JBX.Leaderboard.Controller;
    using UnityEngine;

    public class JamboxController : MonoSingleton<JamboxController>
    {
        private CommonIClient _client;
        public IApiSession CurrentSession;
        private String _gameID, _userName, _appSecret, _userID;
        private string UserLevel;
        private bool _isDebug;
        private String _adID;
        private string _productionServerIP = "api.jambox.games";
        public event Action UserProfileUpdated;
        public event Action<String, int> ErrorFromServer;

        public void SetupSDKVariable(string _gameID, string _AppSecret, String Ad_ID = "")
        {
            if (String.IsNullOrEmpty(_gameID) || String.IsNullOrEmpty(_AppSecret))
            {
                Debug.LogError("Game initialization parameters empty");
                return;
            }
            Init(_gameID, _AppSecret, Ad_ID);
        }

        public void SetUserLevel (string _userLvl)
        {
            UserLevel = _userLvl;
        }

        public string getUserLevel ()
        {
            return UserLevel;
        }

        public string getMyuserId()
        {
            if (CurrentSession == null)
                return null;
            return CurrentSession.MyID;
        }

        public void setAuthToken(string _token)
        {
            JamboxController.Instance.CurrentSession.Token = _token;
        }

        public string getAuthToken()
        {
            if (CurrentSession == null)
                return string.Empty;
            return CurrentSession.Token;
        }

        public bool isGameIdSet()
        {
            if (String.IsNullOrEmpty(_gameID) || String.IsNullOrEmpty(_appSecret))
            {
                return false;
            }
            return true;
        }

        public void Init(string gameID, String AppSecret, String Ad_ID)
        {
            _gameID = gameID;
            _appSecret = AppSecret;
            _adID = Ad_ID;
            _client = new CommonClient("https", _productionServerIP, 0000, _appSecret);
        }

        private void OnDestroy()
        {
            _client = null;
        }

        private bool WaitForSession = false;
        public async Task StartSession(String UserName, String UserID, bool isDebug)
        {
            _userName = UserName;
            _userID = UserID;
            _isDebug = isDebug;

            if (UserID == null)
            {
                _userID = UnityEngine.SystemInfo.deviceUniqueIdentifier;
            }
            if (UnityEngine.Application.internetReachability == UnityEngine.NetworkReachability.NotReachable)
            {
                return;
            }
            if (String.IsNullOrEmpty(_gameID) || String.IsNullOrEmpty(_appSecret))
            {
                Debug.LogError("You are trying to start session without being Set up SDK Variables ");
            }
            if (!ChechkForSession() && !WaitForSession)
            {
                WaitForSession = true;
                if (String.IsNullOrEmpty(_adID))
                {
                    Debug.Log("Advertising ID is not Set >>>>>");
                }
                UnityDebug.Debug.Log("Starting Session with UserID : " + _userID);
                CurrentSession = await _client.AuthenticateUser(_gameID, _userID, UserName, _appSecret, _adID, isDebug);
                JBXLeaderboardCommunicator.Instance.Init(BaseClient());
                UnityDebug.Debug.Log("Session created! - Token of CurrentSession is  :  " + CurrentSession.Token +
                    "  And Id is : " + CurrentSession.MyID);
                WaitForSession = false;
                CommonUserData.Instance.userName = CurrentSession.Name;
                CommonUserData.Instance.MyAvatarURL = CurrentSession.MyAvatar;
                CommonUserData.Instance.AvatarIndex = CurrentSession.AvatarIndex;
                Enum.TryParse(CurrentSession.AvatarType, out CommonUserData.Instance.AvatarGroup);
            }
        }

        public bool ChechkForSession()
        {
            if (JamboxController.Instance.CurrentSession != null && !String.IsNullOrEmpty(JamboxController.Instance.CurrentSession.Token))
                return true;

            return false;
        }

        public bool CheckForNetwork()
        {
            if (UnityEngine.Application.internetReachability == UnityEngine.NetworkReachability.NotReachable)
            {
                return false;
            }
            return true;
        }

        public async Task RefreshSession()
        {
            JamboxController.Instance.CurrentSession = await _client.AuthenticateUser(_gameID, _userID, _userName, _appSecret, _adID,_isDebug);
            CommonUserData.Instance.userName = CurrentSession.Name;
            CommonUserData.Instance.MyAvatarURL = CurrentSession.MyAvatar;
            CommonUserData.Instance.AvatarIndex = CurrentSession.AvatarIndex;
            Enum.TryParse(CurrentSession.AvatarType, out CommonUserData.Instance.AvatarGroup);
        }
        public async Task UpdateUserDetails(String name, int avatarId, string avatarGroup, Action<IAPIUpdateUserData> OnReceived, Action<int, string> OnError)
        {
            if (!ChechkForSession())
            {
                await RefreshSession();
            }
            String authToken = CurrentSession.Token;
            try
            {
                var result = await _client.UpdateUserDetails(authToken, name, avatarId, avatarGroup);
                if (UserProfileUpdated != null)
                {
                    UserProfileUpdated();
                }
                if (OnReceived != null)
                    OnReceived(result);
            }
            catch (Exception Ex)
            {
                Debug.Log("Exception caught Hit >>>> Message : " + Ex.Message);
                if (ErrorFromServer != null)
                {
                    ErrorFromServer(Ex.Message, -1);
                }
            }
        }

        public async Task UpdateUserDetails(String name, string avatar, Action<IAPIUpdateUserData> OnReceived, Action<string> OnError)
        {
            if (!ChechkForSession())
            {
                await RefreshSession();
            }
            String authToken = CurrentSession.Token;
            try
            {
                var result = await _client.UpdateUserDetails(authToken, name, avatar);
                if (UserProfileUpdated != null)
                {
                    UserProfileUpdated();
                }
                if (OnReceived != null)
                    OnReceived(result);
            }
            catch (Exception Ex)
            {
                Debug.Log("Exception caught Hit >>>> Message : " + Ex.Message);
                if (ErrorFromServer != null)
                {
                    ErrorFromServer(Ex.Message, -1);
                }
            }
        }

        public CommonClient BaseClient()
        {
            return (CommonClient)_client;
        }

    }
}

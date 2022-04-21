namespace Jambox.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Jambox.Common.Utility;
    using Jambox.Server;
    using UnityEngine;

    public class JamboxController : MonoSingleton<JamboxController>
    {
        private CommonIClient _client;
        public IApiSession CurrentSession;
        private String _gameID, _userName, _appSecret, _userID;
        
        private string _productionServerIP = "api.jambox.games";

        public event Action UserProfileUpdated;
        public event Action<String> ErrorFromServer;

        public void SetupSDKVariable(string _gameID, string _AppSecret)
        {
            if (String.IsNullOrEmpty(_gameID) || String.IsNullOrEmpty(_AppSecret))
            {
                Debug.LogError("Game initialization parameters empty");
                return;
            }
            Init(_gameID, _AppSecret);
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

        public void Init(string gameID, String AppSecret)
        {
            _gameID = gameID;
            _appSecret = AppSecret;
            
            _client = new CommonClient("https", _productionServerIP, 0000, _appSecret);
            
        }

        private bool WaitForSession = false;
        public async Task StartSession(String UserName, String UserID)
        {
            _userName = UserName;
            _userID = UserID;
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
                UnityDebug.Debug.Log("Starting Session with UserID : " + _userID);
                CurrentSession = await _client.AuthenticateUser(_gameID, _userID, UserName, _appSecret);
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
            JamboxController.Instance.CurrentSession = await _client.AuthenticateUser(_gameID, _userID, _userName, _appSecret);
            CommonUserData.Instance.userName = CurrentSession.Name;
            CommonUserData.Instance.MyAvatarURL = CurrentSession.MyAvatar;
            CommonUserData.Instance.AvatarIndex = CurrentSession.AvatarIndex;
            Enum.TryParse(CurrentSession.AvatarType, out CommonUserData.Instance.AvatarGroup);
        }
        public async Task UpdateUserOnServer (String name, int avatarId, string avatarGroup)
        {
            if (!ChechkForSession())
            {
                await RefreshSession();
            }
            String authToken = CurrentSession.Token;
            try
            {
                var result = await JamboxController.Instance.UpdateUserDetails(name, avatarId, avatarGroup);
                if (UserProfileUpdated != null)
                {
                    UserProfileUpdated();
                }
            }
            catch (Exception Ex)
            {
                Debug.Log("Exception caught Hit >>>> Message : " + Ex.Message);
                if (ErrorFromServer != null)
                {
                    ErrorFromServer(Ex.Message);
                }
            }
        }

        public async Task<IAPIUpdateUserData> UpdateUserDetails(String name, int avatarId, string avatarGroup)
        {
            if (!ChechkForSession())
            {
                await RefreshSession();
            }
            String authToken = CurrentSession.Token;
            var result = await _client.UpdateUserDetails(authToken, name, avatarId, avatarGroup);
            CommonUserData.Instance.userName = name;
            return result;
        }

        public async Task<IAPIUpdateUserData> UpdateUserDetails(String name, string avatarUrl)
        {
            if (!ChechkForSession())
            {
                await RefreshSession();
            }
            String authToken = CurrentSession.Token;
            var result = await _client.UpdateUserDetails(authToken, name, avatarUrl);
            return result;
        }


        public CommonClient BaseClient() {
            return (CommonClient)_client;
        }

    }
}

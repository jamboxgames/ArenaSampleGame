namespace Jambox.Common.Utility
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityDebug;
    using UnityEngine.UIElements;

    public enum ThemeType
    {
        ArenaTheme,
        CustomTheme
    }
    //[System.Serializable]
    public class JamboxSDKParams : MonoBehaviour
    {
        [SerializeField]
        private string gameID;
        [SerializeField]
        private string AppSecret;
        [SerializeField]
        public LogLevel LogLevel;
        //[HideInInspector]
        public ThemeType ActiveTheme = ThemeType.ArenaTheme;

        [Header("Arena ~ Use this if your using Arena SDK")]
        [Tooltip("All the arena related custom assets can be referenced inside this.")]
        public JamboxArenaSDKParams ArenaParameters;
        
        [Header("Rewards ~ Use this if your using Reward SDK")]
        [Tooltip("All the reward related custom assets can be referenced inside this.")]
        public JamboxRewardSDKParams RewardParameters;

        private void Awake()
        {
            string AdvertisingID = "";
            UnityDebug.Debug.LogLevel = LogLevel;
            if (m_Instance == null)
            {
                m_Instance = this;
            }
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass up = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject> ("currentActivity");
            AndroidJavaClass client = new AndroidJavaClass ("com.google.android.gms.ads.identifier.AdvertisingIdClient");
            AndroidJavaObject adInfo = client.CallStatic<AndroidJavaObject> ("getAdvertisingIdInfo",currentActivity);

            AdvertisingID = adInfo.Call<string> ("getId").ToString();
            UnityDebug.Debug.Log("advertisingID : " + AdvertisingID);
#elif !UNITY_EDITOR
            Application.RequestAdvertisingIdentifierAsync(
              (string advertisingId, bool trackingEnabled, string error) =>
              {
                  UnityDebug.Debug.Log("advertisingId : " + advertisingId + " trackingEnabled : " + trackingEnabled + " error : " + error);
                  AdvertisingID = advertisingId;
              });
#endif
            JamboxController.Instance.SetupSDKVariable(gameID, AppSecret, AdvertisingID);
        }

        private static JamboxSDKParams m_Instance = null;
        public static JamboxSDKParams Instance
        {
            get
            {
                // Instance requiered for the first time, we look for it
                if (m_Instance == null)
                {
                    return null;
                }
                return m_Instance;
            }
        }
    }
    [System.Serializable]
    public class JamboxArenaSDKParams
    {
        public Sprite bgSprite;
        public Sprite cardBG;
        public Sprite CoinBG;
    }

    [System.Serializable]
    public class JamboxRewardSDKParams
    {
        public Sprite rewardBG;
        public Sprite rewardRealMoneyImage;
        public Sprite rewardJBXMoneyImage;
        public bool SpinWheelWithAD;
    }
}

namespace Jambox.Common.Utility
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    public enum ThemeType
    {
        //Theme1,
        Theme2
    }
    //[System.Serializable]
    public class JamboxSDKParams : MonoBehaviour
    {
        [SerializeField]
        private string gameID;
        [SerializeField]
        private string AppSecret;
        [SerializeField]
        private bool IsProduction;
        [SerializeField]
        private bool LogEnabled = false;

        [HideInInspector]
        public ThemeType ActiveTheme = ThemeType.Theme2;
        //static string sb = "<b>Arena ~ Use this if your using Arena SDK</b>";
        [Header("Arena ~ Use this if your using Arena SDK")]
        [Tooltip("All the arena related custom assets can be referenced inside this.")]
        public JamboxArenaSDKParams ArenaParameters;

        
        [Header("Rewards ~ Use this if your using Reward SDK")]
        [Tooltip("All the reward related custom assets can be referenced inside this.")]
        public JamboxRewardSDKParams RewardParameters;

        private void Awake()
        {
            UnityDebug.Debug.isDebugBuild = LogEnabled;
            if (m_Instance == null)
            {
                m_Instance = this;
            }
            JamboxController.Instance.SetupSDKVariable(gameID, AppSecret, IsProduction);
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

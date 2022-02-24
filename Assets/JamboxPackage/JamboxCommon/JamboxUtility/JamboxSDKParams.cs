namespace Jambox.Common.Utility
{ 
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public enum ThemeType
    {
        //Theme1,
        Theme2
    }
    public class JamboxSDKParams : MonoBehaviour
    {
        public string gameID;
        public string AppSecret;
        public bool IsProduction;
        public Sprite bgSprite;
        public bool LogEnabled = false;
        [HideInInspector]
        public ThemeType ActiveTheme = ThemeType.Theme2;

        public Sprite cardBG;
        public Sprite CoinBG;

        [Header("Rewards")]
        [HideInInspector]
        public Sprite rewardBG;
        [HideInInspector]
        public Sprite rewardRealMoneyImage;
        [HideInInspector]
        public Sprite rewardJBXMoneyImage;
        [HideInInspector]
        public bool SpinWheelWithAD;

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
}

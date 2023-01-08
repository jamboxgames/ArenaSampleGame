namespace Jambox.Common
{
    using UnityEngine;
    using System.Collections;
    using Jambox.Common.Utility;
    using UnityEngine.UI;

    [System.Serializable]
    public struct Avatars
    {
        public AvatarType groupId;
        public Sprite[] avatars;
        public Button tabButton;
    }

    public enum AvatarType
    {
        robot,
        human
    }

    public class CommonUserData : MonoSingleton<CommonUserData>
    {
        public string userName = "";
        public string MyAvatarURL = "";
        public AvatarType AvatarGroup;
        public int AvatarIndex;
        public Sprite avatarSprite;

        private void Awake()
        {
            userName = "";
            MyAvatarURL = "";
            AvatarIndex = 1;
            avatarSprite = null;
        }
    }
}

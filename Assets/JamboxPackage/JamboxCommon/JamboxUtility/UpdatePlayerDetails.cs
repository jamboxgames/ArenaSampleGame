namespace Jambox.Common.Utility
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System;
    using Jambox.Common;
    using System.Threading.Tasks;

    public class UpdatePlayerDetails : MonoBehaviour
    {
        public Image BG;
        public InputField nameInput;
        public Text namePlaceholder;
        public Image selectedAvatar;
        public Button updateButton;
        public Button closeButton;

        public GameObject[] loadingObjects;

        public List<Avatars> avatarDetails;

        public GameObject avatarPrefab;
        public Transform scrollContent;
        public Button[] allTabButtons;
        public GameObject loadingPanel;

        private AvatarType currenTab;
        private AvatarType currentAvatarGroup;
        private int selectedAvatarId = 1;
        private AvatarPrefab lastSelectedImage;

        private RectTransform rectTransform;
        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                    rectTransform = GetComponent<RectTransform>();
                return rectTransform;
            }
        }

        #region Tablet_Checks
        public void TabletCheck()
        {
            if (TabletDetect.IsTablet())
            {
                this.gameObject.GetComponentInParent<CanvasScaleChange>().SetToTabletView();
            }
            else
            {
                this.gameObject.GetComponentInParent<CanvasScaleChange>().SetToDefault();
            }
        }
        #endregion

        public void OnUpdateClick()
        {
            if (string.IsNullOrWhiteSpace(nameInput.text))
            {
                nameInput.text = "";
                namePlaceholder.text = "Enter Valid Name";
                UnityDebug.Debug.Log("Enter Valid Name >>>> ");
                return;
            }
            UnityDebug.Debug.Log("Updated Name : " + nameInput.text);
            loadingPanel.SetActive(true);
            _ = UpdatePlayerDetailsTask();
        }

        async Task UpdatePlayerDetailsTask()
        {
            await JamboxController.Instance.UpdateUserOnServer(nameInput.text, selectedAvatarId, currentAvatarGroup.ToString());
            Destroy(this.gameObject);
        }

        private void OnEnable()
        {
            closeButton.gameObject.SetActive(false);
            StartCoroutine(Loading());
        }

        private void OnDisable()
        {
            if (GetComponentInParent<CanvasScaleChange>() != null)
            {
                GetComponentInParent<CanvasScaleChange>().SetToDefault();
            }
        }

        IEnumerator Loading()
        {
            selectedAvatar.gameObject.SetActive(false);

            foreach (var v in loadingObjects)
            {
                v.SetActive(true);
            }

            while (selectedAvatar.sprite == null)
            {
                yield return null;
            }
            selectedAvatar.gameObject.SetActive(true);
            foreach (var v in loadingObjects)
            {
                v.SetActive(false);
            }
        }

        public void SetMetaData(string _name, Texture2D _avatar, bool isRewardSDK = false)
        {
            TabletCheck();
            if (isRewardSDK)
            {
                if (JamboxSDKParams.Instance.RewardParameters.rewardBG != null)
                {
                    BG.sprite = JamboxSDKParams.Instance.RewardParameters.rewardBG;
                    BG.color = Color.white;
                }
            }
            else
            {
                if (JamboxSDKParams.Instance.ArenaParameters.bgSprite != null)
                {
                    BG.sprite = JamboxSDKParams.Instance.ArenaParameters.bgSprite;
                    BG.color = Color.white;
                }
            }
            CheckNameInput();
            //Hide Close button if no username already
            if (string.IsNullOrEmpty(CommonUserData.Instance.userName))
            {
                closeButton.gameObject.SetActive(false);
            }
            else
            {
                closeButton.gameObject.SetActive(true);
            }

            SetDefaultAvatarDetails();
            nameInput.text = _name;
            selectedAvatar.sprite = Sprite.Create(_avatar, new Rect(0, 0, _avatar.width, _avatar.height), new Vector2(0, 0));

            ChangeTab((int)currenTab);
        }

        public void ChangeTab(int _index)
        {
            Avatars _current = avatarDetails[_index];

            for (int i = 0; i < scrollContent.childCount; i++)
            {
                scrollContent.GetChild(i).gameObject.SetActive(true);
            }

            //Creating required Avatar prefabs
            int _totalRequiredPrefabs = _current.avatars.Length;
            if (scrollContent.childCount < _current.avatars.Length)
            {
                int _extraRequired = _current.avatars.Length - scrollContent.childCount;
                for (int i = 0; i < _extraRequired; i++)
                {
                    AvatarPrefab _image = Instantiate(avatarPrefab, scrollContent).GetComponent<AvatarPrefab>();
                }
            }
            else if (scrollContent.childCount > _current.avatars.Length)
            {
                int _extra = scrollContent.childCount - _current.avatars.Length;
                for (int i = scrollContent.childCount - _extra; i < scrollContent.childCount; i++)
                {
                    scrollContent.GetChild(i).gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < _current.avatars.Length; i++)
            {
                scrollContent.GetChild(i).GetComponent<AvatarPrefab>().SetData(_current.avatars[i], i + 1, this);
            }

            foreach (Button go in allTabButtons)
            {
                go.interactable = true;
            }
            _current.tabButton.interactable = false;

            currenTab = _current.groupId;
            if (currentAvatarGroup == currenTab)
            {
                if (lastSelectedImage != null)
                    lastSelectedImage.EnableHightlight();
            }
            else
            {
                if (lastSelectedImage != null)
                {
                    lastSelectedImage.Deselect();
                }
            }
        }

        void SetDefaultAvatarDetails()
        {
            selectedAvatarId = CommonUserData.Instance.AvatarIndex;
            currentAvatarGroup = CommonUserData.Instance.AvatarGroup;
            currenTab = currentAvatarGroup;
        }

        public void ImageSelected(AvatarPrefab _lastSelected)
        {
            if (lastSelectedImage != null)
            {
                lastSelectedImage.Deselect();
            }
            lastSelectedImage = _lastSelected;
            selectedAvatar.sprite = _lastSelected.avatarImage.sprite;
            selectedAvatarId = _lastSelected.avatarId;
            currentAvatarGroup = currenTab;
        }

        public void CloseUpdatePanel()
        {
            if (lastSelectedImage != null)
            {
                lastSelectedImage.Deselect();
                lastSelectedImage = null;
            }
            Destroy(this.gameObject);
        }

        public void CheckNameInput()
        {
            if (string.IsNullOrWhiteSpace(nameInput.text))
            {
                updateButton.interactable = false;
            }
            else
            {
                updateButton.interactable = true;
            }
        }

    }
}

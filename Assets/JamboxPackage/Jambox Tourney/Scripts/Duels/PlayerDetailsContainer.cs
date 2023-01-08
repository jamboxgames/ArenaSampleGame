namespace Jambox.Tourney.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Jambox.Common.Utility;
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.UI;

    public class PlayerDetailsContainer : MonoBehaviour
    {

        public Text playerNameText;
        public Image profilePicture;
        public Text playerScoreText;
        public GameObject winnerFrame;

        public GameObject EntryFeeCotainer;
        public Text EntryFeeText;
        public Image EntryFeeImage;

        public GameObject RewardAmtContainer;
        public Text RewardAmtText;
        public Image RewardAmtImage;

        private float rewardIncreaseDuration;
        private float entryFeeDecreaseDuration;
        public float fadeInRate;
        public UIParticleSystem coinsPS;

        public bool informationFilled = false;

        private void OnEnable()
        {
            if (GetComponentInParent<MatchmakingPanel>() != null)
            {
                entryFeeDecreaseDuration = GetComponentInParent<MatchmakingPanel>().updateRewardDuration;
                EntryFeeCotainer.SetActive(false);
            }
            if (GetComponentInParent<DuelResultPanel>() != null)
            {
                rewardIncreaseDuration = GetComponentInParent<DuelResultPanel>().rewardChangeDuration;
            }
            if (RewardAmtContainer != null)
                RewardAmtContainer.SetActive(false);
        }

        public void SetPlayerDetails(string _name, string url, long _score = 0, bool won = false)
        {
            playerNameText.text = _name;
            profilePicture.gameObject.SetActive(true);
            StartCoroutine(GetTextureRequest(url));
            if (playerScoreText != null && _score >= 0)
            {
                playerScoreText.text = _score.ToString();
                if (EntryFeeCotainer != null)
                {
                    EntryFeeCotainer.gameObject.SetActive(false);
                }
            }
            if (won)
            {
                winnerFrame.SetActive(true);
            }

        }

        public void SetPlayerDetails(string _name, Sprite avatarSprite, long entryFee = -1, long _score = -1, bool won = false)
        {
            playerNameText.text = _name;
            profilePicture.gameObject.SetActive(true);
            profilePicture.sprite = avatarSprite;
            informationFilled = true;
            if (_score >= 0)
            {
                if (playerScoreText != null)
                {
                    playerScoreText.text = _score.ToString();
                }
            }
            else
            {
                if (playerScoreText != null)
                {
                    playerScoreText.text = "";
                }
            }
            if (entryFee >= 0)
            {
                if (EntryFeeCotainer != null)
                {
                    EntryFeeCotainer.gameObject.SetActive(true);
                    EntryFeeText.text = entryFee.ToString();
                    EntryFeeImage.sprite = JamboxSDKParams.Instance.ArenaParameters.CoinBG;

                    SetCurrencyPreferredSize();
                }
            }
            else
            {
                if (EntryFeeCotainer != null)
                {
                    EntryFeeCotainer.gameObject.SetActive(false);
                }
            }

            if (won)
            {
                winnerFrame.SetActive(true);
            }

        }

        void SetCurrencyPreferredSize()
        {
            EntryFeeText.rectTransform.sizeDelta = new Vector2(EntryFeeText.preferredWidth, EntryFeeText.rectTransform.sizeDelta.y);
        }

        void SetCurrencyPreferredSize(string _text)
        {
            string _temp = EntryFeeText.text;
            EntryFeeText.text = _text;
            EntryFeeText.rectTransform.sizeDelta = new Vector2(EntryFeeText.preferredWidth, EntryFeeText.rectTransform.sizeDelta.y);
            EntryFeeText.text = _temp;
        }

        public void enableEntryFee(long entryFee)
        {
            if (entryFee >= 0)
            {
                if (EntryFeeCotainer != null)
                {
                    EntryFeeCotainer.gameObject.SetActive(true);
                    EntryFeeText.text = entryFee.ToString();
                    EntryFeeImage.sprite = JamboxSDKParams.Instance.ArenaParameters.CoinBG;

                    SetCurrencyPreferredSize();
                }
            }
        }

        public void IncreaseRewards(long _reward)
        {
            StartCoroutine(IncreaseReward(_reward));
        }

        IEnumerator IncreaseReward(long _reward)
        {
            RewardAmtImage.sprite = JamboxSDKParams.Instance.ArenaParameters.CoinBG;
            if (_reward > 0)
            {
                while (!informationFilled)
                {
                    yield return null;
                }

                RewardAmtContainer.SetActive(true);

                UpdateTextAnimation _textAnim = UIAnimations.Instance.ChangeTextOverTime(RewardAmtText, 0, (int)_reward, rewardIncreaseDuration, true);

            }
            else
            {
                RewardAmtContainer.SetActive(true);
                RewardAmtText.text = 0 + "";
            }

            SetCurrencyPreferredSize(_reward.ToString());
        }

        public void ShowScore()
        {
            Color c = playerScoreText.color;
            playerScoreText.color = new Color(c.r, c.g, c.b, 0f);
            playerScoreText.gameObject.SetActive(true);
            StartCoroutine(FadeInPlayerScore());
        }

        IEnumerator FadeInPlayerScore()
        {
            Color c = playerScoreText.color;
            while (c.a < 1f)
            {
                c.a += fadeInRate * Time.deltaTime;
                playerScoreText.color = c;
                yield return null;
            }
        }

        public void ShowEntryAmount()
        {
            if (EntryFeeCotainer != null)
                EntryFeeCotainer.gameObject.SetActive(true);
        }

        public void HideMoneyText()
        {
            StartCoroutine(HideMoneyProperly());
        }

        IEnumerator HideMoneyProperly()
        {
            coinsPS.GetComponent<UIParticleSystem>().Particle = JamboxSDKParams.Instance.ArenaParameters.CoinBG;
            coinsPS.Play();
            int TotalMoney = Convert.ToInt32(EntryFeeText.text);
            UpdateTextAnimation _textAnim = UIAnimations.Instance.ChangeTextOverTime(EntryFeeText, TotalMoney, 0, entryFeeDecreaseDuration, false);
            while (!_textAnim.done)
            {
                //Color
                float _alphaPercentage = _textAnim.doneRatio;
                Color c = EntryFeeText.color;
                EntryFeeText.color = new Color(c.r, c.g, c.b, (1 - _alphaPercentage));

                yield return null;
            }
            EntryFeeText.text = string.Empty;
            coinsPS.Stop();

            if (EntryFeeCotainer != null)
            {
                EntryFeeCotainer.gameObject.SetActive(false);
            }
            else
            {
                playerScoreText.gameObject.SetActive(false);
            }

        }

        IEnumerator GetTextureRequest(string url)
        {
            UnityDebug.Debug.LogInfo("GetTextureRequest HIt URL : " + url);
            if (string.IsNullOrEmpty(url))
            {
                UnityDebug.Debug.LogWarning("url String is Empty In GetTextureRequest >>>>>>>>");
            }
            while (url == null)
            {
                yield return new WaitForEndOfFrame();
            }
            WWW www = new WWW(url);
            yield return www;
            profilePicture.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
            informationFilled = true;
        }

        public Image GetProfileImage()
        {
            return profilePicture;
        }
    }
}

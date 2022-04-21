namespace Jambox.Tourney.UI
{
    using System;
    using System.Collections.Generic;
    using Jambox.Tourney.Connector;
    using Jambox.Tourney.Data;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class CompTourneyItem : TourneyListViewItem
    {
        public Text TourneyName;
        public Text TDesc;
        public Text PlayerPositionDetail;
        public Text Reward;
        public Text AttemptsDone;
        public Text ClaimBtnText;
        public Button ClaimButton;

        public Sprite normalImage;
        public Sprite claimedImage;

        public Text MyScore;
        public Text EndTime;
        //public Button LeaderBoard;
        private bool isFriendly = false;
        private CompletedTourneyDetail _ComptourneyDet;

        public CompletedTourneyDetail CompTourneyDet
        {
            get { return _ComptourneyDet; }
            set
            {
                _ComptourneyDet = value;
                UpdateUI();
            }
        }

        public void IsFriendlyCompleted(bool _value)
        {
            isFriendly = _value;
        }

        private void UpdateUI()
        {
            TourneyName.text = _ComptourneyDet.TourneyName;
            TDesc.text = "My Rank : " + _ComptourneyDet.MyRank + "\n" + "My Score : " + _ComptourneyDet.Score;
            PlayerPositionDetail.text = _ComptourneyDet.MyRank + "/" + _ComptourneyDet.JoinedPlayer;

            string _currency = "";
            if(!UserDataContainer.Instance.currencyList.TryGetValue(_ComptourneyDet.RewardCurrency, out _currency))
            {
                _currency = _ComptourneyDet.RewardCurrency;
            }
            Reward.text = _ComptourneyDet.RewardAmnt + " " + _currency;
            

            TimeSpan elapsed = (DateTime.UtcNow.Subtract(DateTime.Parse(_ComptourneyDet.EndTime).ToUniversalTime()));
            //2021-07-19T07:54:36.738535Z
            EndTime.text = "ENDED : " + EndTimeInFormat(elapsed) + " ago";

            if (_ComptourneyDet.RewardAmnt <= 0)
            {
                ClaimButton.gameObject.SetActive(false);
            }
            else
            {
                ClaimButton.gameObject.SetActive(true);
                if (_ComptourneyDet.IsClaimed)
                {
                    ClaimBtnText.text = "Claimed";
                    ClaimButton.interactable = false;
                    if (claimedImage != null)
                        ClaimButton.GetComponent<Image>().sprite = claimedImage;
                }
                else
                {
                    ClaimBtnText.text = "Claim";
                    ClaimButton.interactable = true;
                    if (normalImage != null)
                        ClaimButton.GetComponent<Image>().sprite = normalImage;
                }
            }
            //LeaderBoard.gameObject.SetActive(true);
        }

        string EndTimeInFormat(TimeSpan _elasped)
        {
            string timeString = "";

            if (_elasped.Days >= 2)
            {
                timeString = _elasped.Days + "D";
            }
            else if (_elasped.Days >= 1)
            {
                if (_elasped.Hours == 0)
                {
                    timeString = _elasped.Days + "D ";
                    return timeString;
                }
                timeString = _elasped.Days + "D " + _elasped.Hours + "H";
            }
            else if (_elasped.Hours >= 1)
            {
                if (_elasped.Minutes == 0)
                {
                    timeString = _elasped.Hours + "H ";
                    return timeString;
                }
                timeString = _elasped.Hours + "H " + _elasped.Minutes + "M";
            }
            else if (_elasped.Hours < 1 && _elasped.Minutes >= 1)
            {
                timeString = _elasped.Minutes + "M";
            }
            else if (_elasped.Minutes < 1)
            {
                timeString = "0M";
            }

            return timeString;
        }

        public void OnClaimBtnClick ()
        {
            ClaimButton.interactable = false;
            Debug.LogError("OnClaimBtnClick 111111 >>>" + (CompTourneyDet == null) );
            Debug.LogError("OnClaimBtnClick 222222 >>>" + (CompTourneyDet.LeaderBoardID));
            UIPanelController.Instance.LoadingDialogue(true, false);
            _ = CommunicationController.Instance.GetClaim(CompTourneyDet.LeaderBoardID, (data) => { OnClaimSuccess(data); }, (errorMsg) => { UIPanelController.Instance.ErrorFromServerRcvd(errorMsg); });
        }

        private void OnClaimSuccess (IAPIClaimData dataRcvd)
        {
            UIPanelController.Instance.LoadingDialogue(false);
            string UpdatedCurrencyKey = dataRcvd.RewardInfo.Virtual.Key;
            UserDataContainer.Instance.currencyList.TryGetValue(dataRcvd.RewardInfo.Virtual.Key, out UpdatedCurrencyKey);
            UIPanelController.Instance.ShowClaimSuccessPanel(dataRcvd.RewardInfo.Virtual.Value, UpdatedCurrencyKey);
            ClaimBtnText.text = "Claimed";
            ClaimButton.interactable = false;
            UserDataContainer.Instance.UpdateUserMoney (dataRcvd.RewardInfo.Virtual.Value,
                                                    dataRcvd.RewardInfo.Virtual.Key, true);
            UIPanelController.Instance.UpdateMoneyOnUI();

            //Updating info on UserDataContainer
            CompTourneyDet.IsClaimed = true;

            if (UserDataContainer.Instance.UnclaimedRewardsCount > 0)
            {
                UserDataContainer.Instance.UnclaimedRewardsCount--;
                UIPanelController.Instance.CheckForUnclaimedExclamation();
            }

        }

        public void OnMoreButtonClick()
        {
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            metadata.Add("lbid", _ComptourneyDet.LeaderBoardID);
            UIPanelController.Instance.ShowPanel(Panels.DetailsPanel, Panels.CompletedPanel, metadata, _friendlyCompleted: isFriendly);
        }
    }
}

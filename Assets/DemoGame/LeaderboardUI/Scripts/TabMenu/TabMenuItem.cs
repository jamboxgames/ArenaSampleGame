namespace Jambox.Leaderboard.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class TabMenuItem : MonoBehaviour
    {

        public Text tabName;

        private TabMenuDetails tabMenuDetails;

        public void UpdateItem(string _name, TabMenuDetails _tabMenuDetails)
        {
            tabName.text = _name;
            tabMenuDetails = _tabMenuDetails;
        }

        public void OnBtnClicked()
        {
            tabMenuDetails.ChangeTabTo(tabName.text);
        }

    }
}
namespace Jambox.Leaderboard.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class TabMenuDetails : MonoBehaviour
    {

        public string[] tabMenus;
        public Transform tabMenuContainer;

        private Dictionary<string, TabMenuItem> tabMenusDictionary = new Dictionary<string, TabMenuItem>();
        private TabMenuItem currentTab;

        public void InitializeTabs()
        {
            foreach (var menu in tabMenus)
            {
                TabMenuItem item = Instantiate(Resources.Load("JamboxLeaderboardUI/TabMenuItem") as GameObject, tabMenuContainer).GetComponent<TabMenuItem>();
                item.UpdateItem(menu, this);
                tabMenusDictionary.Add(menu, item);
            }
            ChangeTabTo(tabMenus[0]);
        }

        public void ChangeTabTo(string tabName)
        {
            if (currentTab != null)
            {
                Button _prevButton = currentTab.GetComponent<Button>();
                _prevButton.interactable = true;
            }

            currentTab = tabMenusDictionary[tabName];

            Button _button = currentTab.GetComponent<Button>();
            _button.interactable = false;
        }

    }
}


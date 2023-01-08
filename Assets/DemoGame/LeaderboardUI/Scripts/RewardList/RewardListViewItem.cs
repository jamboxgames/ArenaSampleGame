﻿namespace Jambox.Leaderboard.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(RectTransform))]
    public class RewardListViewItem : MonoBehaviour
    {
        private RewardListView parentList;
        public RewardListView ParentList
        {
            get => parentList;
        }

        private int currentRow;
        public int CurrentRow
        {
            get => currentRow;
        }

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

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void NotifyCurrentAssignment(RewardListView v, int row)
        {
            parentList = v;
            currentRow = row;
        }
    }
}

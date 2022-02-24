﻿namespace Jambox.Tourney.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(RectTransform))]
    public class RealtimeLbListViewItem : MonoBehaviour
    {
        private RealtimeLbListView parentList;
        public RealtimeLbListView ParentList
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

        public void NotifyCurrentAssignment(RealtimeLbListView v, int row)
        {
            parentList = v;
            currentRow = row;
        }
    }
}

namespace Jambox.Leaderboard.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class LeaderBoardListView : MonoBehaviour
    {

        [Tooltip("The amount of vertical padding to add between items")]
        public float RowPadding = 15f;
        [Tooltip("Minimum height to pre-allocate list items for. Use to prevent allocations on resizing.")]
        public float PreAllocHeight = 0;
        [SerializeField] private GameObject scrollBlock;
        [SerializeField] private float itemHeight;

        public bool IsPreviousResult = false;

        public PreviousResultPanel previousResultPanel;
        public LeaderboardPanel leaderboardPanel;

        private bool myPlayerAvailable = false;
        private float myPlayerItemLocalPosition;

        /// <summary>
        /// Set the vertical normalized scroll position. 0 is bottom, 1 is top (as with ScrollRect) 
        /// </summary>
        public float VerticalNormalizedPosition
        {
            get => scrollRect.verticalNormalizedPosition;
            set => scrollRect.verticalNormalizedPosition = value;
        }

        protected int rowCount;
        /// <summary>
        /// Get / set the number of rows in the list. If changed, will cause a rebuild of
        /// the contents of the list. Call Refresh() instead to update contents without changing
        /// length.
        /// </summary>
        public int RowCount
        {
            get => rowCount;
            set
            {
                if (rowCount != value)
                {
                    rowCount = value;
                    // avoid triggering double refresh due to scroll change from height change
                    ignoreScrollChange = true;
                    UpdateContentHeight();
                    ignoreScrollChange = false;
                    if (rowCount == 0)
                    {
                        ReorganiseContent(true, true);
                    }
                    else
                    {
                        ReorganiseContent(true);
                    }
                }
            }
        }

        /// <summary>
        /// Delegate which users should implement to populate their custom RecyclingListViewItem
        /// instances when they're needed by the list.
        /// </summary>
        /// <param name="item">The child item being populated. These are recycled as the list scrolls.</param>
        /// <param name="rowIndex">The overall row index of the list item being populated</param>
        public delegate void ItemDelegate(LeaderBoardListViewItem item, int rowIndex);

        /// <summary>
        /// Set the delegate which will be called back to populate items. You must provide this at runtime.
        /// </summary>
        public ItemDelegate ItemCallback;

        protected ScrollRect scrollRect;
        // circular buffer of child items which are reused
        protected LeaderBoardListViewItem[] childItems;
        // the current start index of the circular buffer
        protected int childBufferStart = 0;
        // the index into source data which childBufferStart refers to 
        protected int sourceDataRowStart;

        protected bool ignoreScrollChange = false;
        protected float previousBuildHeight = 0;
        protected const int rowsAboveBelow = 1;

        /// <summary>
        /// Trigger the refreshing of the list content (e.g. if you've changed some values).
        /// Use this if the number of rows hasn't changed but you want to update the contents
        /// for some other reason. All active items will have the ItemCallback invoked. 
        /// </summary>
        public virtual void Refresh()
        {
            ReorganiseContent(true);
        }

        /// <summary>
        /// Refresh a subset of the list content. Any rows which currently have data populated in the view
        /// will cause a call to ItemCallback. The size of the list or positions won't change.
        /// </summary>
        /// <param name="rowStart"></param>
        /// <param name="count"></param>
        public virtual void Refresh(int rowStart, int count)
        {
            // only refresh the overlap
            int sourceDataLimit = sourceDataRowStart + childItems.Length;
            for (int i = 0; i < count; ++i)
            {
                int row = rowStart + i;
                if (row < sourceDataRowStart || row >= sourceDataLimit)
                    continue;

                int bufIdx = WrapChildIndex(childBufferStart + row - sourceDataRowStart);
                if (childItems[bufIdx] != null)
                {
                    UpdateChild(childItems[bufIdx], row);
                }
            }
        }

        /// <summary>
        /// Refresh a single row based on its reference.
        /// </summary>
        /// <param name="item"></param>
        public virtual void Refresh(LeaderBoardListViewItem item)
        {
            for (int i = 0; i < childItems.Length; ++i)
            {
                int idx = WrapChildIndex(childBufferStart + i);
                if (childItems[idx] != null && childItems[idx] == item)
                {
                    UpdateChild(childItems[i], sourceDataRowStart + i);
                    break;
                }
            }
        }

        /// <summary>
        /// Quick way of clearing all the content from the list (alias for RowCount = 0)
        /// </summary>
        public virtual void Clear()
        {
            if (childItems == null)
                return;

            rowCount = 0;
            foreach(LeaderBoardListViewItem item in childItems)
            {
                item.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Scroll the viewport so that a given row is in view, preferably centred vertically.
        /// </summary>
        /// <param name="row"></param>
        public virtual void ScrollToRow(int row)
        {
            scrollRect.verticalNormalizedPosition = GetRowScrollPosition(row);
        }

        /// <summary>
        /// Get the normalised vertical scroll position which would centre the given row in the view,
        /// as best as possible without scrolling outside the bounds of the content.
        /// Use this instead of ScrollToRow if you want to control the actual scrolling yourself.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public float GetRowScrollPosition(int row)
        {
            float rowCentre = (row + 0.5f) * RowHeight();
            float vpHeight = ViewportHeight();
            float halfVpHeight = vpHeight * 0.5f;
            // Clamp to top of content
            float vpTop = Mathf.Max(0, rowCentre - halfVpHeight);
            float vpBottom = vpTop + vpHeight;
            float contentHeight = scrollRect.content.sizeDelta.y;
            // clamp to bottom of content
            if (vpBottom > contentHeight) // if content is shorter than vp always stop at 0
                vpTop = Mathf.Max(0, vpTop - (vpBottom - contentHeight));

            // Range for our purposes is between top (0) and top of vp when scrolled to bottom (contentHeight - vpHeight)
            // ScrollRect normalised position is 0 at bottom, 1 at top
            // so inverted range because 0 is bottom and our calc is top-down
            return Mathf.InverseLerp(contentHeight - vpHeight, 0, vpTop);
        }

        /// <summary>
        /// Retrieve the item instance for a given row, IF it is currently allocated for the view.
        /// Because these items are recycled as the view moves, you should not hold on to this item beyond
        /// the site of the call to this method.
        /// </summary>
        /// <param name="row">The row number</param>
        /// <returns>The list view item assigned to this row IF it's within the window the list currently has
        /// allocated for viewing. If row is outside this range, returns null.</returns>
        public LeaderBoardListViewItem GetRowItem(int row)
        {
            if (childItems != null &&
                row >= sourceDataRowStart && row < sourceDataRowStart + childItems.Length && // within window 
                row < rowCount)
            { // within overall range

                return childItems[WrapChildIndex(childBufferStart + row - sourceDataRowStart)];
            }

            return null;
        }

        protected virtual void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
            //string lbPath = UIPanelController.Instance.ThemePath("LBItem");
            //leaderBoardPrefab = Instantiate(Resources.Load(lbPath) as GameObject).GetComponent<LeaderBoardItem>();
        }

        protected virtual bool CheckChildItems()
        {
            float vpHeight = ViewportHeight();
            float buildHeight = Mathf.Max(vpHeight, PreAllocHeight);
            bool rebuild = childItems == null || buildHeight > previousBuildHeight;
            if (rebuild)
            {
                // create a fixed number of children, we'll re-use them when scrolling
                // figure out how many we need, round up
                int childCount = Mathf.RoundToInt(0.5f + buildHeight / RowHeight());
                childCount += rowsAboveBelow * 2; // X before, X after

                if (childItems == null)
                    childItems = new LeaderBoardListViewItem[childCount];
                else if (childCount > childItems.Length)
                    Array.Resize(ref childItems, childCount);

                for (int i = 0; i < childItems.Length; ++i)
                {
                    if (childItems[i] == null)
                    {
                        //string lbPath = UIPanelController.Instance.ThemePath("LBItem");
                        childItems[i] = Instantiate(Resources.Load("JamboxLeaderboardUI/LBItem") as GameObject).GetComponent<LeaderBoardItem>();
                        //childItems[i] = Instantiate(leaderBoardPrefab);
                    }
                    childItems[i].RectTransform.SetParent(scrollRect.content, false);
                    childItems[i].gameObject.SetActive(false);
                }

                previousBuildHeight = buildHeight;
            }

            return rebuild;
        }


        protected virtual void OnEnable()
        {
            scrollRect.onValueChanged.AddListener(OnScrollChanged);
            ignoreScrollChange = false;
        }

        protected virtual void OnDisable()
        {
            scrollRect.onValueChanged.RemoveListener(OnScrollChanged);
        }

        protected virtual void OnScrollChanged(Vector2 normalisedPos)
        {
            // This is called when scroll bar is moved *and* when viewport changes size
            if (!ignoreScrollChange)
            {
                ReorganiseContent(false);
            }

            if (myPlayerAvailable)
                CheckForMyPlayerInfoPostition();
        }

        void CheckForMyPlayerInfoPostition()
        {

            float diff = myPlayerItemLocalPosition - scrollRect.content.anchoredPosition.y;
            if (diff < 0f)
            {
                if (IsPreviousResult)
                    previousResultPanel.ShowMyPlayerInfo(true, true);
                else
                    leaderboardPanel.ShowMyPlayerInfo(true, true);
                return;
            }

            if (diff > scrollRect.viewport.rect.height - itemHeight)
            {
                if (IsPreviousResult)
                    previousResultPanel.ShowMyPlayerInfo(true, false);
                else
                    leaderboardPanel.ShowMyPlayerInfo(true, false);
                return;
            }

            if (IsPreviousResult)
                previousResultPanel.ShowMyPlayerInfo(false);
            else
                leaderboardPanel.ShowMyPlayerInfo(false);

        }

        public void UpdateMyPlayerRequiredDetails(int _myPlayerIndex)
        {
            myPlayerAvailable= true;
            myPlayerItemLocalPosition = _myPlayerIndex * RowHeight();
            CheckForMyPlayerInfoPostition();
        }

        protected virtual void ReorganiseContent(bool clearContents, bool refreshed = false)
        {
            if (clearContents)
            {
                scrollRect.StopMovement();
                scrollRect.verticalNormalizedPosition = 1; // 1 == top
            }

            StartCoroutine(ReOrganiseContentSeq(clearContents, refreshed));
        }

        private IEnumerator ReOrganiseContentSeq(bool clearContents, bool refreshed)
        {
            //yield return new WaitForEndOfFrame();
            bool childrenChanged = CheckChildItems();
            bool populateAll = childrenChanged || clearContents;

            // Figure out which is the first virtual slot visible
            //float ymin = 0;
            float ymin = scrollRect.content.anchoredPosition.y;
            // round down to find first visible
            int firstVisibleIndex = (int)(ymin / RowHeight());
            // we always want to start our buffer before
            int newRowStart = firstVisibleIndex - rowsAboveBelow;
            // If we've moved too far to be able to reuse anything, same as init case
            int diff = newRowStart - sourceDataRowStart;
            //yield return new WaitForEndOfFrame();
            
            if (populateAll || Mathf.Abs(diff) >= childItems.Length)
            {
                scrollBlock.SetActive(true);
                sourceDataRowStart = newRowStart;
                childBufferStart = 0;
                int rowIdx = newRowStart;
                foreach (var item in childItems)
                {
                    UpdateChild(item, rowIdx++);
                    if (!refreshed)
                        yield return new WaitForSeconds(0.125f);
                }
                scrollBlock.SetActive(false);
            }
            else if (diff != 0)
            {
                // we scrolled forwards or backwards within the tolerance that we can re-use some of what we have
                // Move our window so that we just re-use from back and place in front
                // children which were already there and contain correct data won't need changing
                int newBufferStart = (childBufferStart + diff) % childItems.Length;
                if (diff < 0)
                {
                    // window moved backwards
                    for (int i = 1; i <= -diff; ++i)
                    {
                        int bufi = WrapChildIndex(childBufferStart - i);
                        int rowIdx = sourceDataRowStart - i;
                        UpdateChild(childItems[bufi], rowIdx);
                    }
                }
                else
                {
                    // window moved forwards
                    int prevLastBufIdx = childBufferStart + childItems.Length - 1;
                    int prevLastRowIdx = sourceDataRowStart + childItems.Length - 1;
                    for (int i = 1; i <= diff; ++i)
                    {
                        int bufi = WrapChildIndex(prevLastBufIdx + i);
                        int rowIdx = prevLastRowIdx + i;
                        UpdateChild(childItems[bufi], rowIdx);
                    }
                }
                sourceDataRowStart = newRowStart;
                childBufferStart = newBufferStart;
            }
            yield return null;
        }

        private int WrapChildIndex(int idx)
        {
            while (idx < 0)
                idx += childItems.Length;

            return idx % childItems.Length;
        }

        private float RowHeight()
        {
            float yHit = itemHeight;
            return RowPadding + yHit;
        }

        private float ViewportHeight()
        {
            return scrollRect.viewport.rect.height;
        }

        protected virtual void UpdateChild(LeaderBoardListViewItem child, int rowIdx)
        {
            if (rowIdx < 0 || rowIdx >= rowCount)
            {
                // Out of range of data, can happen
                child.gameObject.SetActive(false);
            }
            else
            {
                if (ItemCallback == null)
                {
                    UnityDebug.Debug.LogWarning("ItemCallback is null");
                    UnityDebug.Debug.LogWarning("LeaderBoardListView is missing an ItemCallback, cannot function", this);
                    return;
                }
                // Move to correct location
                //var childRect = leaderBoardPrefab.RectTransform.rect;
                //Vector2 pivot = leaderBoardPrefab.RectTransform.pivot;
                float ytoppos = RowHeight() * rowIdx;
                float ypos = ytoppos + (1f - 1f) * itemHeight;
                float xpos = 0 + 0.5f * 0;
                child.RectTransform.anchoredPosition = new Vector2(xpos, -ypos);
                child.NotifyCurrentAssignment(this, rowIdx);

                // Populate data
                ItemCallback(child, rowIdx);
                child.gameObject.SetActive(true);
            }
        }

        protected virtual void UpdateContentHeight()
        {
            float height = itemHeight * (rowCount) + (rowCount - 1) * RowPadding;
            // apparently 'sizeDelta' is the way to set w / h 
            var sz = scrollRect.content.sizeDelta;
            scrollRect.content.sizeDelta = new Vector2(sz.x, height);
        }

        protected virtual void DisableAllChildren()
        {
            if (childItems != null)
            {
                for (int i = 0; i < childItems.Length; ++i)
                {
                    childItems[i].gameObject.SetActive(false);
                }
            }
        }
    }
}

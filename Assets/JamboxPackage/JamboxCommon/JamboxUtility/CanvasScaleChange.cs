using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jambox.Common.Utility
{
    public class CanvasScaleChange : MonoBehaviour
    {
        private CanvasScaler scaler;
        public float value;
        public bool auto;

        private void Awake()
        {
            scaler = GetComponent<CanvasScaler>();
            if (auto)
                TabletCheck();
        }

        #region Tablet_Checks
        public void TabletCheck()
        {
            if (TabletDetect.IsTablet())
            {
                SetToTabletView();
            }
            else
            {
                SetToDefault();
            }
        }
        #endregion

        public void SetToDefault()
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        }

        public void SetToTabletView()
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            scaler.scaleFactor = value;
        }

        public void SetToTabletView(float _custom)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            scaler.scaleFactor = _custom;
        }

    }
}
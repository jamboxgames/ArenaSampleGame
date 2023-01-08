using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SafeAreaFit : MonoBehaviour
{

    public static UnityEvent OnResolutionOrOrientationChanged = new UnityEvent();

    private bool screenChangeVarsInitialized = false;
    private ScreenOrientation lastOrientation = ScreenOrientation.Portrait;
    private Vector2 lastResolution = Vector2.zero;
    private Rect lastSafeArea = Rect.zero;

    private RectTransform rectTransform;

    void Awake()
    {

        rectTransform = GetComponent<RectTransform>();

        if (!screenChangeVarsInitialized)
        {
            lastOrientation = Screen.orientation;
            lastResolution.x = Screen.width;
            lastResolution.y = Screen.height;
            lastSafeArea = Screen.safeArea;

            screenChangeVarsInitialized = true;
        }

        ApplySafeArea();
    }

    void Update()
    {

        if (Application.isMobilePlatform && Screen.orientation != lastOrientation)
            OrientationChanged();

        if (Screen.safeArea != lastSafeArea)
            SafeAreaChanged();

        if (Screen.width != lastResolution.x || Screen.height != lastResolution.y)
            ResolutionChanged();
    }

    void ApplySafeArea()
    {
        if (rectTransform == null)
            return;

        var safeArea = Screen.safeArea;

        var anchorMin = safeArea.position;
        var anchorMax = safeArea.position + safeArea.size;

        Canvas canvas = GetComponentInParent<Canvas>();
        anchorMin.x /= canvas.pixelRect.width;
        anchorMin.y /= canvas.pixelRect.height;
        anchorMax.x /= canvas.pixelRect.width;
        anchorMax.y /= canvas.pixelRect.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }

    private void OrientationChanged()
    {
        //Debug.Log("Orientation changed from " + lastOrientation + " to " + Screen.orientation + " at " + Time.time);

        lastOrientation = Screen.orientation;
        lastResolution.x = Screen.width;
        lastResolution.y = Screen.height;

        OnResolutionOrOrientationChanged.Invoke();
    }

    private void ResolutionChanged()
    {
        //Debug.Log("Resolution changed from " + lastResolution + " to (" + Screen.width + ", " + Screen.height + ") at " + Time.time);

        lastResolution.x = Screen.width;
        lastResolution.y = Screen.height;

        OnResolutionOrOrientationChanged.Invoke();
    }

    private void SafeAreaChanged()
    {
        // Debug.Log("Safe Area changed from " + lastSafeArea + " to " + Screen.safeArea.size + " at " + Time.time);

        lastSafeArea = Screen.safeArea;
    }
}
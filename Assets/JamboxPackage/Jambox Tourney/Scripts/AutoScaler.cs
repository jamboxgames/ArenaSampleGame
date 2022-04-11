using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoScaler : MonoBehaviour
{

    public float minScale;
    public float maxScale;

    public Vector2 minResolution;
    public Vector2 maxResolution;

    private float minRatio;
    private float maxRatio;
    private float currentRatio;

    private void Start()
    {
        currentRatio = (float)Screen.height / Screen.width;
        minRatio = minResolution.x / minResolution.y;
        maxRatio = maxResolution.x / maxResolution.y;
        SetRequiredScale();
    }

    private void Update()
    {
        /*
        if (currentRatio != (float)(Screen.height/Screen.width))
        {
            currentRatio = (float)Screen.height / Screen.width;
            SetRequiredScale();
        }*/
    }

    void SetRequiredScale()
    {
        float diffInScale = maxScale - minScale;
        float diffInRatio = maxRatio - minRatio;

        float heightDiffScale = (currentRatio - minRatio) / diffInRatio * diffInScale;
        float requiredScale = minScale + heightDiffScale;

        transform.localScale = new Vector3(requiredScale, requiredScale, 1f);

    }

}

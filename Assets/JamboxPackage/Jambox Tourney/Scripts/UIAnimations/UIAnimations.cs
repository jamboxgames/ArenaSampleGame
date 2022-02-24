using Jambox.Common.Utility;
using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;

public class UIAnimations : MonoSingleton<UIAnimations>
{

    #region SCALE_IN
    private Coroutine _ScaleAnim;
    public ScaleInAnimation ScaleIn(RectTransform _obj, Vector3 _start, Vector3 _end, float _duration)
    {
        ScaleInAnimation anim = new ScaleInAnimation(_start, _end, _duration);
        _ScaleAnim = StartCoroutine(StartScaleInAnimation(_obj, anim));
        return anim;
    }

    IEnumerator StartScaleInAnimation(RectTransform obj, ScaleInAnimation data)
    {
        while (data.timer <= data.duration)
        {
            data.timer += Time.deltaTime;
            float timeDone = data.timer / data.duration;
            Vector3 scaleToBeDone = (data.endScale - data.startScale) * timeDone;
            obj.localScale = data.startScale + scaleToBeDone;
            yield return null;
        }
        data.done = true;
        obj.localScale = data.endScale;
    }
    #endregion

    #region TEXT_CHANGE
    private Coroutine _updateText;
    public UpdateTextAnimation ChangeTextOverTime(Text _obj, int _startValue, int _endValue,float _duration, bool _increase, bool _blinkScale = false, float _maxFontSize = 0f, float _minFontSize = 0f, float _rate = 0f)
    {
        UpdateTextAnimation anim = new UpdateTextAnimation(_startValue, _endValue, _duration);
        _updateText = StartCoroutine(StartUpdateTextAnimation(_obj, anim, _increase, _blinkScale, _maxFontSize, _minFontSize, _rate));
        return anim;
    }

    IEnumerator StartUpdateTextAnimation(Text obj, UpdateTextAnimation data, bool _increase, bool _blinkScale, float _maxFontSize, float _minFontSize, float _rate)
    {
        int _orgFontSize = obj.fontSize;
        float _fontSize = obj.fontSize;
        while (data.timer <= data.duration)
        {
            data.timer += Time.deltaTime;
            float timeDone = data.timer / data.duration;
            data.doneRatio = timeDone;
            int updateDone;
            if (_increase)
            {
                updateDone = (int)((data.endValue - data.startValue) * timeDone);
                obj.text = (data.startValue + updateDone) + "";
            }
            else
            {
                updateDone = (int)((data.startValue - data.endValue) * timeDone);
                obj.text = (data.startValue - updateDone) + "";
            }

            if (_blinkScale)
            {
                //Text Blink Animation
                _fontSize += (_rate * Time.deltaTime);
                obj.fontSize = (int)_fontSize;
                if (obj.fontSize >= _maxFontSize || obj.fontSize <= _minFontSize)
                {
                    _rate = _rate * -1;
                }
            }

            yield return null;
        }
        data.done = true;
        obj.text = data.endValue + "";
        obj.fontSize = _orgFontSize;
    }

    private void OnDisable()
    {
        if(_updateText != null)
            StopCoroutine(_updateText);
        if (_ScaleAnim != null)
            StopCoroutine(_ScaleAnim);
    }
    #endregion
}

public class ScaleInAnimation
{

    public Vector3 startScale;
    public Vector3 endScale;
    public float duration;
    public float timer = 0f;
    public bool done = false;

    public ScaleInAnimation(Vector3 _start, Vector3 _end,float _duration)
    {
        startScale = _start;
        endScale = _end;
        duration = _duration;
    }

}

public class UpdateTextAnimation
{

    public int startValue;
    public int endValue;
    public float duration;
    public float doneRatio = 0f;
    public float timer = 0f;
    public bool done = false;

    public UpdateTextAnimation(int _startValue, int _endValue, float _duration)
    {
        startValue = _startValue;
        endValue = _endValue;
        duration = _duration;
    }

}
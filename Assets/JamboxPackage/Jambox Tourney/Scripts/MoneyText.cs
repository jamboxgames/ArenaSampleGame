using System;
using UnityEngine;
using UnityEngine.UI;

public class MoneyText : MonoBehaviour
{

    public int maxLimit;

    private Text text;
    private string previousValue = "0";
    private void Awake()
    {
        if (text == null)
        {
            text = GetComponent<Text>();
        }
    }

    private void Update()
    {
        if (text.text != previousValue)
        {
            Modify();
        }
    }

    void Modify()
    {
        string _prefixString = null;
        string _requiredString = null;
        for(int i = 0; i < text.text.Length; i++)
        {
            if (System.Char.IsDigit(text.text[i]))
            {
                _requiredString = text.text.Substring(i);
                if (i > 0)
                {
                    _prefixString = text.text.Substring(0, i);
                }
                else
                {
                    _prefixString = "";
                }
                break;
            }
            if (i == text.text.Length - 1)
            {
                UnityDebug.Debug.LogError("NO DIGIT");
                return;
            }
        }

        long _money;
        if(long.TryParse(_requiredString,out _money))
        {
            if (_money > maxLimit)
            {
                if (_money < 1000000)
                {
                    string _s = Convert.ToString(_money / 1000f);
                    if (_s.Contains("."))
                    {
                        for (int i = 0; i < _s.Length; i++)
                        {
                            if (_s[i] == '.')
                            {
                                if (_s[i + 1] == '0')
                                {
                                    text.text = _prefixString + _s.Substring(0, i) + "K";
                                }
                                else
                                {
                                    text.text = _prefixString + _s.Substring(0, i) + "." + _s[i + 1] + "K";
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        text.text = _prefixString + _s + "K";
                    } 
                }
                else if(_money>=1000000&& _money < 1000000000)
                {
                    string _s = Convert.ToString(_money / 1000000f);
                    if (_s.Contains("."))
                    {
                        for (int i = 0; i < _s.Length; i++)
                        {
                            if (_s[i] == '.')
                            {
                                if (_s[i + 1] == '0')
                                {
                                    text.text = _prefixString + _s.Substring(0, i) + "M";
                                }
                                else
                                {
                                    text.text = _prefixString + _s.Substring(0, i) + "." + _s[i + 1] + "M";
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        text.text = _prefixString + _s + "M";
                    }
                }
                else if (_money >= 1000000000)
                {
                    string _s = Convert.ToString(_money / 1000000000f);
                    if (_s.Contains("."))
                    {
                        for (int i = 0; i < _s.Length; i++)
                        {
                            if (_s[i] == '.')
                            {
                                if (_s[i + 1] == '0')
                                {
                                    text.text = _prefixString + _s.Substring(0, i) + "B";
                                }
                                else
                                {
                                    text.text = _prefixString + _s.Substring(0, i) + "." + _s[i + 1] + "B";
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        text.text = _prefixString + _s + "B";
                    }
                }
            }
        }
        else
        {
            UnityDebug.Debug.LogError("INVALID MONEY");
        }
        previousValue = text.text;
    }

}

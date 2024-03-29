using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetAspectRatio : MonoBehaviour
{

    public Image image;
    public AspectRatioFitter fitter;

    private void Start()
    {
        FindAspectRatio();
    }

    public void FindAspectRatio()
    {
        if (image != null)
        {
            StartCoroutine(WaitForSpriteToBeSet());
        }
    }

    IEnumerator WaitForSpriteToBeSet()
    {
        while (image.sprite == null)
            yield return null;

        Texture _texture = image.mainTexture;
        float _value = (float)_texture.width / (float)_texture.height;

        if (fitter != null)
            fitter.aspectRatio = _value;
    }

}

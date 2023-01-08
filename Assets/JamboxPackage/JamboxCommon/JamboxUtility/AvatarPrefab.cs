using System.Collections;
using System.Collections.Generic;
using Jambox.Common.Utility;
using UnityEngine;
using UnityEngine.UI;

public class AvatarPrefab : MonoBehaviour
{

    public Image avatarImage;
    public GameObject HighlightedImage;
    public int avatarId;
    private UpdatePlayerDetails playerDetails;

    private void OnEnable()
    {
        HighlightedImage.SetActive(false);
    }

    public void SetData(Sprite _sprite, int _id, UpdatePlayerDetails _ref)
    {
        avatarImage.sprite = _sprite;
        avatarId = _id;
        playerDetails = _ref;
    }

    public void Selected()
    {
        playerDetails.ImageSelected(this);
        HighlightedImage.SetActive(true);
    }

    public void Deselect()
    {
        HighlightedImage.SetActive(false);
    }

    public void EnableHightlight()
    {
        HighlightedImage.SetActive(true);
    }
}

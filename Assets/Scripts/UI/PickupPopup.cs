using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupPopup : MonoBehaviour
{
    [Header("Elements")]
    public Transform StartingPoint;
    public Transform EndPoint;
    public Transform PopupParent;
    [Header("Popup")]
    public Image PopupBackground;
    public Image ItemIcon;
    public Text ItemName;
    public Text ItemCount;
    [Header("Effect")]
    public float MovementMultiplier = 1.0f;
    private float _step, _time;

    private enum State
    {
        FadeIn,
        Idle,
        FadeOut
    };
    private State _state;

    void Update()
    {
        if (_step < 1.0f)
        {
            // Interpolate the popup appearing in screen to its final position
            _time += Time.deltaTime * MovementMultiplier;
            _step = _time / 2.0f;
            _step = _step * _step * (3f - 2f * _step);

            PopupParent.transform.position = Vector3.Lerp(StartingPoint.position, EndPoint.position, _step);
            _step += Time.deltaTime;
            FadePopup(_step);
        }
    }

    void FadePopup(float fadeFactor)
    {
        // Fade the popup
        PopupBackground.color = new Color(PopupBackground.color.r, PopupBackground.color.g, PopupBackground.color.b, fadeFactor);
        ItemIcon.color = new Color(ItemIcon.color.r, ItemIcon.color.g, ItemIcon.color.b, fadeFactor);
        ItemName.color = new Color(ItemName.color.r, ItemName.color.g, ItemName.color.b, fadeFactor);
        ItemCount.color = new Color(ItemCount.color.r, ItemCount.color.g, ItemCount.color.b, fadeFactor);
    }

    void DisplayPickup()
    {

    }
}

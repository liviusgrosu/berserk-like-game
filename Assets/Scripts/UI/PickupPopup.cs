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
    public float FadeInTime = 1.0f;
    public float IdleTime = 2.0f;
    public float FadeOutTime = 1.0f;

    private enum PopupState
    {
        None,
        FadeIn,
        Idle, 
        FadeOut
    };
    private PopupState _state;

    void Start()
    {
        _state = PopupState.None;
        FadePopup(0.0f);
    }

    void Update()
    {
        // Fade in the popup
        if (_state == PopupState.FadeIn)
        {
            _time += Time.deltaTime * MovementMultiplier;

            PopupParent.transform.position = Vector3.Lerp(StartingPoint.position, EndPoint.position, _time / FadeInTime);
            FadePopup(_time / FadeInTime);

            if (_time >= FadeInTime)
            {
                _state = PopupState.Idle;
                _time = 0.0f;
            }
        }

        // Keep the popup idling
        if (_state == PopupState.Idle)
        {
            _time += Time.deltaTime;
            if (_time >= IdleTime)
            {
                _state = PopupState.FadeOut;
                _time = 0.0f;
            }
        }

        // Fade out the popup
        if (_state == PopupState.FadeOut)
        {
            _time += Time.deltaTime;
            FadePopup(1.0f - _time / FadeOutTime);
            if (_time >= FadeOutTime)
            {
                _state = PopupState.None;
            }
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

    public void DisplayPickup(Sprite icon, string itemName, int amount)
    {
        // Fill in the popup information
        ItemIcon.sprite = icon;
        ItemName.text = itemName;
        ItemCount.text = amount.ToString();
        
        // Start displaying
        _state = PopupState.FadeIn;
        _time = 0.0f;
        FadePopup(0.0f);
    }
}

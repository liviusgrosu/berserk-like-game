using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePopup : MonoBehaviour
{
    [Header("Elements")]
    public Transform StartingPoint;
    public Transform EndPoint;
    public Transform PopupParent;
    [Header("Popup")]
    public Image PopupBackground;
    public Text NextLineIndicator;
    public Text NPCName;
    public Text DialogueText;
    [Header("Effect")]
    public float MovementMultiplier = 1.0f;
    private float _time;
    public float FadeInTime = 1.0f;
    private bool _fadeIn;

    void Start()
    {
        FadePopup(0.0f);
    }

    void Update()
    {
        // Fade in the popup
        if (_fadeIn)
        {
            _time += Time.deltaTime * MovementMultiplier;

            PopupParent.transform.position = Vector3.Lerp(StartingPoint.position, EndPoint.position, _time / FadeInTime);
            FadePopup(_time / FadeInTime);

            if (_time >= FadeInTime)
            {
                _fadeIn = false;
                _time = 0.0f;
            }
        }
    }

    void FadePopup(float fadeFactor)
    {
        // Fade the popup
        PopupBackground.color = new Color(PopupBackground.color.r, PopupBackground.color.g, PopupBackground.color.b, fadeFactor);
        NPCName.color = new Color(NPCName.color.r, NPCName.color.g, NPCName.color.b, fadeFactor);
        NextLineIndicator.color = new Color(NextLineIndicator.color.r, NextLineIndicator.color.g, NextLineIndicator.color.b, fadeFactor);
        DialogueText.color = new Color(DialogueText.color.r, DialogueText.color.g, DialogueText.color.b, fadeFactor);
    }

    public void HidePopup()
    {
        FadePopup(0.0f);
    }

    public void DisplayPickup(string npcName)
    {
        NPCName.text = npcName;
        // Fill in the popup information
        _fadeIn = true;
        _time = 0.0f;
        FadePopup(0.0f);
    }

    public void InsertDialogue(string dialogueText)
    {
        DialogueText.text = dialogueText;
    }
}

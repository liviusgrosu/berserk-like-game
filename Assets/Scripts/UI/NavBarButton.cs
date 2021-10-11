using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavBarButton : MonoBehaviour
{
    public MenuController.Menu MenuToSwitchTo;
    private Button _button;
    private ColorBlock _colours;

    void Awake()
    {
        _button = GetComponent<Button>();
        _colours = _button.colors;
    }

    public void ChangeSelected(bool toggleState)
    {
        _button.interactable = !toggleState;
        Color newColour = toggleState ? _colours.selectedColor : _colours.normalColor;
        GetComponent<Image>().color = newColour;
    }
}

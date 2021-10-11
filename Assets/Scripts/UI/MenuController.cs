using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Serializable]
    public enum Menu
    {
        WeaponUpgrade,
        CharacterSkills,
        Overlay
    }
    [HideInInspector]
    public Menu _currentMenu;

    public GameObject OverlayUI, WeaponUpgradeMenu, CharacterSkillMenu, NavBar;

    void Awake()
    {
        _currentMenu = Menu.Overlay;
        OverlayUI.SetActive(true);
    }

    void Update()
    {
        // Open the weapon upgrade menu
        if (Input.GetKeyDown(KeyCode.U))
        {
            ChangeMenu(Menu.WeaponUpgrade);
        }

        // Open the weapon upgrade menu
        if (Input.GetKeyDown(KeyCode.I))
        {
            ChangeMenu(Menu.CharacterSkills);
        }

        // Go back to the overlay menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChangeMenu(Menu.Overlay);
        }
    }

    private void DeactivateMenus()
    {
        NavBar.SetActive(false);
        OverlayUI.SetActive(false);
        WeaponUpgradeMenu.SetActive(false);
        CharacterSkillMenu.SetActive(false);
    }

    public void ChangeMenu(NavBarButton NavBarButton)
    {
        // Nav bar button press handler
        ChangeMenu(NavBarButton.MenuToSwitchTo);
    }

    public void ChangeMenu(Menu menu)
    {
        DeactivateMenus();
        switch (menu)
        {
            case Menu.CharacterSkills:
                _currentMenu = Menu.CharacterSkills;
                CharacterSkillMenu.SetActive(true);
                NavBar.SetActive(true);
                break;
            case Menu.Overlay:
                _currentMenu = Menu.Overlay;
                OverlayUI.SetActive(true);
                break;
            case Menu.WeaponUpgrade:
                _currentMenu = Menu.WeaponUpgrade;
                WeaponUpgradeMenu.SetActive(true);
                NavBar.SetActive(true);
                break;
            default:
                break;
        }
    }
}

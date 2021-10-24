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
    public NavBarButton CharacterSkillsBtn, WeaponUpgradeBtn;

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
        // Deactivate all the menus
        NavBar.SetActive(false);
        OverlayUI.SetActive(false);
        WeaponUpgradeMenu.SetActive(false);
        CharacterSkillMenu.SetActive(false);
    }

    private void UpdateNavBar()
    {
        // Refresh the selections
        CharacterSkillsBtn.ChangeSelected(false);
        WeaponUpgradeBtn.ChangeSelected(false);

        // Toggle the current menu button
        switch(_currentMenu)
        {
            case Menu.CharacterSkills:
                CharacterSkillsBtn.ChangeSelected(true);
                break;
            case Menu.WeaponUpgrade:
                WeaponUpgradeBtn.ChangeSelected(true);
                break;
            default:
                break;
        }
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
                // Toggle all the character skills UI elements
                _currentMenu = Menu.CharacterSkills;
                CharacterSkillMenu.SetActive(true);
                NavBar.SetActive(true);
                UpdateNavBar();
                break;
            case Menu.Overlay:
                // Toggle all overlay UI elements
                _currentMenu = Menu.Overlay;
                OverlayUI.SetActive(true);
                break;
            case Menu.WeaponUpgrade:
                // Toggle all the weapon upgrade UI elements
                _currentMenu = Menu.WeaponUpgrade;
                WeaponUpgradeMenu.SetActive(true);
                NavBar.SetActive(true);
                UpdateNavBar();
                break;
            default:
                break;
        }
    }

    public bool IsMenuOpen()
    {
        // Check if menu is open or not
        return _currentMenu != Menu.Overlay;
    }
}

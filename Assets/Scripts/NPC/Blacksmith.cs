using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blacksmith : MonoBehaviour, IInteract
{
    private MenuController _menuController;

    void Start()
    {
        _menuController = GameObject.Find("Main Canvas").GetComponent<MenuController>();
    }
    
    public void Interact()
    {
        // Toggle the weapon upgrade tree with write mode turned on
        _menuController.ChangeMenu(MenuController.Menu.WeaponUpgrade, true);
    }
}

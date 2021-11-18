using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blacksmith : MonoBehaviour, IInteract, INPC
{
    private MenuController _menuController;
    public DialogueManager DialogueManager;

    void Start()
    {
        _menuController = GameObject.Find("Main Canvas").GetComponent<MenuController>();
        DialogueManager = new DialogueManager("BlacksmithDialogue");
    }
    
    public void Interact()
    {
        // Toggle the weapon upgrade tree with write mode turned on
        //_menuController.ChangeMenu(MenuController.Menu.WeaponUpgrade, true);

        DialogueManager.Talk();
    }

    public DialogueManager GetDialogueManager()
    {
        return DialogueManager;
    }
}

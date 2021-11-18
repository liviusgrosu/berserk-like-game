using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blacksmith : MonoBehaviour, IInteract, INPC
{
    private MenuController _menuController;
    public DialogueManager DialogueManager;
    private DialoguePopup _dialoguePopup;

    void Start()
    {
        _menuController = GameObject.Find("Main Canvas").GetComponent<MenuController>();
        DialogueManager = new DialogueManager("BlacksmithDialogue");
        _dialoguePopup = GameObject.Find("Dialogue Popup").GetComponent<DialoguePopup>();
    }
    
    public void Interact()
    {
        // Don't interact if the weapon upgrade menu is open
        if (_menuController.IsMenuOpen())
        {
            return;
        }

        if (!DialogueManager.Talking)
        {
            // If its the first time talking to the NPC then start displaying dialogue
            DialogueManager.Talking = true;
            _dialoguePopup.DisplayPickup(gameObject.name);
            _dialoguePopup.InsertDialogue(DialogueManager.GetNextLine());
        }
        else
        {
            // Get the next dialogue line and display
            string nextLine = DialogueManager.GetNextLine();
            if (nextLine == "")
            {
                // Close the dialogue popup and open the weapon upgrade menu
                DialogueManager.Talking = false;
                _dialoguePopup.HidePopup();
                _menuController.ChangeMenu(MenuController.Menu.WeaponUpgrade, true);
                return;
            }
            _dialoguePopup.InsertDialogue(nextLine);
        }
    }

    public DialogueManager GetDialogueManager()
    {
        return DialogueManager;
    }
}

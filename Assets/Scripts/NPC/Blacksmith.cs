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
        if (_menuController.IsMenuOpen())
        {
            return;
        }
        
        if (!DialogueManager.Talking)
        {
            DialogueManager.Talking = true;
            _dialoguePopup.DisplayPickup(gameObject.name);
            _dialoguePopup.InsertDialogue(DialogueManager.GetNextLine());
        }
        else
        {
            string nextLine = DialogueManager.GetNextLine();
            if (nextLine == "")
            {
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

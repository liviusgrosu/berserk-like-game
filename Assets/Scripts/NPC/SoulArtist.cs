using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulArtist : MonoBehaviour, IInteract
{
    private MenuController _menuController;
        private QuestManager _questManager;

    void Start()
    {
        _menuController = GameObject.Find("Main Canvas").GetComponent<MenuController>();
        _questManager = GameObject.Find("Game Manager").GetComponent<QuestManager>();
    }
    
    public void Interact()
    {
        // Toggle the weapon upgrade tree with write mode turned on
        // _menuController.ChangeMenu(MenuController.Menu.CharacterSkills, true);
        _questManager.TriggerEvent(QuestObjective.Type.Talk, name);
    }
}

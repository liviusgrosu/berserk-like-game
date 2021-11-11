using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mayor : MonoBehaviour, IInteract
{
    private QuestManager _questManager;

    void Start()
    {
        _questManager = GameObject.Find("Game Manager").GetComponent<QuestManager>();
    }   
    public void Interact()
    {
        _questManager.TriggerEvent(QuestObjective.Type.Talk, name);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootBehaviour : MonoBehaviour, IInteract
{
    public GameObject LootData;
    public int Amount;
    private Inventory Inventory;

    void Start()
    {
        Inventory = GameObject.Find("Game Manager").GetComponent<Inventory>();
    }

    public void Interact()
    {
        // Add the loot to the players inventory
        Inventory.AddLoot(LootData, Amount);
        Destroy(gameObject);
    }
}

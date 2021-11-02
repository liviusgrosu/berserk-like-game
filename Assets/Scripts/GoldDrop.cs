using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldDrop : MonoBehaviour
{
    public int amount;

    void Start()
    {
        // TODO: add a random amount depending on the players level
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.name == "Player")
        {
            GameObject.Find("Game Manager").GetComponent<LootManager>().Coins += amount;
            Destroy(this.gameObject);
        }
    }
}

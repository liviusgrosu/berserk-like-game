using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityDropLoot : MonoBehaviour
{
    public Transform SpawnPoint;
    public GameObject LootData;
    public GameObject LootContainerPrefab;

    public void DropLoot()
    {
        if (LootData != null)
        {
            // Drop loot container and save the loot item into it
            GameObject loot = Instantiate(LootContainerPrefab, SpawnPoint.position, LootContainerPrefab.transform.rotation);
            loot.GetComponent<LootBehaviour>().LootData = LootData;
        }
    }
}

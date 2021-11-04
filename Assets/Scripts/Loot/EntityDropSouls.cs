using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityDropSouls : MonoBehaviour
{
    public int SoulAmount;
    public Transform SpawnPoint;
    private LootManager _lootManager;

    void Start()
    {
        _lootManager = GameObject.Find("Game Manager").GetComponent<LootManager>();
    }

    public void DropSouls()
    {
        GameObject soulPrefab;

        if (SoulAmount < _lootManager.MediumSoulStartAmount)
        {
            soulPrefab = _lootManager.SmallSoulPrefab; 
        }
        else if(SoulAmount >= _lootManager.MediumSoulStartAmount && SoulAmount < _lootManager.LargeSoulStartAmount)
        {
            soulPrefab = _lootManager.MediumSoulPrefab;
        }
        else
        {
            soulPrefab = _lootManager.LargeSoulPrefab;
        }

        GameObject soul = Instantiate(soulPrefab, SpawnPoint.position, soulPrefab.transform.rotation);
        soul.GetComponent<SoulBehaviour>().SoulAmount = SoulAmount;
    }    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityDropLoot : MonoBehaviour
{
    // TODO: 
    private int CoinPileAmount = 3;
    public GameObject CoinPilePrefab;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            // Pick a random direction 
            // instantiate it and go in that direction

            for(int i = 0; i < CoinPileAmount; i++)
            {
                //GameObject loot = Instantiate(CoinPilePrefab, transform.position, CoinPilePrefab.transform.rotation);
                
                float xDirection = Random.Range(-1.0f, 1.0f);
                float zDirection = Random.Range(-1.0f, 1.0f);

                Vector3 randomDirection = new Vector3(xDirection, 0f, zDirection).normalized * 2f;
            
                RaycastHit hit;
                if (Physics.Raycast(transform.position + randomDirection, -Vector3.up, out hit, 10.0f))
                {
                    GameObject loot = Instantiate(CoinPilePrefab, hit.point, CoinPilePrefab.transform.rotation);
                }
            }
        }
    }
}

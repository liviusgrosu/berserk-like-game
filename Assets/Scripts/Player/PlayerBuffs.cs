using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuffs : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> Effects;
    public Transform BuffParent;

    public void AddBuff(GameObject effect)
    {
        // Instantiate the effect and add it to the list
        GameObject newEffect = Instantiate(effect, BuffParent.position, Quaternion.identity);
        newEffect.transform.parent = BuffParent;
        newEffect.GetComponent<IStatEffect>().ProvideStats(GetComponent<EntityStats>());
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuffs : MonoBehaviour
{
    public List<GameObject> Effects;
    public Transform BuffParent;

    // TODO: change this to AddEffect
    public void AddBuff(GameObject effect)
    {
        // Instantiate the effect and add it to the list
        GameObject newEffect = Instantiate(effect, BuffParent.position, Quaternion.identity);
        Effects.Add(newEffect);
        newEffect.transform.parent = BuffParent;
        newEffect.GetComponent<IStatEffect>().ProvideStats(GetComponent<EntityStats>(), RemoveEffect);
    }

    private void RemoveEffect(GameObject effect)
    {
        // Remove the effect
        Effects.Remove(effect);
    }
}
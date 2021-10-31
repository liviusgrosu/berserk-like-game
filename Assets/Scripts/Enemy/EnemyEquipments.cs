using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class EnemyEquipments : MonoBehaviour, IEquipment
public class EnemyEquipments : MonoBehaviour
{
    public GameObject Weapon;

    public Transform Hand;

    void Awake()
    {
        Equip();
    }

    void Equip()
    {
        // Create the model object
        GameObject weaponEquipment = Instantiate(Weapon, Hand.position, Weapon.transform.rotation);
        weaponEquipment.transform.parent = Hand;
    }

    public WeaponStats GetCurrentEquipmentStats()
    {
        return Weapon.GetComponent<WeaponEquipment>().Stats;
    }
}

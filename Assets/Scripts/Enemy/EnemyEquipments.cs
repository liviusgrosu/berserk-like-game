using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEquipments : MonoBehaviour, IEquipment
{
    public GameObject TempCurrentEquipment;

    public Transform Hand;

    void Awake()
    {
        Equip();
    }

    void Equip()
    {
        GameObject equipmentModel = Instantiate(TempCurrentEquipment.GetComponent<Equipment>().ModelPrefab, Hand.position, TempCurrentEquipment.GetComponent<Equipment>().ModelPrefab.transform.rotation);
        equipmentModel.transform.parent = Hand;
        // Load default stats
        TempCurrentEquipment.GetComponent<Equipment>().LoadDefaultStats();
        // Give the weapon collider the equipment stats
        equipmentModel.GetComponent<WeaponCollider>().AssignData(TempCurrentEquipment.GetComponent<Equipment>());
    }

    public EquipmentStats GetCurrentEquipmentStats()
    {
        return TempCurrentEquipment.GetComponent<Equipment>().Stats;
    }
}

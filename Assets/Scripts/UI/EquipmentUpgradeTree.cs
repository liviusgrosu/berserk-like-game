using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

using Utility;

class EquipmentUpgradeTree : MonoBehaviour
{
    public string EquipmentName;
    public PlayerEquipments PlayerEquipment;
    private GameObject _equipmentObj;
    public LootManager lootManager;
    public bool _writeMode = true;

    [Header("Stats Text")]
    public Text GemsText;

    public GameObject TextPrefab;
    public RectTransform TextRoot;
    private List<GameObject> _statTexts;

    void Awake()
    {
        _statTexts = new List<GameObject>();
    }

    void Start()
    {
        // Find the equipment
        foreach(GameObject equipment in PlayerEquipment.EquipmentIntances)
        {
            if (equipment.name == EquipmentName)
            {
                _equipmentObj = equipment; 
            }
        }

        // Draw the texts for the base equipment class
        CreateText(_equipmentObj.GetComponent<Equipment>().Stats.Durability);

        // Draw the texts for the specific equipment child class
        switch (_equipmentObj.GetComponent<Equipment>().Type)
        {
            case (Equipment.EquipmentType.Weapon):
                CreateText(((WeaponStats)_equipmentObj.GetComponent<Equipment>().Stats).Damage);
                CreateText(((WeaponStats)_equipmentObj.GetComponent<Equipment>().Stats).AttackSpeed);
                break;
            case (Equipment.EquipmentType.Shield):
                CreateText(((ShieldStats)_equipmentObj.GetComponent<Equipment>().Stats).Defence);
                break;
            default:
                break;
        }
    }

    void CreateText(float stat)
    {
        // Establish new text position and instantiate it 
        Vector3 newPosition = TextRoot.position - new Vector3(0, _statTexts.Count() * 100f, 0);
        GameObject newTextPrefab = Instantiate(TextPrefab, newPosition, TextPrefab.transform.rotation);
        newTextPrefab.transform.parent = TextRoot;
        // Update the text field
        newTextPrefab.GetComponent<Text>().text = $"Durability: {stat}";
        _statTexts.Add(newTextPrefab);
    }

    void UpdateText()
    {

    }
}
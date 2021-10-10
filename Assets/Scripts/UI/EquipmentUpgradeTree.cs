using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

using UIUtility;

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
    private List<StatText> _statTexts;

    void Awake()
    {
        _statTexts = new List<StatText>();
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
        CreateText("Durability", _equipmentObj.GetComponent<Equipment>().Stats.Durability);

        // Draw the texts for the specific equipment child class
        switch (_equipmentObj.GetComponent<Equipment>().Type)
        {
            case (Equipment.EquipmentType.Weapon):
                CreateText("Damage", ((WeaponStats)_equipmentObj.GetComponent<Equipment>().Stats).Damage);
                CreateText("Attack Speed", ((WeaponStats)_equipmentObj.GetComponent<Equipment>().Stats).AttackSpeed);
                break;
            case (Equipment.EquipmentType.Shield):
                CreateText("Defence", ((ShieldStats)_equipmentObj.GetComponent<Equipment>().Stats).Defence);
                break;
            default:
                break;
        }
    }

    void CreateText(string statName, float statValue)
    {
        // Establish new text position and instantiate it 
        Vector3 newPosition = TextRoot.position - new Vector3(0, _statTexts.Count() * 100f, 0);
        GameObject newTextPrefab = Instantiate(TextPrefab, newPosition, TextPrefab.transform.rotation);
        newTextPrefab.transform.parent = TextRoot;
        // Update the text field
        UpdateTextElement(newTextPrefab, statName, statValue);
        _statTexts.Add(new StatText(statName, newTextPrefab));
    }

    void RefreshTextValues()
    {
        // Update each text elements
        foreach(StatText statText in _statTexts)
        {
            switch(statText.TextFieldName)
            {
                case "Durability":
                    UpdateTextElement(statText.TextObj, statText.TextFieldName, _equipmentObj.GetComponent<Equipment>().Stats.Durability);
                    break;
                case "Damage":
                    UpdateTextElement(statText.TextObj, statText.TextFieldName, ((WeaponStats)_equipmentObj.GetComponent<Equipment>().Stats).Damage);
                    break;
                case "Attack Speed":
                    UpdateTextElement(statText.TextObj, statText.TextFieldName, ((WeaponStats)_equipmentObj.GetComponent<Equipment>().Stats).AttackSpeed);
                    break;
                case "Defence":
                    UpdateTextElement(statText.TextObj, statText.TextFieldName, ((ShieldStats)_equipmentObj.GetComponent<Equipment>().Stats).Defence);
                    break;
            }
        }
    }

    void UpdateTextElement(GameObject textObj, string statFieldName, float statValue)
    {
        // Update the text element
        textObj.GetComponent<Text>().text = $"{statFieldName}: {statValue}";
    }
}
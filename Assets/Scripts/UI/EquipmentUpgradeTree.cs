using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

using GeneralUtility;
using UIUtility;

class EquipmentUpgradeTree : MonoBehaviour
{
    public string EquipmentName;
    public PlayerEquipments PlayerEquipment;
    private GameObject _equipmentObj;
    public LootManager LootManager;
    public bool _writeMode = true;
    public Text GemsText;
    public EquipmentUpgradeNode StartingNode;
    private List<GameObject> _upgradeTreeNodes;
    private ListHelper<EquipmentUpgradeNode> _listHelper;

    public GameObject TextPrefab;
    public RectTransform TextRoot;
    private List<StatText> _statTexts;

    void Awake()
    {
        _statTexts = new List<StatText>();
        _listHelper = new ListHelper<EquipmentUpgradeNode>();
        _upgradeTreeNodes = new List<GameObject>();
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

        // Add the starting node
        _equipmentObj.GetComponent<Equipment>().CurrentUpgrades.Add(StartingNode);

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

        // Refresh the text values
        RefreshTextValues();

        // Add each skill tree node into a list
        foreach(Transform child in transform)
        {
            if(child.GetComponent<EquipmentUpgradeNode>())
            {
                _upgradeTreeNodes.Add(child.gameObject);
            }
        }

        // Change their interactibility
        foreach(GameObject upgradeNode in _upgradeTreeNodes)
        {
            EquipmentUpgradeNode upgradeScript = upgradeNode.GetComponent<EquipmentUpgradeNode>();
            if(_equipmentObj.GetComponent<Equipment>().CheckIfUpgradeUnlocked(upgradeScript))
            {
                upgradeNode.GetComponent<Image>().color = Color.green;
                continue;
            }
            
            // Check if the skill has the prerequisites
            bool unlocked = false;
            bool sufficentGems = false;

            foreach(EquipmentUpgradeNode prerequiteUpgrade in upgradeScript.PrerequisiteUpgrades)
            {
                if (_equipmentObj.GetComponent<Equipment>().CheckIfUpgradeUnlocked(prerequiteUpgrade))
                {
                    unlocked = true;

                    // Check if the player can afford it
                    sufficentGems = true ? LootManager.Gems >= prerequiteUpgrade.Cost : false;

                    break;
                }
            }

            upgradeNode.GetComponent<Button>().interactable = unlocked && sufficentGems && _writeMode;
            upgradeNode.GetComponent<Image>().color = unlocked ? Color.yellow : Color.grey;
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
        // Display the experience text
        GemsText.text = $"Gems: {LootManager.Gems}";
    }

    void UpdateTextElement(GameObject textObj, string statFieldName, float statValue)
    { 
        // Update the text element
        textObj.GetComponent<Text>().text = $"{statFieldName}: {statValue}";
    }

    public void AddUpgrade(Transform upgrade)
    {
        // Decrease experience 
        LootManager.Gems -= upgrade.GetComponent<EquipmentUpgradeNode>().Cost;

        // Add to equipment stats
        _equipmentObj.GetComponent<Equipment>().AddUpgrade(upgrade.GetComponent<EquipmentUpgradeNode>());
        
        // Refresh the texts
        RefreshTextValues();

        // Update skill tree
        foreach(GameObject currentUpgrade in _upgradeTreeNodes)
        {
            if (!_equipmentObj.GetComponent<Equipment>().CurrentUpgrades.Contains(currentUpgrade.GetComponent<EquipmentUpgradeNode>()) &&
                _listHelper.CheckSublistExists(_equipmentObj.GetComponent<Equipment>().CurrentUpgrades, currentUpgrade.GetComponent<EquipmentUpgradeNode>().PrerequisiteUpgrades))
            {
                // Check if the player can afford it
                bool sufficentGems = true ? LootManager.Gems >= currentUpgrade.GetComponent<EquipmentUpgradeNode>().Cost : false;

                currentUpgrade.GetComponent<Image>().color = Color.yellow;
                currentUpgrade.GetComponent<Button>().interactable = sufficentGems && _writeMode;   
            }
        }

        // Change the UI
        upgrade.GetComponent<Button>().interactable = false;
        upgrade.GetComponent<Image>().color = Color.green;
    }
}
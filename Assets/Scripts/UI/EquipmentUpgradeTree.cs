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
    public Text SoulsText;
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

        // REM: need to get the stats 
        // TODO: for now this will only pull off of the weapon equipment class
        // Maybe include a type in this script to choose preemptively the correct class

        // Draw the texts for the base equipment class
        CreateText("Durability", _equipmentObj.GetComponent<WeaponEquipment>().Stats.Durability);

        // Draw the texts for the specific equipment child class
        CreateText("Damage", _equipmentObj.GetComponent<WeaponEquipment>().Stats.Damage);
        CreateText("Attack Speed", _equipmentObj.GetComponent<WeaponEquipment>().Stats.AttackSpeed);
        CreateText("Stamina Use", _equipmentObj.GetComponent<WeaponEquipment>().Stats.StaminaUse);


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
            if(_equipmentObj.GetComponent<WeaponEquipment>().CheckIfUpgradeUnlocked(upgradeScript))
            {
                upgradeNode.GetComponent<Image>().color = Color.green;
                continue;
            }
            
            // Check if the skill has the prerequisites
            bool unlocked = false;
            bool sufficentSouls = false;

            // Check that the prerequisited are obtained
            if (upgradeScript.PrerequisiteUpgrades.Count == 0 || CheckSublistExists(_equipmentObj.GetComponent<WeaponEquipment>().CurrentUpgradeIds,  upgradeScript.PrerequisiteUpgrades))
            {
                // Unlock the upgrade if all prerequisites are obtained
                unlocked = true;

                // Check if the player can afford it
                sufficentSouls = true ? LootManager.SoulCount >= upgradeScript.Cost : false;
            }

            upgradeNode.GetComponent<Button>().interactable = unlocked && sufficentSouls && _writeMode;
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
                    UpdateTextElement(statText.TextObj, statText.TextFieldName, _equipmentObj.GetComponent<WeaponEquipment>().Stats.Durability);
                    break;
                case "Damage":
                    UpdateTextElement(statText.TextObj, statText.TextFieldName, _equipmentObj.GetComponent<WeaponEquipment>().Stats.Damage);
                    break;
                case "Attack Speed":
                    UpdateTextElement(statText.TextObj, statText.TextFieldName, _equipmentObj.GetComponent<WeaponEquipment>().Stats.AttackSpeed);
                    break;
            }
        }
        // Display the souls text
        SoulsText.text = $"Souls: {LootManager.SoulCount}";
    }

    void UpdateTextElement(GameObject textObj, string statFieldName, float statValue)
    { 
        // Update the text element
        textObj.GetComponent<Text>().text = $"{statFieldName}: {statValue}";
    }

    public void AddUpgrade(Transform upgrade)
    {
        // Decrease souls 
        LootManager.SoulCount -= upgrade.GetComponent<EquipmentUpgradeNode>().Cost;

        // Add to equipment stats
        _equipmentObj.GetComponent<WeaponEquipment>().AddUpgrade(upgrade.GetComponent<EquipmentUpgradeNode>());
        
        // Refresh the texts
        RefreshTextValues();

        // Update skill tree
        foreach(GameObject currentUpgrade in _upgradeTreeNodes)
        {
            if (!_equipmentObj.GetComponent<WeaponEquipment>().CurrentUpgradeIds.Contains(currentUpgrade.GetComponent<EquipmentUpgradeNode>().ID) &&
                CheckSublistExists(_equipmentObj.GetComponent<WeaponEquipment>().CurrentUpgradeIds, currentUpgrade.GetComponent<EquipmentUpgradeNode>().PrerequisiteUpgrades))
            {
                // Check if the player can afford it
                bool sufficentSouls = true ? LootManager.SoulCount >= currentUpgrade.GetComponent<EquipmentUpgradeNode>().Cost : false;

                currentUpgrade.GetComponent<Image>().color = Color.yellow;
                currentUpgrade.GetComponent<Button>().interactable = sufficentSouls && _writeMode;   
            }
        }

        // Change the UI
        upgrade.GetComponent<Button>().interactable = false;
        upgrade.GetComponent<Image>().color = Color.green;
    }

    private bool CheckSublistExists(List<int> upgradeIds, List<EquipmentUpgradeNode> PrerequisiteUpgrades)
    {
        // Check if all prerequisites are found in the upgrade IDs
        int count = 0;
        foreach(EquipmentUpgradeNode node in PrerequisiteUpgrades)
        {
            if (upgradeIds.Contains(node.ID))
            {
                count++;
            }
        }
        return count == PrerequisiteUpgrades.Count;
    }
}
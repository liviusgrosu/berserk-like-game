using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentUpgradeNode : MonoBehaviour
{
    public string UpgradeName;
    public int Cost;
    public int Amount;
    public List<EquipmentUpgradeNode> PrerequisiteUpgrades;
    public Text _costText;

    void Awake()
    {
        // Display the cost
        if (_costText)
        {
            _costText.text = Cost.ToString();
        }

        if (PrerequisiteUpgrades == null)
        {
            PrerequisiteUpgrades = new List<EquipmentUpgradeNode>();
        }
    }
}
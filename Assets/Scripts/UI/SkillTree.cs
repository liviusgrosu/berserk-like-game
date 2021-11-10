using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using GeneralUtility;

class SkillTree : MonoBehaviour
{
    public EntityStats PlayerStats;
    public LootManager LootManager;
    // TEMP: for now set the mode to write
    public bool WriteMode = true;

    [Header("Stats Text")]
    public Text SoulCountText;
    public Text HealthText;
    public Text StaminaText;

    private List<GameObject> _skillTreeNodes;
    private ListHelper<SkillTreeNode> _listHelper;

    void Awake()
    {
        _listHelper = new ListHelper<SkillTreeNode>();
        _skillTreeNodes = new List<GameObject>();
    }

    public void Initialize()
    {
        // Display the texts
        UpdateTextElement();

        // Add each skill tree node into a list
        foreach(Transform child in transform)
        {
            if(child.GetComponent<SkillTreeNode>())
            {
                _skillTreeNodes.Add(child.gameObject);
            }
        }

        // Change their interactibility
        foreach(GameObject skillUpgradeNode in _skillTreeNodes)
        {
            SkillTreeNode skillScript = skillUpgradeNode.GetComponent<SkillTreeNode>();
            if(PlayerStats.CheckIfUpgradeUnlocked(skillScript))
            {
                skillUpgradeNode.GetComponent<Image>().color = Color.green;
                continue;
            }

            // Check if the skill has the prerequisites
            bool unlocked = false;
            bool sufficentSouls = false;
            if (skillScript.PrerequisiteSkills.Count == 0 || CheckSublistExists(PlayerStats.CurrentSkillsId,  skillScript.PrerequisiteSkills))
            {
                unlocked = true;

                // Check if the player can afford it
                sufficentSouls = true ? LootManager.SoulCount >= skillScript.Cost : false;
            }

            if (WriteMode)
            {
                skillUpgradeNode.GetComponent<Button>().interactable = unlocked && sufficentSouls;
                skillUpgradeNode.GetComponent<Image>().color = unlocked ? Color.yellow : Color.grey;
            }
            else
            {
                skillUpgradeNode.GetComponent<Button>().interactable = false;
                skillUpgradeNode.GetComponent<Image>().color = Color.grey;
            }
        }
    }

    public void AddSkill(Transform skill)
    {
        // Decrease souls 
        LootManager.SoulCount -= skill.GetComponent<SkillTreeNode>().Cost;

        // Add to player stats
        PlayerStats.AddUpgrade(skill.GetComponent<SkillTreeNode>());

        // Refresh the texts
        UpdateTextElement();

        // Update skill tree
        foreach(GameObject currentSkill in _skillTreeNodes)
        {
            if (!PlayerStats.CurrentSkillsId.Contains(currentSkill.GetComponent<SkillTreeNode>().ID) && 
                CheckSublistExists(PlayerStats.CurrentSkillsId, currentSkill.GetComponent<SkillTreeNode>().PrerequisiteSkills))
            {
                // Check if the player can afford it
                bool sufficentSouls = true ? LootManager.SoulCount >= currentSkill.GetComponent<SkillTreeNode>().Cost : false;

                currentSkill.GetComponent<Image>().color = Color.yellow;
                currentSkill.GetComponent<Button>().interactable = sufficentSouls;
            }
        }

        // Change the UI
        skill.GetComponent<Button>().interactable = false;
        skill.GetComponent<Image>().color = Color.green;
    }

    private void UpdateTextElement()
    {
        // Display the souls text
        SoulCountText.text = $"Souls: {LootManager.SoulCount}";
        // Display the player stats
        HealthText.text = $"Health: {PlayerStats.Stats.Health}";
        StaminaText.text = $"Stamina: {PlayerStats.Stats.Stamina}";
    }

    private bool CheckSublistExists(List<int> skillIds, List<SkillTreeNode> PrerequisiteSkills)
    {
        // Check if all prerequisites are found in the upgrade IDs
        int count = 0;
        foreach(SkillTreeNode node in PrerequisiteSkills)
        {
            if (skillIds.Contains(node.ID))
            {
                count++;
            }
        }
        return count == PrerequisiteSkills.Count;
    }
}
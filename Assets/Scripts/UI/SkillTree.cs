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
    public bool _writeMode = true;

    [Header("Stats Text")]
    public Text ExperienceText;
    public Text HealthText;
    public Text StaminaText;
    public Text AttackSpeedText;

    public SkillTreeNode StartingNode;

    private List<GameObject> _skillTreeNodes;
    private ListHelper<SkillTreeNode> _listHelper;

    void Awake()
    {
        _listHelper = new ListHelper<SkillTreeNode>();
        _skillTreeNodes = new List<GameObject>();
    }
    void Start()
    {
        // Display the texts
        UpdateTextElement();

        // Add the starting node
        PlayerStats.CurrentSkills.Add(StartingNode);

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
            bool sufficentExperience = false;
            foreach(SkillTreeNode prerequiteSkill in skillScript.PrerequisiteSkills)
            {
                if (PlayerStats.CheckIfUpgradeUnlocked(prerequiteSkill))
                {
                    unlocked = true;

                    // Check if the player can afford it
                    sufficentExperience = true ? LootManager.Experience >= prerequiteSkill.Cost : false;

                    break;
                }
            }

            skillUpgradeNode.GetComponent<Button>().interactable = unlocked && sufficentExperience && _writeMode;
            skillUpgradeNode.GetComponent<Image>().color = unlocked ? Color.yellow : Color.grey;
        }
    }

    public void AddSkill(Transform skill)
    {
        // Decrease experience 
        LootManager.Experience -= skill.GetComponent<SkillTreeNode>().Cost;

        // Add to player stats
        PlayerStats.AddUpgrade(skill.GetComponent<SkillTreeNode>());

        // Refresh the texts
        UpdateTextElement();

        // Update skill tree
        foreach(GameObject currentSkill in _skillTreeNodes)
        {
            if (!PlayerStats.CurrentSkills.Contains(currentSkill.GetComponent<SkillTreeNode>()) && 
                _listHelper.CheckSublistExists(PlayerStats.CurrentSkills, currentSkill.GetComponent<SkillTreeNode>().PrerequisiteSkills))
            {
                // Check if the player can afford it
                bool sufficentExperience = true ? LootManager.Experience >= currentSkill.GetComponent<SkillTreeNode>().Cost : false;

                currentSkill.GetComponent<Image>().color = Color.yellow;
                currentSkill.GetComponent<Button>().interactable = sufficentExperience && _writeMode;   
            }
        }

        // Change the UI
        skill.GetComponent<Button>().interactable = false;
        skill.GetComponent<Image>().color = Color.green;
    }

    private void UpdateTextElement()
    {
        // Display the experience text
        ExperienceText.text = $"Experience: {LootManager.Experience}";
        // Display the player stats
        HealthText.text = $"Health: {PlayerStats.Health}";
        StaminaText.text = $"Stamina: {PlayerStats.Stamina}";
        AttackSpeedText.text = $"Attack Speed: {PlayerStats.AttackSpeed}";
    }
}
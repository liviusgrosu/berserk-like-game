using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class SkillTree : MonoBehaviour
{
    public EntityStats PlayerEntityStats;
    public LootManager LootManager;
    private List<GameObject> _skillTreeNodes;

    void Awake()
    {
        _skillTreeNodes = new List<GameObject>();
    }
    void Start()
    {
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
            if(PlayerEntityStats.CheckIfUpgradeUnlocked(skillScript))
            {
                skillUpgradeNode.GetComponent<Image>().color = Color.green;
                continue;
            }

            // Check if the skill has the prerequisites
            bool unlocked = false;
            bool sufficentExperience = false;
            foreach(SkillTreeNode prerequiteSkill in skillUpgradeNode.GetComponent<SkillTreeNode>().PrerequisiteSkills)
            {
                if (PlayerEntityStats.CheckIfUpgradeUnlocked(prerequiteSkill))
                {
                    unlocked = true;

                    // Check if the player can afford it
                    sufficentExperience = true ? LootManager.Experience >= prerequiteSkill.Cost : false;

                    break;
                }
            }

            skillUpgradeNode.GetComponent<Button>().interactable = unlocked && sufficentExperience;
            skillUpgradeNode.GetComponent<Image>().color = unlocked ? Color.yellow : Color.grey;
        }
    }

    public void AddSkill(Transform skill)
    {
        // Decrease experience 
        LootManager.Experience -= skill.GetComponent<SkillTreeNode>().Cost;

        // Add to player stats
        PlayerEntityStats.AddUpgrade(skill.GetComponent<SkillTreeNode>());

        // Update skill tree
        foreach(GameObject currentSkill in _skillTreeNodes)
        {
            if (currentSkill.GetComponent<SkillTreeNode>().PrerequisiteSkills.Contains(skill.GetComponent<SkillTreeNode>()))
            {
                currentSkill.GetComponent<Image>().color = Color.yellow;
                currentSkill.GetComponent<Button>().interactable = true;   
            }
        }

        // Change the UI
        skill.GetComponent<Button>().interactable = false;
        skill.GetComponent<Image>().color = Color.green;
    }
}
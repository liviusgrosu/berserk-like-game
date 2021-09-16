using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class SkillTree : MonoBehaviour
{
    public EntityStats PlayerEntityStats;
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
                skillUpgradeNode.GetComponent<Image>().color = Color.cyan;
                continue;
            }

            // Check if the skill has the prerequisites
            bool unlocked = false;
            foreach(SkillTreeNode prerequiteSkill in skillUpgradeNode.GetComponent<SkillTreeNode>().PrerequisiteSkills)
            {
                if (PlayerEntityStats.CheckIfUpgradeUnlocked(prerequiteSkill))
                {
                    unlocked = true;
                    break;
                }
            }

            skillUpgradeNode.GetComponent<Button>().interactable = unlocked;
        }
    }
}
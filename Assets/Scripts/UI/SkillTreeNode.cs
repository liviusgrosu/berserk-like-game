using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTreeNode : MonoBehaviour
{
    public string Skill;
    public int Amount;
    public List<SkillTreeNode> PrerequisiteSkills;

    void Awake()
    {
        if (PrerequisiteSkills == null)
        {
            PrerequisiteSkills = new List<SkillTreeNode>();
        }
    }
}

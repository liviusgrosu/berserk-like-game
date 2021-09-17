using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeNode : MonoBehaviour
{
    public string Skill;
    public int Cost;
    public List<SkillTreeNode> PrerequisiteSkills;
    public Text _costText;

    void Awake()
    {
        // Display the cost
        if (_costText)
        {
            _costText.text = Cost.ToString();
        }

        if (PrerequisiteSkills == null)
        {
            PrerequisiteSkills = new List<SkillTreeNode>();
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeNode : MonoBehaviour
{
    public int ID;
    public string SkillName;
    public int Cost;
    public int Amount;
    public List<SkillTreeNode> PrerequisiteSkills;
    public Text _costText;
    public bool StartingNode;

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTreeNode : MonoBehaviour
{
    public int ID;
    public string Skill;
    public int Amount;
    public List<int> PrerequisiteIDs;

    void Awake()
    {
        PrerequisiteIDs = new List<int>();
    }
}

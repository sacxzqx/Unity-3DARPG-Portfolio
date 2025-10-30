using System;
using System.Collections.Generic;

[Serializable]
public class SkillSaveData
{
    public int SkillPoint;
    public List<string> LearnedSkillIDs = new List<string>();
    public List<SkillBindingData> SkillBindings = new List<SkillBindingData>();
}

[Serializable]
public struct SkillBindingData
{
    public string Key;
    public string SkillId;
}
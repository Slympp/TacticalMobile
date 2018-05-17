using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job {

    public string                   JobName;
    public int                      Level = 1;
    public List<JobRequirement>     Requirements;
    public List<WeaponType>         AllowedWeapons;
    //public LearningCurve          statsLearningCurve;
    public List<SkillBase>          Skills;
}
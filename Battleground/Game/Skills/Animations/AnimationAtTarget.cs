using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationAtTarget", menuName = "Skill/Animation/At Target", order = 2)]
public class AnimationAtTarget : SkillAnimation {

    protected override void DoAnimation(Cell caster, Cell target) {
        base.DoAnimation(caster, target);

        if (AnimationObject != null) {
            AnimationObject.transform.position = target.GetVector3Position();
            AnimationObject.SetActive(true);
        }
    }
}

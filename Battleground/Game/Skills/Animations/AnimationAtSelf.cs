using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationAtSelf", menuName = "Skill/Animation/At Self", order = 1)]
public class AnimationAtSelf : SkillAnimation {

    public bool RotateTowardsTarget = false;

    protected override void DoAnimation(Cell caster, Cell target) {
        base.DoAnimation(caster, target);

        if (AnimationObject != null) {
            AnimationObject.transform.position = caster.GetVector3Position();

            if (RotateTowardsTarget) {
                Vector3 targetPos = target.GetVector3PositionSimple();
                Vector3 moveDir = (targetPos - AnimationObject.transform.position).normalized;
                AnimationObject.transform.rotation = Quaternion.LookRotation(moveDir);
            }

            AnimationObject.SetActive(true);
        }
    }
}

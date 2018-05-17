using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationProjectileStraight", menuName = "Skill/Animation/Projectile Straight", order = 3)]
public class AnimationProjectileStraight : SkillAnimation {

    [Header("Projectile")]
    public float        ProjectileSpeed = 10f;
    public float        ProjectileRotationSpeed = 0f;
    public Vector3      ProjectileInitialRotation = Vector3.zero;

    [Header("Impact")]
    public GameObject   ImpactObjectReference;
    public float        ImpactDuration = 0f;
    
    private GameObject  ImpactAnimationObject;

    public override void Init() {

        base.Init();

        if (ImpactObjectReference != null && ImpactAnimationObject == null) {
            ImpactAnimationObject = Instantiate(ImpactObjectReference);
            ImpactAnimationObject.SetActive(false);
        }
    }

    public override IEnumerator Play(Animator animator, Cell caster, Cell target, System.Action endAction) {

        yield return new WaitForSeconds(DelayBeforeStart);

        if (AnimationString != "") animator.SetBool(UseAnimationPrefix ? caster.IsTaken.AnimationPrefix + AnimationString : AnimationString, true);

        Vector3 targetPos = target.GetVector3Position() + new Vector3(0, 0.5f, 0);
        AnimationObject.transform.position = caster.GetVector3Position() + new Vector3(0, 0.5f, 0);
        Vector3 moveDir = (targetPos - AnimationObject.transform.position).normalized;
        AnimationObject.transform.rotation = Quaternion.LookRotation(moveDir) * Quaternion.Euler(ProjectileInitialRotation);

        AnimationObject.SetActive(true);

        while (AnimationObject.transform.position != targetPos) {
            AnimationObject.transform.position = Vector3.MoveTowards(AnimationObject.transform.position, targetPos, Time.deltaTime * ProjectileSpeed);
            AnimationObject.transform.Rotate(0, 0, ProjectileRotationSpeed * Time.deltaTime, Space.Self);
            yield return new WaitForEndOfFrame();
        }

        if (AnimationString != "") animator.SetBool(UseAnimationPrefix ? caster.IsTaken.AnimationPrefix + AnimationString : AnimationString, false);

        if (AnimationObject != null) AnimationObject.SetActive(false);

        if (ImpactAnimationObject != null) {
            ImpactAnimationObject.transform.position = targetPos;
            ImpactAnimationObject.SetActive(true);
            yield return new WaitForSeconds(ImpactDuration);
            ImpactAnimationObject.SetActive(false);
        }

        if (DoEndAction) endAction();
        yield return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationProjectileParabola", menuName = "Skill/Animation/Projectile Parabola", order = 4)]
public class AnimationProjectileParabola : SkillAnimation {

    [Header("Projectile")]
    public float ProjectileSpeed = 10f;
    public float ProjectileRotationSpeed = 0f;
    public Vector3 ProjectileInitialRotation = Vector3.zero;

    [Header("Parabola")]
    public float        ParabolaAngle = 45f;
    private Rigidbody   Rigidbody;

    [Header("Impact")]
    public GameObject ImpactObjectReference;
    public float ImpactDuration = 0f;

    private GameObject ImpactAnimationObject;

    public override void Init() {

        if ((Rigidbody = AnimationObject.GetComponent<Rigidbody>()) == null)
            Rigidbody = AnimationObject.AddComponent<Rigidbody>();

        if (ImpactObjectReference != null && ImpactAnimationObject == null) {
            ImpactAnimationObject = Instantiate(ImpactObjectReference);
            ImpactAnimationObject.SetActive(false);
        }
    }

    public override IEnumerator Play(Animator animator, Cell caster, Cell target, System.Action endAction) {

        yield return new WaitForSeconds(DelayBeforeStart);

        if (AnimationString != "") animator.SetBool(UseAnimationPrefix ? caster.IsTaken.AnimationPrefix + AnimationString : AnimationString, true);


        Vector3 targetPos = target.GetVector3Position() + new Vector3(0, 0.5f, 0);
        Vector3 startPos = AnimationObject.transform.position = caster.GetVector3Position() + new Vector3(0, 0.5f, 0);
        Vector3 moveDir = (targetPos - AnimationObject.transform.position).normalized;
        AnimationObject.transform.rotation = Quaternion.LookRotation(moveDir) * Quaternion.Euler(ProjectileInitialRotation);

        
        Rigidbody.velocity = BallisticVel(startPos, targetPos, moveDir, ParabolaAngle);

        AnimationObject.SetActive(true);

        while (AnimationObject.transform.position != targetPos) {

            /*AnimationObject.transform.rotation = */
            //AnimationObject.transform.LookAt(targetPos);// * Quaternion.Euler(ProjectileInitialRotation);

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

    public Vector3 BallisticVel(Vector3 startPos, Vector3 targetPos, Vector3 dir, float angle) {
        float h = dir.y;  // get height difference
        dir.y = 0;  // retain only the horizontal direction
        float dist = dir.magnitude;  // get horizontal distance
        float a = angle * Mathf.Deg2Rad;  // convert angle to radians
        dir.y = dist* Mathf.Tan(a);  // set dir to the elevation angle
        dist += h / Mathf.Tan(a);  // correct for small height differences
     
        // calculate the velocity magnitude
        float vel = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        return vel* dir.normalized;
    }
}

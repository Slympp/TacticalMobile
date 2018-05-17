using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillAnimation : ScriptableObject {

    public float            Duration = 0f;
    public float            DelayBeforeStart= 0f;
    public bool             DoEndAction = true;
    public string           AnimationString = "";
    public bool             UseAnimationPrefix = true;
    public GameObject       ObjectReference;

    protected GameObject    AnimationObject;

    public virtual void Init() {

        if (ObjectReference != null && AnimationObject == null) {
            AnimationObject = Instantiate(ObjectReference);
            AnimationObject.SetActive(false);
        }
    }

    public virtual IEnumerator Play(Animator animator, Cell caster, Cell target, System.Action endAction) {

        yield return new WaitForSeconds(DelayBeforeStart);

        if (AnimationString != "") animator.SetBool(UseAnimationPrefix ? caster.IsTaken.AnimationPrefix + AnimationString : AnimationString, true);

        DoAnimation(caster, target);

        yield return new WaitForSeconds(Duration);

        if (AnimationString != "") animator.SetBool(UseAnimationPrefix ? caster.IsTaken.AnimationPrefix + AnimationString : AnimationString, false);
        if (AnimationObject != null) AnimationObject.SetActive(false);

        if (DoEndAction) endAction();

        yield return null;
    }

    protected virtual void DoAnimation(Cell caster, Cell target) { }
}

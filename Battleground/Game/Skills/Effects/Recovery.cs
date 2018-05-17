using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Recovery", menuName = "Skill/Effect/Recovery", order = 2)]
public class Recovery : SkillEffect {

    public List<RecoveryLine> RecoveryLine;

    public override EffectType GetEffectType() {
        return EffectType.Recovery;
    }

    public override List<RecoveryLine> GetRecovery() {
        return RecoveryLine;
    }
}
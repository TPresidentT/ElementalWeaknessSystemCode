using UnityEngine;

[CreateAssetMenu(fileName = "AquaStunEffectData", menuName = "Scriptable Objects / Aqua Stun Effect Data")]
public class AquaStunEffectData : StunEffectData
{
    [SerializeField] private float totalTime;
    [SerializeField] private StatusEffectData statusEffect;

    public override void Apply(GameObject userMonster, GameObject targetMonster)
    {
        userMonster.GetComponent<MonsterStatus>().ApplyStatusEffect(statusEffect, totalTime);
    }
}
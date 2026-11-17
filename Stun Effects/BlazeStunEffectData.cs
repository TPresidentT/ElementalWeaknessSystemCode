using UnityEngine;

[CreateAssetMenu(fileName = "BlazeStunEffectData", menuName = "Scriptable Objects / Blaze Stun Effect Data")]
public class BlazeStunEffectData : StunEffectData
{
    [SerializeField] private float totalTime;
    [SerializeField] private StatusEffectData statusEffect;

    public override void Apply(GameObject userMonster, GameObject targetMonster)
    {
        targetMonster.GetComponent<MonsterStatus>().ApplyStatusEffect(statusEffect, totalTime);
    }
}

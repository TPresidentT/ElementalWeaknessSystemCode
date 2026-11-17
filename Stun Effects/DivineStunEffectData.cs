using UnityEngine;

[CreateAssetMenu(fileName = "DivineStunEffectData", menuName = "Scriptable Objects / Divine Stun Effect Data")]
public class DivineStunEffectData : StunEffectData
{
    [SerializeField] private int effectsToApplyMin;
    [SerializeField] private int effectsToApplyMax;

    public override void Apply(GameObject userMonster, GameObject targetMonster)
    {
        int effectsToApply = Random.Range(effectsToApplyMin, effectsToApplyMax + 1);
        userMonster.GetComponent<MonsterStun>().ApplyDivineOffering(effectsToApply);
    }
}

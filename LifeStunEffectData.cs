using UnityEngine;

[CreateAssetMenu(fileName = "LifeStunEffectData", menuName = "Scriptable Objects / Life Stun Effect Data")]
public class LifeStunEffectData : StunEffectData
{
    [SerializeField] private int gemsToCreateMin;
    [SerializeField] private int gemsToCreateMax;

    public override void Apply(GameObject userMonster, GameObject targetMonster)
    {
        int gemsToCreate = Random.Range(gemsToCreateMin, gemsToCreateMax + 1);
        userMonster.GetComponent<MonsterCombat>().GameManager.GetComponent<GameManager>().CreateGemCurrency(gemsToCreate);
    }
}
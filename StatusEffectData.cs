using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectData", menuName = "Scriptable Objects/StatusEffectData")]
public class StatusEffectData : ScriptableObject
{
    [SerializeField] new private string name;
    public Sprite image;
    public int amountPerTick;
    public float timePerTick;
    public StatusEffectType statusEffectType;
}

public enum StatusEffectType
{
    WaterVeil,
    BlazeBurn
}

public class AppliedStatusEffect
{
    public StatusEffectData statusEffect;
    public float timeRemaining;
    public float maxTime;

    public AppliedStatusEffect(StatusEffectData statusEffect, float duration)
    {
        this.statusEffect = statusEffect;
        timeRemaining = duration;
        maxTime = duration;
    }
}

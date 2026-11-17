using UnityEngine;

[CreateAssetMenu(fileName = "StunEffectData", menuName = "Scriptable Objects / Stun Effect Data")]
public abstract class StunEffectData : ScriptableObject
{
    public AudioClip stunEffectSound;
    public float stunEffectSoundVolume;

    public abstract void Apply(GameObject userMonster, GameObject targerMonster);
}
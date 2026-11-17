using UnityEngine;

[CreateAssetMenu(fileName = "ElementData", menuName = "Scriptable Objects / Element Data")]
public class ElementData : ScriptableObject
{
    new public string name;
    public Sprite image;
    public Sprite offeringOrbSprite;
    public StunEffectData stunEffect;

    public void ApplyStunEffect(GameObject userMonster, GameObject targetMonster)
    {
        stunEffect.Apply(userMonster, targetMonster);
    }
}

public enum AffinityStatus
{
    Negative,
    Neutral,
    Positive
}

[System.Serializable]
public struct ElementalAffinity
{
    public ElementData element;
    public AffinityStatus affinityStatus;
}
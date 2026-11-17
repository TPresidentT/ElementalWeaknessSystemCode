using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable Objects / Monster Data")]
public class MonsterData : ScriptableObject
{
    //General variables
    new public string name;
    public Sprite image;

    //Combat variables
    public int healthMax;
    public int attack;
    public float defence;
    public float autoAttackInterval;
    public MoveData[] moves;
    public ElementalAffinity[] elementalAffinities;
}
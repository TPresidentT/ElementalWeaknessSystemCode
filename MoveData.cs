using UnityEngine;

[CreateAssetMenu(fileName = "MoveData", menuName = "Scriptable Objects / Move Data")]
public class MoveData : ScriptableObject
{
    new public string name;
    public ElementData element;
    public int power;
    public int recharge;
}
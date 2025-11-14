using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Character /CharacterData")]
public class CharacterData : ScriptableObject
{
    public string characterName;

    public BuildableObject turrets;
    public Sprite sprite;

    [Header("Character Stats")]
    public int damageBonus;
    public int lv;

    [Header("Character Dialogue")]
    public string[] lines;

    bool isTalking;
}

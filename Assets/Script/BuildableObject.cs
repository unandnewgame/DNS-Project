using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Building/Buildable Object")]
public class BuildableObject : ScriptableObject
{
    public string objectName;
    public GameObject previewPrefab;
    public GameObject finalPrefab;
    public Vector3 placementOffset;
    public Tower tower;
    public Sprite turretSprite;

    [Header("Turrets Stats")]
    public int damage;
    public int lv;

    [Header("Build Requirements")]
    public ItemCost[] buildCost;
    public int turretCost;

    [Header("Refund Settings")]
    [Range(0f, 1f)] public float refundRate = 0.5f; // 50% refund

    public void LevelUp()
    {
        damage += 1;
        lv += 1;
        tower.range += 5;
        tower.fireRate += 1;
    }

}

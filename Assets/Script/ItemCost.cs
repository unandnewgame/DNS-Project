using System;
using UnityEngine;

[Serializable]
public class ItemCost
{
    public ResourceItem resource;
    public int amount;

    [Header("Optional Refund Settings")]
    public GameObject pickupPrefab; // prefab to drop on refund (optional)
}

using System;
using UnityEngine;

public enum FlowerType // sounds weird but its internal name for ground things like flowers and mushrooms
{
    YELLOW = 0, RED = 2, GRASS = 4, MUSHROOM = 6, BUSH = 8
}

[Serializable]
public class FlowerData
{
    public FlowerType type;
    public Vector3Int pos;
    public bool Grown = false;
}
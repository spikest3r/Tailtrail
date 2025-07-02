using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct FurnitureTile
{
    public Tile tile; // texture
    public Vector3Int pos; // position
}

[System.Serializable]
public struct FurnitureItem
{
    public Tile tile; // texture for placement
    public Sprite sprite; // texture for inventory
}
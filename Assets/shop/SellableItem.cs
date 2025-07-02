using UnityEngine;
using UnityEngine.Tilemaps;

public enum ItemType
{
    HOE,AXE,WATERING,
    WHEAT_SEED,TOMATO_SEED
}

public class SellableItem : MonoBehaviour
{
    [SerializeField] Tilemap tilemap = null;
    [SerializeField] internal int Price;
    [SerializeField] internal bool IsFurniture;
    [SerializeField] internal string Name;
    [SerializeField] internal FurnitureItem furniture;
    [SerializeField] internal ItemType itemType;

    void Start()
    {
        if(tilemap != null)
        {
            Tile tile = tilemap.GetTile(tilemap.WorldToCell(transform.position)) as Tile;
            furniture.tile = tile;
            furniture.sprite = tile.sprite;
        }
    }
}

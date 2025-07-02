using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

[System.Serializable]
public struct CatalogEntry
{
    public int BuyingPrice, SellingPrice;
    public string Name;
    public bool IsFurniture;
    public FurnitureItem furniture;
}

public class DailyItemCatalog : MonoBehaviour
{
    public CatalogEntry[] decoEntries;
    [SerializeField] Tilemap decoMap;
    [SerializeField] GameObject[] decoCells; // world pos to cell
    List<CatalogEntry> usedEntries;
    Random rnd;

    private void Awake()
    {
        usedEntries = new List<CatalogEntry>();
        DateTime today = DateTime.Today;
        int seed = today.Year * 10000 + today.Month * 100 + today.Day;
        rnd = new Random(seed);

        for (int i = 0; i < decoCells.Length; i++)
        {
            GameObject cellObj = decoCells[i];
            SellableItem decoCell = cellObj.GetComponent<SellableItem>();
            Vector3 worldPos = cellObj.transform.position;
            Vector3Int cellPos = decoMap.WorldToCell(worldPos);

            CatalogEntry item;
            do
            {
                item = decoEntries[rnd.Next(decoEntries.Length)];
            } while (usedEntries.Contains(item));

            usedEntries.Add(item);

            decoMap.SetTile(cellPos, item.furniture.tile);

            decoCell.Price = item.BuyingPrice;
            decoCell.IsFurniture = item.IsFurniture;
            decoCell.furniture = item.furniture;
            decoCell.Name = item.Name;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

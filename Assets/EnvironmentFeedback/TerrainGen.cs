using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using SSC = StateSavingController;

[System.Serializable]
public class DecoTile
{
    public Vector3Int pos;
    public int tile;
    public bool isTree;

    public DecoTile(Vector3Int pos, int tileIndex, bool isTree)
    {
        this.pos = pos;
        this.tile = tileIndex;
        this.isTree = isTree;
    }
}

public class TerrainGen : MonoBehaviour
{
    [SerializeField] Tile[] decoTiles;
    [SerializeField] Tile[] tree;
    [SerializeField] Vector3 minRange, maxRange;
    [SerializeField] int minDeco, maxDeco;
    [SerializeField] int DecoFlowerIndexStart;
    [SerializeField] FlowerType[] FlowerTypeByIndex;

    [SerializeField] Tilemap DecoMap, FlowerMap;
    [SerializeField] Tilemap[] OtherMaps;

    bool IsTileFree(Vector3Int pos)
    {
        foreach(Tilemap tilemap in OtherMaps)
        {
            if (tilemap.GetTile(pos) != null) return false;
        }
        return true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(SSC.saveState.decoInWorld.Count == 0)
        {
            int DecoCount = Random.Range(minDeco, maxDeco+1);
            Vector3 rawPos = new();
            for (int i = 0; i < DecoCount; i++)
            {
                bool isTree = Helpers.RandomChance(60);
                rawPos = Helpers.RandomVector3(minRange, maxRange);
                Vector3Int pos = DecoMap.WorldToCell(rawPos);
                if (isTree)
                {
                    int j = 0;
                    while (!IsTileFree(pos) && !IsTileFree(pos - Vector3Int.up))
                    {
                        if (j > 15) break; // avoid hanging
                        rawPos = Helpers.RandomVector3(minRange, maxRange);
                        pos = DecoMap.WorldToCell(rawPos);
                        j++;
                    }
                    DecoMap.SetTile(pos, tree[1]);
                    DecoMap.SetTile(pos - Vector3Int.up, tree[0]);
                    SSC.saveState.decoInWorld.Add(new DecoTile(pos, 1, true));
                    SSC.saveState.decoInWorld.Add(new DecoTile(pos - Vector3Int.up, 0, true));
                } else
                {
                    int RandomTile = Random.Range(0, decoTiles.Length);
                    int j = 0;
                    while (!IsTileFree(pos))
                    {
                        if (j > 15) break; // avoid hanging
                        rawPos = Helpers.RandomVector3(minRange, maxRange);
                        pos = DecoMap.WorldToCell(rawPos);
                        j++;
                    }
                    Tile t = decoTiles[RandomTile];
                    if (RandomTile >= DecoFlowerIndexStart)
                    {
                        FlowerData flower = new();
                        flower.Grown = true;
                        flower.pos = pos;
                        flower.type = FlowerTypeByIndex[RandomTile - DecoFlowerIndexStart];
                        FlowerMap.SetTile(pos, decoTiles[RandomTile]);
                        SSC.saveState.flowersInWorld.Add(pos, flower);
                    }
                    else
                    {
                        DecoMap.SetTile(pos, t);
                        SSC.saveState.decoInWorld.Add(new DecoTile(pos, RandomTile, false));
                    }
                }
            }
        } else
        {
            foreach(DecoTile dt in SSC.saveState.decoInWorld)
            {
                if(dt.isTree)
                {
                    DecoMap.SetTile(dt.pos, tree[dt.tile]);
                } else
                {
                    DecoMap.SetTile(dt.pos, decoTiles[dt.tile]);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

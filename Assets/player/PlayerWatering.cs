using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using SSC = StateSavingController;

public class PlayerWatering : MonoBehaviour
{
    public bool WateringActive = false;
    [SerializeField] Tilemap flowerMap, farmlandMap;
    [SerializeField] Tile[] wheatTiles, tomatoTiles;
    [SerializeField] Tile[] flowerTiles;
    Animator anim;

    public Tile GetCropTile(CropData crop)
    {
        Debug.Log(crop.growPhase);
        return crop.type switch
        {
            CropType.WHEAT => wheatTiles[crop.growPhase],
            CropType.TOMATO => tomatoTiles[crop.growPhase],
            _ => null,
        };
    }

    public Tile GetFlowerTile(FlowerData flower)
    {
        return flowerTiles[flower.Grown ? (int)flower.type + 1 : (int)flower.type];
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        if (SSC.saveState.flowersInWorld.Count > 0)
        {
            foreach (FlowerData flower in SSC.saveState.flowersInWorld.Values)
            {
                flowerMap.SetTile(flower.pos, GetFlowerTile(flower));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!WateringActive) return;
        if(Keyboard.current.rKey.wasPressedThisFrame)
        {
            if (SSC.saveState.WateringDurability == 0) return;
            Vector3Int pos = farmlandMap.WorldToCell(transform.position);
            Tile farmlandTile = farmlandMap.GetTile(pos) as Tile;
            Tile flowerTile = farmlandMap.GetTile(pos) as Tile;
            if(farmlandTile != null)
            {
                if (!SSC.saveState.cropsInWorld.TryGetValue(pos, out CropData crop)) return;
                if (crop.growPhase >= 3) return;
                anim.SetTrigger("Water");
                int growBy = Random.Range(1, 3);
                for(int i = 0; i < growBy; i++)
                {
                    if (crop.growPhase >= 3) break;
                    crop.growPhase++;
                }
                crop.whenPlanted = DateTime.Now;
                farmlandMap.SetTile(pos, GetCropTile(crop));
                SSC.saveState.WateringDurability--;
                return;
            } else if(flowerTile == null)
            {
                anim.SetTrigger("Water");
                if (Helpers.RandomChance(60))
                {
                    int randomFlower = Random.Range(0, 4) * 2;
                    FlowerData flower = new();
                    flower.type = (FlowerType)randomFlower;
                    flower.pos = pos;
                    Tile randomFlowerTile = GetFlowerTile(flower);
                    flowerMap.SetTile(pos, randomFlowerTile);
                    SSC.saveState.flowersInWorld.Add(pos, flower);
                }
                SSC.saveState.WateringDurability--;
                return;
            }
        }
    }
}

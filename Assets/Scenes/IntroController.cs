using UnityEngine;
using UnityEngine.Tilemaps;
using SSC = StateSavingController;

public class IntroController : MonoBehaviour
{
    [SerializeField] Tile[] wheatTiles, tomatoTiles;
    [SerializeField] Tilemap farmlandTilemap, flowerMap;
    [SerializeField] Tile[] flowerTiles;
    public Tile GetFlowerTile(FlowerData flower)
    {
        return flowerTiles[flower.Grown ? (int)flower.type + 1 : (int)flower.type];
    }

    Tile GetCropTile(CropData crop)
    {
        Debug.Log(crop.growPhase);
        return crop.type switch
        {
            CropType.WHEAT => wheatTiles[crop.growPhase],
            CropType.TOMATO => tomatoTiles[crop.growPhase],
            _ => null,
        };
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // load crops
        if (SSC.saveState.cropsInWorld.Count > 0)
        {
            foreach (CropData crop in SSC.saveState.cropsInWorld.Values)
            {
                farmlandTilemap.SetTile(crop.position, GetCropTile(crop));
            }
        }
        // load flowers
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
        
    }
}

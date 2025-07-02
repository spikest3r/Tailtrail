using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using SSC = StateSavingController;
using WAH = WorldAmbianceHelper;
public class PlayerFarming : MonoBehaviour
{
    /* We have 2 types of crops
     * Wheat and Tomato
     * when press digit1Key, select wheat
     * same with digit2Key, but tomato
     */

    [SerializeField] Tilemap farmlandTilemap;
    [SerializeField] Tile[] wheatTiles, tomatoTiles;
    [SerializeField] ItemCounterController icc;
    [SerializeField] Sprite ICC_WheatIcon, ICC_TomatoIcon, ICC_WheatSeedIcon, ICC_TomatoSeedIcon;
    public bool FarmingActive = true;

    Animator anim;
    int selectedCrop = 0; // 0 - wheat, 1 - tomato
    KeyControl[] selectionKeys;

    private void Awake()
    {
        selectionKeys = new KeyControl[2] { Keyboard.current.digit1Key, Keyboard.current.digit2Key };
        anim = GetComponent<Animator>();
        if(icc == null)
        {
            Debug.LogError("ICC is not assigned!!!");
        }
    }

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

    // restore crops from save file
    void Start()
    {
        if(SSC.saveState.cropsInWorld.Count > 0)
        {
            foreach(CropData crop in SSC.saveState.cropsInWorld.Values)
            {
                farmlandTilemap.SetTile(crop.position, GetCropTile(crop));
            }
        }
    }
    
    // planting logic here
    // Update is called once per frame
    void Update()
    {
        if (!FarmingActive) return;
        for(int i = 0; i < 2; i++)
        {
            if (selectionKeys[i].wasPressedThisFrame)
            {
                selectedCrop = i;
                Sprite seedIcon = selectedCrop == 0 ? ICC_WheatSeedIcon : ICC_TomatoSeedIcon;
                int seeds = selectedCrop == 0 ? SSC.saveState.WheatSeeds : SSC.saveState.TomatoSeeds;
                icc.ShowValue(seedIcon, seeds);
                break;
            }
        }

        if(Keyboard.current.rKey.wasPressedThisFrame)
        {
            if (SSC.saveState.HoeDurability == 0) return;
            Vector3Int pos = farmlandTilemap.WorldToCell(transform.position); // get position
            Tile t = farmlandTilemap.GetTile(pos) as Tile;
            bool usable = WAH.Instance.IsTileUsable(pos);
            if (!usable) return;
            if (t == null)
            {
                Sprite seedIcon = selectedCrop == 0 ? ICC_WheatSeedIcon : ICC_TomatoSeedIcon;
                int seeds = selectedCrop == 0 ? SSC.saveState.WheatSeeds : SSC.saveState.TomatoSeeds;
                if (seeds == 0)
                {
                    icc.ShowValue(seedIcon, 0);
                    return;
                }
                CropData crop = new()
                {
                    position = pos,
                    whenPlanted = DateTime.Now,
                    growPhase = 0, // new plant
                    type = (CropType)selectedCrop
                };
                SSC.saveState.cropsInWorld.Add(pos,crop);
                farmlandTilemap.SetTile(pos, (selectedCrop == 0 ? wheatTiles : tomatoTiles)[0]);
                SSC.saveState.HoeDurability--;
                switch (selectedCrop)
                {
                    case 0:
                        SSC.saveState.WheatSeeds--;
                        break;
                    case 1:
                        SSC.saveState.TomatoSeeds--;
                        break;
                }
                seeds = selectedCrop == 0 ? SSC.saveState.WheatSeeds : SSC.saveState.TomatoSeeds;
                icc.ShowValue(seedIcon, seeds);
                anim.SetTrigger("Farm");
            } else
            {
                CropData cropData = SSC.saveState.cropsInWorld[pos];
                Sprite icon = null;
                int valueToShow = 0;
                if (cropData.growPhase == 3)
                {
                    switch(cropData.type)
                    {
                        case CropType.WHEAT:
                            SSC.saveState.UsableWheat++;
                            icon = ICC_WheatIcon;
                            valueToShow = SSC.saveState.UsableWheat;
                            SSC.saveState.WheatSeeds += Random.Range(0, 3); // 0 - 2 random seeds per harvest
                            break;
                        case CropType.TOMATO:
                            SSC.saveState.UsableTomato++;
                            icon = ICC_TomatoIcon;
                            valueToShow = SSC.saveState.UsableTomato;
                            SSC.saveState.TomatoSeeds += Random.Range(0, 3); // 0 - 2 random seeds per harvest
                            break;
                    }
                    icc.ShowValue(icon, valueToShow);
                }
                SSC.saveState.cropsInWorld.Remove(pos); // !
                farmlandTilemap.SetTile(pos, null);
            }
        }
    }

    // farm logic here
    private void FixedUpdate()
    {
        var keys = SSC.saveState.cropsInWorld.Keys.ToList(); // safe copy to iterate
        List<Vector3Int> cropsToRemove = new();
        foreach (var pos in keys)
        {
            if (!SSC.saveState.cropsInWorld.TryGetValue(pos, out CropData crop))
                continue;
            if (crop.growPhase >= 3) continue;

            TimeSpan timeSpan = DateTime.Now - crop.whenPlanted;

            int[] growIntervals = new int[] { 2, 4, 6 };

            bool grew = false;
            foreach (int interval in growIntervals)
            {
                if (timeSpan.TotalMinutes >= interval)
                {
                    if (crop.growPhase < 3)
                    {
                        crop.growPhase++;
                        crop.whenPlanted = DateTime.Now;
                        grew = true;
                    }
                    else
                    {
                        if (Helpers.RandomChance(60))
                        {
                            cropsToRemove.Add(pos);
                        }
                        grew = true;
                        break;
                    }
                }
            }

            if (grew)
            {
                farmlandTilemap.SetTile(pos, GetCropTile(crop));
                SSC.saveState.cropsInWorld[pos] = crop;
            }
        }

        foreach (var posToRemove in cropsToRemove)
        {
            SSC.saveState.cropsInWorld.Remove(posToRemove);
            farmlandTilemap.SetTile(posToRemove, null);
        }
    }

}

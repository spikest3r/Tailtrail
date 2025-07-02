using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Tilemaps;

public class StateSavingController
{
    internal static string SaveFilePath => Path.Combine(Application.persistentDataPath, "savefile.json");

    // state machine for save file (.json)
    [System.Serializable]
    public class State
    {
        public List<FurnitureTile> playerHouseFurniture;
        public List<FurnitureItem> playerFurnitureInventory;
        [System.NonSerialized] public Dictionary<Vector3Int,CropData> cropsInWorld;
        [System.NonSerialized] public Dictionary<Vector3Int, FlowerData> flowersInWorld;
        public List<DecoTile> decoInWorld;
        public List<CropData> cropsList;
        public List<FlowerData> flowersList;
        public int coins;
        public int HoeDurability; // 0 means no item (and its broken)
        public int WateringDurability;
        public string PlayerName;
        public int UsableWheat, UsableTomato;
        public int WheatSeeds, TomatoSeeds;

        public State()
        {
            playerHouseFurniture = new List<FurnitureTile>();
            playerFurnitureInventory = new List<FurnitureItem>();
            cropsInWorld = new Dictionary<Vector3Int,CropData>();
            flowersInWorld = new Dictionary<Vector3Int, FlowerData>();
            cropsList = new List<CropData>();
            flowersList = new List<FlowerData>();
            decoInWorld = new List<DecoTile>();
            coins = 200; // as a bonus from the start you get some money
            HoeDurability = 0;
            WateringDurability = 0;
            PlayerName = "name"; // default name for debugging 
            UsableWheat = 0;
            UsableTomato = 0;
            WheatSeeds = 0;
            TomatoSeeds = 0;
        }

        public void SaveState()
        {
            foreach(CropData data in cropsInWorld.Values)
            {
                cropsList.Add(data);
            }
            foreach(FlowerData data in flowersInWorld.Values)
            {
                flowersList.Add(data);
            }
            try
            {
                string json = JsonConvert.SerializeObject(this, Formatting.Indented);
                if (Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    PlayerPrefs.SetString("json", json);
                    PlayerPrefs.Save();
                }
                else
                {
                    File.WriteAllText(SaveFilePath, json);
                }
                Debug.Log("Saved");
                return;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to save: " + e.Message);
            }
        }

        public void LoadState()
        {
            try
            {
                string json = null;
                if (Application.platform == RuntimePlatform.WebGLPlayer) 
                {
                    json = PlayerPrefs.GetString("json", "");
                    if (json.Length == 0) return;
                }
                else
                {
                    if (!File.Exists(SaveFilePath))
                    {
                        Debug.LogWarning("Save is not found");
                    }
                    json = File.ReadAllText(SaveFilePath);
                }
                State data = JsonConvert.DeserializeObject<State>(json);
                AssignValues(data);
                foreach(CropData crop in cropsList)
                {
                    try
                    {
                        cropsInWorld.Add(crop.position, crop);
                    } catch
                    {
                        
                    }
                }
                foreach(FlowerData flower in flowersList)
                {
                    try
                    {
                        flowersInWorld.Add(flower.pos, flower);
                    } catch
                    {

                    }
                }
                Debug.Log("Loaded");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to load: " + e.Message);
            }
        }

        private void AssignValues(State state)
        {
            playerHouseFurniture = state.playerHouseFurniture;
            playerFurnitureInventory = state.playerFurnitureInventory;
            coins = state.coins;
            HoeDurability = state.HoeDurability;
            WateringDurability = state.WateringDurability;
            PlayerName = state.PlayerName;
            UsableWheat = state.UsableWheat;
            UsableTomato = state.UsableTomato;
            cropsList = state.cropsList;
            flowersList = state.flowersList;
            decoInWorld = state.decoInWorld;
            WheatSeeds = state.WheatSeeds;
            TomatoSeeds = state.TomatoSeeds;
        }
    }

    // instance of state
    public static State saveState = new(); // var for saving

    // classes for states that are runtime only
    public enum TransitionType
    {
        FIRST_TIME, // Only when first game load (unused right now)
        EXIT_PLAYER_HOUSE, // when transitioning from player house to world
        EXIT_SHOP
    }

    public class RuntimeState
    {
        public static bool IsInsideBuilding = false;
        public static TransitionType transitionType = TransitionType.EXIT_PLAYER_HOUSE; // for stylish game start
        public static bool DidLoadSave = false; // at least tried

        // music
        public static List<int> indexesPlayed = new();
        public static int indexPlaying = -1;
    }
}

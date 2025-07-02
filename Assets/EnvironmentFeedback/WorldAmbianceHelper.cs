using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldAmbianceHelper : MonoBehaviour
{
    [SerializeField] Tilemap[] tilemaps;
    [SerializeField] Tilemap farmland;
    public static WorldAmbianceHelper Instance;

    private void Awake()
    {
        Instance = this;
    }

    public bool IsTileUsable(Vector3Int pos, bool includeFarmland = false)
    {
        foreach(Tilemap tilemap in tilemaps)
        {
            if(tilemap.GetTile(pos) != null)
            {
                return false;
            }
        }
        if(includeFarmland)
        {
            if(farmland.GetTile(pos) != null)
            {
                return false;
            }
        }
        return true;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AnimatedDoor : MonoBehaviour
{
    [SerializeField] Tile[] DoorOpening, DoorClosing;
    [SerializeField] Tilemap tilemap; // tilemap with our door
    [SerializeField] Vector3 DoorTile; // position of door tile that we will modify
    public Direction direction; // store flag of direction the door is
    public string SceneDestination = "House";

    [System.NonSerialized] public bool opposite = true; // true because first time it flips back to false
    public Direction GetDirection()
    {
        opposite = !opposite;
        return opposite ? DirectionHelper.Opposite(direction) : direction;
    }

    Vector3Int pos;

    void Awake()
    {
        pos = tilemap.WorldToCell(DoorTile);
    }

    public IEnumerator SetDoorState(bool open, float animSpeed = 0.2f) // animSpeed is delay between frames
    {
        Tile[] anim = open ? DoorOpening : DoorClosing; // select what anim to execute
        for(int i = 0; i < anim.Length; i++)
        {
            tilemap.SetTile(pos, anim[i]);
            yield return new WaitForSeconds(animSpeed);
        }
    }
}

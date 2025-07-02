using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class HouseDecorPlacementManager : MonoBehaviour
{
    [SerializeField] PlayerMovement player;
    [SerializeField] TMP_Text editModeText, statusText;
    [SerializeField] Tilemap decorMap, previewMap;
    [SerializeField] Vector3 CursorMinimal, CursorMaximal;
    [SerializeField] InventoryAnimation editModeInventory;
    [SerializeField] InventoryRenderer inventoryData;

    bool editMode = false;
    Vector3Int oldPos;

    bool IsInside2DBounds(Vector3 pos, Vector3 min, Vector3 max)
    {
        return pos.x >= Mathf.Min(min.x, max.x) && pos.x <= Mathf.Max(min.x, max.x) &&
               pos.y >= Mathf.Min(min.y, max.y) && pos.y <= Mathf.Max(min.y, max.y);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(FurnitureTile furniture in StateSavingController.saveState.playerHouseFurniture)
        {
            decorMap.SetTile(furniture.pos, furniture.tile);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.rKey.wasPressedThisFrame && editModeInventory.Available)
        {
            editMode = !editMode;
            editModeInventory.ToggleInventory(editMode);
            previewMap.ClearAllTiles(); // doesnt matter, clear anyway

            Cursor.visible = editMode;
            player.SetAllowedToMove(!editMode);
            Debug.Log(string.Format("Edit mode ", editMode ? "enabled" : "disabled"));
            editModeText.text = editMode ? "Edit mode" : "";
        }
        if(editMode)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0;
            Vector3Int tilePos = previewMap.WorldToCell(worldPos);
            if(tilePos != oldPos)
            {
                previewMap.ClearAllTiles();
                if (decorMap.GetTile(tilePos) == null && IsInside2DBounds(worldPos,CursorMinimal,CursorMaximal)) {
                    previewMap.SetTile(tilePos, inventoryData.GetSelectedFurniture());
                }
            }
            oldPos = tilePos;
            if (!IsInside2DBounds(worldPos, CursorMinimal, CursorMaximal)) return;
            if(Mouse.current.leftButton.wasPressedThisFrame)
            {
                if(decorMap.GetTile(tilePos) == null && inventoryData.GetSelectedFurniture() != null)
                {
                    Tile tile = inventoryData.GetSelectedFurniture();
                    FurnitureTile furniture;
                    furniture.tile = tile;
                    furniture.pos = tilePos;
                    StateSavingController.saveState.playerHouseFurniture.Add(furniture);
                    StateSavingController.saveState.playerFurnitureInventory.RemoveAt(inventoryData.GetSelectedPointer());
                    decorMap.SetTile(tilePos, tile);
                }
            } else if(Mouse.current.rightButton.wasPressedThisFrame)
            {
                Tile tile = decorMap.GetTile(tilePos) as Tile;
                if (tile != null)
                {
                    StateSavingController.saveState.playerHouseFurniture.RemoveAll(f =>
                         f.pos == tilePos && f.tile == tile);
                    decorMap.SetTile(tilePos, null);
                    FurnitureItem item;
                    item.tile = tile;
                    item.sprite = tile.sprite;
                    StateSavingController.saveState.playerFurnitureInventory.Add(item);
                    oldPos = Vector3Int.zero;
                }
            }
        }
    }
}

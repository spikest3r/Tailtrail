using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class InventoryRenderer : MonoBehaviour
{
    InventoryAnimation inventoryAnimation;
    bool renderVisible = false;
    Image[] inventoryItemSlots;
    int itemPointer = 0, invPointer = 0;
    int minPointer, maxPointer;
    int oldInvCount = 0;
    public bool IsInventoryOpen;
    bool noInvAnim;

    bool IsOpen()
    {
        return noInvAnim ? IsInventoryOpen : inventoryAnimation.IsInventoryOpen;
    }

    internal int GetSelectedPointer()
    {
        return itemPointer;
    }

    public Tile GetSelectedFurniture()
    {
        try
        {
            return StateSavingController.saveState.playerFurnitureInventory[itemPointer].tile;
        } catch(System.Exception)
        {
            return null;
        }
    }

    void Awake()
    {
        inventoryAnimation = GetComponent<InventoryAnimation>();
        noInvAnim = inventoryAnimation == null;
        inventoryItemSlots = new Image[transform.childCount];
        for(int i = 0; i < transform.childCount; i++)
        {
            try
            {
                GameObject obj = transform.GetChild(i).GetChild(0).gameObject;
                Image img = obj.GetComponent<Image>();
                inventoryItemSlots[i] = img;
            } catch(System.Exception)
            {
                continue;
            }
        }
        oldInvCount = StateSavingController.saveState.playerFurnitureInventory.Count;
    }

    void Rerender()
    {
        int j = minPointer;
        for(int i = 0; i < inventoryItemSlots.Length; i++)
        {
            try
            {
                inventoryItemSlots[i].sprite = StateSavingController.saveState.playerFurnitureInventory[j].sprite;
                j++;
            } catch(System.Exception)
            {
                Debug.Log("Item is null");
                try
                {
                    inventoryItemSlots[i].sprite = null;
                } catch(System.Exception)
                {

                }
            }
            inventoryItemSlots[i].color = Color.white; // clear selections
        }
        inventoryItemSlots[invPointer].color = Color.yellow; // mark as selected
    }

    // Update is called once per frame
    void Update()
    {
        if(IsOpen())
        {
            if (!renderVisible)
            {
                renderVisible = true;
                itemPointer = 0; // always start from first item
                minPointer = 0;
                maxPointer = 4;
                Rerender();
            }
            Vector2 scroll = Mouse.current.scroll.ReadValue();
            if (scroll.y > 0)
            {
                // scroll up
                if (itemPointer > 0)
                {
                    itemPointer--;
                    if(minPointer > itemPointer)
                    {
                        minPointer--;
                    }
                    if (invPointer > 0)
                    {
                        invPointer--;
                    }
                    Rerender();
                }
            }
            else if (scroll.y < 0)
            {
                // scroll down
                if (itemPointer < StateSavingController.saveState.playerFurnitureInventory.Count)
                {
                    itemPointer++;
                    if(maxPointer < itemPointer)
                    {
                        minPointer++;
                    }
                    if (invPointer < 4 && inventoryItemSlots[invPointer + 1].sprite != null)
                    {
                        invPointer++;
                    }
                    Rerender();
                }
            }
            if(oldInvCount != StateSavingController.saveState.playerFurnitureInventory.Count)
            {
                Rerender();
                oldInvCount = StateSavingController.saveState.playerFurnitureInventory.Count;
            }
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class RawInventoryRenderer : MonoBehaviour
{
    InventoryAnimation inventoryAnimation;
    bool renderVisible = false;
    Image[] inventoryItemSlots;

    void Awake()
    {
        inventoryAnimation = GetComponent<InventoryAnimation>();
        inventoryItemSlots = new Image[transform.childCount];
        for(int i = 0; i < transform.childCount; i++)
        {
            Image img = transform.GetChild(i).GetChild(0).gameObject.GetComponent<Image>();
            inventoryItemSlots[i] = img;
        }
    }

    void Rerender()
    {
        for(int i = 0; i < inventoryItemSlots.Length; i++)
        {
            try
            {
                inventoryItemSlots[i].sprite = StateSavingController.saveState.playerFurnitureInventory[i].sprite;
            } catch(System.Exception)
            {
                Debug.Log("Item is null");
                inventoryItemSlots[i].sprite = null;
            }
            inventoryItemSlots[i].color = Color.white; // clear selections
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(inventoryAnimation.IsInventoryOpen)
        {
            
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
using SSC = StateSavingController;

public class ShopBuyItem : MonoBehaviour
{
    PlayerMovement player;
    [SerializeField] LayerMask ItemsMask;
    [SerializeField] NPCDialogController shopkeeperController;
    SellableItem currentItem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.qKey.wasPressedThisFrame)
        {
            if (shopkeeperController.Active) return;
            Vector2 direction = player.GetVectorDirection();
            RaycastHit2D ray = Physics2D.Raycast(transform.position, direction, .2f, ItemsMask);
            Debug.DrawRay(transform.position, direction,Color.cyan);
            if (ray.collider == null) return;
            GameObject obj = ray.collider.gameObject;
            currentItem = obj.GetComponent<SellableItem>();
            shopkeeperController.TextArguments = new object[] { currentItem.Price, currentItem.Name };
            shopkeeperController.Activate(3);
        }
    }

    public void AcceptBuying()
    {
        if(currentItem.Price > SSC.saveState.coins)
        {
            shopkeeperController.SetPointer(8);
            return;
        }
        if(currentItem.IsFurniture)
        {
            shopkeeperController.SetPointer(4);
            SSC.saveState.playerFurnitureInventory.Add(currentItem.furniture);
            SSC.saveState.coins -= currentItem.Price;
        } else
        {
            switch(currentItem.itemType)
            {
                case ItemType.HOE:
                    if(SSC.saveState.HoeDurability > 0)
                    {
                        shopkeeperController.SetPointer(9);
                        break;
                    }
                    SSC.saveState.HoeDurability = 20;
                    shopkeeperController.SetPointer(10);
                    break;
                case ItemType.WATERING:
                    if (SSC.saveState.WateringDurability > 0)
                    {
                        shopkeeperController.SetPointer(9);
                        break;
                    }
                    SSC.saveState.WateringDurability = 5;
                    shopkeeperController.SetPointer(10);
                    break;
                case ItemType.WHEAT_SEED:
                    SSC.saveState.WheatSeeds += Random.Range(5, 8); // 5 - 7 seeds per packet
                    shopkeeperController.SetPointer(11);
                    break;
                case ItemType.TOMATO_SEED:
                    SSC.saveState.TomatoSeeds += Random.Range(5, 8); // 5 - 7 seeds per packet
                    shopkeeperController.SetPointer(11);
                    break;
            }
            SSC.saveState.coins -= currentItem.Price;
        }
        currentItem = null;
    }

    public void DenyBuying()
    {
        currentItem = null;
        Debug.Log("rejected offer");
        shopkeeperController.avatarAnimator.SetBool("Annoyed", true);
        shopkeeperController.SetPointer(5);
    }
}

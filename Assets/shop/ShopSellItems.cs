using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using Image = UnityEngine.UI.Image;

public class ShopSellItems : MonoBehaviour
{
    // furniture inventory
    [SerializeField] InventoryRenderer invRend;
    // raycaster for ui
    [SerializeField] GraphicRaycaster raycaster;
    // 
    [SerializeField] DailyItemCatalog dic;
    [SerializeField] TMP_Text totalText;
    [SerializeField] TMP_Text maxWheat, maxTomato;
    [SerializeField] NPCDialogController shopkeeper;
    [SerializeField] PlayerMovement player;
    [SerializeField] TMP_InputField wheatCount, tomatoCount;
    int total = 0;
    [SerializeField] Sprite[] acceptButton, denyButton, furnitureSellButton;
    [SerializeField] string acceptButtonName, denyButtonName, funitureSellButtonName;
    [SerializeField] float LerpDuration = 1f;
    List<FurnitureItem> oldList;
    string[] btnNames;
    GameObject buttonPressedATM; // button pressed at the moment

    List<int> furnitureSubTotal;
    int wheatSubtotal = 0, tomatoSubtotal = 0;

    public bool IsSellingActive { private set; get; } = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        furnitureSubTotal = new List<int>();
        btnNames = new string[] { acceptButtonName, denyButtonName, funitureSellButtonName };
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
        wheatCount.onValueChanged.AddListener(onWheatChange);
        tomatoCount.onValueChanged.AddListener(onTomatoChange);
    }

    int GetFormattedResult(string text, int max)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
        {
            return 0;
        }
        if (int.TryParse(text, out int value))
        {
            if (value > max)
            {
                return max;
            }
            if (value < 0)
            {
                return 0;
            }
            return value;
        }
        return 0;
    }

    void onWheatChange(string text)
    {
        int max = StateSavingController.saveState.UsableWheat;
        int value = GetFormattedResult(text, max);
        wheatCount.text = value.ToString();
        // price for both is 10
        wheatSubtotal = value * 10;
        CalculateTotal();
    }

    void onTomatoChange(string text)
    {
        int max = StateSavingController.saveState.UsableTomato;
        int value = GetFormattedResult(text, max);
        tomatoCount.text = value.ToString();
        // price for both is 10
        tomatoSubtotal = value * 10;
        CalculateTotal();
    }

    void CalculateTotal()
    {
        total = wheatSubtotal + tomatoSubtotal;
        foreach(int price in furnitureSubTotal)
        {
            total += price;
        }
        totalText.text = $"Total:\n{total} coins";
    }

    int GetFurniturePrice(Tile tile)
    {
        foreach(CatalogEntry c in dic.decoEntries)
        {
            if (!c.IsFurniture) continue;
            if(c.furniture.tile == tile)
            {
                return c.SellingPrice;
            }
        }
        return 0;
    }

    Sprite GetButtonSprite(string name, bool pressed)
    {
        int index = pressed ? 1 : 0;
        if(name == acceptButtonName)
        {
            return acceptButton[index];
        }
        if (name == denyButtonName)
        {
            return denyButton[index];
        }
        if (name == funitureSellButtonName)
        {
            return furnitureSellButton[index];
        }
        return null;
    }

    void HandleButtonAction(string name)
    {
        if (name == acceptButtonName)
        {
            if (total == 0) return; // do not proceed if total is 0
            HideSellingUI();
            StateSavingController.saveState.coins += total;
            StateSavingController.saveState.UsableWheat -= wheatSubtotal / 10;
            StateSavingController.saveState.UsableTomato -= tomatoSubtotal / 10;
            shopkeeper.Activate(13);
            return;
        }
        if (name == denyButtonName)
        {
            HideSellingUI(true);
            shopkeeper.Activate(12);
            return;
        }
        if (name == funitureSellButtonName)
        {
            Tile f = invRend.GetSelectedFurniture();
            if (f != null)
            {
                StateSavingController.saveState.playerFurnitureInventory.RemoveAt(invRend.GetSelectedPointer());
                furnitureSubTotal.Add(GetFurniturePrice(f));
                CalculateTotal();
            }
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(IsSellingActive)
        {
            if(Mouse.current.leftButton.wasPressedThisFrame)
            {
                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition
                };

                List<RaycastResult> results = new List<RaycastResult>();
                raycaster.Raycast(pointerData, results);

                foreach (RaycastResult result in results)
                {
                    GameObject obj = result.gameObject;
                    string objectName = obj.name;
                    if (!btnNames.Contains(objectName)) continue;
                    Debug.Log("Hit UI: " + objectName);
                    buttonPressedATM = obj;
                    obj.GetComponent<Image>().sprite = GetButtonSprite(objectName,true);
                    HandleButtonAction(objectName);
                    break;
                }
            }
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                if(buttonPressedATM != null)
                {
                    string objectName = buttonPressedATM.name;
                    buttonPressedATM.GetComponent<Image>().sprite = GetButtonSprite(objectName, false);
                }
            }
        }
    }

    public void ShowSellingUI()
    {
        Cursor.visible = true;
        player.SetAllowedToMove(false);
        shopkeeper.EndDialog("selling ui dialog end");
        maxWheat.text = $"/ {StateSavingController.saveState.UsableWheat}";
        maxTomato.text = $"/ {StateSavingController.saveState.UsableTomato}";
        total = 0;
        wheatSubtotal = 0;
        tomatoSubtotal = 0;
        furnitureSubTotal.Clear();
        CalculateTotal();
        IsSellingActive = true;
        gameObject.SetActive(true);
        StartCoroutine(Anim());
    }

    public void HideSellingUI(bool restoreOldList = false)
    {
        Cursor.visible = false;
        player.SetAllowedToMove(true);
        IsSellingActive = false;
        StartCoroutine(Anim());
        if (!restoreOldList) return;
        StateSavingController.saveState.playerFurnitureInventory = oldList;
        oldList.Clear();
    }

    IEnumerator Anim()
    {
        Vector3 a = IsSellingActive ? Vector3.zero : Vector3.one;
        Vector3 b = IsSellingActive ? Vector3.one : Vector3.zero;
        float elapsed = 0f;
        while (elapsed < LerpDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / LerpDuration);
            transform.localScale = Vector3.Lerp(a, b, t);
            yield return null;
        }
        transform.localScale = Vector3.one;

        if(!IsSellingActive)
        {
            gameObject.SetActive(false);
            invRend.IsInventoryOpen = false;
        } else
        {
            invRend.IsInventoryOpen = true;
            oldList = StateSavingController.saveState.playerFurnitureInventory;
        }
    }
}

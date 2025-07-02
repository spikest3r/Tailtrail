using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryAnimation : MonoBehaviour
{
    public bool IsInventoryOpen { private set; get; } = false;
    public bool Available { get; private set; } = true;

    [SerializeField] float Duration = 2f;
    [SerializeField] float OpenY, CloseY;
    RectTransform[] slots;

    void Awake()
    {
        int len = transform.childCount;
        slots = new RectTransform[len];
        for(int i = 0; i < len; i++)
        {
            slots[i] = transform.GetChild(i).gameObject.GetComponent<RectTransform>();
        }
    }

    public void ToggleInventory(bool state)
    {
        IsInventoryOpen = state;
        StartCoroutine(PlayAnim());
    }

    IEnumerator PlayAnim()
    {
        Available = false;
        for(int i = 0; i < slots.Length; i++)
        {
            RectTransform slot = slots[i];
            StartCoroutine(AnimateSlot(slot));
            yield return new WaitForSeconds(0.2f);
        }
        Available = true;
    }

    IEnumerator AnimateSlot(RectTransform slot)
    {
        float elapsed = 0f;
        Vector3 a = IsInventoryOpen ? new Vector3(slot.anchoredPosition.x,CloseY,0) : new Vector3(slot.anchoredPosition.x, OpenY, 0);
        Vector3 b = IsInventoryOpen ? new Vector3(slot.anchoredPosition.x, OpenY, 0) : new Vector3(slot.anchoredPosition.x, CloseY, 0);
        while (elapsed < Duration)
        {
            float t = Mathf.Clamp01(elapsed / Duration);
            float easedT = Mathf.SmoothStep(0f, 1f, t);
            slot.anchoredPosition = Vector3.Lerp(a, b, easedT);

            elapsed += Time.deltaTime;
            yield return null;
        }
        slot.anchoredPosition = b; // snap to destination
        Debug.Log("Done");
    }
}

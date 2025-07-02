using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemCounterController : MonoBehaviour
{
    public static ItemCounterController Instance;

    Image Icon;
    TMP_Text Value;
    RectTransform Parent;
    Coroutine routine;
    bool Open = false;

    [SerializeField] float OpenY, CloseY, TimeoutTime = 3f, Duration = 1f;
    internal UnityAction OnHide;

    void Awake()
    {
        Icon = GameObject.Find("ItemCounterIcon").GetComponent<Image>();
        Value = GameObject.Find("ItemCounterValue").GetComponent<TMP_Text>();
        Parent = Icon.gameObject.transform.parent.gameObject.GetComponent<RectTransform>();
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Parent.anchoredPosition = new Vector3(Parent.anchoredPosition.x, CloseY);
    }

    public void ShowValue(Sprite icon, int value)
    {
        Icon.sprite = icon;
        Value.text = value.ToString();
        if (!Open)
        {
            Open = true;
            StartCoroutine(Lerp());
            return;
        }
        HandleRoutine();
    }

    IEnumerator Lerp(bool startRoutine = true)
    {
        float elapsed = 0f;
        Vector3 a = Open ? new Vector3(Parent.anchoredPosition.x, CloseY, 0) : new Vector3(Parent.anchoredPosition.x, OpenY, 0);
        Vector3 b = Open ? new Vector3(Parent.anchoredPosition.x, OpenY, 0) : new Vector3(Parent.anchoredPosition.x, CloseY, 0);
        while (elapsed < Duration)
        {
            float t = Mathf.Clamp01(elapsed / Duration);
            float easedT = Mathf.SmoothStep(0f, 1f, t);
            Parent.anchoredPosition = Vector3.Lerp(a, b, easedT);

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!startRoutine) yield break;
        HandleRoutine();
    }

    void HandleRoutine()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
        }
        routine = StartCoroutine(Timeout());
    }

    IEnumerator Timeout()
    {
        yield return new WaitForSeconds(TimeoutTime);
        Open = false;
        OnHide.Invoke();
        StartCoroutine(Lerp(false));
    }
}

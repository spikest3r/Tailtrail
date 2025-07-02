using System.Collections;
using TMPro;
using UnityEngine;
using SSC = StateSavingController;

public class CoinsSign : MonoBehaviour
{
    int oldCoins;
    TMP_Text coinsText;
    Coroutine countCoroutine;

    private void Awake()
    {
        oldCoins = SSC.saveState.coins;
        coinsText = GameObject.Find("CoinsText").GetComponent<TMP_Text>();
        if (coinsText == null)
        {
            Debug.LogError("CoinsText is not found!");
            Destroy(this);
        }
    }

    private void Start()
    {
        SetTextInstant(oldCoins);
    }

    private void LateUpdate()
    {
        int currentCoins = SSC.saveState.coins;
        if (currentCoins != oldCoins)
        {
            if (countCoroutine != null)
                StopCoroutine(countCoroutine);

            countCoroutine = StartCoroutine(CountCoins(oldCoins, currentCoins));
            oldCoins = currentCoins;
        }
    }

    private void SetTextInstant(int value)
    {
        coinsText.text = value.ToString();
    }

    private IEnumerator CountCoins(int from, int to)
    {
        int step = from < to ? 1 : -1;
        int current = from;

        while (current != to)
        {
            current += step;
            coinsText.text = current.ToString();
            yield return new WaitForSeconds(0.005f); // Faster, smoother animation
        }

        coinsText.text = to.ToString(); // Make sure final value is exact
    }
}

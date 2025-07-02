using System;
using UnityEngine;

public class HourlyBell : MonoBehaviour
{
    int hourBellPlayed = -1; // didnt play yet
    [SerializeField] AudioSource bellSource;
    [SerializeField] AudioClip bellClip;
    [SerializeField] bool DebugPlayNow = false;

    // Update is called once per frame
    void Update()
    {
        if((DateTime.Now.Minute == 0 && DateTime.Now.Hour != hourBellPlayed) || DebugPlayNow)
        {
            DebugPlayNow = false;
            hourBellPlayed = DateTime.Now.Hour;
            bellSource.loop = false;
            bellSource.clip = bellClip;
            bellSource.Play();
        }
    }
}

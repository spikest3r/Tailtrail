using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using RS = StateSavingController.RuntimeState;

public class AmbientMusicPlayer : MonoBehaviour
{
    [SerializeField] float DayTime, NightTime;
    [SerializeField] AudioClip[] DayTimeClips, NightTimeClips;
    [SerializeField] AudioSource AmbientMusicSource;

    AudioClip[] DecideClips()
    {
        if (DateTime.Now.Hour < NightTime && DateTime.Now.Hour > DayTime)
        {
            return DayTimeClips;
        }
        return NightTimeClips;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(RS.indexPlaying != -1)
        {
            AmbientMusicSource.clip = DecideClips()[RS.indexPlaying];
            AmbientMusicSource.loop = false;
            AmbientMusicSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!AmbientMusicSource.isPlaying)
        {
            AudioClip[] clips = DecideClips();

            List<int> availableIndexes = Enumerable.Range(0, clips.Length)
                                      .Where(i => !RS.indexesPlayed.Contains(i))
                                      .ToList();
            if (availableIndexes.Count == 0)
            {
                RS.indexesPlayed.Clear();
                RS.indexPlaying = 0;
            }
            else
            {
                RS.indexPlaying = availableIndexes[Random.Range(0, availableIndexes.Count)];
            }

            AmbientMusicSource.clip = clips[RS.indexPlaying];
            AmbientMusicSource.loop = false;
            AmbientMusicSource.Play();
        }
    }
}

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    // static fields
    public static SceneTransition Instance { private set; get; }

    // flags
    [SerializeField] bool FadeOnStart = true;
    [SerializeField] float Speed = 1f;
    [SerializeField] AudioSource[] audioSources;

    [SerializeField] UnityEvent onAwakeTransitionEnd;
    bool awakeTransitionFinished = false;

    // system
    RawImage image;

    void Awake()
    {
        image = GetComponent<RawImage>();
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (FadeOnStart) StartCoroutine(DoTransition());
        else awakeTransitionFinished = true; // didnt happen so we set flag
    }

    public void LoadScene(string Name) // load scene but with transition
    {
        StartCoroutine(ChangeScene(Name));
    }

    IEnumerator ChangeScene(string Name)
    {
        yield return DoTransition();
        SceneManager.LoadScene(Name);
    }

    IEnumerator DoTransition()
    {
        Debug.Log("Requested transition");
        bool isStartDark = image.color.a > 0; // if a > 0 its opaque and means dark
        float[] startVolumes = new float[0];
        if (!isStartDark && audioSources != null)
        {
            startVolumes = new float[audioSources.Length];
            for (int i = 0; i < audioSources.Length; i++)
                startVolumes[i] = audioSources[i].volume;
        }

        while (isStartDark ? image.color.a > 0f : image.color.a < 1f)
        {
            float direction = (isStartDark ? -1 : 1) * Speed * Time.deltaTime;
            Color c = image.color;
            c.a += direction;
            image.color = c;

            if (!isStartDark && audioSources != null)
            {
                float t = 1f - c.a;
                for (int i = 0; i < audioSources.Length; i++)
                    audioSources[i].volume = startVolumes[i] * t;
            }

            yield return null;
        }

        if(!awakeTransitionFinished)
        {
            onAwakeTransitionEnd.Invoke();
            awakeTransitionFinished = true;
        }
    }
}

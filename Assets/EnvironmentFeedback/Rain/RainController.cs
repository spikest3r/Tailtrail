using System;
using UnityEngine;

public class RainController : MonoBehaviour
{
    [Range(0f, 1f)] public float rainThreshold = 0.6f;
    public float noiseScale = 0.1f;
    public float seed = 1337f;

    public bool IsRainy { get; private set; }
    public float RainStrength { get; private set; }

    [SerializeField] ParticleSystem rainParticles;

    private void Awake()
    {
        GenerateWeather();
        if (!IsRainy) return;
        Debug.Log("Today is rainy day");
        ParticleSystem.EmissionModule emission = rainParticles.emission;
        emission.rateOverTime = RainStrength * 50;
        rainParticles.Play();
    }

    void GenerateWeather()
    {
        DateTime today = DateTime.Now;
        int year = today.Year;
        int month = today.Month;
        int day = today.Day;

        float dayAsFloat = year * 365 + month * 30 + day;

        float noise = Mathf.PerlinNoise(dayAsFloat * noiseScale, seed);

        IsRainy = noise < rainThreshold;
        RainStrength = IsRainy ? Mathf.InverseLerp(rainThreshold, 1f, noise) : 0f;
        Debug.Log(string.Format("IsRainy: {0}, RainStrength: {1}, noise: {2}", IsRainy, RainStrength, noise));
    }
}

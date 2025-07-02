using UnityEngine;

static class Helpers
{
    [System.Serializable]
    public struct Vector3DTO
    {
        public float x;
        public float y;
        public float z;

        public Vector3DTO(Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public Vector3 ToVector3() => new Vector3(x, y, z);
    }

    public static float[] seed;

    public static bool RandomChance(float chancePercent)
    {
        // chancePercent should be from 0 to 100
        return Random.value < (chancePercent / 100f);
    }

    public static Vector3 RandomVector3(Vector3 min, Vector3 max)
    {
        return new Vector3(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z));
    }

    public static Vector3 GetNoisyRandomPos(Vector3 minRange, Vector3 maxRange)
    {
        Vector3 pos = new Vector3(
            Mathf.Lerp(minRange.x, maxRange.x, Mathf.PerlinNoise(seed[0], Time.time)),
            Mathf.Lerp(minRange.y, maxRange.y, Mathf.PerlinNoise(seed[1], Time.time)),
            Mathf.Lerp(minRange.z, maxRange.z, Mathf.PerlinNoise(seed[2], Time.time))
        );

        float jitterAmount = 5f;
        pos += new Vector3(
            Random.Range(-jitterAmount, jitterAmount),
            Random.Range(-jitterAmount, jitterAmount),
            Random.Range(-jitterAmount, jitterAmount)
        );

        return pos;
    }
}
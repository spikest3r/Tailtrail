using System;
using UnityEngine;

public enum CropType
{
    WHEAT = 0, TOMATO = 1
}

[System.Serializable]
public class CropData
{
    public Vector3Int position;
    public int growPhase;
    public DateTime whenPlanted;
    public CropType type;
}

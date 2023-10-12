using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers 
{
    public static float Wrap01(float value)
    {
        float temp = value;
        
        while (temp > 1.0f) temp -= 1.0f;
        while (temp < 0.0f) temp += 1.0f;

        return temp;
    }

    public static void RecursiveLayerSet(Transform root, int layer)
    {
        root.gameObject.layer = layer;
        foreach (Transform t in root)
        {
            RecursiveLayerSet(t, layer);
        }
    }

    public const int HighlightingLayer = 8;
    public const int HubStationLayer = 29;
    public const int SelectableInfoLayer = 30;
}

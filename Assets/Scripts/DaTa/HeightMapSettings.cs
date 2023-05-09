using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[CreateAssetMenu()]
public class HeightMapSettings : UpdateData
{
    public NoiseSettings noiseSettings;

    public bool useFallOff;



    //-----
    //Bien cho xu ly mesh
    public float HeightMultiplier;
    public AnimationCurve HeightCurve;

    public float minHeight
    {
        get
        {
            return HeightMultiplier * HeightCurve.Evaluate(0);
        }
    }
    public float maxHeight
    {
        get
        {
            return HeightMultiplier * HeightCurve.Evaluate(1);
        }
    }
    public void CopyFrom(HeightMapSettings other)
    {
        // Copy the values from the other instance to this instance
        noiseSettings = other.noiseSettings;
        useFallOff = other.useFallOff;
        HeightMultiplier = other.HeightMultiplier;
        HeightCurve = new AnimationCurve(other.HeightCurve.keys); // Create a new instance of AnimationCurve with the keys from the other instance
    }
#if UNITY_EDITOR
    protected override void OnValidate()
    {
        noiseSettings.ValidateValues();

        base.OnValidate();
    }
#endif


}

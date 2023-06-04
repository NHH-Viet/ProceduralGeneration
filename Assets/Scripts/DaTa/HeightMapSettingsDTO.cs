using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class HeightMapSettingsDTO
{

    public bool useFallOff;



    //-----
    //Bien cho xu ly mesh
    public float HeightMultiplier;
    public AnimationCurveData HeightCurve;
    public Vector2Data vectorData2;
    public NoiseSettingsDTO noiseSettings;

    public float minHeight;

    public float maxHeight;

}

[Serializable]
public class AnimationCurveData
{
    public KeyframeData[] keys;
    public float preWrapMode;
    public float postWrapMode;

    public AnimationCurveData(AnimationCurve curve)
    {
        keys = new KeyframeData[curve.length];
        for (int i = 0; i < curve.length; i++)
        {
            keys[i] = new KeyframeData(curve[i]);
        }
        preWrapMode = (float)curve.preWrapMode;
        postWrapMode = (float)curve.postWrapMode;
    }

    public AnimationCurve ToAnimationCurve()
    {
        Keyframe[] keyframes = new Keyframe[keys.Length];
        for (int i = 0; i < keys.Length; i++)
        {
            keyframes[i] = keys[i].ToKeyframe();
        }
        return new AnimationCurve(keyframes)
        {
            preWrapMode = (WrapMode)preWrapMode,
            postWrapMode = (WrapMode)postWrapMode
        };
    }
    // public AnimationCurve ToAnimationCurve()
    // {
    //     AnimationCurve curve = new AnimationCurve();

    //     foreach (KeyframeData keyframeDto in keyframes)
    //     {
    //         Keyframe keyframe = new Keyframe
    //         {
    //             time = keyframeDto.time,
    //             value = keyframeDto.value,
    //             inTangent = keyframeDto.inTangent,
    //             outTangent = keyframeDto.outTangent
    //         };

    //         curve.AddKey(keyframe);
    //     }

    //     curve.preWrapMode = this.preWrapMode;
    //     curve.postWrapMode = this.postWrapMode;

    //     return curve;
    // }
}
[Serializable]
public class Vector2Data
{
    public float x;
    public float y;

    public Vector2Data(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public static implicit operator Vector2(Vector2Data data)
    {
        return new Vector2(data.x, data.y);
    }

    public static implicit operator Vector2Data(Vector2 vector)
    {
        return new Vector2Data(vector.x, vector.y);
    }
}

[Serializable]
public class NoiseSettingsDTO
{
    public int normallizeMode;
    public float scale;
    public int octaves;
    public float persistence;
    public float lacunarity;
    public int seed;
    public Vector2Data offset;

    public void ValidateValues()
    {
        scale = Mathf.Max(scale, 0.01f);
        octaves = Mathf.Max(octaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1);
        persistence = Mathf.Clamp01(persistence);
    }
    public NoiseSettings ToNoiseSettings()
    {
        NoiseSettings noiseSettings = new NoiseSettings
        {
            scale = this.scale,
            octaves = this.octaves,
            persistence = this.persistence,
            lacunarity = this.lacunarity,
            seed = this.seed,
            offset = this.offset
        };

        return noiseSettings;
    }
}

[Serializable]
public class KeyframeData
{
    public float time;
    public float value;
    public float inTangent;
    public float outTangent;

    public KeyframeData(Keyframe keyframe)
    {
        time = keyframe.time;
        value = keyframe.value;
        inTangent = keyframe.inTangent;
        outTangent = keyframe.outTangent;
    }

    public Keyframe ToKeyframe()
    {
        Keyframe keyframe = new Keyframe(time, value, inTangent, outTangent);
        return keyframe;
    }
}


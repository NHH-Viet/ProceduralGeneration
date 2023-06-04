using UnityEngine;

public static class HeightMapGenerator
{
    public static float[,] CombineFalloff(float [,] noisemap, int size)
    {
        float[,] falloffMap = FalloffGenerator.GenerateFalloffMap(size);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {   
                    noisemap[x, y] = Mathf.Clamp01(noisemap[x, y] - falloffMap[x, y]);
                }
            }
            return noisemap;
    }
    public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings setting)
    {
        AnimationCurve heightCure_thread = new AnimationCurve(setting.HeightCurve.keys);
        float[,] values = Noise.GenerateNoiseMap(width, height, setting.noiseSettings);
        float minValue = float.MaxValue;
        float maxValue = float.MinValue;
        if (setting.useFallOff)
        {
            values = CombineFalloff(values,width);
        }
        if(setting.HeightMultiplier == 0)
        {
           setting.HeightMultiplier = 1; 
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                values[i, j] *= heightCure_thread.Evaluate(values[i, j]) * setting.HeightMultiplier;
                if (values[i, j] > maxValue)
                {
                    maxValue = values[i, j];
                }
                if (values[i, j] < minValue)
                {
                    minValue = values[i, j];
                }
            }
        }
        return new HeightMap(values, minValue, maxValue);
    }
}
// Struct lưu giá trị heightMap (perlinnoise map) và colour map để tiện cho các hàm khác sử dụng
public struct HeightMap //heightmap
{
    public readonly float[,] heightMap; //value
    public readonly float minValue;
    public readonly float maxValue;

    //public readonly Color[] colourMap; 
    //chuyển

    public HeightMap(float[,] heightMap, float minValue, float maxValue /*Color[] colourMap*/)
    {
        this.heightMap = heightMap;
        this.minValue = minValue;
        this.maxValue = maxValue;
        //this.colourMap = colourMap;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    // mode chuẩn hóa, local nếu là render 1 khối, globa là nhiều khối
    public enum NormallizeMode { Local, Global }
    //---
    //Hàm chính xử lý trả về giá trị perlin noise cho từng tọa độ x,y tương ứng
    //Input chính là chiều dài và rộng
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings, Vector2 sampleCenter  /*int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormallizeMode normallizeMode*/)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight]; // mảng 2 chiều lưu giá trị noise với điểm tương ứng (output)

        System.Random prng = new System.Random(settings.seed); // random seed để khi kết hợp với tọa độ để cho ra nhiều mẫu hơn


        // biến liên quan hàm endless terrain (cắt bỏ sau)
        float maxPosHeight = 0;
        float amplitude = 1;
        float frequency = 1;
        //-----
        //Xử lý để lấy các phần khác nhau của noiseMap
        Vector2[] octaveOffsets = new Vector2[settings.octaves];
        for (int i = 0; i < settings.octaves; i++)
        {
            // offsetX và Y là tọa độ ngẫu nhiên, offset.x và y là tọa độ ta chọn
            //-100000,100000 vì cao quá thì lúc nào cũng ra 1 số
            float offsetX = prng.Next(-100000, 100000) + settings.offset.x + sampleCenter.x;
            float offsetY = prng.Next(-100000, 100000) - settings.offset.y - sampleCenter.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            // biến liên quan hàm endless terrain (cắt bỏ sau)
            maxPosHeight += amplitude;
            amplitude *= settings.persistance;
        }

        // biến lưu độ cao nhất của map 
        float minNoiseHeight = float.MaxValue;
        float maxNoiseHeight = float.MinValue;
        // Khi thay scale thì map sẽ zoom từ góc trên phải, biến dưới để xử lý zoom từ giữa ra
        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;
        //vòng lặp xử lý Chính
        //Chạy vòng lặp hết các tọa độ rồi với từng tọa độ đó tính perlin value riêng của nó
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                // biến liên quan hàm endless terrain (cắt bỏ sau)
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;
                //-----
                //Chạy hết các octaves 
                for (int i = 0; i < settings.octaves; i++)
                {
                    //Ta chỉ cần x và y là đủ nhưng để xử lý: zoom chính giữa, tỉ lệ phóng to nhỏ, offset, octaves thì ta cần biến tấu lại x, y
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / settings.scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / settings.scale * frequency;
                    //----
                    //Hàm perlin noise của unity: input:x,y output: [0,1] tương ứng x,y
                    // Gia tri tra ve cua thu vien perlinnoise la tu 0 den 1 nhung de ket qua them da dang ta co the *2-1 de cho ket qua ra tu -1 den 1
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    // xử lý biên độ và tần xuất octaves
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= settings.persistance;
                    frequency *= settings.lacunarity;
                }
                // tìm khoảng giá trị của kết quả [max,min] để tiện cho chuẩn hóa
                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight; // lưu kết quả tính đc

                if (settings.normallizeMode == NormallizeMode.Global)
                {
                    //mode xử lý bug endlessmode (cắt bỏ sau)
                    float normallizeHeight = (noiseMap[x, y] + 1) / (2f * maxPosHeight / 1.9f);
                    noiseMap[x, y] = Mathf.Clamp(normallizeHeight, 0, int.MaxValue);

                }

            }
        }

        // Vì noisemap của ta đang có giá trị từ -1 đến 1 nên ta chuẩn hóa nó lại
        if (settings.normallizeMode == NormallizeMode.Local)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {

                    // ham inverlerp tra ve gia tri tu 0 den 1 gia su noisemap bang minNoise thi tra ve 0 va nguoc lai
                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]); //(value - point1) / (point2 - point1)

                }
            }

        }
        return noiseMap; // mảng kết quả
    }
}

[System.Serializable]
public class NoiseSettings
{
    public Noise.NormallizeMode normallizeMode;


    public float scale = 50; //scale
    public int octaves = 6;
    [Range(0, 1)]
    public float persistance = 0.5f;
    public float lacunarity = 2;

    public int seed;
    public Vector2 offset;

    public void ValidateValues()
    {
        scale = Mathf.Max(scale, 0.01f);
        octaves = Mathf.Max(octaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1);
        persistance = Mathf.Clamp01(persistance);

    }
}
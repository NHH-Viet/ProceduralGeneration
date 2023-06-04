using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight]; // mảng 2 chiều lưu giá trị noise với điểm tương ứng (output)
        float amplitude;
        float frequency;
        System.Random prng = new System.Random(settings.seed); // random seed để khi kết hợp với tọa độ để cho ra nhiều mẫu hơn
        //-----
        //Xử lý để lấy các phần khác nhau của noiseMap
        Vector2[] octaveOffsets = new Vector2[settings.octaves];
        for (int i = 0; i < settings.octaves; i++)
        {
            // offsetX và Y là tọa độ ngẫu nhiên, offset.x và y là tọa độ ta chọn
            //-100000,100000 vì cao quá thì lúc nào cũng ra 1 số
            float offsetX = prng.Next(-100000, 100000) + settings.offset.x;
            float offsetY = prng.Next(-100000, 100000) - settings.offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
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
                    // Giá trị trả về của thư viện perlinnoise là từ 0 đến 1 nhưng để kết quả thêm đa dạng ta có thể thay miền giá trị thành -1 đến 1
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    // xử lý biên độ và tần xuất octaves
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= settings.persistence;
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

            }
        }

        // Vì noisemap của ta đang có giá trị từ -1 đến 1 nên ta chuẩn hóa nó lại
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {

                // ham inverlerp tra ve gia tri tu 0 den 1 gia su noisemap bang minNoise thi tra ve 0 va nguoc lai
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]); //(value - point1) / (point2 - point1)

            }
        }
        return noiseMap; // mảng kết quả
    }

}

[System.Serializable]
public class NoiseSettings
{
    public float scale;
    public int octaves;
    [Range(0, 1)]
    public float persistence;
    public float lacunarity;

    public int seed;
    public Vector2 offset;
}
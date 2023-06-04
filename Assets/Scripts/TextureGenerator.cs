using UnityEngine;

public static class TextureGenerator
{
    // hàm tạo texture từ heightmap
    //---
    //Tạo texture có màu (theo vùng)
    public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height){
        Texture2D texture = new Texture2D (width, height,TextureFormat.RGB24, false);
        //Xử lý độ mờ và bo viền
        //texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        //------------
        texture.SetPixels (colourMap);
        texture.Apply();
        return texture;
    }
    //tạo texture trắng đen (perlin noisemap cổ điển)
    public static Texture2D TextureFromHeightMap(HeightMap heightMap){
        int width = heightMap.heightMap.GetLength (0);
        int height = heightMap.heightMap.GetLength (1);


        Color[] colourMap = new Color[width*height];
        for(int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++){
                //gán màu với độ cao (từ 0 đến 1) tương ứng
                colourMap[ y* width + x] = Color.Lerp (Color.black, Color.white, Mathf.InverseLerp(heightMap.minValue,heightMap.maxValue,heightMap.heightMap[x,y]));
            }
        }
        return TextureFromColourMap(colourMap,width,height);
    }

    public static Color[] colorMapGenerator(int mapChunkSize, float[,] noiseMap, TextureData textureData){
        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
		for (int y = 0; y < mapChunkSize; y++) {
			for (int x = 0; x < mapChunkSize; x++) {
				float currentHeight = noiseMap [x, y];
				for (int i = 0; i < 7; i++) {
					if (currentHeight <= textureData.layers[i].startHeight && textureData.layers[i].active==1) {
						colorMap [y * mapChunkSize + x] = textureData.layers[i].tint;
						break;
					}
				}
			}
		}
        return colorMap;
    }
}

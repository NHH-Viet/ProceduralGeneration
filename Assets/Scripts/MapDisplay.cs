using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapDisplay : MonoBehaviour
{
    //class in kết quả
    public Renderer textureRender; // thư viện renderer của unity
    public MeshFilter meshFilter; // Lưu lại mesh (thông tin của mesh)
    public MeshRenderer meshRenderer; // Lấy dữ liệu từ Mesh Filter rồi render ra mesh

    // enum để xác định mode vẽ
    public enum DrawMode { NoiseMap, /*ColourMap*/ Mesh, FallOffMap };
    public DrawMode drawMode;

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;

    public TextureData textureData;

    public Material terrainMat;

    //-----
    //Bien cho xu ly mesh
    //public float meshHeightMultiplier;
    //public AnimationCurve meshHeightCurve;
    [Range(0, MeshSettings.numofLODs - 1)]
    public int levelOfDetail;
    //-----
    //bien autoupdate
    public bool autoUpdate;

    //--
    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            DrawMapInEditor();
        }
        textureData.ApplytoMaterial(terrainMat);
    }
    void OnTextureValuesUpdated()
    {
        textureData.ApplytoMaterial(terrainMat);
    }

    //Lua mode ve trong editor (ve noisemap, ve noisemap co mau, ve mesh)
    public void DrawMapInEditor()
    {
        textureData.ApplytoMaterial(terrainMat);
        textureData.UpdateMeshHeight(terrainMat, heightMapSettings.minHeight, heightMapSettings.maxHeight);
        HeightMap mapData = HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerline, meshSettings.numVertsPerline, heightMapSettings, Vector2.zero);
        //MapDisplay display = FindObjectOfType<MapDisplay>();
        // Lựa draw Mode
        if (drawMode == DrawMode.NoiseMap)
        {
            DrawTextureMap(TextureGenerator.TextureFromHeightMap(mapData));
        }
        // else if (drawMode == DrawMode.ColourMap)
        // {
        //     display.DrawTextureMap(TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
        // }
        else if (drawMode == DrawMode.Mesh)
        {
            DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshSettings, levelOfDetail)/*, TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize)*/);
        }
        else if (drawMode == DrawMode.FallOffMap)
        {
            DrawTextureMap(TextureGenerator.TextureFromHeightMap(new HeightMap(FalloffGenerator.GenerateFalloffMap(meshSettings.numVertsPerline), 0, 1)));
        }
    }
    //-----
    public float[,] accessArray;
    public void DrawMapInRuntime(string mode)
    {
        textureData.ApplytoMaterial(terrainMat);
        textureData.UpdateMeshHeight(terrainMat, heightMapSettings.minHeight, heightMapSettings.maxHeight);
        HeightMap mapData = HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerline, meshSettings.numVertsPerline, heightMapSettings, Vector2.zero);
        accessArray = Noise.GenerateNoiseMap(meshSettings.numVertsPerline, meshSettings.numVertsPerline, heightMapSettings.noiseSettings, Vector2.zero);
        //MapDisplay display = FindObjectOfType<MapDisplay>();
        // Lựa draw Mode
        if (mode == "Noise")
        {
            //if(textureRender.gameObject.activeSelf != true)
            textureRender.gameObject.SetActive(true);
            meshFilter.gameObject.SetActive(false);
            DrawTextureMap(TextureGenerator.TextureFromHeightMap(mapData));
        }
        else if (mode == "Mesh")
        {
            textureRender.gameObject.SetActive(false);
            meshFilter.gameObject.SetActive(true);
            DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshSettings, levelOfDetail)/*, TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize)*/);
        }
        else
        {
            textureRender.gameObject.SetActive(false);
            meshFilter.gameObject.SetActive(true);
            DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshSettings, levelOfDetail)/*, TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize)*/);

        }
    }
    // ham valid gia tri
    void OnValidate()
    {
        // if (lacunarity < 1)
        //     lacunarity = 1;
        // if (octaves < 0)
        //     octaves = 0;
        // if (noiseScale < 0.01f)
        //     noiseScale = 0.01f;
        //chuyển sang noisedata

        if (meshSettings != null)
        {
            meshSettings.OnValuesUpdated -= OnValuesUpdated;
            meshSettings.OnValuesUpdated += OnValuesUpdated;
        }
        if (heightMapSettings != null)
        {
            heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
            heightMapSettings.OnValuesUpdated += OnValuesUpdated;
        }
        //falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
        if (textureData != null)
        {
            textureData.OnValuesUpdated -= OnValuesUpdated;
            textureData.OnValuesUpdated += OnValuesUpdated;
        }
    }

    // gán texture cho material
    public void DrawTextureMap(Texture2D texture)
    {
        // int width = noiseMap.GetLength(0);
        // int height = noiseMap.GetLength(1);

        // Texture2D texture = new Texture2D(width, height);

        // Color[] colourMap = new Color[width * height];
        // for (int y = 0; y < height; y++)
        // {
        //     for (int x = 0; x < width; x++)
        //     {
        //         colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
        //     }
        // }
        // texture.SetPixels(colourMap);
        // texture.Apply();

        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);

        //textureRender.gameObject.SetActive (true);
        //meshFilter.gameObject.SetActive(false);
    }

    // vẽ mesh từ meshData bên meshGenerator và gán texture cho nó
    public void DrawMesh(MeshData meshData /*Texture2D texture*/)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        //meshRenderer.sharedMaterial.mainTexture = texture;
        //AssetDatabase.CreateAsset(meshFilter.sharedMesh, "Assets/Saved/test2.asset");
        //meshFilter.transform.localScale = Vector3.one * FindObjectOfType<MapGenerator>().meshSettings.scale;
    }
}

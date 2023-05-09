using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EndlessTerrain : MonoBehaviour // TerrainGenerator
{
    //Class nâng cao, nghiên cứu và cắt bỏ sau
    const float scale = 5;
    const float viewerMoveforUpdate = 25f;
    const float sqtviewerMoveforUpdate = viewerMoveforUpdate * viewerMoveforUpdate;
    //public static float maxViewDist;
    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureSettings;
    public LODInfo[] detailsLv;
    public Transform viewer;
    public Material mapMaterial;
    Vector2 viewerPos; // was public static
    Vector2 viewerPosOld;
    //static MapGenerator mapGenerator;
    float meshWorldSize;
    int chunksVisibleInView;

    //Lưu lại những chunk đã redenr tránh render lại
    Dictionary<Vector2, TerrainChunk> terrainchunkDic = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>(); // was static
    void Start()
    {
        //mapGenerator = FindObjectOfType<MapGenerator>();
        textureSettings.ApplytoMaterial(mapMaterial);
        textureSettings.UpdateMeshHeight(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

        float maxViewDist = detailsLv[detailsLv.Length - 1].visibleDistThreshHold;
        meshWorldSize = meshSettings.meshWorldSize;
        chunksVisibleInView = Mathf.RoundToInt(maxViewDist / meshWorldSize);
        UpdateVisibleChunk();
    }

    void Update()
    {
        viewerPos = new Vector2(viewer.position.x, viewer.position.z);

        if ((viewerPosOld - viewerPos).sqrMagnitude > sqtviewerMoveforUpdate)
        {
            viewerPosOld = viewerPos;
            UpdateVisibleChunk();
        }

    }

    void UpdateVisibleChunk()
    {
        HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();
        for (int i = visibleTerrainChunks.Count - 1; i >= 0; i--)
        {
            alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].coord);
            visibleTerrainChunks[i].UpdateTerrainChunk();
        }
        // for (int i = 0; i < visibleTerrainChunks.Count; i++)
        // {
        //     visibleTerrainChunks[i].SetVisible(false);
        // }
        int currentChunkX = Mathf.RoundToInt(viewerPos.x / meshWorldSize);
        int currentChunkY = Mathf.RoundToInt(viewerPos.y / meshWorldSize);

        for (int yOffset = -chunksVisibleInView; yOffset <= chunksVisibleInView; yOffset++)
        {
            for (int xOffset = -chunksVisibleInView; xOffset <= chunksVisibleInView; xOffset++)
            {
                Vector2 viewedChunkCord = new Vector2(currentChunkX + xOffset, currentChunkY + yOffset);
                if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCord))
                {
                    if (terrainchunkDic.ContainsKey(viewedChunkCord))
                    {
                        terrainchunkDic[viewedChunkCord].UpdateTerrainChunk();
                        // if (terrainchunkDic[viewedChunkCord].IsVisible())
                        // {
                        //     terrainChunkVisibleLastUpdate.Add(terrainchunkDic[viewedChunkCord]);
                        // }
                    }
                    else
                    {
                        TerrainChunk newChunk = new TerrainChunk(viewedChunkCord, heightMapSettings, meshSettings, detailsLv, transform, viewer, mapMaterial);
                        terrainchunkDic.Add(viewedChunkCord, newChunk);
                        newChunk.onVisibilityChange += OnTerrainChunkVisibilityChange;
                        newChunk.Load();
                    }
                }

            }
        }
    }
    void OnTerrainChunkVisibilityChange(TerrainChunk chunk, bool IsVisible)
    {
        if (IsVisible)
        {
            visibleTerrainChunks.Add(chunk);
        }
        else
        {
            visibleTerrainChunks.Remove(chunk);
        }
    }
    /* chuyen
    public class TerrainChunk
    {
        Vector2 sampleCentre;
        GameObject meshObj;

        Bounds bounds;

        HeightMap heightMap;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        LODInfo[] detailLv;
        LODMesh[] lodMeshes;

        //HeightMap MapData;
        bool heightmapGet;

        int prevLODIndex = -1;

        HeightMapSettings heightMapSettings;
        MeshSettings meshSettings;
        public TerrainChunk(Vector2 coord,HeightMapSettings heightMapSettings, MeshSettings meshSettings/*float meshWorldSize, LODInfo[] detailLv, Transform parent, Material material)
        {
            this.detailLv = detailLv;
            this.heightMapSettings = heightMapSettings;
            this.meshSettings = meshSettings;
            sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.scale;
            Vector2 position = coord * meshSettings.meshWorldSize;

            bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);

            // meshObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
            meshObj = new GameObject("Terrain Chunk");
            meshRenderer = meshObj.AddComponent<MeshRenderer>();
            meshFilter = meshObj.AddComponent<MeshFilter>();
            meshRenderer.material = material;
            meshObj.transform.position = new Vector3(position.x,0,position.y);
            //meshObj.transform.localScale = Vector3.one * size / 10f;
            meshObj.transform.parent = parent;
            //meshObj.transform.localScale = Vector3.one * mapGenerator.meshSettings.scale;
            SetVisible(false);

            lodMeshes = new LODMesh[detailLv.Length];
            for (int i = 0; i < detailLv.Length; i++)
            {
                lodMeshes[i] = new LODMesh(detailLv[i].lod, UpdateTerrainChunk);
            }
            //mapGenerator.RequestMapData(sampleCentre, OnMapDataGet);
            DataRequest.RequestData(() =>HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerline,meshSettings.numVertsPerline, heightMapSettings,sampleCentre), OnHeightMapGet);
        }

        void OnHeightMapGet(object heightmapObject)
        {
            //print("da nhan data");
            //mapGenerator.RequestMeshData(mapData, OnMeshDataGet);
            this.heightMap = (HeightMap)heightmapObject;
            heightmapGet = true;

            //Texture2D texture = TextureGenerator.TextureFromColourMap(mapData.colourMap,MapGenerator.mapChunkSize,MapGenerator.mapChunkSize);
            //meshRenderer.material.mainTexture = texture;
            //chuyển
            UpdateTerrainChunk();
        }

        // void OnMeshDataGet( MeshData meshData){
        //     meshFilter.mesh = meshData.CreateMesh();
        // }
        public void UpdateTerrainChunk()
        {
            if (heightmapGet)
            {
                float viewerDistfromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPos));
                bool visible = viewerDistfromNearestEdge <= maxViewDist;
                if (visible)
                {
                    int lodIndex = 0;
                    for (int i = 0; i < detailLv.Length - 1; i++)
                    {
                        if (viewerDistfromNearestEdge > detailLv[i].visibleDistThreshHold)
                        {
                            lodIndex = i + 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (lodIndex != prevLODIndex)
                    {
                        LODMesh lodMesh = lodMeshes[lodIndex];
                        if (lodMesh.hasMesh)
                        {
                            prevLODIndex = lodIndex;
                            meshFilter.mesh = lodMesh.mesh;
                        }
                        else if (!lodMesh.hasResMesh)
                        {
                            lodMesh.ResMesh(heightMap);
                        }
                    }
                    terrainChunkVisibleLastUpdate.Add(this);
                }
                SetVisible(visible);
            }

        }

        public void SetVisible(bool visible)
        {
            meshObj.SetActive(visible);
        }

        public bool IsVisible()
        {
            return meshObj.activeSelf;
        }
    }

    class LODMesh
    {
        public Mesh mesh;
        public bool hasResMesh;
        public bool hasMesh;
        int lod;

        System.Action updateCallback;

        public LODMesh(int lod, System.Action updateCallback)
        {
            this.lod = lod;
            this.updateCallback = updateCallback;
        }

        void OnMeshDataGet(object meshDataObject)
        {
            mesh = ((MeshData)meshDataObject).CreateMesh();
            hasMesh = true;

            updateCallback();
        }

        public void ResMesh(HeightMap heightmap, MeshSettings meshSettings)
        {
            hasResMesh = true;
            //mapGenerator.RequestMeshData(mapData, lod, OnMeshDataGet);
            DataRequest.RequestData(()=> MeshGenerator.GenerateTerrainMesh(heightmap.heightMap, meshSettings, lod), OnMeshDataGet);
        }
    }*/


}

[System.Serializable]
public struct LODInfo
{
    [Range(0, MeshSettings.numofLODs - 1)]
    public int lod;
    public float visibleDistThreshHold;
}

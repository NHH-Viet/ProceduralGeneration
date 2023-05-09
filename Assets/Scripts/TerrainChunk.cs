using UnityEngine;

public class TerrainChunk
{
    public event System.Action<TerrainChunk, bool> onVisibilityChange;

    public Vector2 coord;
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
    float maxViewDist;

    HeightMapSettings heightMapSettings;
    MeshSettings meshSettings;
    Transform viewer;
    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings/*float meshWorldSize*/, LODInfo[] detailLv, Transform parent, Transform viewer, Material material)
    {
        this.coord = coord;
        this.detailLv = detailLv;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;
        this.viewer = viewer;
        sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.scale;
        Vector2 position = coord * meshSettings.meshWorldSize;

        bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);

        // meshObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
        meshObj = new GameObject("Terrain Chunk");
        meshRenderer = meshObj.AddComponent<MeshRenderer>();
        meshFilter = meshObj.AddComponent<MeshFilter>();
        meshRenderer.material = material;
        meshObj.transform.position = new Vector3(position.x, 0, position.y);
        //meshObj.transform.localScale = Vector3.one * size / 10f;
        meshObj.transform.parent = parent;
        //meshObj.transform.localScale = Vector3.one * mapGenerator.meshSettings.scale;
        SetVisible(false);

        lodMeshes = new LODMesh[detailLv.Length];
        for (int i = 0; i < detailLv.Length; i++)
        {
            lodMeshes[i] = new LODMesh(detailLv[i].lod);
            lodMeshes[i].updateCallback += UpdateTerrainChunk;
        }

        maxViewDist = detailLv[detailLv.Length - 1].visibleDistThreshHold;
        //mapGenerator.RequestMapData(sampleCentre, OnMapDataGet);
        //DataRequest.RequestData(() =>HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerline,meshSettings.numVertsPerline, heightMapSettings,sampleCentre), OnHeightMapGet);
    }

    public void Load()
    {
        DataRequest.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerline, meshSettings.numVertsPerline, heightMapSettings, sampleCentre), OnHeightMapGet);
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

    Vector2 viewerPos
    {
        get
        {
            return new Vector2(viewer.position.x, viewer.position.z);
        }
    }
    // void OnMeshDataGet( MeshData meshData){
    //     meshFilter.mesh = meshData.CreateMesh();
    // }
    public void UpdateTerrainChunk()
    {
        if (heightmapGet)
        {
            float viewerDistfromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPos));
            bool wasVisible = IsVisible();
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
                        lodMesh.ResMesh(heightMap, meshSettings);
                    }
                }

            }
            if (wasVisible != visible)
            {
                SetVisible(visible);
                if (onVisibilityChange != null)
                {
                    onVisibilityChange(this, visible);
                }
            }
            //SetVisible(visible);
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

    public System.Action updateCallback;

    public LODMesh(int lod)
    {
        this.lod = lod;
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
        DataRequest.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightmap.heightMap, meshSettings, lod), OnMeshDataGet);
    }
}

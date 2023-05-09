using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class DataRequest : MonoBehaviour
{
    //bien cho thread
    static DataRequest instance;
    Queue<ThreadInfo> DataThreadInfosQ = new Queue<ThreadInfo>();
    // Queue<MapDataThreadInfo<MeshData>> meshDataThreadInfosQ = new Queue<MapDataThreadInfo<MeshData>>();

    void Awake(){
        instance = FindObjectOfType<DataRequest> ();
    }

    //Threading xu ly lay data ben endless (cat bo sau)
    public static void RequestData(Func<object> generateData, Action<object> callback)
    {
        ThreadStart threadStart = delegate
        {
            instance.DataThread(generateData, callback);
        };
        new Thread(threadStart).Start();
    }

    void DataThread(Func<object> generateData, Action<object> callback)
    {
        //HeightMap mapData = HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerline, meshSettings.numVertsPerline, heightMapSettings, centre); ;
        object data = generateData ();

        lock (DataThreadInfosQ)
        {
            //lock de can cac thread khac dam bao cac thread xu ly theo thu tu
            DataThreadInfosQ.Enqueue(new ThreadInfo(callback, data));
        }
    }

    // public void RequestMeshData(HeightMap mapData, int lod, Action<MeshData> callback)
    // {
    //     ThreadStart threadStart = delegate
    //     {
    //         MeshDataThread(mapData, lod, callback);
    //     };

    //     new Thread(threadStart).Start();
    // }

    // void MeshDataThread(HeightMap mapData, int lod, Action<MeshData> callback)
    // {
    //     MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshSettings, lod);
    //     lock (meshDataThreadInfosQ)
    //     {
    //         meshDataThreadInfosQ.Enqueue(new MapDataThreadInfo<MeshData>(callback, meshData));
    //     }
    // }
    void Update()
    {
        if (DataThreadInfosQ.Count > 0)
        {
            for (int i = 0; i < DataThreadInfosQ.Count; i++)
            {
                ThreadInfo threadInfo = DataThreadInfosQ.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        // if (meshDataThreadInfosQ.Count > 0)
        // {
        //     for (int i = 0; i < meshDataThreadInfosQ.Count; i++)
        //     {
        //         MapDataThreadInfo<MeshData> threadInfo = meshDataThreadInfosQ.Dequeue();
        //         threadInfo.callback(threadInfo.parameter);
        //     }
        // }
    }

    //Struct luu gia tri xu ly thread
    struct ThreadInfo
    {
        public readonly Action<object> callback;
        public readonly object parameter;

        public ThreadInfo(Action<object> callback, object parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}

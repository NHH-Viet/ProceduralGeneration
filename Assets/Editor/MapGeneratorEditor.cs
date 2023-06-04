using UnityEngine;
using UnityEditor;
[CustomEditor (typeof (MapDisplay))]

public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI(){
        MapDisplay mapGen = (MapDisplay)target;

        if(DrawDefaultInspector()) {
            if(mapGen.autoUpdate){
                mapGen.DrawMapInEditor ();
            }
        };

        if(GUILayout.Button ("Generate")){
            mapGen.DrawMapInEditor();
            //Debug.Log("hey im in editor");
        }
    }
}

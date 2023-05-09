using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class UpdateData : ScriptableObject
{
    public event System.Action OnValuesUpdated;
    public bool autoUpdate;
    
    #if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        if (autoUpdate)
        {
            //NotifyUpdate();
            UnityEditor.EditorApplication.update += NotifyUpdate;
        }
    }
    public void NotifyUpdate()
    {
        UnityEditor.EditorApplication.update -= NotifyUpdate;
        if (OnValuesUpdated != null)
        {
            OnValuesUpdated();
        }
    }
    #endif
}

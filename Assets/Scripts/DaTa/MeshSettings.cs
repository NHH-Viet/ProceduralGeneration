using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class MeshSettings : UpdateData
{
    public const int numofLODs = 5;
    public const int numSupportLOD = 9;
    public static readonly int[] supportedLODs = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };

    public float scale;

    [Range(0, numSupportLOD - 1)]
    public int chunkSizeIndex;


    //--- 
    //lựa size
    // unity có cap vertex là 65,000
    public int numVertsPerline
    {
        get
        {
            return supportedLODs [chunkSizeIndex] + 1;
        }
    }

    // public float meshWorldSize{
    //     get{
    //         return (numVertsPerline - 3) * scale;
    //     }
    // }


    // public bool useFallOff;



    // //-----
    // //Bien cho xu ly mesh
    // public float meshHeightMultiplier;
    // public AnimationCurve meshHeightCurve;

    // public float minHeight{
    //     get{
    //         return scale * meshHeightMultiplier * meshHeightCurve.Evaluate(0);
    //     }
    // }
    // public float maxHeight{
    //     get{
    //         return scale * meshHeightMultiplier * meshHeightCurve.Evaluate(1);
    //     }
    // }

}

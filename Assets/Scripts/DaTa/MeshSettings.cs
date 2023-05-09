using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class MeshSettings : UpdateData
{
    public const int numofLODs = 5;
    public const int numSupportLOD = 9;
    public const int numFlatSupportLOD = 3;
    public static readonly int[] supportedLODs = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };
    public static readonly int[] supportedFlatLODs = { 48, 72, 96 };

    public float scale;
    public bool useFlatShading;

    [Range(0, numSupportLOD - 1)]
    public int chunkSizeIndex;

    [Range(0, numFlatSupportLOD - 1)]
    public int FlatchunkSizeIndex;

    //--- 
    //lựa size
    // unity có cap vertex là 65,000 mà dùng flat shading tăng gần gấp đôi số vertex nên phải giảm Size tùy khi dùng hay không
    public int numVertsPerline
    {
        get
        {
            return supportedLODs [(useFlatShading) ? FlatchunkSizeIndex : chunkSizeIndex] + 1;
        }
    }

    public float meshWorldSize{
        get{
            return (numVertsPerline - 3) * scale;
        }
    }


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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CamControl
{
    public class CameraInput : MonoBehaviour
    {
        public const string MouseXString = "Mouse X";
        public const string MouseYString = "Mouse Y";
        public const string MouseScrollString = "Mouse ScrollWheel";

        public static float MouseXInput {get => UnityEngine.Input.GetAxis(MouseXString);}
        public static float MouseYInput {get => UnityEngine.Input.GetAxis(MouseYString);}
        public static float MouseScrollInput {get => UnityEngine.Input.GetAxis(MouseScrollString);}

        public static bool EscInput {get => UnityEngine.Input.GetKeyDown(KeyCode.Escape);}
    }
}


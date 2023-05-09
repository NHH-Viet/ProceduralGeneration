using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class noiseArray : MonoBehaviour
{
    public TextMeshProUGUI arrayText;
    MapDisplay getMap;
    // Start is called before the first frame update
    void Start()
    {
        getMap = GameObject.FindGameObjectWithTag("MapDisplay").GetComponent<MapDisplay>();
        getMap.DrawMapInRuntime("Noise");
        float[,] myArray = getMap.accessArray;
        string arrayString = "";

        for (int i = 0; i < myArray.GetLength(0); i++)
        {
            for (int j = 0; j < myArray.GetLength(1); j++)
            {
                arrayString += myArray[i, j].ToString() + " ";
            }
            arrayString += "\n";
        }

        arrayText.text = arrayString;
    }

    // Update is called once per frame
    void Update()
    {

    }
}

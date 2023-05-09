using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spining : MonoBehaviour
{
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void spinSpeed(float speed){
        transform.RotateAround(this.transform.position,new Vector3(0f,1f,0f),speed * Time.deltaTime);
    }
    // Update is called once per frame
    void Update()
    {
        spinSpeed(speed);
    }
}

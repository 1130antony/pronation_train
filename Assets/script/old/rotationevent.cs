using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotationevent : MonoBehaviour
{
    //判斷Gyro的offset
    private List<float> G_offset_regx = new List<float> { };//250點一次
    private List<float> G_offset_regy = new List<float> { };//250點一次
    private List<float> G_offset_regz = new List<float> { };//250點一次
    private float[] G_offset = new float[3];//gyro offset :[x y z]
    public float Rest_time = 2.5f;

    //gyro梯形積分
    private float gyro_oldx = 0, gyro_oldy = 0, gyro_oldz = 0;
    private float[] An_g = new float[3];//gyro梯形積分得角度:[x y z]

    private int j = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        

    // Update is called once per frame
    void Update()
    {
           // if (transform.eulerAngles.x > -100)
           // {
              //  print("a");
           // }
            if (transform.rotation.x > -70)
            {
                print("b");
            }
        }
    }
}

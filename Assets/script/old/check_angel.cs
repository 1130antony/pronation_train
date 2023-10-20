using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.PlayerLoop;

public class check_angel : MonoBehaviour
{
    public GameObject right;
    public float pitch;
    public float yaw;
    public float roll;
    public Vector3 euler;
    public Quaternion realrb_rotation;
    // Start is called before the first frame update
    void Start()
    {
        //right = GameObject.Find("Cube");
    }

    // Update is called once per frame
    public void show()
    {
        //if (right == null)
        //{
        //    Debug.Log("找不到名稱為 group_right 的物件");
        //}
        //else
        //{
        //    Debug.Log("成功找到名稱為 group_right 的物件");
        //}
        realrb_rotation = right.transform.rotation;
        euler = realrb_rotation.eulerAngles;
        pitch = euler.x;
        yaw = euler.y;
        roll = euler.z;
    }
    void FixedUpdate()
    {
        show();
    }
}

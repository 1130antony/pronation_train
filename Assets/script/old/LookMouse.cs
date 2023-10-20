using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookMouse : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject hand;
    public GameObject left_hand;
    public float rotationSpeed = 5f;
    void Start()
    {
        left_hand = GameObject.Find("group_0");
        hand = GameObject.Find("hand06");
    }

    // Update is called once per frame
    void Update()
    {
        left_hand.transform.rotation = Quaternion.Euler(0f,-180f, 300f);
        //Vector3 mouse = Input;
        ////mouse.z = 1;
        ////Vector3 look = Camera.main.ScreenToWorldPoint(mouse);
        ////left_hand.transform.rotation = Quaternion.LookRotation(look, Vector3.back);
        //hand.transform.rotation = Quaternion.Euler(-180f, 0f, 0f);
        //left_hand.transform.rotation = Quaternion.Euler(0f,0f,mouse.x);

        #region Ai Code
        // 获取鼠标在屏幕上的位置
        //Vector3 mousePosition = Input.mousePosition;

        //// 将鼠标位置转换为世界坐标
        //Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, transform.position.z));

        //// 计算物体与鼠标之间的方向向量
        //Vector3 direction = worldMousePosition - transform.position;

        //// 计算旋转角度（仅针对Z轴旋转）
        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //// 创建旋转四元数
        //Quaternion rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, angle);

        //// 应用旋转
        //transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed );

        #endregion
    }
}

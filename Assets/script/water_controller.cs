using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading;
using UnityEngine;

/// <summary>
/// 2023/07/24
/// 控制水龍頭開關的程式
/// </summary>

public class water_controller : MonoBehaviour
{

    #region 判斷實驗流程是否正確布林值
    public bool Isstart = false;
    bool outmostset = false;
    bool innermostset = false;
    int controller = 4;
    float tempLF;
    #endregion

    #region 計算實驗手的角度變數
    float startpitch = 0f;
    float nowpitch = 0f;
    float outpitch = 0f;
    float innerpitch = 0f;
    float tempf = 0f;
    #endregion

    #region 獲取物件變數
    public UnitySimpleLiquid.LiquidContainer hand_cup;//手拿的杯子
    public UnitySimpleLiquid.LiquidContainer bottle;//倒水的水瓶
    public GameObject hand;
    public GameObject left_hand;
    public GameObject btn_start;
    public GameObject btn_innermost;
    public GameObject btn_outmost;
    #endregion

    #region 延遲時間
    private bool shouldExecute = true;
    private float delayTime = 2f;
    private float timer = 0f;
    #endregion

    #region 取SerioPortControl2的資料
    public SerioPortControl2 sp2;
    #endregion

    

    //float mappedValue;


    void Start()
    {
        #region 物件初始化
        left_hand = GameObject.Find("group_0");
        hand = GameObject.Find("hand06");
        btn_start = GameObject.Find("Btn_Start");
        btn_innermost = GameObject.Find("Btn_Innermost");
        btn_outmost = GameObject.Find("Btn_OutMost");
        bottle.IsOpen = false;
        left_hand.transform.localEulerAngles = new Vector3(0f, -180f, 267.7f);
        #endregion



    }

    // Update is called once per frame
    void Update()
    {
        #region 實驗開始
        if (Isstart)
        {
            HandCupControll();
            if (hand_cup.FillAmountPercent >= 0.9f && controller == 4)
            {
                tempf = left_hand.transform.localEulerAngles.z;
                bottle.IsOpen = false;
                controller = 1;



                Debug.Log(controller);
            }
            else if (hand_cup.FillAmountPercent <= 0.9f && hand_cup.FillAmountPercent > 0f && controller == 1)
            {
                //HandCupControll();
                controller = 2;
                Debug.Log(controller);
            }
            else if (hand_cup.FillAmountPercent == 0 && controller == 2)
            {

                controller = 3;
                Debug.Log(controller);
                #region MyRegion
                //timer += Time.deltaTime;
                //if (timer >= delayTime)
                //{
                //    bottle.IsOpen = true;
                //    timer = 0f;
                //}
                #endregion

            }
            else if (controller == 3 && left_hand.transform.localEulerAngles.z <= tempf + 50f && left_hand.transform.localEulerAngles.z >= 220f)
            {
                bottle.IsOpen = true;
                controller = 4;
                Debug.Log(controller);
            }




        }
        #endregion


        #region 舊的程式碼(已註解)
        //left_hand.transform.eulerAngles = new Vector3(0f, -180f, -sp2.pitch + 100f);
        //left_hand.transform.eulerAngles = new Vector3(sp2.roll, sp2.pitch, sp2.yaw);

        //if (hand_cup.FillAmountPercent >= 0.8)//當手裡杯子滿了
        //{
        //    bottle.IsOpen = false;//把倒水的瓶子關起來
        //    //Debug.Log("關瓶子");
        //    timer += Time.deltaTime;
        //    if (timer >= delayTime)
        //    {

        //        left_hand.transform.rotation = Quaternion.Euler(0f, -180f, 300f);
        //        //Debug.Log("1");
        //        timer = 0f;
        //    }

        //}

        //else if (hand_cup.FillAmountPercent == 0)
        //{
        //    timer += Time.deltaTime;
        //    if (timer >= delayTime)
        //    {
        //        // 執行延遲後的事件
        //        bottle.IsOpen = true;//把倒水的瓶子打開
        //        //Debug.Log("開瓶子");
        //        left_hand.transform.rotation = Quaternion.Euler(0f, -180f, 181f);
        //        //Debug.Log("2");

        //        // 重置計時器和標誌位
        //        timer = 0f;
        //    }


        //}
        #endregion




    }

    #region 倒水事件控制
    void HandCupControll()
    {

        //nowpitch = Mathf.Abs(sp2.pitch);
        nowpitch = sp2.pitch;
        left_hand.transform.rotation = Quaternion.Euler(0f, -180f, (nowpitch * 2.1f));
        //left_hand.transform.localEulerAngles = new Vector3(0f, -180f, (((nowpitch/nowpitch)*267.7f) * 2.1f));
    }
    #endregion

    #region Btn_Start

    public void Btn_Start_Click()
    {
        if (outmostset && innermostset)
        {
            startpitch = Mathf.Abs(sp2.pitch);
            Isstart = true;
            Debug.Log("實驗開始!!!");
            bottle.IsOpen = true;
            #region 實驗開始後 將button隱藏
            btn_start.SetActive(false);
            btn_innermost.SetActive(false);
            btn_outmost.SetActive(false);
            #endregion
        }
        else
        {
            Debug.Log("inner or out 未設定!!!");
        }

    }

    #endregion

    #region Btn_outmost
    public void Btn_Outmost_Click()
    {
        outpitch = Mathf.Abs(sp2.pitch);
        outmostset = true;
        Debug.Log("已設定outmost!!! " + outpitch);
    }
    #endregion

    #region Btn_innermost
    public void Btn_Innermost_Click()
    {
        innerpitch = sp2.pitch;
        innermostset = true;
        Debug.Log("已設定innermost!!! " + innerpitch);
    }
    #endregion

    #region 線性插值(以註解)
    //float Map(float value, float inMin, float inMax, float outMin, float outMax)
    //{
    //    return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    //}
    #endregion




}

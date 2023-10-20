using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainFunc : MonoBehaviour
{
    #region 參數
    // 要控制的物件參數
    public Objdefine[] objList = new Objdefine[(int)PortDefine.PORT_CNT.MAX_PORT]
    {
        // LINK
        new Objdefine {
            bodyPart = null,
            axis = new Axis(),
            axisRawData = new Axis(),
            euler = new Euler(),
            gyroCalibration = new GyroCalibration(),
            magCalibration = new MagCalibration(),
            q = new float[4] { 1f, 0f, 0f, 0f }
        }
    };
    public GameObject Obj;
    public GameObject Obj1;
    public GameObject Obj2;
    public GameObject Obj3;    
    public GameObject Obj4;
    public GameObject Obj5;

    // 各mode的function
    [SerializeField] private RealtimeModeFunc realtimeMode;

    private MagCalibrationFunc magCali = new MagCalibrationFunc();



    #endregion

    private void Start()
    {
        Init_BodyPart();
    }

    private void Update()
    {
        UpdateFunc();
    }

    private void FixedUpdate()
    {
        FixedUpdateFunc();
    }

    public void Init_BodyPart()
    {
        ref Objdefine part = ref objList[0];
        for (int cnt = 0; cnt < (int)PortDefine.PORT_CNT.MAX_PORT; cnt++)
        {
            part = ref objList[cnt];
            part.bodyPart = Obj.transform;
            part.bodyPart1 = Obj1.transform;
            //part.bodyPart2 = Obj2.transform;
            //part.bodyPart3 = Obj3.transform;
           // part.bodyPart4 = Obj4.transform;
            //part.bodyPart5 = Obj5.transform;

            // 用九軸的話, 從資料夾中載入磁力計校正數值
            if (SerialPortControl.inputDataIsNineAxis)
            {
                magCali.LoadMagCalibrationValue(ref part);
            }
        }
    }
  
    public void UpdateFunc()

    {
        for (int i = 0; i < (int)PortDefine.PORT_CNT.MAX_PORT; i++)
        {
         
            realtimeMode.Update_RealtimeMode(i, ref objList[i]);

        }
    }

    public void FixedUpdateFunc()
    {
        for (int i = 0; i < (int)PortDefine.PORT_CNT.MAX_PORT; i++)
        {
            realtimeMode.FixedUpdate_RealtimeMode(i, ref objList[i]);
           
        }
    }

    #region realtime control function
    public void SetCalibrationOn()
    {
        for (int i = 0; i < (int)PortDefine.PORT_CNT.MAX_PORT; i++)
        {
            bool portIsOpen = SerialPortControl.func.portAll[i].sp.IsOpen;
            if (portIsOpen)
            {
                realtimeMode.ResetGyroCali(ref objList[i]);
                
            }
        }
    }
    #endregion
}

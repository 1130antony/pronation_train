using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;

// ** 使用Asset Store所下載之File Browser (free)作為開啟資料夾之功能

public class UIFunction : MonoBehaviour
{
    public static UIFunction func;
    [SerializeField] private MainFunc mainFunc;

    public Canvas canvasReplay;
    public Canvas canvasRealTime;

    public void Start()
    {
        func = this;
    }

    public void BtnClick_Connect(int portCnt)
    {
        if (SerialPortControl.func.ConnectPort(portCnt))
        {
            // connect成功
            print("LINK connection ON");
        }
        else
        {
            print("LINK connection Fail");
        }
    }

    #region 校正相關
    public void BtnClick_Cali()
    {
        mainFunc.SetCalibrationOn();
        SerialPortControl.func.ClearSerialPortReceiveBuffer();
    }
 
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.SceneManagement;
/*
 * 負責控制serial port的連接
 */
public class PortDefine //建立常數設定值
{
    // port相關設定
#if true // true = 6軸的資料, false = 9軸的資料
    public const byte PACK_LEN = 22; // 每次接收的包裹訊號量
    public const byte DATA_LEN = 18; // 實際的Data量: 包裹訊號量減去兩個byte的標頭黨

    //public const byte PACK_LEN = 22; // 每次接收的包裹訊號量
    //public const byte DATA_LEN = PACK_LEN - 4; // 實際的Data量: 包裹訊號量減去兩個byte的標頭黨
#else
    public const byte PACK_LEN = 22; // 每次接收的包裹訊號量
    public const byte DATA_LEN = PACK_LEN-4; // 實際的Data量: 包裹訊號量減去兩個byte的標頭黨
#endif
    public const int BAUD_RATE = 115200; //原460800

    // 每個port在陣列中對應的INDEX
    public enum PORT_CNT : int { LINK = 0, MAX_PORT};
    
    // ComPort及按鈕名稱
    // 注意: com port必須小於com10(不包含10)
    public const string COM_PORT_LINK = "COM40";
}

public class DataObject_Decoded
{
    public float acc_x, acc_y, acc_z, gyro_x, gyro_y, gyro_z = 0;
    public float mag_x, mag_y, mag_z = 0;
}

public class PortContent
{
    public SerialPort sp;
    public SerialPort ap;
    public SerialPort bp;
    public byte[] bufTemp;
    public byte[] dataUndecode;
    public DataObject_Decoded dataDecoded;
    public int decodeState;
    public bool dataReady; // Ready後即可傳入四元數運算中
    public int zeroCnt; // 太多次沒有收到Data的話判斷失去連線,關閉該serial port
    public int reconnectTimes; // 重新連線的次數
  
    
}


public class SerialPortControl : MonoBehaviour
{
    public static SerialPortControl func;

    public static bool inputDataIsNineAxis = false;
    public int ticks = 0;

    public PortContent[] portAll = new PortContent[(int)PortDefine.PORT_CNT.MAX_PORT]
    {
        // LINK
        new PortContent {
        sp = new SerialPort(PortDefine.COM_PORT_LINK, 2*PortDefine.BAUD_RATE),
        bufTemp = new byte[2*PortDefine.PACK_LEN],
        dataUndecode = new byte[PortDefine.DATA_LEN],
        dataDecoded = new DataObject_Decoded(),
        decodeState = 0,
        dataReady = false,
        zeroCnt = 0,
        reconnectTimes = 0,
       
        }
    };

 

    private bool TryToOpen(ref PortContent port)
    {

        try
        {
            print(port.sp);
            print(port.bp);
            if ( port.sp != null)
            {
                port.sp.Close();
                port.sp.ReadBufferSize = 10000;
                port.sp.Open();
                port.sp.DiscardInBuffer();
                port.sp.DiscardOutBuffer();
               
                Array.Clear(port.bufTemp,0,PortDefine.PACK_LEN);
                port.decodeState = 1;

                return true;
            }
            else
            {
                print("BluetoothPort Not Found");
                return false;
            }
        }
        catch
        {
            print("BluetoothPort Not Found");
            return false;
        }
    }
    public void ClearDecodeState()
    {
        for (int i = 0; i < (int)PortDefine.PORT_CNT.MAX_PORT; i++)
        {
            portAll[i].decodeState = 1;
        }
    }
    public bool ConnectPort(int portCnt)
    {
        if ((portCnt < 0) || (portCnt >= (int)PortDefine.PORT_CNT.MAX_PORT))
        {
            print("ConnectPort: PortCnt out of range");
            return false;
        }
        
        if (TryToOpen(ref portAll[portCnt]) == false)
        {
            return false;
        }
        else
        {
            portAll[portCnt].zeroCnt = 0;
            ClearSerialPortReceiveBuffer();
            return true;
        }
    }

    public void ClearSerialPortReceiveBuffer()
    {
        ref PortContent port = ref portAll[0]; // 傳址變數需要設定初始值

        for (int i = 0; i < (int)PortDefine.PORT_CNT.MAX_PORT; i++)
        {
            port = ref portAll[i];

            if (port.sp.IsOpen)
            {
                port.sp.DiscardInBuffer();
            }
        }
    }
  

    private bool ConnectionFail(int zeroCnt)
    {
        // 太多次沒有東西可以接收則判定為失聯
        if (zeroCnt > 20)
            return true;
        else
            return false;
    }


    public void DisconnectPort(int portCnt)
    {
        portAll[portCnt].sp.Close();
        print("no data input, LINK Disconnect");
    }


    private void ReceiveData(ref PortContent port)
    {
        int bytesToRead;
       int readByteCnt;

        bytesToRead = port.sp.BytesToRead;
        if (bytesToRead == 0) // 沒有東西可以接收
        {
            //port.zeroCnt++;
        }
        else
        {
            port.zeroCnt = 0;
            if (bytesToRead < 2 * PortDefine.PACK_LEN)
            {
                // 不夠時只處理有的byte數量
                readByteCnt = bytesToRead;
            }
            else
            {
                // 夠的話處理兩包pack
                readByteCnt = 2 * PortDefine.PACK_LEN;
            }
            port.sp.Read(port.bufTemp, 0, PortDefine.PACK_LEN);
            for (int cnt = 0; cnt < PortDefine.PACK_LEN; cnt++)
            {
                // 輸入解碼函式
                // 20190819 判斷是六軸還是九軸
                GetData.func.DataDecode(port.bufTemp[cnt], ref port);
            }
        }
    }

   private void Awake()
    {
        func = this;

        //if (PortDefine.PACK_LEN == 22)
        //{
        //    inputDataIsNineAxis = true;
        //    print(inputDataIsNineAxis);
        //}


        //else 
        //{
        //    inputDataIsNineAxis = false;
        //    print(inputDataIsNineAxis);
        //}
        inputDataIsNineAxis =true;
        print(inputDataIsNineAxis);
    }
    public void sendevent()
    {
    
    }

#if true
    private void FixedUpdate()
    {
        ref PortContent port = ref portAll[0]; // 傳址變數需要設定初始值
        

        for (int i = 0; i < (int)PortDefine.PORT_CNT.MAX_PORT; i++)
        {
            port = ref portAll[i];

            if (port.sp.IsOpen == false) continue; // port沒開,直接跳下一個迴圈

            ReceiveData(ref port);

            if (ConnectionFail(port.zeroCnt) == true)
            {
                DisconnectPort(i);
            }
        }
    }
#endif

}

using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading;
using UnityEngine;

/// <summary>
/// 2023/10/20 test
/// </summary>

public class EEG_Event : MonoBehaviour
{
    #region 計算實驗手的角度變數
    float outpitch = 0f;
    float innerpitch = 0f;
    float tempf = 0f;
    #endregion  

    #region 取SerioPortControl2的資料
    public SerioPortControl2 sp2;
    #endregion

    #region 腦波Event
    private SerialPort arduinoStream;
    public string port;
    private Thread readThread; // 宣告執行緒
    public string readMessage;
    bool isNewMessage;
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

    #region 判斷實驗流程是否正確布林值
    public bool Isstart = false;
    bool outmostset = false;
    bool innermostset = false;
    int controller = 4;
    float tempLF;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        #region 腦波Event
        if (port != "")
        {
            arduinoStream = new SerialPort(port, 9600); //指定連接埠、鮑率並實例化SerialPort
            try
            {
                arduinoStream.Open(); //開啟SerialPort連線
                readThread = new Thread(new ThreadStart(ArduinoRead)); //實例化執行緒與指派呼叫函式
                readThread.Start(); //開啟執行緒
                Debug.Log("EEG SerialPort開啟連接");
            }
            catch
            {
                Debug.Log("EEG SerialPort連接失敗");
            }
        }
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        if (hand_cup.FillAmountPercent >= 0.9f && controller == 4)
        {
            tempf = left_hand.transform.localEulerAngles.z;
            bottle.IsOpen = false;
            controller = 1;



            Debug.Log(controller);
        }

        //ArduinoWrite("g");
    }

    #region 腦波Event
    private void ArduinoRead()
    {
        StringBuilder messageBuilder = new StringBuilder(); // 使用 StringBuilder 來組合完整的訊息

        while (arduinoStream.IsOpen)
        {
            try
            {
                char c = (char)arduinoStream.ReadChar(); // 逐字讀取資料
                if (c == '\n')
                { // 換行符號表示一條完整的訊息
                    readMessage = messageBuilder.ToString();
                    messageBuilder.Clear();
                    isNewMessage = true;
                }
                else
                {
                    messageBuilder.Append(c); // 將字符加入到 StringBuilder 中
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
    }

    public void ArduinoWrite(string message)
    {
        Debug.Log(message);
        try
        {
            arduinoStream.Write(message);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }
    #endregion

    #region Btn_Start

    public void Btn_Start_Click()
    {

        Isstart = true;
        

    }

    #endregion

    #region Btn_outmost
    public void Btn_Outmost_Click()
    {
        outpitch = Mathf.Abs(sp2.pitch);
        outmostset = true;
    }
    #endregion

    #region Btn_innermost
    public void Btn_Innermost_Click()
    {
        innerpitch = sp2.pitch;
        innermostset = true;
    }
    #endregion
}

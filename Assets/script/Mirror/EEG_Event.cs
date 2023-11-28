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
    float startpitch = 0f;
    float outpitch = 0f;
    float innerpitch = 0f;
    float tempf = 0f;
    float nowpitch = 0f;
    #endregion

    #region 取water_controller的資料
    public water_controller wc;
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
    private GameObject hand;
    private GameObject left_hand;
    private GameObject btn_start;
    private GameObject btn_innermost;
    private GameObject btn_outmost;
    #endregion

    #region 判斷實驗流程是否正確布林值
    bool Isstart = false;
    bool outmostset = false;
    bool innermostset = false;
    int controller = 4;
    float tempLF;
    #endregion

    #region 儲存tick
    public int full = 0;
    public int Pronation = 0;
    public int Supination = 0;
    public int pour = 0;
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
        if (Isstart == true)
        {
            HandCupControll();
            if (hand_cup.FillAmountPercent >= 0.9f && controller == 5)//水杯倒滿
            {
                ArduinoWrite("1");
                full += 1;
                sp2.Savetick($"full0_{full}");
                Debug.Log("EEG OK1!!");

                controller = 1;
            }
            else if ((Mathf.Abs(nowpitch) <= 80f) && controller == 1)//代表開始旋前
            {
                ArduinoWrite("2");
                Pronation += 1;
                sp2.Savetick($"Pronation1_{Pronation}");
                Debug.Log("EEG OK2!!");
                controller = 2;
            }
            else if (Mathf.Abs(nowpitch) <= 10f && Mathf.Abs(nowpitch) >= 0f && controller == 2)//檢查是否有完成整個旋前
            {
                Debug.Log("旋前完成!!!");
                controller = 3;
            }
            else if ((Mathf.Abs(nowpitch) >= 10f) && controller == 3)//開始旋後
            {
                ArduinoWrite("3");
                Supination += 1;
                sp2.Savetick($"Supination2_{Supination}");
                Debug.Log("EEG OK3!!");
                controller = 4;
            }
            else if (controller == 4 && bottle.IsOpen)//開始倒水
            {
                ArduinoWrite("0");
                pour += 1;
                sp2.Savetick($"pour0_{pour}");
                Debug.Log("EEG OK0!!");
                controller = 5;
            }
        }
        
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

    void OnApplicationQuit()
    {
        if (arduinoStream != null)
        {
            if (arduinoStream.IsOpen)
            {
                arduinoStream.Close();
            }
        }
    }
    #endregion

    #region 倒水事件控制
    void HandCupControll()
    {
        nowpitch = sp2.pitch;
    }
    #endregion

    #region Btn_Start

    public void Btn_Start_Click()
    {
        if (outmostset && innermostset)
        {
            startpitch = Mathf.Abs(sp2.pitch);
            Isstart = true;
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

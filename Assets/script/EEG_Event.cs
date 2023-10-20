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
                Debug.Log("SerialPort開啟連接");
            }
            catch
            {
                Debug.Log("SerialPort連接失敗");
            }
        }
        #endregion
    }

    // Update is called once per frame
    void Update()
    {

        ArduinoWrite("g");
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

    #region Btn_outmost
    public void Btn_Outmost_Click()
    {
        outpitch = Mathf.Abs(sp2.pitch);
    }
    #endregion

    #region Btn_innermost
    public void Btn_Innermost_Click()
    {
        innerpitch = sp2.pitch;
    }
    #endregion
}

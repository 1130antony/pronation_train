using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.IO;
using UnityEngine;

/// <summary>
/// Start 判斷有無連線成功、判斷實驗上下午
/// FixedUpdate Function 接收資料跟計算位置
/// Update Function 給位置資訊(四元數q)
/// 
/// </summary>


public class SerioPortControl2 : MonoBehaviour
{

    #region 變數

    #region Com,serioport
    string comPortName = "COM40";
    SerialPort serialPort;
    #endregion

    #region 接收資料
    int[] rawdata = new int[20];
    string[] start = new string[2];
    string[] end = new string[2];
    byte[] endByte = new byte[2];
    byte[] startByte = new byte[2];
    int[] rawdatahigh = new int[9];
    int[] rawdatalow = new int[9];

    #endregion

    #region 儲存資料
    string[] savedata = new string[20];
    string path = "D:\\蔡俊安\\收案資料\\";
    string date = DateTime.Now.ToString("yyyy-MM-dd");
    int time = int.Parse(DateTime.Now.ToString("HH"));
    public string scenes;
    public water_controller wc;
    public int tick = 0;
    #endregion

    #region 遊戲物件資料

    public int[] objectdata = new int[9];
    public float[] objectdataf = new float[9];

    #endregion

    #region QuaternionAlgorithm變數
    public float[] q = new float[4];
    public float[] qq = new float[4];
    public float ax;
    public float ay;
    public float az;
    public float gx;
    public float gy;
    public float gz;
    public float mx;
    public float my;
    public float mz;

    private float beta = Mathf.Sqrt(3.0f / 4.0f) * Mathf.PI * (40.0f / 180.0f) * 3f;
    private float zeta = Mathf.Sqrt(3.0f / 4.0f) * Mathf.PI * (2.0f / 180.0f);
    private float Kp = 2.0f * 5.0f;
    private float Ki = 0.0f;
    private const float deltaT = 0.01f;
    public float yaw, pitch, roll;
    #endregion

    #endregion

    #region Start
    void Start()
    {
        #region 連線到九軸
        InitializeSerialPort();
        #endregion



        #region 判斷上下午
        if (time < 12)
            path += date + " 上午\\";
        else
            path += date + " 下午\\";
        Debug.Log(path);
        #endregion

        qq[0] = 1.0f;
        for (int a = 1; a < 4; a++)
        {
            qq[a] = 0.0f;
        }
        for (int a = 0; a < 4; a++)
        {
            q[a] = qq[a];
        }
    }
    #endregion

    #region Update
    //private void Update()
    //{



    //    testbbb();



    //}
    #endregion

    #region fixupdate

    private void FixedUpdate()
    {

        if (serialPort != null && serialPort.IsOpen)
        {
            #region 讀九軸資料+儲存

            #region 讀字首
            for (int a = 0; a <= 1; a++)
            {
                startByte[a] = (byte)serialPort.ReadByte();// 读取一个字节
                start[a] = startByte[a].ToString("X2");  // "X2" 表示以两位16进制表示
            }

            if (start[0] == "FF" && start[1] == "BE")
            {
                #region 存標頭
                rawdata[0] = startByte[0];
                rawdata[1] = startByte[1];
                #endregion

                #region 存原始資料
                for (int a = 2; a < 20; a++)
                {
                    rawdata[a] = (byte)serialPort.ReadByte();
                }
                #endregion

            }
            #endregion

            #region 讀字尾
            for (int a = 0; a <= 1; a++)
            {
                endByte[a] = (byte)serialPort.ReadByte();// 读取一个字节
                end[a] = endByte[a].ToString("X2");  // "X2" 表示以两位16进制表示
            }
            if (end[0] == "FF" && end[1] == "53")
            {
                #region 將原始資料給儲存資料陣列並儲存

                for (int a = 0; a < 20; a++)
                    savedata[a] = rawdata[a].ToString();

                if (wc.Isstart)//判斷實驗是否開始 開始後再進行資料儲存
                {
                    tick += 1;
                    SaveRawdata();
                }
                    
                #endregion

            }
            #endregion
            #endregion 
        }
        #region 校正九軸資料
        RawData_Processing();
        #endregion

        GetAGData();
        QuaternionAlgorithm();


        #region 清空陣列 以便下一輪儲存
        Array.Clear(startByte, 0, startByte.Length);
        Array.Clear(start, 0, start.Length);
        Array.Clear(rawdatahigh, 0, rawdatahigh.Length);
        Array.Clear(rawdatalow, 0, rawdatalow.Length);
        Array.Clear(rawdata, 0, rawdata.Length);
        Array.Clear(endByte, 0, endByte.Length);
        Array.Clear(end, 0, end.Length);
        Array.Clear(objectdata, 0, objectdata.Length);
        Array.Clear(objectdataf, 0, objectdataf.Length);
        Array.Clear(savedata, 0, savedata.Length);
        #endregion
    }

    #endregion

    #region 九軸連接資訊
    void InitializeSerialPort()
    {
        serialPort = new SerialPort(comPortName, 115200); // 替换为正确的波特率
        serialPort.ReadTimeout = 1000; // 设置读取超时时间（根据需要调整）
        serialPort.Open();
        if (serialPort != null && serialPort.IsOpen && serialPort.BytesToRead != 0)
            Debug.Log("成功連接!!!");
        else
            Debug.Log("連接失敗???");
    }
    #endregion

    #region 儲存RawData程式
    void SaveRawdata()
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            using (StreamWriter sw = File.CreateText(path + $"{scenes}.dat"))
            {
                sw.Write($"{tick}:");
                for (int a = 0; a < 20; a++)
                    sw.Write(savedata[a].ToString() + " ");
                sw.Write("\n");

            }
        }
        else
        {
            using (StreamWriter sw = File.AppendText(path + $"{scenes}.dat"))
            {
                sw.Write($"{tick}:");
                for (int a = 0; a < 20; a++)
                    sw.Write(savedata[a].ToString() + " ");
                sw.Write("\n");


            }
        }
    }
    #endregion

    #region 標記旋前旋後的tick
    public void Savetick(string action)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            using (StreamWriter sw = File.CreateText(path + $"{scenes}-動作tick.dat"))
            {
                sw.Write($"{tick}: {action}");
                //for (int a = 0; a < 20; a++)
                //    sw.Write(savedata[a].ToString() + " ");
                sw.Write("\n");

            }
        }
        else
        {
            using (StreamWriter sw = File.AppendText(path + $"{scenes}-動作tick.dat"))
            {
                sw.Write($"{tick}: {action}");
                //for (int a = 0; a < 20; a++)
                //    sw.Write(savedata[a].ToString() + " ");
                sw.Write("\n");


            }
        }
    }

    #endregion

    #region 校正九軸資料方法
    private void RawData_Processing()
    {
        #region 將原始資料分類高低位元
        int highindex = 0;
        int lowindex = 0;
        for (int a = 2; a < rawdata.Length; a++)
        {
            if (a % 2 == 0)//高位元資料
            {
                rawdatahigh[highindex] = rawdata[a];
                highindex++;
            }
            else//低位元資料
            {
                rawdatalow[lowindex] = rawdata[a];
                lowindex++;
            }
        }
        #endregion

        #region 將高低位元結合後 給object陣列
        for (int a = 0; a < 9; a++)
        {
            if ((rawdatahigh[a] * 256 + rawdatalow[a]) > 32767)
            {
                //Debug.Log("yy:" + rawdatahigh[0]);
                objectdata[a] = (((rawdatahigh[a] * 256) + rawdatalow[a]) - 65536);
            }
            else
            {
                //Debug.Log("nn:" + rawdatahigh[0]);
                objectdata[a] = ((rawdatahigh[a] * 256) + rawdatalow[a]);
            }
        }

        #endregion


        #region 對object陣列(九軸資料)分別校正
        //for (int a = 0; a < objectdata.Length; a++)
        //{

        //    if (a < 3)//加速規
        //        objectdataf[a] = (objectdata[a] / 32768) * 4;

        //    else if (a < 6 && a > 2)//陀螺儀
        //        objectdataf[a] = (objectdata[a] / 32768) * 2000;
        //    else if (a > 5)//磁力計
        //        objectdataf[a] = (objectdata[a] / 32768) * 16;
        //    
        //}

        #endregion

        #region 原始人寫法
        ////加速規
        //objectdataf[0] = (float)objectdata[0] / 32768f * 4f;
        //objectdataf[1] = (float)objectdata[1] / 32768f * 4f;
        //objectdataf[2] = (float)objectdata[2] / 32768f * 4f;

        //加速規
        objectdataf[0] = ((float)objectdata[0] * 4) / 32768f * 4f;
        objectdataf[1] = ((float)objectdata[1] * 4) / 32768f * 4f;
        objectdataf[2] = ((float)objectdata[2] * 4) / 32768f * 4f;

        ////陀螺儀
        //objectdataf[3] = (float)objectdata[3] / 32768f * 2000f;
        //objectdataf[4] = (float)objectdata[4] / 32768f * 2000f;
        //objectdataf[5] = (float)objectdata[5] / 32768f * 2000f;

        //陀螺儀
        objectdataf[3] = (float)objectdata[3] / Mathf.Pow(2, 16) * 4000f;
        objectdataf[4] = (float)objectdata[4] / Mathf.Pow(2, 16) * 4000f;
        objectdataf[5] = (float)objectdata[5] / Mathf.Pow(2, 16) * 4000f;

        //磁力計
        objectdataf[6] = (float)objectdata[6] / 32768 * 16;
        objectdataf[7] = (float)objectdata[7] / 32768 * 16;
        objectdataf[8] = (float)objectdata[8] / 32768 * 16;
        //Debug.Log("OK2" + objectdataf[0]);
        #endregion


    }
    #endregion

    #region 有用
    private float[] zeroError = new float[3] { 0, 0, 0 };
    public float GetZeroError(int index)
    {

        try
        {
            return zeroError[index];
        }
        catch
        {
            return 0;
        }
    }
    #endregion


    #region get_acc,gyro

    public void GetAGData()
    {
        ax = -(objectdataf[0]);
        ay = -(objectdataf[1]);
        az = -(objectdataf[2]);
        gx = (-(objectdataf[3] * Mathf.PI) / 180f) - GetZeroError(0);
        gy = (-(objectdataf[4] * Mathf.PI) / 180f) - GetZeroError(1);
        gz = (-(objectdataf[5] * Mathf.PI) / 180f) - GetZeroError(2);

    }

    #endregion

    #region 計算物體位置(四元數)

    public void QuaternionAlgorithm()
    {

        float q1 = q[0], q2 = q[1], q3 = q[2], q4 = q[3];
        float norm;                                               // vector norm
        float f1, f2, f3;                                         // objetive funcyion elements
        float J_11or24, J_12or23, J_13or22, J_14or21, J_32, J_33; // objective function Jacobian elements
        //float gerrx, gerry, gerrz, gbiasx = 0, gbiasy = 0, gbiasz = 0;        // gyro bias error
        //float qDot1, qDot2, qDot3, qDot4;
        float hatDot1, hatDot2, hatDot3, hatDot4;
        //float zeta;
        float vx, vy, vz;
        float ex, ey, ez;
        float pa, pb, pc;

        float _halfq1 = 0.5f * q1;
        float _halfq2 = 0.5f * q2;
        float _halfq3 = 0.5f * q3;
        float _halfq4 = 0.5f * q4;
        float _2q1 = 2.0f * q1;
        float _2q2 = 2.0f * q2;
        float _2q3 = 2.0f * q3;
        float _2q4 = 2.0f * q4;
        float _2q1q3 = 2.0f * q1 * q3;
        float _2q3q4 = 2.0f * q3 * q4;

        // Normalise accelerometer measurement
        norm = Mathf.Sqrt(ax * ax + ay * ay + az * az);

        if (norm == 0.0f) return; // handle NaN
        norm = 1.0f / norm;
        ax *= norm;
        ay *= norm;
        az *= norm;

        // Compute the objective function and Jacobian
        f1 = _2q2 * q4 - _2q1 * q3 - ax;
        f2 = _2q1 * q2 + _2q3 * q4 - ay;
        f3 = 1.0f - _2q2 * q2 - _2q3 * q3 - az;
        J_11or24 = _2q3;
        J_12or23 = _2q4;
        J_13or22 = _2q1;
        J_14or21 = _2q2;
        J_32 = 2.0f * J_14or21;
        J_33 = 2.0f * J_11or24;

        // Compute the gradient (matrix multiplication)
        hatDot1 = J_14or21 * f2 - J_11or24 * f1;
        hatDot2 = J_12or23 * f1 + J_13or22 * f2 - J_32 * f3;
        hatDot3 = J_12or23 * f2 - J_33 * f3 - J_13or22 * f1;
        hatDot4 = J_14or21 * f1 + J_11or24 * f2;

        // Normalize the gradient
        norm = Mathf.Sqrt(hatDot1 * hatDot1 + hatDot2 * hatDot2 + hatDot3 * hatDot3 + hatDot4 * hatDot4);
        hatDot1 /= norm;
        hatDot2 /= norm;
        hatDot3 /= norm;
        hatDot4 /= norm;

        vx = 2.0f * (q2 * q4 - q1 * q3);
        vy = 2.0f * (q1 * q2 + q3 * q4);
        vz = q1 * q1 - q2 * q2 - q3 * q3 + q4 * q4;
        ex = ay * vz - az * vy;
        ey = az * vx - ax * vz;
        ez = ax * vy - ay * vx;

        gx += Kp * ex;
        gy += Kp * ey;
        gz += Kp * ez;

        pa = q2;
        pb = q3;
        pc = q4;
        q1 = q1 + (-q2 * gx - q3 * gy - q4 * gz) * (0.5f * deltaT);
        q2 = pa + (q1 * gx + pb * gz - pc * gy) * (0.5f * deltaT);
        q3 = pb + (q1 * gy - pa * gz + pc * gx) * (0.5f * deltaT);
        q4 = pc + (q1 * gz + pa * gy - pb * gx) * (0.5f * deltaT);

        // Normalize the quaternion
        norm = Mathf.Sqrt(q1 * q1 + q2 * q2 + q3 * q3 + q4 * q4);    // normalise quaternion
        norm = 1.0f / norm;

        q[0] = q1 * norm;
        q[1] = q2 * norm;
        q[2] = q3 * norm;
        q[3] = q4 * norm;

        yaw = Mathf.Atan2(2.0f * (q[1] * q[2] + q[0] * q[3]), q[0] * q[0] + q[1] * q[1] - q[2] * q[2] - q[3] * q[3]);
        pitch = -Mathf.Asin(2.0f * (q[1] * q[3] - q[0] * q[2]));
        roll = Mathf.Atan2(2.0f * (q[0] * q[1] + q[2] * q[3]), q[0] * q[0] - q[1] * q[1] - q[2] * q[2] + q[3] * q[3]);
        pitch *= 180.0f / Mathf.PI;
        yaw *= 180.0f / Mathf.PI;
        roll *= 180.0f / Mathf.PI;

    }
    #endregion

    #region testbbb

    //public void testbbb()
    //{
    //    try
    //    {
    //        float[] a = new float[4];
    //        for (int i = 0; i < 4; i++)
    //        {
    //            a[i] = q[i];
    //        }
    //        //hand.transform.rotation = Quaternion.Euler(roll,pitch, yaw);//帶roll,yaw,pitch
    //        //hand.transform.rotation = new Quaternion(-q[0], -q[2], q[3], q[1]);//帶四元數
    //    }
    //    catch
    //    {
    //        print("Object is Null...");
    //    }

    //    //Debug.Log("bbb");
    //}

    #endregion



}
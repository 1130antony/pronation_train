using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;
using System.Text;



public class GetData : MonoBehaviour
{
    public int ticks = 0;
    int preEventTick;
    int action = 0;
    int task;
    public int[] states = new int[4];
    string[] eventtype = new string[] { "A", "B", "C", "D", "E", "F", "G", "H" };

    //public char[] eventtype = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };

    //string[,] tasks = new string[,] { { "A", "B" }, { "C", "D" }, { "E", "F" }, { "G", "H" } };

    //int[] eventtype = new int[8];

    //string[,] tasks = new string[,] { { "A", "B" }, { "C", "D" }, { "E", "F" }, { "G", "H" } };
    //{ { 'A', 'B' }, { 'C', 'D' }, { 'E', 'F' }, { 'G', 'H' } };//new int[,] { { 65, 66 }, { 67, 68 }, { 69, 70 }, { 71, 72 } };
    //優化判斷式
    //eventtype[0]=A;eventtype[1]=B;eventtype[2]=C;eventtype[3]=D;
    //eventtype[4]=E;eventtype[5]=F;eventtype[6]=G;eventtype[7]=H;
    //task[0]={'A','B'} //task[1]={'C','D'} //task[2]={'E','F'} //task[3]={'G','H'}

    //GameObject light;
    //public time timerecord;
    public static GetData func;
    public MainFunc mainFunc;
    public DataSave dataSave;
    //double st =0.25;
    private int signal_count, tick, signal;
    float startTime;
    public int state;
    // private bool state;
    private float gyrox, gyroy, gyroz, gyroall;
    private double test, a;
    private bool b = true;
    // public float destroytime = 3f;
    //public GameObject plane;
    int sec = 0;
    int sec1 = 0;
    public bool isSavingData = false;
    public bool waitingToStopSaving = false;
    public int[] lastByteToSave = new int[(int)PortDefine.PORT_CNT.MAX_PORT];
    public byte[] buftemp;
    //private List<byte> InputData = new List<byte> { };
    public static SerialPort bp = new SerialPort("COM5", 115200);
    //public static SerialPort ap = new SerialPort("COM9", 9600);

    //傳event右手_左右邊順序
    public void RRF()
    {
        Debug.Log("RRF");
        //states[0] = -1;
        state = -1;
        states[0] = state;
        task = 0;
        fivesecond();

    }
    public void RLF()
    {
        Debug.Log("RLF");
        state = 1;
        states[1] = state;
        fivesecond();
        task = 1;
    }

    //傳event左手_左右邊順序
    public void LRF()
    {
        Debug.Log("LRF");
        state = -1;
        states[2] = state;
        fivesecond();
        task = 2;
    }

    public void LLF()
    {
        Debug.Log("LLF");
        state = 1;
        states[3] = state;
        fivesecond();
        task = 3;
    }
    private void FixedUpdate()
    {
        ticks = ticks + 1;
    }

    /* public void Onclick(bool isON)
     {
         if (isON)
         {
             state = 1;
         }
         else
         {
             state = -1;
         }
     }*/

    //紀錄時間
    void Update()
    {
        showtime();

    }

    public void showtime()
    {
        float nowTime = Time.time - startTime;
        //Debug.Log("開始時間:" + nowTime);
        GameObject.Find("timer").GetComponent<UnityEngine.UI.Text>().text = nowTime.ToString("F2");
        // InvokeRepeating("seccond", 5f, 5f);
    }

    public void TT()
    {
        startTime = Time.time;

    }

    public void end()
    {
        float nowTime = Time.time - startTime;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
Application.Quit();
#endif
        print("結束時間" + nowTime.ToString("F2"));
        dataSave.SaveENDTimeData(dataSave.SaveList, "ENDTime");
    }

    public void fivesecond()
    {
        // InvokeRepeating("ledhigh", 5f, 8f);
        // InvokeRepeating("ledlow", 5f, 8.3f);
        action = -1;
        Invoke("ledhigh", 5f);
        Invoke("ledlow", 6.5f);
    }
    public void ledhigh()//開燈
    {
        sec += 1;
        Debug.Log(sec);
        //bp.Write("1");
        action = 0;
        preEventTick = ticks;
        state = states[task];
        //GameObject.Find("light").GetComponent<Renderer>().enabled = true;


    }
    public void ledlow()//關燈
    {
        sec1 += 1;
        Debug.Log(sec1);
        //bp.Write("2");
        //GameObject.Find("light").GetComponent<Renderer>().enabled = false;

        // state = states[task];
    }

    void Start()
    {
        //GameObject.Find("light").GetComponent<Renderer>().enabled = false;

        func = this;
        dataSave.InitSaveCnt();
        buftemp = new byte[14];
        signal_count = 1;
        tick = 1;
        //bp.Open();


    }

    private void CombineData(ref PortContent port)
    {
        int dataBase;
        float tempValue;
        float[] tempData = new float[(PortDefine.DATA_LEN / 2)];
        float[] tempDataTest = new float[(PortDefine.DATA_LEN / 2)];
        // 加速規x,y,z
        for (int i = 0; i < 3; i++) // 每筆資料兩個byte
        {
            dataBase = i * 2;
            tempValue = (port.dataUndecode[dataBase] << 8) + port.dataUndecode[dataBase + 1]; // 結合高低位元

            tempValue -= ((port.dataUndecode[dataBase] & 0x80) == 0x80) ? 65536 : 0; // 判斷是否為負值
            tempData[i] = (tempValue * 4f) / 32768f; // 校正
                                                     // tempData[i] = (tempValue / Mathf.Pow(2, 16)) * 8;
            tempDataTest[i] = tempValue; // 校正

            if (tempValue < 0)
            {
                tempValue -= -tempValue;
            }

        }

        // 陀螺儀x,y,z
        for (int i = 3; i < 6; i++) // 每筆資料兩個byte
        {
            dataBase = i * 2;
            tempValue = (port.dataUndecode[dataBase] << 8) + port.dataUndecode[dataBase + 1]; // 結合高低位元
            tempValue -= ((port.dataUndecode[dataBase] & 0x80) == 0x80) ? 65536 : 0; // 判斷是否為負值
            tempData[i] = (tempValue / Mathf.Pow(2, 16)) * 4000;


        }

        // 是否為九軸, 是的話處理磁力計x,y,z
        if (SerialPortControl.inputDataIsNineAxis)
        {
            float[] offset = new float[3] { -0.5750f, 0.5574f, 1.4128f };
            //float[] scale = new float[3] { 1.028f, 1.0011f, 0.9725f };
            float[] scale = new float[3] { 1.028f, 1.0011f, 0.9725f };
            for (int i = 6, j = 0; i < 9; i++, j++) // 每筆資料兩個byte, 參數j用於取得校正參數x,y,z位置
            {
                dataBase = i * 2;
                tempValue = (port.dataUndecode[dataBase] << 8) + port.dataUndecode[dataBase + 1]; // 結合高低位元
                tempValue -= ((port.dataUndecode[dataBase] & 0x80) == 0x80) ? 65536 : 0; // 判斷是否為負值
                tempValue = (short)(tempValue * 4f) / 32768f;

                tempData[i] = tempValue;

            }
        }

        lock (port.dataDecoded)
        {
            int cnt = 0;
            port.dataDecoded.acc_x = tempData[cnt++];
            port.dataDecoded.acc_y = tempData[cnt++];
            port.dataDecoded.acc_z = tempData[cnt++];
            port.dataDecoded.gyro_x = tempData[cnt++];
            port.dataDecoded.gyro_y = tempData[cnt++];
            port.dataDecoded.gyro_z = tempData[cnt++];

            if (tempData.Length == 9)
            {
                port.dataDecoded.mag_x = tempData[cnt++];
                port.dataDecoded.mag_y = tempData[cnt++];
                port.dataDecoded.mag_z = tempData[cnt++];
            }
            port.dataReady = true;

            /*if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "CalibrationScene")
              {
                  SaveDataPoint(ref mainFunc.lowerBody[port.portCnt], ref port);
                  mainFunc.dataCnt[port.portCnt]++;
                  //print(tempData[0]+" "+ tempData[1] + " " + tempData[2] + " " + tempData[3] + " " + tempData[4] + " " + tempData[5] + " ");
              }*/
        }

    }

    public void DataDecode(byte input, ref PortContent port)
    {
        Debug.Log("DATA IN！");
        #region 標頭
        if (port.decodeState == 1) // 標頭 FF
        {
            if (input == 0xFF)
                port.decodeState++;
        }
        else if (port.decodeState == 2) // 標頭 F0
        {
            if (input == 0xBE)
                port.decodeState++;
            else

                port.decodeState = 1;
        }
        #endregion

        #region 加速規
        else if (port.decodeState == 3) // ax high byte
        {
            port.dataUndecode[0] = input;
            port.decodeState++;
        }
        else if (port.decodeState == 4) // ax low byte
        {
            port.dataUndecode[1] = input;
            port.decodeState++;
        }
        else if (port.decodeState == 5) // ay high byte
        {
            port.dataUndecode[2] = input;
            port.decodeState++;
        }
        else if (port.decodeState == 6) // ay low byte
        {
            port.dataUndecode[3] = input;
            port.decodeState++;
        }
        else if (port.decodeState == 7) // az high byte
        {
            port.dataUndecode[4] = input;
            port.decodeState++;
        }
        else if (port.decodeState == 8) // az low byte
        {
            port.dataUndecode[5] = input;
            port.decodeState++;
        }
        #endregion

        #region 陀螺儀
        else if (port.decodeState == 9) // gx high byte
        {
            port.dataUndecode[6] = input;
            port.decodeState++;
        }
        else if (port.decodeState == 10) // gx low byte
        {
            port.dataUndecode[7] = input;
            port.decodeState++;
        }
        else if (port.decodeState == 11) // gy high byte
        {
            port.dataUndecode[8] = input;
            port.decodeState++;
        }
        else if (port.decodeState == 12) // gy low byte
        {
            port.dataUndecode[9] = input;
            port.decodeState++;
        }
        else if (port.decodeState == 13) // gz high byte
        {
            port.dataUndecode[10] = input;
            port.decodeState++;
        }
        else if (port.decodeState == 14) // gz high byte
        {
            port.dataUndecode[11] = input;
            port.decodeState++;
        }
        #endregion

        #region 學長之前判斷六軸中斷點
        //else if (port.decodeState == 14) // gz low byte
        //{
        //    // 判斷是六軸或是九軸
        //    // 九軸
        //    //if (SerialPortControl.inputDataIsNineAxis)
        //    //{
        //    //    port.dataUndecode[11] = input;
        //    //    port.decodeState++; // 繼續下一個state
        //    //}
        //    // 六軸
        //    //else
        //    //{
        //        port.dataUndecode[11] = input;
        //        //timerecord.showtime();
        //        //startTime = Time.time;
        //        float nowTime = Time.time - startTime;

        //        CombineData(ref port);

        //        //抓陀螺儀資料
        //        gyrox = 0;
        //        gyroy = 0;
        //        gyroz = 0;
        //        gyrox = -(port.dataDecoded.gyro_x);
        //        gyroy = (port.dataDecoded.gyro_y);
        //        gyroz = port.dataDecoded.gyro_z;
        //        gyroall = Mathf.Pow(gyrox, 2) + Mathf.Pow(gyroy, 2) + Mathf.Pow(gyroz, 2);
        //        test = Convert.ToDouble(gyroall);
        //        a = (Math.Pow(test, 0.5));

        //        //print(a);
        //        //print(gyroy);
        //        //print(gyrox);
        //        //print(gyroz);


        //        //0.2秒內
        //        /*if (st >0.25)
        //         {
        //             st = 0;*/

        //        /*if (gyroz > 0)
        //        {
        //       if (gyroz < 80)
        //       {
        //           state = 0;
        //           //print(state);
        //       }
        //       else
        //       {
        //           state = 1;
        //           print(state);
        //           //bp.Write("A")
        //       }
        //            }
        //        else
        //        {
        //            if (gyroz > -80)
        //            {
        //                state = 0;
        //                //print(state);
        //            }
        //            else
        //            {
        //                state = -1;
        //            //print(state);
        //            //bp.Write("B");                             
        //            }
        //        }
        //   // st += Time.deltaTime;*/


        //        /* if (action == 0)
        //         {
        //             tasks[0, 0] = ("A");
        //             tasks[1, 0] = ("C");
        //             tasks[2, 0] = ("E");
        //             tasks[3, 0] = ("G");
        //         }
        //         else if (action == 1)
        //         {
        //             tasks[0, 1] = ("B");
        //               tasks[1, 1] = ("D");
        //               tasks[2, 1] = ("F"); 
        //                tasks[3, 1] = ("H");

        //         }*/


        //        if (ticks > preEventTick + 100 * 10)
        //        {
        //            action = 0;
        //            preEventTick = ticks;
        //            bp.Write("1");
        //            Invoke("ledlow", 1.5f);
        //            state = states[task];
        //            //Invoke("ledhigh", 0.1f);
        //        }

        //        if (state == 1)      //RLF
        //        {
        //            if (gyroy < -60 && action >= 0)
        //            {
        //                state = -1;
        //                print(state);
        //                bp.Write(eventtype[task * 2 + action]);
        //                print(eventtype[task * 2 + action]);

        //                print("時間" + nowTime.ToString("F2"));
        //                dataSave.SaveLTimeData(dataSave.SaveList, eventtype[task * 2 + action]);

        //                //Destroy(plane, destroytime);
        //                // plane.SetActive(true);
        //                if (action == 0)
        //                {
        //                    action = 1;
        //                }
        //                else
        //                {
        //                    action = 0;
        //                    fivesecond();
        //                }
        //            }
        //        }
        //        else if (state == -1)//RRF
        //        {
        //            if (gyroy > 60 && action >= 0)
        //            {
        //                state = 1;
        //                print(state);
        //                bp.Write(eventtype[task * 2 + action]);
        //                print(eventtype[task * 2 + action]);
        //                print("時間" + nowTime.ToString("F2"));
        //                dataSave.SaveRTimeData(dataSave.SaveList, eventtype[task * 2 + action]);
        //                //plane.SetActive(false);
        //                //Destroy(plane, destroytime);
        //                if (action == 0)
        //                {
        //                    action = 1;
        //                }
        //                else
        //                {
        //                    action = 0;
        //                    fivesecond();
        //                }
        //            }
        //        }





        //        /* if (state == -1)      //RF
        //                    {
        //                        if (gyroy >60)
        //                        {
        //                            state = 1;
        //                            print(state);
        //                            // bp.Write("A");
        //                        }
        //                    }
        //                    else if (state ==1)
        //                    {
        //                        if (gyroy <-60)
        //                        {
        //                            state = -1;
        //                            print(state);
        //                            //bp.Write("B");
        //                        }
        //                    }*/

        //        port.decodeState = 1; // 回到state 1
        //    //}
        //}
        //// 以下程式九軸解碼才會使用
        #endregion

        #region 學長之前判斷六軸中斷點註解版
        //#region 學長之前判斷六軸中斷點
        //else if (port.decodeState == 14) // gz low byte
        //{
        //    // 判斷是六軸或是九軸
        //    // 九軸
        //    if (SerialPortControl.inputDataIsNineAxis)
        //    {
        //        port.dataUndecode[11] = input;
        //        port.decodeState++; // 繼續下一個state
        //    }
        //    // 六軸
        //    else
        //    {
        //        port.dataUndecode[11] = input;
        //        //timerecord.showtime();
        //        //startTime = Time.time;
        //        float nowTime = Time.time - startTime;

        //        CombineData(ref port);

        //        //抓陀螺儀資料
        //        gyrox = 0;
        //        gyroy = 0;
        //        gyroz = 0;
        //        gyrox = -(port.dataDecoded.gyro_x);
        //        gyroy = (port.dataDecoded.gyro_y);
        //        gyroz = port.dataDecoded.gyro_z;
        //        gyroall = Mathf.Pow(gyrox, 2) + Mathf.Pow(gyroy, 2) + Mathf.Pow(gyroz, 2);
        //        test = Convert.ToDouble(gyroall);
        //        a = (Math.Pow(test, 0.5));

        //        //print(a);
        //        //print(gyroy);
        //        //print(gyrox);
        //        //print(gyroz);


        //        //0.2秒內
        //        /*if (st >0.25)
        //         {
        //             st = 0;*/

        //        /*if (gyroz > 0)
        //        {
        //       if (gyroz < 80)
        //       {
        //           state = 0;
        //           //print(state);
        //       }
        //       else
        //       {
        //           state = 1;
        //           print(state);
        //           //bp.Write("A")
        //       }
        //            }
        //        else
        //        {
        //            if (gyroz > -80)
        //            {
        //                state = 0;
        //                //print(state);
        //            }
        //            else
        //            {
        //                state = -1;
        //            //print(state);
        //            //bp.Write("B");                             
        //            }
        //        }
        //   // st += Time.deltaTime;*/


        //        /* if (action == 0)
        //         {
        //             tasks[0, 0] = ("A");
        //             tasks[1, 0] = ("C");
        //             tasks[2, 0] = ("E");
        //             tasks[3, 0] = ("G");
        //         }
        //         else if (action == 1)
        //         {
        //             tasks[0, 1] = ("B");
        //               tasks[1, 1] = ("D");
        //               tasks[2, 1] = ("F"); 
        //                tasks[3, 1] = ("H");

        //         }*/


        //        if (ticks > preEventTick + 100 * 10)
        //        {
        //            action = 0;
        //            preEventTick = ticks;
        //            bp.Write("1");
        //            Invoke("ledlow", 1.5f);
        //            state = states[task];
        //            //Invoke("ledhigh", 0.1f);
        //        }

        //        if (state == 1)      //RLF
        //        {
        //            if (gyroy < -60 && action >= 0)
        //            {
        //                state = -1;
        //                print(state);
        //                bp.Write(eventtype[task * 2 + action]);
        //                print(eventtype[task * 2 + action]);

        //                print("時間" + nowTime.ToString("F2"));
        //                dataSave.SaveLTimeData(dataSave.SaveList, eventtype[task * 2 + action]);

        //                //Destroy(plane, destroytime);
        //                // plane.SetActive(true);
        //                if (action == 0)
        //                {
        //                    action = 1;
        //                }
        //                else
        //                {
        //                    action = 0;
        //                    fivesecond();
        //                }
        //            }
        //        }
        //        else if (state == -1)//RRF
        //        {
        //            if (gyroy > 60 && action >= 0)
        //            {
        //                state = 1;
        //                print(state);
        //                bp.Write(eventtype[task * 2 + action]);
        //                print(eventtype[task * 2 + action]);
        //                print("時間" + nowTime.ToString("F2"));
        //                dataSave.SaveRTimeData(dataSave.SaveList, eventtype[task * 2 + action]);
        //                //plane.SetActive(false);
        //                //Destroy(plane, destroytime);
        //                if (action == 0)
        //                {
        //                    action = 1;
        //                }
        //                else
        //                {
        //                    action = 0;
        //                    fivesecond();
        //                }
        //            }
        //        }





        //        /* if (state == -1)      //RF
        //                    {
        //                        if (gyroy >60)
        //                        {
        //                            state = 1;
        //                            print(state);
        //                            // bp.Write("A");
        //                        }
        //                    }
        //                    else if (state ==1)
        //                    {
        //                        if (gyroy <-60)
        //                        {
        //                            state = -1;
        //                            print(state);
        //                            //bp.Write("B");
        //                        }
        //                    }*/

        //        port.decodeState = 1; // 回到state 1
        //    }
        //}
        //// 以下程式九軸解碼才會使用
        //#endregion
        #endregion


        #region 磁力計
        else if (port.decodeState == 15) // mx high byte
        {
            print(port.decodeState);
            port.dataUndecode[12] = input;
            port.decodeState++;
        }
        else if (port.decodeState == 16) // mx low byte
        {
            port.dataUndecode[13] = input;
            port.decodeState++;
        }
        else if (port.decodeState == 17) // my high byte
        {
            port.dataUndecode[14] = input;
            port.decodeState++;
        }
        else if (port.decodeState == 18) // my low byte
        {
            port.dataUndecode[15] = input;
            port.decodeState++;
        }
        else if (port.decodeState == 19) // mz high byte
        {
            port.dataUndecode[16] = input;
            port.decodeState++;
        }
        else if (port.decodeState == 20) // mz low byte
        {
            port.dataUndecode[17] = input;
            port.decodeState++;
        }
        #endregion

        #region 足壓資料
        //else if (port.decodeState == 21) 
        //{
        //    port.dataUndecode[18] = input;
        //    port.decodeState++;
        //}
        //else if (port.decodeState == 22) 
        //{
        //    port.dataUndecode[19] = input;
        //    port.decodeState++;
        //}
        //else if (port.decodeState == 23) 
        //{
        //    port.dataUndecode[20] = input;
        //    port.decodeState++;
        //}
        //else if (port.decodeState == 24) 
        //{
        //    port.dataUndecode[21] = input;
        //    port.decodeState++;
        //}
        //else if (port.decodeState == 25) 
        //{
        //    port.dataUndecode[22] = input;
        //    port.decodeState++;
        //}
        //else if (port.decodeState == 26) 
        //{
        //    port.dataUndecode[23] = input;
        //    port.decodeState++;
        //}
        //else if (port.decodeState == 27) 
        //{
        //    port.dataUndecode[24] = input;
        //    port.decodeState++;
        //}
        //else if (port.decodeState == 28) 
        //{
        //    port.dataUndecode[25] = input;
        //    port.decodeState++;
        //}
        //else if (port.decodeState == 29) 
        //{
        //    port.dataUndecode[26] = input;
        //    port.decodeState++;
        //}
        //else if (port.decodeState == 30) 
        //{
        //    port.dataUndecode[27] = input;
        //    port.decodeState++;
        //}
        //else if (port.decodeState == 31) 
        //{
        //    port.dataUndecode[28] = input;
        //    port.decodeState++;
        //}
        //else if (port.decodeState == 32) 
        //{
        //    port.dataUndecode[29] = input;
        //    port.decodeState++;
        //}
        //else if (port.decodeState == 33) 
        //{
        //    port.dataUndecode[30] = input;
        //    port.decodeState++;
        //}
        //else if (port.decodeState == 34) 
        //{
        //    port.dataUndecode[31] = input;
        //    port.decodeState++;
        //}
        //else if (port.decodeState == 35) 
        //{
        //    port.dataUndecode[32] = input;
        //    port.decodeState++;
        //}
        //else if (port.decodeState == 36) 
        //{
        //    port.dataUndecode[33] = input;
        //    port.decodeState++;
        //}
        //else if (port.decodeState == 37) 
        //{
        //    port.dataUndecode[34] = input;
        //    port.decodeState++;
        //}
        //else if (port.decodeState == 38) 
        //{
        //    port.dataUndecode[35] = input;
        //    port.decodeState++;
        //}
        //else if (port.decodeState == 39) 
        //{
        //    port.dataUndecode[36] = input;
        //    port.decodeState++;
        //}
        //else if (port.decodeState == 40) 
        //{
        //    port.dataUndecode[37] = input;
        //    port.decodeState++;
        //}
        //else if (port.decodeState == 41) 
        //{
        //    port.dataUndecode[38] = input;
        //    port.decodeState++;
        //}
        //else if (port.decodeState == 42) 
        //{
        //    port.dataUndecode[39] = input;
        //    port.decodeState++;
        //}
        #endregion

        #region 標尾
        else if (port.decodeState == 21) // 標尾 FF
        {
            if (input == 0xFF)
                port.decodeState++;
            else
                port.decodeState = 1; // 回到state 1
        }
        else if (port.decodeState == 22) // 標尾 53
        {
            if (input == 0x53)//new 9 axis
            {
                CombineData(ref port);

                #region 儲存data方法
                if (signal_count <= 100)
                {
                    signal_count++;
                    tick++;
                    for (int k = 0; k < 12; k++)
                    {
                        dataSave.SaveList.Add(port.dataUndecode[k]);
                    }
                }
                else
                {
                    dataSave.SaveData(dataSave.SaveList, "1_1_L");

                    signal_count = 1;
                    // print("OK");
                }
                #endregion
            }
            port.decodeState = 1; // 回到state 1
        }
        #endregion



    }

}





#region 沒有用到的程式
/*#if true
    #region
    private void SaveGyroCaliValue()
    {
        string saveStr = "";
        for (int i = 0; i < (int)PortDefine.PORT_CNT.MAX_PORT; i++)
        {
            saveStr = "";
            saveStr += mainFunc.objList[i].gyroCalibration.GetZeroError(GyroCalibration.INDEX_GX).ToString();
            saveStr += ",";
            saveStr += mainFunc.objList[i].gyroCalibration.GetZeroError(GyroCalibration.INDEX_GY).ToString();
            saveStr += ",";
            saveStr += mainFunc.objList[i].gyroCalibration.GetZeroError(GyroCalibration.INDEX_GZ).ToString();
            saveStr += "\r\n";

            dataSave.SaveData(ref saveStr, mainFunc.objList[i].bodyPart + "GyroCali", FileType.TXT);
        }
    }

    public void StartSavingData()
    {
        isSavingData = true;
        dataSave.NextSave();
        SaveGyroCaliValue();
    }

    public void StopSavingData()
    {
        ref PortContent port = ref SerialPortControl.func.portAll[0];

        isSavingData = false;

        for (int i = 0; i < (int)PortDefine.PORT_CNT.MAX_PORT; i++)
        {
            port = ref SerialPortControl.func.portAll[i];
            if (port.sp.IsOpen == false) continue;

            int bytesToRead = port.sp.BytesToRead;
            if (port.sp.BytesToRead > PortDefine.PACK_LEN)
            {
                // 結束的當下尚有data沒有收完
                lastByteToSave[i] = bytesToRead / PortDefine.PACK_LEN;
                waitingToStopSaving = true;
            }
            else
            {
                lastByteToSave[i] = 0;

                // 把未存的Data存完
                if (mainFunc.objList[i]> 0)
                    SaveDataIntoFile(ref mainFunc.objList[i]);
            }
        }

        if (waitingToStopSaving == false)
        {
            print("Trial " + dataSave.saveCnt.ToString() + " save success..");
        }
    } 

    private void SaveDataIntoFile(ref Objdefine bodyObj)
    {
        dataSave.SaveData(ref bodyObj.saveList, bodyObj.bodyPart, FileType.BIN);
        bodyObj.saveDataCnt = 0;
    }

    public void SetDataIntoCsvType(ref Objdefine bodyObj, ref PortContent port)
    {
        //ref List<string> list = ref bodyObj.saveList;
        //string data = "";

        //data += port.dataDecoded.acc_x.ToString();
        //data += ",";
        //data += port.dataDecoded.acc_y.ToString();
        //data += ",";
        //data += port.dataDecoded.acc_z.ToString();
        //data += ",";
        //data += port.dataDecoded.gyro_x.ToString();
        //data += ",";
        //data += port.dataDecoded.gyro_y.ToString();
        //data += ",";
        //data += port.dataDecoded.gyro_z.ToString();
        //data += ",";
        //data += port.dataDecoded.mag_x.ToString();
        //data += ",";
        //data += port.dataDecoded.mag_y.ToString();
        //data += ",";
        //data += port.dataDecoded.mag_z.ToString();
        //data += "\r\n"; // 換行
        //list.Add(data);

        //bodyObj.saveDataCnt++;
    }

    public void SetDataIntoBinList(ref Objdefine bodyObj, ref PortContent port)
    {
        ref List<byte> list = ref bodyObj.saveList;

        for (int i = 0; i < port.dataUndecode.Length; i++)
        {
            list.Add(port.dataUndecode[i]);
        }

        bodyObj.saveDataCnt++;
    }

    public void SaveDataPoint(ref Objdefine bodyObj, ref PortContent port)
    {
        if (isSavingData)
        {
            // Save Raw Data to bin
            SetDataIntoBinList(ref bodyObj, ref port);

            if (bodyObj.saveDataCnt >= 100)
            {
                SaveDataIntoFile(ref bodyObj);
            }
        }
        else if (waitingToStopSaving) // 結束時要把剩下的data存完
        {
            if (lastByteToSave[port.portCnt] == 0)
                return;
            else
            {
                SetDataIntoBinList(ref bodyObj, ref port);

                if (bodyObj.saveDataCnt == 100)
                {
                    SaveDataIntoFile(ref bodyObj);
                }
                if ((--lastByteToSave[port.portCnt]) == 0)
                {
                    if (mainFunc.objList[port.portCnt].saveDataCnt > 0)
                        SaveDataIntoFile(ref mainFunc.objList[port.portCnt]);
                }
            }
            if (System.Linq.Enumerable.SequenceEqual(lastByteToSave, new int[] { 0, 0, 0, 0, 0, 0, 0 }))
            {
                print("Trial " + dataSave.saveCnt.ToString() + " save success..");
                waitingToStopSaving = false;
            }
        }
    }
    #endregion
#endif
}*/
#endregion


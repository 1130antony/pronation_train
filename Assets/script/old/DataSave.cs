using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;

/*
 * 儲存資料
 */

/* 修改資訊
 * ----------------------------------------------------------------
 * Date. 20191230
 * 
 * 執行時以當日日期來建立主要Folder(已存在時則跳過)
 * 若當日Folder已存在, 偵測當前Folder內Save資料夾, 以取得接下來的Save數
 * 開始存資料時, 建立一個文件以紀錄當下的儲存時間點
 * 重整Byte/Double/Float/String存檔程序
 * ----------------------------------------------------------------
 */

public class FileType
{
    public const string TXT = ".txt";
    public const string CSV = ".csv";
    public const string BIN = ".bin";
}

public class DataSave : MonoBehaviour
{
    float startTime;
    public string dateDir;
    public string saveDir;
    public int saveCnt;
    public List<byte> SaveList = new List<byte> { };

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
    }
    public void TT()
    {
        startTime = Time.time;
    }



    public void InitSaveCnt()
    {
        dateDir = Application.dataPath + "/SaveData/" +
               DateTime.Now.Year.ToString("0000") +
               DateTime.Now.Month.ToString("00") +
               DateTime.Now.Day.ToString("00");

        CheckFolder(dateDir);

        saveCnt = 1;
        NextSave();
    }

    public int GetSaveCnt()
    {
        return saveCnt;
    }

    public void NextSave()
    {
        saveDir = "/save" + saveCnt.ToString();

        string dir = dateDir + saveDir;
        while (Directory.Exists(dir) == true)
        {
            // 當偵測到Save的資料夾存在時
            saveCnt++;
            saveDir = "/save" + saveCnt.ToString();
            dir = dateDir + saveDir;
        }
    }

    private void CheckFolder(string dir)
    {
        if (Directory.Exists(dir))
        {
            return;
        }
        else
        {
            Directory.CreateDirectory(dir);
            string time =
                DateTime.Now.Year.ToString("0000") +
                "/" +
                DateTime.Now.Month.ToString("00") +
                "/" +
                DateTime.Now.Day.ToString("00") +
                " " +
                DateTime.Now.Hour.ToString("00") +
                ":" +
                DateTime.Now.Minute.ToString("00") +
                ":" +
                DateTime.Now.Second.ToString("00");
            SaveBuildTime(time, dir);
        }
    }

    private void SaveBuildTime(string saveString, string dir)
    {
        StreamWriter file = new StreamWriter(System.IO.Path.Combine(Application.dataPath, dir, "BuildTime.txt"));

        file.Write(saveString);
        file.Close();
    }

    public void SaveCalibrationPara(string saveString, string saveName, string type)
    {
        //string saveString = JsonUtility.ToJson(calibPara.GetParaMag());

        StreamWriter file = new StreamWriter(System.IO.Path.Combine(Application.dataPath, "CalibrationPara", saveName + type));

        file.Write(saveString);
        file.Close();

        print(saveName + " save success.");
    }

    public void SaveData(string saveStr, string saveName)
    {
        string fileDir = dateDir + saveDir;
        CheckFolder(fileDir);

        FileStream myFile = new FileStream(fileDir + "/" + saveName + ".dat", FileMode.Append, FileAccess.Write);
        BinaryWriter myWriter = new BinaryWriter(myFile);

        byte[] ASCIIbytes = Encoding.ASCII.GetBytes(saveStr); // by using system.text
                                                              // BinaryWriter寫入string時會在前面加上長度碼
                                                              // 我們不需要那個數值, 所以先轉換成byte即可避免此問題
        myWriter.Write(ASCIIbytes);

        myWriter.Close();
        myFile.Close();
    }

    public void SaveDataDecode(List<string> saveList, string saveName)
    {
        string fileDir = dateDir + saveDir;
        CheckFolder(fileDir);

        FileStream myFile = new FileStream(fileDir + "/" + saveName + ".dat", FileMode.Append, FileAccess.Write);
        BinaryWriter myWriter = new BinaryWriter(myFile);

        foreach (string data in saveList)
        {
            byte[] ASCIIbytes = Encoding.ASCII.GetBytes(data); // by using system.text
            //BinaryWriter寫入string時會在前面加上長度碼
            //我們不需要那個數值, 所以先轉換成byte即可避免此問題
            myWriter.Write(ASCIIbytes);
        }

        myWriter.Close();
        myFile.Close();
        saveList.Clear();
    }

    /*public void SaveData(ref List<float> saveList, string saveName) // 未測試
      {
          string fileDir = dateDir + saveDir;
          CheckFolder(fileDir);

          FileStream myFile = new FileStream(fileDir + "/" + saveName + ".dat", FileMode.Append, FileAccess.Write);
          BinaryWriter myWriter = new BinaryWriter(myFile);

          foreach (float data in saveList)
          {
              myWriter.Write(data);
          }

          myWriter.Close();
          myFile.Close();
          saveList.Clear();
      }*/

    public void SaveDataSignal(List<double> saveList, string saveName, string type) // 未測試
    {
        string fileDir = dateDir + saveDir;
        CheckFolder(fileDir);

        FileStream myFile = new FileStream(fileDir + "/" + saveName + type, FileMode.Append, FileAccess.Write);
        BinaryWriter myWriter = new BinaryWriter(myFile);

        foreach (double data in saveList)
        {
            myWriter.Write(data);
        }

        myWriter.Close();
        myFile.Close();
        saveList.Clear();
    }

    public void SaveData(List<byte> saveList, string saveName)
    {
        string fileDir = dateDir + saveDir;
        CheckFolder(fileDir);

        FileStream myFile = new FileStream(fileDir + "/" + saveName + ".dat", FileMode.Append, FileAccess.Write);
       
        BinaryWriter myWriter = new BinaryWriter(myFile);

        myWriter.Write(saveList.ToArray());

        myWriter.Close();
        
        myFile.Close();
        saveList.Clear();
        // print(saveName + " save success.");
    }

    public void SaveRTimeData(List<byte> saveList, string saveName)
    {
        float nowTime = Time.time-startTime ;
        string fileDir = dateDir + saveDir;
        CheckFolder(fileDir);

        FileStream Rfs = new FileStream(fileDir + "/" + saveName + ".csv", FileMode.Append, FileAccess.Write);
        //FileStream Lfs = new FileStream(Application.dataPath + "/Save Ltime.csv", FileMode.Append, FileAccess.Write);
        StreamWriter Rsw = new StreamWriter(Rfs);
        //StreamWriter Lsw = new StreamWriter(Lfs);
        Rsw.Write(nowTime.ToString("F2")+"\nRtime"+"\n");
        //Lsw.Write(nowTime.ToString("F2"));
        Rsw.Close();
       // Lsw.Close();
        Rfs.Close();
        // Lfs.Close();
        saveList.Clear();
        // print(saveName + " save success.");
    }
    public void SaveLTimeData(List<byte> saveList, string saveName)
    {
        float nowTime = Time.time - startTime;
        string fileDir = dateDir + saveDir;
        CheckFolder(fileDir);

        FileStream Lfs = new FileStream(fileDir + "/" + saveName + ".csv", FileMode.Append, FileAccess.Write);
        //FileStream Lfs = new FileStream(Application.dataPath + "/Save Ltime.csv", FileMode.Append, FileAccess.Write);
        StreamWriter Lsw = new StreamWriter(Lfs);
        //StreamWriter Lsw = new StreamWriter(Lfs);
        Lsw.Write(nowTime.ToString("F2") + "\nLtime" + "\n");
        //Lsw.Write(nowTime.ToString("F2"));
        Lsw.Close();
        // Lsw.Close();
        Lfs.Close();
        // Lfs.Close();
        saveList.Clear();
        // print(saveName + " save success.");
    }
    public void SaveENDTimeData(List<byte> saveList, string saveName)
    {
        float nowTime = Time.time - startTime;
        string fileDir = dateDir + saveDir;
        CheckFolder(fileDir);

        FileStream ends = new FileStream(fileDir + "/" + saveName + ".txt", FileMode.Append, FileAccess.Write);
        //FileStream Lfs = new FileStream(Application.dataPath + "/Save Ltime.csv", FileMode.Append, FileAccess.Write);
        StreamWriter endsw = new StreamWriter(ends);
        //StreamWriter Lsw = new StreamWriter(Lfs);
        endsw.Write(nowTime.ToString("F2") + "\nendtime" + "\n");
        //Lsw.Write(nowTime.ToString("F2"));
        endsw.Close();
        // Lsw.Close();
        ends.Close();
        // Lfs.Close();
        saveList.Clear();
        // print(saveName + " save success.");
    }
}

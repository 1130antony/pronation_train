using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
/* MagCalibration****************************************************************************************************
 * 磁力計校正相關函式及參數
 */
public class MagCalibration
{
    public float offset_mx = 0f;
    public float offset_my = 0f;
    public float offset_mz = 0f;

    public float scale_mx = 1f;
    public float scale_my = 1f;
    public float scale_mz = 1f;
}

public class MagCalibrationFunc : MonoBehaviour
{
    // 讀取磁力計校正值
    private bool ReadJsonFile(string fileName, ref MagCalibration loadPara)
    {
        try
        {
            StreamReader file = new StreamReader(System.IO.Path.Combine(Application.dataPath, "CalibrationPara", fileName + ".txt"));
            string readString = file.ReadToEnd();
            if (readString != "")
            {
                file.Close();

                loadPara = JsonUtility.FromJson<MagCalibration>(readString);
                print(fileName + " load success.");
                return true;
            }
            else
            {
                // file 沒有內容
                file.Close();
                print(fileName + " file is empty, please check the file.");
                return false;
            }
        }
        catch
        {
            // file不存在
            return false;
        }
    }

    public void LoadMagCalibrationValue(ref Objdefine part)
    {
        string fileName = "MagCali";
        if (ReadJsonFile(fileName, ref part.magCalibration) == false)
        {
            print("Magmatic calibration parameter of " + part.bodyPart + " is fail, use default parameter.");
        }
    }
}

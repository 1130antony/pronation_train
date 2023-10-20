using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


#region 陀螺儀校正相關函式及參數
public class GyroCalibration
{
    public const int INDEX_GX = 0, INDEX_GY = 1, INDEX_GZ = 2;

    private bool doCalibration = false;
    public int dataCnt = 0;
    private float[] countBuf = new float[3];
    private float[] lastGyro = new float[3];
    private float[] zeroError = new float[3] { 0, 0, 0 };


    private void CountZeroError()
    {
        if (dataCnt == 0) return;

        for (int i = 0; i < 3; i++)
        {
            zeroError[i] = (countBuf[i] / dataCnt);
        }
    }

    private void SetLastGyro(float gx, float gy, float gz)
    {
        lastGyro[0] = gx;
        lastGyro[1] = gy;
        lastGyro[2] = gz;
    }

    private bool CheckThreshold(float gx, float gy, float gz)
    {
        if (Mathf.Abs(gx - lastGyro[0]) > 0.1) return false;
        if (Mathf.Abs(gy - lastGyro[1]) > 0.1) return false;
        if (Mathf.Abs(gz - lastGyro[2]) > 0.1) return false;

        return true;
    }

    public void ResetCali()
    {
        doCalibration = true;
        dataCnt = 0;
        countBuf[0] = 0;
        countBuf[1] = 0;
        countBuf[2] = 0;
    }

    public bool CaliMode(float gx, float gy, float gz)
    {
        dataCnt++;
        countBuf[0] += gx;
        countBuf[1] += gy;
        countBuf[2] += gz;

        if ((dataCnt >= 2) && (CheckThreshold(gx, gy, gz) == false))
        {
            ResetCali();
            return false;
        }
        else
        {
            SetLastGyro(gx, gy, gz);
        }

        if (dataCnt == 200)
        {
            CountZeroError();
            doCalibration = false;
        }

        return true;
    }

    public bool CaliMode(Axis axis)
    {
        dataCnt++;
        countBuf[0] += axis.gx;
        countBuf[1] += axis.gy;
        countBuf[2] += axis.gz;

        if ((dataCnt >= 2) && (CheckThreshold(axis.gx, axis.gy, axis.gz) == false))
        {
            ResetCali();
            return false;
        }
        else
        {
            SetLastGyro(axis.gx, axis.gy, axis.gz);
        }

        if (dataCnt == 200)
        {
            CountZeroError();
            doCalibration = false;
        }

        return true;
    }
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

    public bool IsDoingCalibration()
    {
        return doCalibration;
    }
}
#endregion

public class RealtimeModeFunc : MonoBehaviour
{

    
    
    Quateraion quateraion = new Quateraion();
    //public Toggle Reverse;

    public void Update_RealtimeMode(int partCnt, ref Objdefine part)

    {
        //bool on = gameObject.GetComponent<Toggle>().isOn;

        float[] q = new float[4];

        try
        {
            if (SerialPortControl.func.portAll[partCnt].sp.IsOpen == false) return;

            q[0] = part.q[0];
            q[1] = part.q[1];
            q[2] = part.q[2];
            q[3] = part.q[3];

            part.bodyPart.rotation = new Quaternion(q[0], q[2], q[3],q[1]);
            part.bodyPart1.rotation = new Quaternion(q[0], q[2], -q[3], q[1]);// 因Sensor與unity軸向不同, 需要將Quaternion的z與y旋轉值互換才能讓兩者旋轉軸相同
            part.bodyPart2.rotation = new Quaternion(q [0], q[2],q[3], q[1]);
            part.bodyPart3.rotation = new Quaternion(q[0], q[2], -q[3], q[1]);


            
            //part.bodyPart4.rotation = new Quaternion(q[0], q[2], -q[3], q[1]);
            //part.bodyPart4.rotation = new Quaternion(q[0], q[2], q[3], q[1]);

            //part.bodyPart5.rotation = new Quaternion(q[0], q[2], -q[3], q[1]);
           // part.bodyPart5.rotation = new Quaternion(q[0], q[2], q[3], q[1]);

           
        }


        catch
        {
            print("Object is Null...");
        }


    }
   



    public void FixedUpdate_RealtimeMode(int partCnt, ref Objdefine part)
    {
        if (SerialPortControl.func.portAll[partCnt].dataReady == false) return;

        GetAxisData(partCnt, ref part);
        RunCalculate(partCnt, ref part);

        SerialPortControl.func.portAll[partCnt].dataReady = false;
    }

    #region 陀螺儀校正重置
    public void ResetGyroCali(ref Objdefine part)
    {
        part.gyroCalibration.ResetCali();

        part.q[0] = 1f;
        part.q[1] = 0f;
        part.q[2] = 0f;
        part.q[3] = 0f;
    }
    #endregion

    private void QuaternionAlgorithm(ref Objdefine part)
    {
        ref float[] q = ref part.q;

        float[] partAcc = new float[3];
        float[] partGyro = new float[3];
        float[] partMag = new float[3];
        // 將裝置的軸方向轉換成Unity物件方向, 只有在處理四元數法時, 將軸轉向, 存檔皆以裝置軸方向為主
        // gyro減去零點偏移
        partAcc[0] = part.axis.ax;
        partAcc[1] = part.axis.ay;
        partAcc[2] = part.axis.az;
        partGyro[0] = part.axis.gx - part.gyroCalibration.GetZeroError(GyroCalibration.INDEX_GX);
        partGyro[1] = part.axis.gy - part.gyroCalibration.GetZeroError(GyroCalibration.INDEX_GY);
        partGyro[2] = part.axis.gz - part.gyroCalibration.GetZeroError(GyroCalibration.INDEX_GZ);
        partMag[0] = (part.axis.mx - part.magCalibration.offset_mx) * part.magCalibration.scale_mx;
        partMag[1] = (part.axis.my - part.magCalibration.offset_my) * part.magCalibration.scale_my;
        partMag[2] = (part.axis.mz - part.magCalibration.offset_mz) * part.magCalibration.scale_mz;

        // 四元數演算
        if (SerialPortControl.inputDataIsNineAxis) // 九軸
        {
            quateraion.MahonyQuaternionUpdate(ref q, partAcc[0], partAcc[1], partAcc[2], partGyro[0], partGyro[1], partGyro[2], partMag[0], partMag[1], partMag[2]);
        }
        else // 六軸
        {
            quateraion.MadgwickQuaternionUpdate(ref q, partAcc[0], partAcc[1], partAcc[2], partGyro[0], partGyro[1], partGyro[2]); // 綁在鞋子上的方位
        }

        // 計算尤拉角
        part.euler.yaw = Mathf.Atan2(2.0f * (q[1] * q[2] + q[0] * q[3]), q[0] * q[0] + q[1] * q[1] - q[2] * q[2] - q[3] * q[3]);
        part.euler.pitch = -Mathf.Asin(2.0f * (q[1] * q[3] - q[0] * q[2]));
        part.euler.roll = Mathf.Atan2(2.0f * (q[0] * q[1] + q[2] * q[3]), q[0] * q[0] - q[1] * q[1] - q[2] * q[2] + q[3] * q[3]);
        part.euler.pitch *= 180.0f / Mathf.PI;
        part.euler.yaw *= 180.0f / Mathf.PI;
        part.euler.roll *= 180.0f / Mathf.PI;
    }

    public void RunCalculate(int portCnt, ref Objdefine part)
    {
        if (part.gyroCalibration.IsDoingCalibration() == true)
        {
            if (part.gyroCalibration.CaliMode(part.axis) == false)
            {
                print("偵測到晃動,重新校正......" + part.axis.gx + " " + part.axis.gy + " " + part.axis.gz);
            }
            // 執行完校正後確認是否取消校正模式,代表校正已完成,修改UI顯示
            if (part.gyroCalibration.IsDoingCalibration() != true)
            {
                print("校正完成");
            }
        }
        else
        {
            QuaternionAlgorithm(ref part);
        }
    }

    // 取得資料
    public void GetAxisData(int partCnt, ref Objdefine part)
    {
        ref DataObject_Decoded decodedData = ref SerialPortControl.func.portAll[partCnt].dataDecoded;
        ref Axis axis = ref part.axis;
        ref Axis rawAxis = ref part.axisRawData;

        // raw axis data for saving
        /*rawAxis.ax = decodedData.acc_x;
        rawAxis.ay = decodedData.acc_y;
        rawAxis.az = decodedData.acc_z;
        rawAxis.gx = decodedData.gyro_x;
        rawAxis.gy = decodedData.gyro_y;
        rawAxis.gz = decodedData.gyro_z;
        rawAxis.mx = decodedData.mag_x;
        rawAxis.my = decodedData.mag_y;
        rawAxis.mz = decodedData.mag_z;
        rawAxis.ax = decodedData.acc_x; //原本的軸

        rawAxis.ax = decodedData.acc_x;
        rawAxis.ay = (decodedData.acc_z);
        rawAxis.az = (decodedData.acc_y);
        rawAxis.gx = decodedData.gyro_x;
        rawAxis.gy = (decodedData.gyro_z);
        rawAxis.gz = (decodedData.gyro_y);
        rawAxis.mx = decodedData.mag_x;
        rawAxis.my = (decodedData.mag_z);
        rawAxis.mz = (decodedData.mag_y);*/ //放前側的軸
       ///*
        rawAxis.ax = -(decodedData.acc_z);
        rawAxis.ay = -(decodedData.acc_x);
        rawAxis.az = -(decodedData.acc_y);
        rawAxis.gx = -(decodedData.gyro_z);
        rawAxis.gy = -(decodedData.gyro_x);
        rawAxis.gz = -(decodedData.gyro_y);
        rawAxis.mx = -(decodedData.mag_z);
        rawAxis.my =-(decodedData.mag_x);
        rawAxis.mz = -(decodedData.mag_y); //放右側邊的軸
     //   */
      /*
        rawAxis.ax = (decodedData.acc_z);
        rawAxis.ay = (decodedData.acc_x);
        rawAxis.az = -(decodedData.acc_y);
        rawAxis.gx = (decodedData.gyro_z);
        rawAxis.gy = (decodedData.gyro_x);
        rawAxis.gz =- (decodedData.gyro_y);
        rawAxis.mx = (decodedData.mag_z);
        rawAxis.my = (decodedData.mag_x);
        rawAxis.mz = -(decodedData.mag_y); //放左側邊的軸
       */

        //print(rawAxis.ax + " " + rawAxis.ay + " " + rawAxis.az);

        // 用來處理四元數的data
        /* axis.ax = decodedData.acc_x;
         axis.ay = decodedData.acc_y;
         axis.az = decodedData.acc_z;
         axis.gx = (decodedData.gyro_x * Mathf.PI) / 180f;
         axis.gy = (decodedData.gyro_y * Mathf.PI) / 180f;
         axis.gz = (decodedData.gyro_z * Mathf.PI) / 180f;
         axis.mx = decodedData.mag_x;
         axis.my = decodedData.mag_y;
         axis.mz = decodedData.mag_z; //原本的軸

        axis.ax = decodedData.acc_x;
        axis.ay = (decodedData.acc_z);
        axis.az = (decodedData.acc_y);
        axis.gx = (decodedData.gyro_x * Mathf.PI) / 180f;
        axis.gy = (decodedData.gyro_z * Mathf.PI) / 180f;
        axis.gz = (decodedData.gyro_y * Mathf.PI) / 180f;
        axis.mx = decodedData.mag_x;
        axis.my = (decodedData.mag_z);
        axis.mz = (decodedData.mag_y);*/   //放前側的軸
        ///*
        axis.ax = -(decodedData.acc_z);
        axis.ay = -(decodedData.acc_x);
        axis.az = -(decodedData.acc_y);
        axis.gx =-(decodedData.gyro_z * Mathf.PI) / 180f;
        axis.gy = -(decodedData.gyro_x * Mathf.PI) / 180f;
        axis.gz =-(decodedData.gyro_y * Mathf.PI) / 180f;
        axis.mx = -(decodedData.mag_z);
        axis.my = -(decodedData.mag_x);
        axis.mz = -(decodedData.mag_y);     //放右側邊的軸
        //*/
       /*
        axis.ax = (decodedData.acc_z);
        axis.ay = (decodedData.acc_x);
        axis.az = -(decodedData.acc_y);
        axis.gx = (decodedData.gyro_z * Mathf.PI) / 180f;
        axis.gy = (decodedData.gyro_x * Mathf.PI) / 180f;
        axis.gz = -(decodedData.gyro_y * Mathf.PI) / 180f;
        axis.mx = (decodedData.mag_z);
        axis.my = (decodedData.mag_x);
        axis.mz = -(decodedData.mag_y);     //放左側邊的軸
        */
        SerialPortControl.func.portAll[partCnt].dataReady = false;
    }

 
}


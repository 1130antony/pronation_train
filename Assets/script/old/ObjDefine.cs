using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Axis
{
    public float ax, ay, az, gx, gy, gz, mx, my, mz;
}

public class Euler
{
    public float yaw, pitch, roll;
}

public class Objdefine
{
    public Transform bodyPart;
    public Transform bodyPart1;
    public Transform bodyPart2;
    public Transform bodyPart3;
    public Transform bodyPart4;
    public Transform bodyPart5;
    public Axis axis;
    public Axis axisRawData;
    public Euler euler;
    public float[] q;
    public GyroCalibration gyroCalibration;
    public MagCalibration magCalibration;
}






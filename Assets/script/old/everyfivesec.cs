using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;
using System.Text;

public class everyfivesec : MonoBehaviour
{
    int sec = 0;
    //public static SerialPort ap = new SerialPort("COM9", 9600);
  //  public static SerialPort bp = new SerialPort("COM9", 9600);
    // Start is called before the first frame update
    void Start()
    {
        //bp.Open();
    }
    public void fivesecond()
    {
        InvokeRepeating("second", 5f, 5f);
        //ap.Write("1");
    }
    public void second()
    {
        sec+= 1;
        Debug.Log(sec);
        //bp.Write("1");
    }
    void Update()
    {

    }
}




    // Update is called once per frame
    

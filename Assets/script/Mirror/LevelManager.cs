using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class LevelManager : MonoBehaviour
{
    Random r = new Random();
    int randomNumber;
    public string date = DateTime.Now.ToString("yyyy-MM-dd");
    int time = int.Parse(DateTime.Now.ToString("HH"));
    public InputField txtName;
    public InputField txtDate;
    public InputField txtSex;
    public InputField txtAge;
    private string path = "C:\\Users\\USER\\Desktop\\蔡俊安\\收案資料\\";
    void Start()
    {
        randomNumber = r.Next(2);
        if (time < 12)
            txtDate.text = date + " 上午";
        else
            txtDate.text = date + " 下午";
        path += txtDate.text+"基本資料.txt";
        Debug.Log(path);
    }
    #region 開始測驗，跳轉畫面(Btn_start)
    public void StartGame()
    {
        Debug.Log(randomNumber);
        if (txtName.text != "" && txtSex.text != "" && txtAge.text != "")
        {
            switch (randomNumber)
            {
                case 0:
                    SceneManager.LoadScene("left_hand");
                    break;
                case 1:
                    SceneManager.LoadScene("right_hand");
                    break;
                case 2:
                    SceneManager.LoadScene("left_hand_delay");
                    break;
            }

        }
        else
        {
            Debug.Log("資料尚未填寫完整");
        }
    }
    #endregion
    #region 儲存基本資料(Btn_save)
    public void SaveInformation()
    {
        if (!File.Exists(path))
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.Write("姓名:" + txtName.text + "\n");
                sw.Write("日期:" + txtDate.text + "\n");
                sw.Write("性別:" + txtSex.text + "\n");
                sw.Write("年齡:" + txtAge.text);
            }
            Debug.Log("儲存完成!!!!");
        }
        else
        {
            Debug.Log("此時段檔案已存在!!!!");
        }
    }
    #endregion

}

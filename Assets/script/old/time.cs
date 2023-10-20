using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class time : MonoBehaviour
{
    float startTime;
   
    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        showtime();
    }
   public void showtime()
    {
        
        float nowTime = Time.time-startTime;
        //Debug.Log("開始時間:" + nowTime);
        GameObject.Find("timer").GetComponent<UnityEngine.UI.Text>().text = nowTime.ToString("F2");
    }
    public void onclick()
    {
        startTime = Time.time;

    }
}

   
    
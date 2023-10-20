using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reverse : MonoBehaviour
{
    private const bool Value = false;
    public GameObject A;
    public GameObject B;
    public GameObject C;
    public GameObject D;
    public GameObject E;
    public GameObject F;
    public GameObject G;
    public GameObject H;
    public void OnClick(bool isOn)
    {
        if (isOn)
        {
            A.SetActive(true);
            B.SetActive(true);
            C.SetActive(true);
            D.SetActive(true);
            E.SetActive(true);
            F.SetActive(true);
            G.SetActive(true);
            H.SetActive(true);
            print("yes");
        }
        else
        {
            A.SetActive(false);
            B.SetActive(Value);
            C.SetActive(false);
            D.SetActive(false);
            E.SetActive(false);
            F.SetActive(false);
            G.SetActive(false);
            H.SetActive(false);
            print("no");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class savedate : MonoBehaviour
{
    public string outputDir;
    public List<byte> SaveList = new List<byte> { };
    public List<string> SaveDecode = new List<string> { };
    public List<double> SaveSignal = new List<double> { };
    void Start()
    {
        outputDir = Application.dataPath + "/SaveData";
    }
    void Update()
    {
    }
    public void BinSave(List<byte> Save_List, string SaveName)
    {
        FileStream myFile = new FileStream(outputDir + "/" + SaveName + ".dat", FileMode.Append, FileAccess.Write);
        BinaryWriter myWriter = new BinaryWriter(myFile);
        myWriter.Write(SaveList.ToArray());
        myWriter.Close();
        myFile.Close();
        Save_List.Clear();
        print(SaveName + " save success.");
    }
    public void BinSaveDecode(List<string> Save_List, string SaveName)
    {
        FileStream myFile = new FileStream(outputDir + "/" + SaveName + ".txt", FileMode.Append, FileAccess.Write);
        BinaryWriter myWriter = new BinaryWriter(myFile);
        foreach (string decode in Save_List)
        {
            myWriter.Write(decode);
        }
        myWriter.Close();
        myFile.Close();
        SaveDecode = new List<string> { };
    }

    public void BinSaveSignal(List<double> Save_List, string SaveName)
    {
        FileStream myFile = new FileStream(outputDir + "/" + SaveName + ".dat", FileMode.Append, FileAccess.Write);
        BinaryWriter myWriter = new BinaryWriter(myFile);
        foreach (double signal in Save_List)
        {
            myWriter.Write(signal);
        }
        myWriter.Close();
        myFile.Close();
        SaveSignal = new List<double> { };
    }
}

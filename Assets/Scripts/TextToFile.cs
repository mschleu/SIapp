using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class TextToFile : MonoBehaviour
{

    string file_l;
    string file_r;
    string data_file;

    void Start()
    {
        //KeyStrokeLog-yyyy-MM-dd-HH-mm-ss-ms
        file_l = Application.persistentDataPath + "/SmartInsoleL-" + DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt";
        file_r = Application.persistentDataPath + "/SmartInsoleR-" + DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt";
        data_file = Application.persistentDataPath + "/SmartInsole-DataFile.txt";
    }

    public void storeFile(int fil, string mes)
    {
        try
        {
            if(fil == 1)
            {
                if (!File.Exists(file_l))
                {
                    File.WriteAllText(file_l, mes);
                }
                else
                {
                    File.AppendAllText(file_l, mes);
                }
            }
            else
            {
                if (!File.Exists(file_r))
                {
                    File.WriteAllText(file_r, mes);
                }
                else
                {
                    File.AppendAllText(file_r, mes);
                }
            }
        }

        catch (System.Exception e)
        {
            Debug.Log(e);
        }
    }
}

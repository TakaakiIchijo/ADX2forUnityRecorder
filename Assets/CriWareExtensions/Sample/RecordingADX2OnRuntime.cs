using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RecordingADX2OnRuntime : MonoBehaviour
{
    public CriAtomRecorder atomRecoder;
    public string fileName = "recordedaudio";

    private const string BackUpDirectory = "SavedGameData";
    private string deviceSavedGameDataPath;
    
    //PC: C:\Users\[UserName]\AppData\LocalLow\[OrganizationName]\[AppName]\SavedGameData
    //iOS: Application/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/Documents\SavedGameData
    //Android: /data/[PackageName]/files\SavedGameData

    private void Awake()
    {
        deviceSavedGameDataPath = Path.Combine(Application.persistentDataPath, BackUpDirectory);

        if (Directory.Exists(deviceSavedGameDataPath) == false)
        {
            Directory.CreateDirectory(deviceSavedGameDataPath);
        }
    }

    public void OnRecordStart()
    {
        var path = deviceSavedGameDataPath +"/"+fileName + "_" + DateTime.Now.ToString("yyyyMMdd_hh_mm_ss");
        
        atomRecoder.StartRecording(path);
    }

    public void OnRecordStop()
    {
        atomRecoder.StopRecording();
    }
    
}

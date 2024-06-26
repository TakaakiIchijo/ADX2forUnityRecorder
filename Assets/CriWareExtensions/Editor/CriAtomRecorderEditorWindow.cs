﻿using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using CriWare;

public class CriAtomRecorderEditorWindow : EditorWindow
{
    public string savePath = "Recordings";
    public string fileName = "recorded_audio";
    public bool addTimeStampToFileName = true;
    
    private string savedFileName;
    private string savedDirectoryPath;
    
    private CriAtomRecorderInstance recorderInstance;

    private bool isRecording = false;
    private string recordButtonName = "Start recording";

    [MenuItem("Window/CRIWARE/CriAtomRecorder")]
    private static void Create()
    {
        var window = GetWindow<CriAtomRecorderEditorWindow>("CriAtomRecorder");
        window.minSize = new Vector2(300, 250);
    }
    private void OpenDirectory()
    {
        Process.Start(GetDataPathDirectoryOrCreate(savePath));
    }

    private void OnGUI()
    {
        EditorGUI.BeginDisabledGroup(isRecording); 
        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Label("Save Directory");
            savePath = GUILayout.TextField(savePath);
        }
        
        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Label("File Name");
            fileName = GUILayout.TextField(fileName);
        }

        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Label("Add TimeStamp to FileName");
            addTimeStampToFileName = EditorGUILayout.Toggle (addTimeStampToFileName);
        }
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying); 
        using (new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button(recordButtonName, GUILayout.Width(200), GUILayout.Height(100)))
            {
                if (isRecording == false)
                {
                    recordButtonName = "Stop recording";
                    StartRecording();
                }
                else
                {
                    recordButtonName = "Start recording";
                    StopRecording();
                }
            }
        }
        EditorGUI.EndDisabledGroup();
        
        using (new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Open Directory", GUILayout.Width(200), GUILayout.Height(50)))
            {
                OpenDirectory();
            }
        }
        
        using (new GUILayout.HorizontalScope())
        {
            if(EditorApplication.isPlaying && isRecording)
            {
                GUILayout.Label("Directory: " + savedDirectoryPath);
            }
        }
        
        using (new GUILayout.HorizontalScope())
        {
            if(EditorApplication.isPlaying && isRecording)
            {
                GUILayout.Label("Recording to: " + savedFileName);
            }
        }
    }

    private void StartRecording()
    {
        isRecording = true;
        
        GameObject recorderObj = new GameObject("CriAtomRecorderInstance");
        recorderInstance = recorderObj.AddComponent<CriAtomRecorderInstance>();
        recorderObj.hideFlags = HideFlags.HideInHierarchy;
        
        var tempFileName = fileName;

        if (addTimeStampToFileName)
        {
            tempFileName += "_" + DateTime.Now.ToString("yyyyMMdd_hh_mm_ss");
        }

        var initializer = FindObjectOfType<CriWareInitializer>();
        int samplingRate = initializer.atomConfig.outputSamplingRate;
        recorderInstance.SetParameters(GetDataPathDirectoryOrCreate(savePath) + "/" + tempFileName, samplingRate);
        recorderInstance.StartRecordingCoroutine();

        var fullPath = recorderInstance.GetCreatedFileFullPath();
        savedFileName = Path.GetFileName(fullPath);
        savedDirectoryPath = fullPath.Replace(savedFileName, "");
    }

    private void StopRecording()
    {
        isRecording = false;

        Destroy(recorderInstance.gameObject);
    }

    public static string GetDataPathDirectoryOrCreate(string directoryName)
    {
        var path = Directory.GetParent(Application.dataPath) + "/" + directoryName;

        if (Directory.Exists(path) == false)
        {
            Directory.CreateDirectory(path);
        }

        return path;
    }
}

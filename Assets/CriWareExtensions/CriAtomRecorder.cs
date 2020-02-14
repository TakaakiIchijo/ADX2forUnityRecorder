using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class CriAtomRecorder: MonoBehaviour
{
    private WaveFileCreator waveFileCreator;

    private CriAtomExOutputAnalyzer analyzer;

    private bool IsRecording = false;
    private int numSamples = 512;

    private IEnumerator recordingCoroutine;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void StartRecording(string filePath, int samplingRate = 48000)
    {
        waveFileCreator = new WaveFileCreator(
            filePath,
            numChannels:2,
            samplingRate,
            numbites:16
            );
        IsRecording = false;

        recordingCoroutine = Record();
        
        StartCoroutine(recordingCoroutine);
    }

    public string GetCreatedFileFullPath()
    {
       return waveFileCreator.FileFullPath;
    }

    IEnumerator Record()
    {
        // Wait for Loading ACB...
        while (CriAtom.CueSheetsAreLoading) {
            yield return null;
        }

        /* Initialize CriAtomExOutputAnalyzer for PCM capture. */
        CriAtomExOutputAnalyzer.Config config = new CriAtomExOutputAnalyzer.Config
        {
            enablePcmCapture = true, 
            enablePcmCaptureCallback = true, 
            numCapturedPcmSamples = numSamples
        };
        
        analyzer = new CriAtomExOutputAnalyzer(config);
        analyzer.SetPcmCaptureCallback(PcmCapture);
        
        analyzer.AttachDspBus("MasterOut");

        IsRecording = true;
    }

    public void StopRecording()
    {
        if (recordingCoroutine != null)
        {
            StopAndWrite();
            
            IsRecording = false;
            
            StopCoroutine(recordingCoroutine);
            recordingCoroutine = null;
        }
    }

    public void PcmCapture(float[] dataL, float[] dataR, int numChannels, int numData)
    {
        if (!IsRecording || waveFileCreator == null)
            return;
        
        waveFileCreator.CapturePcm(dataL, dataR, numData);
        //Debug.Log(numData);
    }

    private void StopAndWrite()
    {
        if (waveFileCreator == null) return;
        
        waveFileCreator.StopAndWrite();
        
        if (analyzer != null) {
            analyzer.DetachDspBus();
            analyzer.Dispose();
        }
    }

    private void Update()
    {
        if (IsRecording)
        {
            analyzer.ExecutePcmCaptureCallback();
        }
    }

    private void OnDisable()
    {
        StopAndWrite();
    }
}

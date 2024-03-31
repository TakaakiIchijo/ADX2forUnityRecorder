using System;
using System.Collections;
using CriWare;
using UnityEngine;

#region R3+UniTask version
//using Cysharp.Threading.Tasks;
//using R3;
#endregion

public class CriAtomRecorderInstance: MonoBehaviour
{
    private WaveFileCreator waveFileCreator;
    private CriAtomExOutputAnalyzer analyzer;

    public bool IsRecording {get; private set;} = false;    
    private int numSamples = 512;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void SetParameters(string filePath, int outPutSamplingRate = 0)
    {
        if (outPutSamplingRate == 0)
        {
            outPutSamplingRate = 48000;
        }

        waveFileCreator = new WaveFileCreator(
            filePath,
            numChannels:2,
            outPutSamplingRate,
            numbites:16
        );
        IsRecording = false;
    }

    public string GetCreatedFileFullPath()
    {
       return waveFileCreator?.FileFullPath;
    }

#region R3+UniTask version
    /* 
    IDisposable recordingUpdate;
    
    public async UniTask StartRecording()
    {
        await UniTask.WaitUntil(() => CriAtom.CueSheetsAreLoading == false);

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

        recordingUpdate = Observable.EveryUpdate().Subscribe(_ =>
        {
            analyzer.ExecutePcmCaptureCallback();
        });
    }
    
    public void StopRecording()
    {
        if (IsRecording == false) return;
        
        StopAndWrite();
        IsRecording = false;

        recordingUpdate?.Dispose();
    }
    */
#endregion
    
#region Coroutine version
    private IEnumerator recordingCoroutine;

    public void StartRecordingCoroutine()
    {
        recordingCoroutine = RecordCoroutine();
        StartCoroutine(recordingCoroutine);
    }
    
    IEnumerator RecordCoroutine()
    {
        while (CriAtom.CueSheetsAreLoading) {
            yield return null;
        }

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
    private void Update()
    {
        if (IsRecording)
        {
            analyzer.ExecutePcmCaptureCallback();
        }
    }
    
    public void StopRecording()
    {
        if (IsRecording == false) return;
        
        StopAndWrite();
        IsRecording = false;
        StopCoroutine(recordingCoroutine);
        recordingCoroutine = null;
    }
#endregion
    
    private void PcmCapture(float[] dataL, float[] dataR, int numChannels, int numData)
    {
        if (!IsRecording || waveFileCreator == null)
            return;
        
        waveFileCreator.CapturePcm(dataL, dataR, numData);
    }

    private void StopAndWrite()
    {
        if (waveFileCreator == null　|| IsRecording == false) return;
        
        waveFileCreator.StopAndWrite();
        
        if (analyzer != null) {
            analyzer.DetachDspBus();
            analyzer.Dispose();
        }
    }

    private void OnDisable()
    {
        StopAndWrite();
    }
}

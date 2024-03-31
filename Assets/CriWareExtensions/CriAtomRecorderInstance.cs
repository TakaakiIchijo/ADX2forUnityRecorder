using CriWare;
using System.Collections;
using UnityEngine;

public class CriAtomRecorderInstance: MonoBehaviour
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
    
    public void StartRecordingCoroutine()
    {
        recordingCoroutine = RecordCoroutine();
        
        StartCoroutine(recordingCoroutine);
    }

    public string GetCreatedFileFullPath()
    {
       return waveFileCreator?.FileFullPath;
    }

    IEnumerator RecordCoroutine()
    {
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
        if (IsRecording == false) return;
        
        StopAndWrite();
        IsRecording = false;
        StopCoroutine(recordingCoroutine);
        recordingCoroutine = null;
    }

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

using System;
using UnityEngine;
using CriWare;

namespace UnityEditor.Recorder
{
    class AtomRecorder : GenericRecorder<AtomRecorderSettings>
    {
        private WaveFileCreator waveFileCreator;
        private CriAtomExOutputAnalyzer analyzer;
        
        protected override bool BeginRecording(RecordingSession session)
        {
            if (!base.BeginRecording(session))
                return false;
            
            try
            {
                Settings.FileNameGenerator.CreateDirectory(session);
            }
            catch (Exception)
            {
                Debug.LogError(string.Format( "Atom recorder output directory \"{0}\" could not be created.", Settings.FileNameGenerator.BuildAbsolutePath(session)));
                return false;
            }
            
            var initializer = FindObjectOfType<CriWareInitializer>();
            int samplingRate = initializer.atomConfig.outputSamplingRate;
            
            if (samplingRate == 0)
            {
                samplingRate = 48000;
            }
            
            try
            {
                var path =  Settings.FileNameGenerator.BuildAbsolutePath(session);

                waveFileCreator = new WaveFileCreator(
                    path,
                    numChannels:2,
                    samplingRate,
                    numbites:16
                );

                CriAtomExOutputAnalyzer.Config config = new CriAtomExOutputAnalyzer.Config
                {
                    enablePcmCapture = true, 
                    enablePcmCaptureCallback = true, 
                    numCapturedPcmSamples = Settings.NumSamples
                };
        
                analyzer = new CriAtomExOutputAnalyzer(config);
                analyzer.SetPcmCaptureCallback(PcmCapture);
        
                analyzer.AttachDspBus("MasterOut");

                return true;
            }
            catch
            {
                if (RecorderOptions.VerboseMode)
                    Debug.LogError("AudioRecorder unable to create MovieEncoder.");
            }
            
            return false;
        }
        
        private void PcmCapture(float[] dataL, float[] dataR, int numChannels, int numData)
        {
            if (waveFileCreator == null)
                return;
        
            waveFileCreator.CapturePcm(dataL, dataR, numData);
        }
        
        protected override void RecordFrame(RecordingSession session)
        {
            analyzer?.ExecutePcmCaptureCallback();
        }

        protected override void EndRecording(RecordingSession session)
        {
            base.EndRecording(session);

            if (waveFileCreator != null)
            {
                waveFileCreator.StopAndWrite();
            }

            if (analyzer != null) {
                analyzer.DetachDspBus();
                analyzer.Dispose();
            }
        }
    }
}

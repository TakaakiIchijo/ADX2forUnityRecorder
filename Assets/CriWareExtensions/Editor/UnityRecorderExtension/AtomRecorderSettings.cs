using System;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using UnityEditor.Media;
using UnityEditor.Recorder.Input;
using UnityEngine;


namespace UnityEditor.Recorder
{
    [RecorderSettings(typeof(AtomRecorder), "ADX2Audio")]
    class AtomRecorderSettings : RecorderSettings
    {
        protected override string Extension => "wav";
        
        public int NumSamples
        {
            get { return numSamples; }
            set { numSamples = value; }
        }

        [SerializeField] private int numSamples = 512;
        
        [SerializeField] AtomInputSettings m_AtomInputSettings = new AtomInputSettings();

        public override IEnumerable<RecorderInputSettings> InputsSettings
        {
            get { yield return m_AtomInputSettings; }
        }
        
        public AtomRecorderSettings()
        {
            FileNameGenerator.FileName = "mixdown";
        }
    }
}

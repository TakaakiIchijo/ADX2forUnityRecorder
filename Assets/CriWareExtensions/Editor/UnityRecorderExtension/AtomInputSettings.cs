
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace UnityEditor.Recorder.Input
{
    /// <summary>
    /// Use this class to manage all the information required to record audio from a Scene.
    /// </summary>
    [DisplayName("ADX2")]
    [Serializable]
    public class AtomInputSettings : RecorderInputSettings
    {
        protected override Type InputType
        {
            get { return typeof(RecorderInput); }
        }
        
        protected override bool ValidityCheck(List<string> errors)
        {
            return true;
        }
    }
}

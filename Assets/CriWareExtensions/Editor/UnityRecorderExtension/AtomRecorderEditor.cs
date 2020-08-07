using UnityEngine;

namespace UnityEditor.Recorder
{
    [CustomEditor(typeof(AtomRecorderSettings))]
    class AtomRecorderEditor : RecorderEditor
    {
        SerializedProperty m_NumSamples;

        static class Styles
        {
            internal static readonly GUIContent NumSamples = new GUIContent("NumSamples");
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();

            if (target == null)
                return;
            
            m_NumSamples = serializedObject.FindProperty("numSamples");

        }

        protected override void FileTypeAndFormatGUI()
        {
            EditorGUILayout.PropertyField(m_NumSamples, Styles.NumSamples);
        }
    }
}
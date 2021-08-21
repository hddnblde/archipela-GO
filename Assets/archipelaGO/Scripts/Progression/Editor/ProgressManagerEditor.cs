using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace archipelaGO.Progression
{
    [CustomEditor(typeof(ProgressManager))]
    public class ProgressManagerEditor : Editor
    {
        private ProgressManager m_progressManager = null;

        private SerializedProperty m_progressCollection = null,
            m_progressIndex = null,
            m_unlockableKeys = null;

        private string[] m_cachedProgressKeys = null;
        
        private void OnEnable()
        {
            m_progressManager = target as ProgressManager;
            m_progressCollection = serializedObject.FindProperty("m_progressCollection");
            m_progressIndex = serializedObject.FindProperty("m_progressIndex");
            m_unlockableKeys = serializedObject.FindProperty("m_unlockableKeys");

            CacheProgressKeys();
        }

        public override void OnInspectorGUI()
        {
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                DrawCustomInspectorGUI();

                if (scope.changed)
                    serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawCustomInspectorGUI()
        {
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(m_progressCollection, new GUIContent("Collection"));

                if (scope.changed)
                    CacheProgressKeys();
            }

            if (m_cachedProgressKeys != null)
            {
                m_progressIndex.intValue = EditorGUILayout.Popup("Key",
                    m_progressIndex.intValue + 1, m_cachedProgressKeys) - 1;
            }

            EditorGUILayout.Space();

            using (new EditorGUI.DisabledGroupScope(!Application.isPlaying))
            {
                if (GUILayout.Button("Unlock"))
                    m_progressManager.UnlockAssignedKeys();
            }

            EditorGUILayout.PropertyField(m_unlockableKeys, true);
        }

        private void CacheProgressKeys()
        {
            m_cachedProgressKeys = null;

            if (m_progressCollection.objectReferenceValue == null)
                return;

            SerializedObject progressCollectionObject = new SerializedObject(m_progressCollection.objectReferenceValue);
            SerializedProperty keys = progressCollectionObject.FindProperty("m_keys");
            m_cachedProgressKeys = new string[keys.arraySize + 1];
            m_cachedProgressKeys[0] = "Pre-unlocked";

            for (int i = 0; i < keys.arraySize; i++)
            {
                SerializedProperty key = keys.GetArrayElementAtIndex(i);
                int keyIndex = (i + 1);

                if (key.objectReferenceValue == null)
                    m_cachedProgressKeys[keyIndex] = $"Key = { i }";

                else
                    m_cachedProgressKeys[keyIndex] = key.objectReferenceValue.name;
            }
        }
    }
}
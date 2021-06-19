using UnityEngine;
using UnityEditor;

namespace archipelaGO.VisualNovel.StorySystem.Narratives
{
    [CustomEditor(typeof(LinearNarrative))]
    public class LinearNarrativeEditor : Editor
    {
        #region Fields
        private SerializedProperty m_characters = null,
            m_dialogues = null;
        
        private string[] m_characterNames = null;
        #endregion


        #region Editor Implementation
        private void OnEnable() => InitializeProperties();

        public override void OnInspectorGUI()
        {
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                DrawCharactersProperty();
                DrawDialoguesProperty();

                if (scope.changed)
                    serializedObject.ApplyModifiedProperties();
            }
        }
        #endregion


        #region Internal Methods
        private void InitializeProperties()
        {
            m_characters = serializedObject.FindProperty("m_characters");
            m_dialogues = serializedObject.FindProperty("m_dialogues");
            m_characterNames = GetCharacterNamesFromCharacterSet();
        }

        private void DrawCharactersProperty()
        {
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(m_characters);

                if (scope.changed)
                    m_characterNames = GetCharacterNamesFromCharacterSet();
            }
        }

        private void DrawDialoguesProperty()
        {
            m_dialogues.isExpanded = EditorGUILayout.Foldout(m_dialogues.isExpanded, m_dialogues.displayName);

            if (!m_dialogues.isExpanded)
                return;

            SerializedProperty arraySize = m_dialogues.FindPropertyRelative("Array.size");
            
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(arraySize);

                for (int i = 0; i < m_dialogues.arraySize; i++)
                {
                    SerializedProperty dialogue = m_dialogues.GetArrayElementAtIndex(i);
                    DrawDialogueProperty(i, dialogue);
                }
            }
        }

        private void DrawDialogueProperty(int dialogueIndex, SerializedProperty dialogue)
        {
            Rect rect = EditorGUILayout.GetControlRect();
            const float FoldoutWidth = 2f;
            float propertyWidth = (rect.width - FoldoutWidth);
            rect.width = FoldoutWidth;
            dialogue.isExpanded = EditorGUI.Foldout(rect, dialogue.isExpanded, GUIContent.none);
            rect.x = rect.xMax;

            rect.width = propertyWidth;
            SerializedProperty characterIndex = dialogue.FindPropertyRelative("m_characterIndex");
            DrawCharacterIndexProperty(rect, dialogueIndex, characterIndex);

            if (!dialogue.isExpanded)
                return;

            SerializedProperty lines = dialogue.FindPropertyRelative("m_lines");

            using (new EditorGUI.IndentLevelScope())
                EditorGUILayout.PropertyField(lines, true);
        }

        private void DrawCharacterIndexProperty(Rect rect, int index, SerializedProperty characterIndex)
        {
            if (m_characters.objectReferenceValue == null)
                EditorGUI.LabelField(rect, $"Dialogue { index }");
            else
                characterIndex.intValue = EditorGUI.Popup(rect, $"Dialogue { index }", characterIndex.intValue, m_characterNames);
        }

        private string[] GetCharacterNamesFromCharacterSet()
        {
            if (m_characters.objectReferenceValue == null)
                return new string[] {};
            
            SerializedObject charactersObject = new SerializedObject(m_characters.objectReferenceValue);
            SerializedProperty characters = charactersObject.FindProperty("m_characters");
            string[] names = new string[characters.arraySize];

            for (int i = 0; i < names.Length; i++)
            {
                SerializedProperty character = characters.GetArrayElementAtIndex(i);
                SerializedProperty characterName = character.FindPropertyRelative("m_name");
                names[i] = characterName.stringValue;
            }

            return names;
        }
        #endregion
    }
}
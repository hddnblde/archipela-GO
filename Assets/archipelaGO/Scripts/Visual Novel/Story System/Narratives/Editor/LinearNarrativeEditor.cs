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
                    DrawDialogueProperty(dialogue);
                }
            }
        }

        private void DrawDialogueProperty(SerializedProperty dialogue)
        {
            dialogue.isExpanded = EditorGUILayout.Foldout(dialogue.isExpanded, dialogue.displayName);

            if (!dialogue.isExpanded)
                return;

            SerializedProperty characterIndex = dialogue.FindPropertyRelative("m_characterIndex"),
                lines = dialogue.FindPropertyRelative("m_lines");

            using (new EditorGUI.IndentLevelScope())
            {
                DrawCharacterIndexProperty(characterIndex);
                EditorGUILayout.PropertyField(lines, true);
            }
        }

        private void DrawCharacterIndexProperty(SerializedProperty characterIndex)
        {
            using (var scope = new EditorGUI.DisabledGroupScope(m_characters.objectReferenceValue == null))
                characterIndex.intValue = EditorGUILayout.Popup("Character", characterIndex.intValue, m_characterNames);
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
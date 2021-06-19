using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace archipelaGO.VisualNovel.StorySystem.Narratives
{
    [CustomEditor(typeof(LinearNarrative))]
    public class LinearNarrativeEditor : Editor
    {
        #region Fields
        private SerializedProperty m_characters = null,
            m_dialogues = null;
        
        private string[] m_characterNames = null;
        private ReorderableList m_reorderableDialogues = null;
        #endregion


        #region Editor Implementation
        private void OnEnable() => InitializeProperties();
        private void OnDisable() => UninitializeReorderableDialogues();

        public override void OnInspectorGUI()
        {
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                DrawCharactersProperty();
                m_reorderableDialogues.DoLayoutList();

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
            InitializeReorderableDialogues();
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
        #endregion


        #region Dialogues Property Drawing Implementation
        private void InitializeReorderableDialogues()
        {
            m_reorderableDialogues = new ReorderableList(serializedObject, m_dialogues);
            m_reorderableDialogues.drawHeaderCallback += DrawDialoguesHeader;
            m_reorderableDialogues.drawElementCallback += DrawDialogueItems;
            m_reorderableDialogues.elementHeightCallback += GetDialogueElementHeight;
        }

        private void UninitializeReorderableDialogues()
        {
            if (m_reorderableDialogues == null)
                return;

            m_reorderableDialogues.drawHeaderCallback -= DrawDialoguesHeader;
            m_reorderableDialogues.drawElementCallback -= DrawDialogueItems;
            m_reorderableDialogues.elementHeightCallback -= GetDialogueElementHeight;
        }

        private void DrawDialogueItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty reorderableDialogue = m_reorderableDialogues.serializedProperty.GetArrayElementAtIndex(index);
            DrawDialogueProperty(rect, index, reorderableDialogue);
        }

        private void DrawDialoguesHeader(Rect rect) =>
            EditorGUI.LabelField(rect, m_dialogues.displayName);

        private float GetDialogueElementHeight(int index)
        {
            SerializedProperty dialogue = m_dialogues.GetArrayElementAtIndex(index);
            float propertyHeight = EditorGUI.GetPropertyHeight(dialogue, true);

            if (dialogue.isExpanded)
            {
                propertyHeight -= (EditorGUIUtility.singleLineHeight +
                    EditorGUIUtility.standardVerticalSpacing);
            }

            return propertyHeight;
        }

        private void DrawDialogueProperty(Rect rect, int dialogueIndex, SerializedProperty dialogue)
        {
            DrawDialogueHeader(ref rect, dialogue, dialogueIndex);

            if (!dialogue.isExpanded)
                return;

            rect.y = rect.yMax + EditorGUIUtility.standardVerticalSpacing;
            rect.height = rect.height - (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            SerializedProperty lines = dialogue.FindPropertyRelative("m_lines");

            using (new EditorGUI.IndentLevelScope())
                EditorGUI.PropertyField(rect, lines, true);
        }

        private void DrawDialogueHeader(ref Rect rect, SerializedProperty dialogue, int dialogueIndex)
        {
            const float HorizontalOffset = 8f;
            float propertyWidth = rect.width;
            rect.width = 0f;
            rect.x += HorizontalOffset;
            rect.height = EditorGUIUtility.singleLineHeight;
            dialogue.isExpanded = EditorGUI.Foldout(rect, dialogue.isExpanded, GUIContent.none);
            rect.width = propertyWidth - HorizontalOffset;
            SerializedProperty characterIndex = dialogue.FindPropertyRelative("m_characterIndex");
            DrawCharacterIndexProperty(rect, dialogueIndex, characterIndex);
        }

        private void DrawCharacterIndexProperty(Rect rect, int index, SerializedProperty characterIndex)
        {
            rect.y += (EditorGUIUtility.standardVerticalSpacing / 2f);

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
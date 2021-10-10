using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using archipelaGO.VisualNovel.DialogueSystem.Elements;

namespace archipelaGO.VisualNovel.StorySystem.Narratives
{
    public class DialoguePostProcessorWindow : EditorWindow
    {
        [SerializeField]
        private WordBank m_wordBank = null;

        [SerializeField]
        private List<LinearNarrative> m_narrativeLines = new List<LinearNarrative>();

        [SerializeField]
        private bool m_automaticallyAssignStageBlocking = true;

        [SerializeField]
        private DialogueCharacter m_mainCharacter = null;

        private SerializedObject m_serializedObject = null;

        private SerializedProperty m_wordBankProperty = null,
            m_narrativeLinesProperty = null,
            m_automaticallyAssignStageBlockingProperty = null,
            m_mainCharacterProperty = null;

        [MenuItem("Utility/Dialogue Post-processor")]
        private static void InitializeWindow()
        {
            DialoguePostProcessorWindow window = EditorWindow.GetWindow<DialoguePostProcessorWindow>();
            window.titleContent = new GUIContent("Dialogue Post-processor");
            window.Show();
        }

        private void Awake() => InitializeSerializedProperties();

        private void OnGUI()
        {
            if (m_serializedObject == null)
                InitializeSerializedProperties();

            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                DrawProperties();

                if (scope.changed)
                    m_serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Process"))
            {
                ProcessWords();
                AssetDatabase.Refresh();
            }
        }

        private void DrawProperties()
        {
            EditorGUILayout.PropertyField(m_automaticallyAssignStageBlockingProperty);

            if (m_automaticallyAssignStageBlockingProperty.boolValue)
            {
                using (new EditorGUI.IndentLevelScope())
                    EditorGUILayout.PropertyField(m_mainCharacterProperty);
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_wordBankProperty);
            EditorGUILayout.PropertyField(m_narrativeLinesProperty);
        }

        private void InitializeSerializedProperties()
        {
            m_serializedObject = new SerializedObject(this);
            m_wordBankProperty = m_serializedObject.FindProperty("m_wordBank");
            m_narrativeLinesProperty = m_serializedObject.FindProperty("m_narrativeLines");
            m_automaticallyAssignStageBlockingProperty = m_serializedObject.FindProperty("m_automaticallyAssignStageBlocking");
            m_mainCharacterProperty = m_serializedObject.FindProperty("m_mainCharacter");
        }

        private void ProcessWords()
        {
            List<string> orderedWords = GetWordCollectionFromBank();

            if (orderedWords == null)
                return;
            
            for (int i = 0; i < m_narrativeLinesProperty.arraySize; i++)
            {
                SerializedProperty narrativeLine = m_narrativeLinesProperty.GetArrayElementAtIndex(i);
                ProcessWordsOnNarrativeLine(narrativeLine, orderedWords);
            }
        }

        private void ProcessWordsOnNarrativeLine(SerializedProperty narrativeLine, List<string> words)
        {
            SerializedObject narrativeLineObject = new SerializedObject(narrativeLine.objectReferenceValue);

            SerializedProperty dialoguesProperty = narrativeLineObject.FindProperty("m_dialogues"),
                characterSetProperty = narrativeLineObject.FindProperty("m_characters");
            
            SerializedProperty characters = GetCharactersFromCharacterSet(characterSetProperty);

            for (int i = 0; i < dialoguesProperty.arraySize; i++)
            {
                SerializedProperty dialogueProperty = dialoguesProperty.GetArrayElementAtIndex(i);

                if (m_automaticallyAssignStageBlockingProperty.boolValue)
                    ProcessBlocking(characters, dialogueProperty, m_mainCharacterProperty);

                ProcessDialogueLine(dialogueProperty, words);
            }

            narrativeLineObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private SerializedProperty GetCharactersFromCharacterSet(SerializedProperty characterSet)
        {
            if (characterSet.objectReferenceValue == null)
                return null;
            
            SerializedObject serializedObject = new SerializedObject(characterSet.objectReferenceValue);

            return serializedObject.FindProperty("m_characters");
        }

        private void ProcessDialogueLine(SerializedProperty dialogueLine, List<string> words)
        {
            SerializedProperty lines = dialogueLine.FindPropertyRelative("m_lines");
            

            for (int i = 0; i < lines.arraySize; i++)
            {
                SerializedProperty line = lines.GetArrayElementAtIndex(i);
                SerializedProperty text = line.FindPropertyRelative("m_text");
                ProcessText(text, words);
                RemoveNewLineFromText(text);
            }
        }

        private void ProcessBlocking(SerializedProperty characters, SerializedProperty dialogueLine, SerializedProperty mainCharacter)
        {
            SerializedProperty characterIndex = dialogueLine.FindPropertyRelative("m_characterIndex");

            if (characterIndex.intValue < 0 || characterIndex.intValue >= characters.arraySize)
                return;

            SerializedProperty character = characters.GetArrayElementAtIndex(characterIndex.intValue);
            bool isMainCharacter = (character.objectReferenceValue == mainCharacter.objectReferenceValue);
            
            SerializedProperty blocking = dialogueLine.FindPropertyRelative("m_blocking");
            blocking.enumValueIndex = (int)(isMainCharacter ? DialogueCharacterBlocking.StageLeft : DialogueCharacterBlocking.StageRight);
        }

        private void ProcessText(SerializedProperty textProperty, List<string> words)
        {
            string textValue = textProperty.stringValue;
            textValue = textValue.Replace("{", string.Empty).Replace("}", string.Empty);
            const string Prefix = @"(?<![\w\d])", Suffix = @"(?![\w\d])";

            foreach (string word in words)
            {
                string pattern = $@"{ Prefix }{ word }{ Suffix }";
                Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
                MatchCollection matches = regex.Matches(textValue);

                foreach (Match match in matches)
                    textValue = textValue.Replace(match.Value, $"{{{match.Value}}}");
            }

            textProperty.stringValue = textValue;
        }

        private void RemoveNewLineFromText(SerializedProperty textProperty) =>
            textProperty.stringValue = RemoveLineEndings(textProperty.stringValue);
        
        private string RemoveLineEndings(string stringValue)
        {
            if(string.IsNullOrEmpty(stringValue))
                return stringValue;

            string lineSeparator = ((char) 0x2028).ToString();
            string paragraphSeparator = ((char)0x2029).ToString();
            string whiteSpace = " ";

            return stringValue.Replace("\r\n", whiteSpace)
                        .Replace("\n", whiteSpace)
                        .Replace("\r", whiteSpace)
                        .Replace(lineSeparator, whiteSpace)
                        .Replace(paragraphSeparator, whiteSpace);
        }

        private List<string> GetWordCollectionFromBank()
        {
            if (m_wordBank == null)
                return null;

            List<string> collection = new List<string>();
            
            for (int i = 0; i < m_wordBank.wordCount; i++)
            {
                List<string> keywords = m_wordBank.GetWord(i).GetKeywords();
                collection.AddRange(keywords);
            }

            return collection.OrderByDescending(s => s).ToList();
        }
    }
}
using UnityEngine;
using UnityEditor;

namespace archipelaGO.Crossword
{
    [CustomEditor(typeof(CrosswordConfig))]
    public class CrosswordConfigEditor : Editor
    {
        #region Fields
        private SerializedProperty m_wordBank = null,
            m_gridSize = null,
            m_puzzlePieces = null;
        
        private GUIContent[] m_cachedWords = null;
        private int m_longestCharacterCount = 0;
        #endregion


        #region Editor Implementation
        private void OnEnable()
        {
            m_wordBank = serializedObject.FindProperty("m_wordBank");
            m_gridSize = serializedObject.FindProperty("m_gridSize");
            m_puzzlePieces = serializedObject.FindProperty("m_puzzlePieces");
            CacheWordsFromBank();
            Undo.undoRedoPerformed += OnUndoRedoPerformed;
        }

        private void OnDisable() => Undo.undoRedoPerformed -= OnUndoRedoPerformed;

        public override void OnInspectorGUI()
        {
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                DrawCustomGUI();

                if (scope.changed)
                    serializedObject.ApplyModifiedProperties();
            }
        }
        #endregion


        #region Internal Methods
        private void OnUndoRedoPerformed() => serializedObject.Update();

        private void DrawCustomGUI()
        {
            DrawWordBankProperty();
            DrawGridSizeProperty();
            EditorGUILayout.Space();
            DrawPuzzlePiecesProperty();
        }

        private void DrawWordBankProperty()
        {
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(m_wordBank);

                if (scope.changed)
                    CacheWordsFromBank();
            }

        }

        private void DrawGridSizeProperty()
        {
            Rect rect = EditorGUILayout.GetControlRect();
            rect = EditorGUI.PrefixLabel(rect, new GUIContent(m_gridSize.displayName));
            rect.width /= 2f;

            const float Spacing = 8f;
            const int MinCellCount = 3;
            rect.width -= Spacing / 2f;

            int column = m_gridSize.vector2IntValue.x;
            int row = m_gridSize.vector2IntValue.y;
            float defaultLabelWidth = EditorGUIUtility.labelWidth;

            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                EditorGUIUtility.labelWidth = 32f;
                column = Mathf.Max(EditorGUI.IntField(rect, "Col", column), MinCellCount);
                rect.x = rect.xMax + Spacing;

                EditorGUIUtility.labelWidth = 32f;
                row = Mathf.Max(EditorGUI.IntField(rect, "Row", row), MinCellCount);
                
                if (scope.changed)
                    m_gridSize.vector2IntValue = new Vector2Int(column, row);
            }

            EditorGUIUtility.labelWidth = defaultLabelWidth;

            if (m_wordBank.objectReferenceValue == null)
                return;
            
            bool longestWordCanFit = (row >= m_longestCharacterCount ||
                column >= m_longestCharacterCount);

            if (longestWordCanFit)
                return;

            EditorGUILayout.HelpBox($"Longest character count: { m_longestCharacterCount }",
                MessageType.Warning);
        }

        private void DrawPuzzlePiecesProperty()
        {
            m_puzzlePieces.isExpanded =
                EditorGUILayout.Foldout(m_puzzlePieces.isExpanded,
                m_puzzlePieces.displayName);

            if (!m_puzzlePieces.isExpanded)
                return;

            SerializedProperty arraySize =
                m_puzzlePieces.FindPropertyRelative("Array.size");

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(arraySize);

                for (int i = 0; i < m_puzzlePieces.arraySize; i++)
                {
                    SerializedProperty puzzlePiece =
                        m_puzzlePieces.GetArrayElementAtIndex(i);

                    DrawPuzzlePieceProperty(puzzlePiece);
                }
            }
        }

        private void DrawPuzzlePieceProperty(SerializedProperty puzzlePiece)
        {
            puzzlePiece.isExpanded = EditorGUILayout.Foldout(puzzlePiece.isExpanded,
                puzzlePiece.displayName);
            
            if (!puzzlePiece.isExpanded)
                return;

            SerializedProperty direction = puzzlePiece.FindPropertyRelative("m_direction"),
                position = puzzlePiece.FindPropertyRelative("m_position"),
                wordBankIndex = puzzlePiece.FindPropertyRelative("m_wordBankIndex");

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(direction);

                if (m_cachedWords != null && m_cachedWords.Length > 0)
                {
                    using (var scope = new EditorGUI.ChangeCheckScope())
                    {
                        wordBankIndex.intValue =
                            EditorGUILayout.Popup(new GUIContent("Word"),
                            wordBankIndex.intValue, m_cachedWords);

                        if (scope.changed)
                            VerifyLongestCharacterCountFromPuzzlePieces();
                    }
                }

                EditorGUILayout.PropertyField(position);
            }
        }

        private void CacheWordsFromBank()
        {
            m_longestCharacterCount = 0;

            if (m_wordBank.objectReferenceValue == null)
            {
                m_cachedWords = null;
                return;
            }

            SerializedObject wordBankObject =
                new SerializedObject(m_wordBank.objectReferenceValue);

            SerializedProperty words = wordBankObject.FindProperty("m_words");
            m_cachedWords = new GUIContent[words.arraySize];

            for (int i = 0; i < words.arraySize; i++)
            {
                SerializedProperty word = words.GetArrayElementAtIndex(i),
                    title = word.FindPropertyRelative("m_title"),
                    description = word.FindPropertyRelative("m_description");

                m_cachedWords[i] = new GUIContent(title.stringValue.ToUpper(), description.stringValue);
            }

            VerifyLongestCharacterCountFromPuzzlePieces();
        }

        private void VerifyLongestCharacterCountFromPuzzlePieces()
        {
            m_longestCharacterCount = 0;

            if (m_cachedWords == null)
                return;

            for (int i = 0; i < m_puzzlePieces.arraySize; i++)
            {
                SerializedProperty puzzlePiece = m_puzzlePieces.GetArrayElementAtIndex(i),
                    wordBankIndex = puzzlePiece.FindPropertyRelative("m_wordBankIndex");
                
                int index = wordBankIndex.intValue;

                if (index < 0 || index >= m_cachedWords.Length)
                    continue;
                
                GUIContent word = m_cachedWords[index];

                if (word.text.Length > m_longestCharacterCount)
                    m_longestCharacterCount = word.text.Length;
            }
        }
        #endregion
    }
}
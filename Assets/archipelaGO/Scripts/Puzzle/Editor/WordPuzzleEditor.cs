using UnityEngine;
using UnityEditor;
using Word = archipelaGO.WordBank.Word;

namespace archipelaGO.Puzzle
{
    [CustomEditor(typeof(WordPuzzle))]
    public class WordPuzzleEditor : Editor
    {
        #region Fields
        private SerializedProperty m_timeLimit = null,
            m_passingScore = null,
            m_hint = null,
            m_endMessage = null,
            m_failMessage = null,
            m_puzzleType = null,
            m_wordBank = null,
            m_gridSize = null,
            m_puzzlePieces = null;

        private GUIContent[] m_cachedWords = null;
        private int m_longestCharacterCount = 0;
        private CrosswordCharacter[,] m_cachedCrossword = null;
        private GUIStyle m_crosswordGUIStyle = null;

        private readonly string[] CrosswordDirections = { "Across", "Down" };
        private readonly string[] WordHuntDirections = { "→", "↓", "↗", "↘", "←", "↑", "↙", "↖" };
        private const int CrosswordIndex = 0, WordHuntIndex = 1;
        #endregion


        #region Data Structure
        private class CrosswordCharacter
        {
            private char m_character;

            private bool m_initialized = false,
                m_collided = false;
            
            public bool GetCharacter(out char character)
            {
                character = (m_collided ? '?' : m_character);
                return !m_collided;
            }

            public void SetCharacter(char character)
            {
                if (m_initialized)
                    m_collided = (m_character != character);

                m_character = character;
                m_initialized = true;
            }
        }

        private class ColorScope : GUI.Scope
        {
            public ColorScope(Color color)
            {
                m_defaultGUIColor = GUI.color;
                GUI.color = color;
            }

            private Color m_defaultGUIColor;

            protected override void CloseScope() =>
                GUI.color = m_defaultGUIColor;
        }
        #endregion


        #region Editor Implementation
        private void OnEnable()
        {
            m_timeLimit = serializedObject.FindProperty("m_timeLimit");
            m_passingScore = serializedObject.FindProperty("m_passingScore");
            m_hint = serializedObject.FindProperty("m_hint");
            m_endMessage = serializedObject.FindProperty("m_endMessage");
            m_failMessage = serializedObject.FindProperty("m_failMessage");
            m_puzzleType = serializedObject.FindProperty("m_puzzleType");
            m_wordBank = serializedObject.FindProperty("m_wordBank");
            m_gridSize = serializedObject.FindProperty("m_gridSize");
            m_puzzlePieces = serializedObject.FindProperty("m_puzzlePieces");
            CacheWordsFromBank();
            CacheCrossword();
            Undo.undoRedoPerformed += OnUndoRedoPerformed;
        }

        private void OnDisable() =>
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;

        public override void OnInspectorGUI()
        {
            if (m_crosswordGUIStyle == null)
                InitializeCrosswordGUIStyle();

            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                DrawCustomGUI();

                if (scope.changed)
                    serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.Space();
            VisualizeCrossword();
        }
        #endregion


        #region Internal Methods
        private void OnUndoRedoPerformed()
        {
            serializedObject.Update();
            CacheCrossword();
        }

        private void InitializeCrosswordGUIStyle()
        {
            m_crosswordGUIStyle = new GUIStyle(EditorStyles.helpBox);
            m_crosswordGUIStyle.alignment = TextAnchor.MiddleCenter;
            m_crosswordGUIStyle.fontStyle = FontStyle.Bold;
        }

        private void DrawCustomGUI()
        {
            EditorGUILayout.PropertyField(m_timeLimit);
            EditorGUILayout.PropertyField(m_passingScore);
            EditorGUILayout.PropertyField(m_hint);
            EditorGUILayout.PropertyField(m_endMessage);
            EditorGUILayout.PropertyField(m_failMessage);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_puzzleType);
            DrawWordBankProperty();
            DrawGridSizeProperty();
            EditorGUILayout.Space();

            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                DrawPuzzlePiecesProperty();

                if (scope.changed)
                    CacheCrossword();
            }
        }

        private void DrawWordBankProperty()
        {
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(m_wordBank);

                if (scope.changed)
                {
                    CacheWordsFromBank();
                    CacheCrossword();
                }
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

                    DrawPuzzlePieceProperty(i, puzzlePiece);
                }
            }
        }

        private void DrawPuzzlePieceProperty(int index, SerializedProperty puzzlePiece)
        {
            Rect rect = EditorGUILayout.GetControlRect();
            float propertyWidth = (rect.width - EditorGUIUtility.labelWidth);
            rect.width = EditorGUIUtility.labelWidth;

            puzzlePiece.isExpanded = EditorGUI.Foldout(rect, puzzlePiece.isExpanded,
                $"Word { index }");

            rect.x = rect.xMax;
            rect.width = propertyWidth;
            
            SerializedProperty wordBankIndex = puzzlePiece.
                FindPropertyRelative("m_wordBankIndex");

            if (m_cachedWords != null && m_cachedWords.Length > 0)
            {
                using (var scope = new EditorGUI.ChangeCheckScope())
                {
                    using (new EditorGUI.IndentLevelScope(-EditorGUI.indentLevel))
                    {
                        wordBankIndex.intValue = EditorGUI.
                            Popup(rect, wordBankIndex.intValue, m_cachedWords);
                    }

                    if (scope.changed)
                        VerifyLongestCharacterCountFromPuzzlePieces();
                }
            }
            
            if (!puzzlePiece.isExpanded)
                return;

            SerializedProperty direction = puzzlePiece.FindPropertyRelative("m_direction"),
                position = puzzlePiece.FindPropertyRelative("m_position");


            using (new EditorGUI.IndentLevelScope())
            {
                if (m_puzzleType.enumValueIndex == WordHuntIndex)
                {
                    SerializedProperty hintColor = puzzlePiece.FindPropertyRelative("m_hintColor");
                    EditorGUILayout.PropertyField(hintColor);
                }

                DrawDirectionProperty(direction);
                EditorGUILayout.PropertyField(position);
            }
        }

        private void DrawDirectionProperty(SerializedProperty direction)
        {
            string[] directionsContext = (m_puzzleType.enumValueIndex == CrosswordIndex ?
                CrosswordDirections : WordHuntDirections);

            direction.intValue = EditorGUILayout.Popup(direction.displayName,
                direction.intValue, directionsContext);
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
                    description = word.FindPropertyRelative("m_definition");

                string tooltip = (string.IsNullOrEmpty(description.stringValue) ||
                    string.IsNullOrWhiteSpace(description.stringValue) ?
                    "No description." : description.stringValue);

                m_cachedWords[i] = new GUIContent(title.stringValue.ToUpper(), tooltip);
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
        
        private void VisualizeCrossword()
        {
            Rect rect = GenerateCrosswordRect(m_gridSize.vector2IntValue);
            DrawCells(rect, m_gridSize.vector2IntValue, m_cachedCrossword);
        }

        private void DrawCells(Rect rect, Vector2Int grid, CrosswordCharacter[,] crossword)
        {
            if (crossword == null || crossword.GetLength(0) != grid.x || crossword.GetLength(1) != grid.y)
                return;

            const float Spacing = 4f;
            float yPivot = rect.position.y;
            rect.size = new Vector2(rect.width / grid.x, rect.height / grid.y) - (Vector2.one * Spacing);

            for (int column = 0; column < grid.x; column++)
            {
                rect.y = yPivot;

                for (int row = 0; row < grid.y; row++)
                {
                    CrosswordCharacter character = crossword[column, row];
                    char characterValue;
                    bool collided = !character.GetCharacter(out characterValue);
                    Color boxColor = (collided ? Color.red : GUI.color);

                    using (new ColorScope(boxColor))
                        GUI.Box(rect, $"{ characterValue }", m_crosswordGUIStyle);

                    rect.y = rect.yMax + Spacing;
                }

                rect.x = rect.xMax + Spacing;
            }
        }

        private Rect GenerateCrosswordRect(Vector2Int size)
        {
            const float GridSize = 30f;
            float height = (GridSize * size.y);

            return EditorGUILayout.GetControlRect(GUILayout.Height(height));
        }

        private CrosswordCharacter[,] GenerateEmptyCrossword(Vector2Int gridSize)
        {
            CrosswordCharacter[,] crossword = new CrosswordCharacter[gridSize.x, gridSize.y];

            for (int column = 0; column < gridSize.x; column++)
            {
                for (int row = 0; row < gridSize.y; row++)
                    crossword[column, row] = new CrosswordCharacter();
            }

            return crossword;
        }

        private void CacheCrossword()
        {
            Vector2Int gridSize = m_gridSize.vector2IntValue;
            m_cachedCrossword = GenerateEmptyCrossword(gridSize);
            WordBank wordBank = m_wordBank.objectReferenceValue as WordBank;

            if (wordBank == null)
            {
                Debug.LogWarning("Failed to cache crossword because Word Bank isn't assigned.");
                return;
            }

            for (int i = 0; i < m_puzzlePieces.arraySize; i++)
            {
                SerializedProperty puzzlePiece = m_puzzlePieces.GetArrayElementAtIndex(i);

                SerializedProperty wordBankIndex = puzzlePiece.FindPropertyRelative("m_wordBankIndex"),
                    direction = puzzlePiece.FindPropertyRelative("m_direction"),
                    position = puzzlePiece.FindPropertyRelative("m_position");
                
                Word word = wordBank.GetWord(wordBankIndex.intValue);
                string wordString = word.title.ToUpper();
                int directionValue = direction.intValue;
                Vector2Int positionValue = position.vector2IntValue;

                for (int characterIndex = 0; characterIndex < wordString.Length; characterIndex++)
                {
                    (int column, int row) cell = WordHuntPuzzleModule.CalculateCellPosition(positionValue, directionValue, wordString.Length, characterIndex);

                    if (cell.column >= m_cachedCrossword.GetLength(0) || cell.row >= m_cachedCrossword.GetLength(1) ||
                        cell.column < 0 || cell.row < 0)
                        break;

                    m_cachedCrossword[cell.column, cell.row].SetCharacter(wordString[characterIndex]);
                }
            }
        }
        #endregion
    }
}
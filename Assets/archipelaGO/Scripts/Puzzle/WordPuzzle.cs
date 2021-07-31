using System.Collections.Generic;
using UnityEngine;
using Word = archipelaGO.WordBank.Word;

namespace archipelaGO.Puzzle
{
    
    [System.Serializable]
    public enum CrosswordDirection
    {
        Across = 0,
        Down = 1
    }

    [System.Serializable]
    public enum WordHuntDirection
    {
        Vertical = 0,
        Horizontal = 1,
        DiagonalUp = 2,
        DiagonalDown = 3
    }

    [CreateAssetMenu(fileName = "Word Puzzle", menuName = "archipelaGO/Word Puzzle", order = 1)]
    public class WordPuzzle : ScriptableObject
    {
        #region Fields
        [SerializeField] 
        private WordBank m_wordBank = null;

        [SerializeField]
        private Vector2Int m_gridSize = Vector2Int.one * 7;

        [SerializeField, NonReorderable]
        private List<PuzzlePiece> m_puzzlePieces = new List<PuzzlePiece>();
        #endregion


        #region Properties
        public WordBank wordBank => m_wordBank;

        public Vector2Int gridSize => m_gridSize;
        public int wordSize => m_puzzlePieces.Count;
        #endregion


        #region Methods
        public GridWord GetWord(int index)
        {
            if (index < 0 || index >= wordSize)
                return null;

            return new GridWord(m_puzzlePieces[index], m_wordBank);
        }
        #endregion


        #region Data Structure
        [System.Serializable]
        public struct PuzzlePiece
        {
            [SerializeField]
            private int m_direction;

            [SerializeField]
            private Vector2Int m_position;

            [SerializeField]
            private int m_wordBankIndex;

            public int wordBankIndex => m_wordBankIndex;
            public int direction => m_direction;
            public Vector2Int position => m_position;
        }

        public class GridWord
        {
            public GridWord (PuzzlePiece gridWord, WordBank wordBank)
            {
                m_direction = gridWord.direction;
                m_position = gridWord.position;
                m_word = wordBank.GetWord(gridWord.wordBankIndex);
            }

            #region Fields
            private int m_direction = 0;
            private Vector2Int m_position = Vector2Int.zero;
            private Word m_word = new Word();
            #endregion


            #region Properties
            public int direction => m_direction;
            public Vector2Int position => m_position;
            public Word word => m_word;
            #endregion
        }
        #endregion
    }
}
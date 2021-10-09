using System.Collections.Generic;
using UnityEngine;
using Word = archipelaGO.WordBank.Word;
using GameModuleConfig = archipelaGO.Game.GameModuleConfig;

namespace archipelaGO.Puzzle
{
    public enum PuzzleType
    {
        Crossword = 0,
        WordHunt = 1
    }

    [CreateAssetMenu(fileName = "Word Puzzle", menuName = "archipelaGO/Game Module/Word Puzzle", order = 1)]
    public class WordPuzzle : GameModuleConfig
    {
        #region Fields
        [SerializeField]
        private PuzzleType m_puzzleType = PuzzleType.Crossword;

        [SerializeField] 
        private WordBank m_wordBank = null;

        [SerializeField]
        private Vector2Int m_gridSize = Vector2Int.one * 7;

        [SerializeField, NonReorderable]
        private List<PuzzlePiece> m_puzzlePieces = new List<PuzzlePiece>();
        #endregion


        #region Properties
        public PuzzleType puzzleType => m_puzzleType;
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
            private Color m_hintColor;

            [SerializeField]
            private int m_wordBankIndex;

            public int wordBankIndex => m_wordBankIndex;
            public int direction => m_direction;
            public Vector2Int position => m_position;
            public Color hintColor => m_hintColor;
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
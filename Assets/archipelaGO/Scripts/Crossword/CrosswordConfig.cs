using System.Collections.Generic;
using UnityEngine;

namespace achipelaGO.Crossword
{
    [CreateAssetMenu(fileName = "Crossword", menuName = "archipelaGO/Crossword", order = 1)]
    public class CrosswordConfig : ScriptableObject
    {
        #region Fields
        [SerializeField] 
        private WordBank m_wordBank = null;

        [SerializeField]
        private Vector2Int m_gridSize = Vector2Int.one * 7;

        [SerializeField]
        private List<GridWord> m_gridWords = new List<GridWord>();
        #endregion


        #region Properties
        public WordBank wordBank => m_wordBank;

        public Vector2Int gridSize => m_gridSize;
        public int wordSize => m_gridWords.Count;
        #endregion


        #region Methods
        public GridWord GetWord(int index)
        {
            if (index < 0 || index >= wordSize)
                return null;

            return m_gridWords[index];
        }
        #endregion


        #region Data Structure
        [System.Serializable]
        public class GridWord
        {
            [SerializeField]
            private Direction m_direction;

            [SerializeField]
            private Vector2Int m_position;

            [SerializeField]
            private int m_wordBankIndex;

            public int wordBankIndex => m_wordBankIndex;
            public Direction direction => m_direction;
            public Vector2Int position => m_position;
        }

        [System.Serializable]
        public enum Direction
        {
            Across,
            Down
        }
        #endregion
    }
}
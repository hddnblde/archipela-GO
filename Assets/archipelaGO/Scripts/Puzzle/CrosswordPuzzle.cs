using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GridDirection = archipelaGO.Puzzle.WordPuzzle.Direction;
using GridWord = archipelaGO.Puzzle.WordPuzzle.GridWord;

namespace archipelaGO.Puzzle
{
    public class CrosswordPuzzle : WordPuzzleController
    {
        #region Fields
        [SerializeField]
        private InputField m_answerField = null;
        #endregion


        #region Data Structure
        private class CrosswordPuzzlePiece : PuzzlePiece
        {
            public CrosswordPuzzlePiece(string word, WordCell[] cells) : base(word, cells) {}

            public override void Reveal()
            {
                if (m_cells == null)
                    return;

                foreach (WordCell cell in m_cells)
                {
                    if (cell != null)
                        cell.SetState(WordCell.State.CharacterShown);
                }
            }
        }

        private class CrosswordHint : WordHint
        {
            public CrosswordHint (int order, GridDirection direction, string description) =>
                (m_order, m_direction, m_description) = (order, direction, description);

            private int m_order;
            private GridDirection m_direction;
            private string m_description;

            public GridDirection direction => m_direction;
            public override string GetHint() => $"{ m_order }. { m_description }";
        }
        #endregion


        #region MonoBehaviour Implementation
        protected override void Awake()
        {
            base.Awake();

            if (m_answerField != null)
                m_answerField.onEndEdit.AddListener(OnSubmitAnswer);
        }

        private void OnDestroy()
        {
            if (m_answerField != null)
                m_answerField.onEndEdit.RemoveListener(OnSubmitAnswer);
        }
        #endregion


        #region Word Puzzle Implementation
        protected override void AssignCharacterToCell(char character, int characterIndex, int puzzleIndex, WordCell cell)
        {
            bool isFirstCharacter = (characterIndex == 0);

            if (isFirstCharacter)
                cell.SetAsCharacterTileWithIndex(character, puzzleIndex);

            else
                cell.SetAsCharacterTile(character);

            cell.SetState(WordCell.State.CharacterHidden);
        }

        protected override void InitializeCell(int column, int row, WordCell cell)
        {
            base.InitializeCell(column, row, cell);
            cell.SetAsEmptyTile(true);
        }

        protected override PuzzlePiece GeneratePuzzlePiece(string word, WordCell[] cells) =>
            new CrosswordPuzzlePiece(word, cells);

        protected override WordHint GenerateHint(int order, GridWord gridWord) =>
            new CrosswordHint(order, gridWord.direction, gridWord.word.description);

        protected override string GenerateHintText(List<WordHint> hints)
        {
            StringBuilder across = new StringBuilder();
            StringBuilder down = new StringBuilder();

            foreach (WordHint hint in hints)
            {
                if (!(hint is CrosswordHint))
                    continue;
                
                CrosswordHint crosswordHint = (hint as CrosswordHint);

                if (crosswordHint.direction == GridDirection.Across)
                    across.AppendLine(crosswordHint.GetHint());
                else
                    down.AppendLine(crosswordHint.GetHint());
            }

            return $"<b>ACROSS</b>\n<i>{ across }</i>\n<b>DOWN</b>\n<i>{ down }</i>";
        }
        #endregion


        #region Answering Implementation
        private void OnSubmitAnswer(string answer)
        {
            ClearAnswerField();
            VerifyAnswer(answer);
        }

        private void ClearAnswerField()
        {
            if (m_answerField != null)
                m_answerField.text = string.Empty;
        }

        protected override void OnPuzzleCompleted()
        {
            Debug.Log("Crossword Puzzle: Puzzle finished! You have won!!");
            
            if (m_answerField == null)
                return;

            m_answerField.interactable = false;
            m_answerField.text = "Puzzle finished!";
        }
        #endregion
    }
}
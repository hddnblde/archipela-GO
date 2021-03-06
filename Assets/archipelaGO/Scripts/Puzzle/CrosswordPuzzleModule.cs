using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GridWord = archipelaGO.Puzzle.WordPuzzle.GridWord;
using Word = archipelaGO.WordBank.Word;
using CellState = archipelaGO.Puzzle.WordCell.State;

namespace archipelaGO.Puzzle
{
    public class CrosswordPuzzleModule : WordPuzzleModule
    {
        #region Fields
        [SerializeField]
        private float m_answerDelay = 0.75f;

        [SerializeField]
        private bool m_revealFirstLetterOfAllWords = false;

        [SerializeField]
        private float m_revealAnimationInterval = 0.12f;

        [SerializeField]
        private InputField m_answerField = null;
        #endregion


        #region Data Structure
        private class CrosswordPuzzlePiece : PuzzlePiece
        {
            private bool m_animationFinished = false;

            public override bool revealAnimationFinished => m_animationFinished;

            public CrosswordPuzzlePiece(WordPuzzleModule puzzleModule, float revealAnimationInterval,
                string word, WordCell[] cells) : base(puzzleModule, word, cells, revealAnimationInterval) {}

            protected override IEnumerator OnRevealPuzzle()
            {
                m_animationFinished = false;

                if (m_cells == null)
                    goto End;

                foreach (WordCell cell in m_cells)
                {
                    if (cell == null)
                        continue;

                    cell.SetState(CellState.CharacterRevealed);
                    yield return new WaitForSeconds(revealAnimationInterval);
                }

                End:
                m_animationFinished = true;
            }
        }

        private class CrosswordHint : WordHint
        {
            public CrosswordHint (Word word, WordHintType hintType, int order, int direction) : base(word, hintType)=>
                (m_order, m_direction) = (order, direction);

            private int m_order;
            private int m_direction;

            public int direction => m_direction;
            public override string GetHint() => $"{ m_order }. { base.GetHint() }";
        }
        #endregion


        #region Game Module Implementation
        protected override void OnInitialize()
        {
            if (m_answerField != null)
                m_answerField.onEndEdit.AddListener(OnSubmitAnswer);

            base.OnInitialize();
        }
        #endregion


        #region MonoBehaviour Implementation
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

            CellState beginState = (isFirstCharacter && m_revealFirstLetterOfAllWords) ?
                CellState.CharacterShown : CellState.CharacterHidden;

            cell.SetState(beginState);
        }

        protected override void InitializeCell(int column, int row, WordCell cell)
        {
            base.InitializeCell(column, row, cell);
            cell.SetAsEmptyTile(true);
        }

        protected override PuzzlePiece GeneratePuzzlePiece(GridWord gridWord, WordCell[] cells) =>
            new CrosswordPuzzlePiece(this, m_revealAnimationInterval, gridWord.word.title, cells);

        protected override WordHint GenerateHint(int order, GridWord gridWord) =>
            new CrosswordHint(gridWord.word, gridWord.hintType, order, gridWord.direction);

        protected override string GenerateHintText(List<WordHint> hints)
        {
            StringBuilder across = new StringBuilder();
            StringBuilder down = new StringBuilder();

            foreach (WordHint hint in hints)
            {
                if (!(hint is CrosswordHint))
                    continue;
                
                const int AcrossDirection = 0;
                CrosswordHint crosswordHint = (hint as CrosswordHint);

                if (crosswordHint.direction == AcrossDirection)
                    across.AppendLine(crosswordHint.GetHint());
                else
                    down.AppendLine(crosswordHint.GetHint());
            }

            return $"<align=center><b>ACROSS</b></align>\n<align=left>{ across }</align>\n<align=center><b>DOWN</b></align>\n<align=left>{ down }</align>";
        }

        protected override (int column, int row) GetCellPosition(Vector2Int anchor, int direction, int length, int currentPosition) =>
            CalculateCellPosition(anchor, direction, length, currentPosition);

        public static (int column, int row) CalculateCellPosition(Vector2Int anchor, int direction, int length, int currentPosition)
        {
            const int AcrossDirection = 0, DownDirection = 1;
            direction = Mathf.Clamp(direction, AcrossDirection, DownDirection);
            int columnOffset = (direction == AcrossDirection ? currentPosition : 0);
            int rowOffset = (direction == DownDirection ? currentPosition : 0);

            int column = (anchor.x + columnOffset);
            int row = (anchor.y + rowOffset);

            return (column, row);
        }
        #endregion


        #region Answering Implementation
        private void OnSubmitAnswer(string answer) =>
            StartCoroutine(SubmitAnswerRoutine(answer));

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

        private IEnumerator SubmitAnswerRoutine(string answer)
        {
            m_answerField.interactable = false;

            if (Application.isMobilePlatform)
                yield return new WaitForSeconds(m_answerDelay);

            m_answerField.interactable = true;
            ClearAnswerField();
            VerifyAnswer(answer);
        }

        #endregion


        #if ARCHIPELAGO_DEBUG_MODE
        public override IEnumerator Debug_Autoplay()
        {
            m_answerField.interactable = false;
            yield return base.Debug_Autoplay();
            m_answerField.interactable = true;
        }
        #endif
    }
}
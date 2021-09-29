using System.Text;
using System.Collections.Generic;
using UnityEngine;
using GridWord = archipelaGO.Puzzle.WordPuzzle.GridWord;
using CursorEvent = archipelaGO.Puzzle.WordCell.CursorEvent;
using CursorState = archipelaGO.Puzzle.WordCell.CursorState;

namespace archipelaGO.Puzzle
{
    public class WordHuntPuzzleModule : WordPuzzleModule
    {
        #region Fields
        private const string LettersInTheAlphabet =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private bool m_cellDragBegan = false;

        private Vector2Int m_cellDragStartPosition = Vector2Int.zero,
            m_cellDragEndPosition = Vector2Int.zero;
        
        private string m_pendingWord = string.Empty;
        #endregion


        #region Data Structure
        private class WordHuntPuzzlePiece : PuzzlePiece
        {
            public WordHuntPuzzlePiece(string word, WordCell[] cells) : base(word, cells) {}

            public override void Reveal()
            {
                if (m_cells == null)
                    return;

                foreach (WordCell cell in m_cells)
                {
                    if (cell != null)
                        cell.SetAsHighlighted(true);
                }
            }
        }

        private class WordHuntHint : WordHint
        {
            public WordHuntHint (int order, string word, bool guessed)
            {
                m_order = order;
                m_word = word;

                if (guessed)
                    m_word = $"<u>{ m_word }</u>";
            }

            private int m_order;
            private string m_word;

            public override string GetHint() => $"{ m_word.ToUpper() }";
        }
        #endregion


        #region Word Puzzle Implementation
        protected override void OnPuzzleCompleted()
        {
            Debug.Log("Word Hunt Puzzle: Puzzle finished! You have won!!");
        }

        protected override void AssignCharacterToCell(char character, int characterIndex, int puzzleIndex, WordCell cell)
        {
            cell.SetAsCharacterTile(character);
            cell.SetState(WordCell.State.CharacterShown);
        }

        protected override void InitializeCell(int column, int row, WordCell cell)
        {
            base.InitializeCell(column, row, cell);
            cell.SetAsEmptyTile(false);
            cell.SetAsCharacterTile(GenerateRandomCharacter());
            cell.OnCursorEventInvoked += GenerateCursorEventHandler(column, row);
        }

        protected override PuzzlePiece GeneratePuzzlePiece(string word, WordCell[] cells) =>
            new WordHuntPuzzlePiece(word, cells);

        protected override WordHint GenerateHint(int order, GridWord gridWord)
        {
            bool guessed = AlreadyAnswered(gridWord.word.title);

            return new WordHuntHint(order, gridWord.word.title, guessed);
        }

        protected override string GenerateHintText(List<WordHint> hints)
        {
            StringBuilder wordHuntHints = new StringBuilder();

            foreach (WordHint hint in hints)
                wordHuntHints.AppendLine(hint.GetHint());

            return $"<b>Words</b>\n{ wordHuntHints }";
        }

        protected override (int column, int row) GetCellPosition(Vector2Int anchor, int direction, int length, int currentPosition) =>
            CalculateCellPosition(anchor, direction, length, currentPosition);
        #endregion


        #region Internal Methods
        private void UpdateCellVisuals()
        {
            UnhighlightAllCells();
            HighlightAllAnsweredCells();
        }

        private void UnhighlightAllCells()
        {
            for (int column = 0; column < Columns; column++)
            {
                for (int row = 0; row < Rows; row++)
                {
                    WordCell cell = GetCell(column, row);
                    cell.SetAsHighlighted(false);
                }
            }
        }

        private void HighlightAllAnsweredCells()
        {
            for (int i = 0; i < SolvedPiecesCount; i++)
            {
                PuzzlePiece solvedPiece = GetSolvedPuzzlePiece(i);

                if (solvedPiece != null)
                    solvedPiece.Reveal();
            }
        }
        #endregion


        #region Helper Methods
        private char GenerateRandomCharacter()
        {
            int randomIndex = Random.Range(0, LettersInTheAlphabet.Length);

            return LettersInTheAlphabet[randomIndex];
        }

        private CursorEvent GenerateCursorEventHandler(int column, int row)
        {
            return (CursorState state) =>
            {
                UpdateCellVisuals();

                switch (state)
                {
                    case CursorState.Pressed:
                        OnCellPressedDown(column, row);
                    break;

                    case CursorState.Released:
                        OnCellPressedUp(column, row);
                    break;

                    case CursorState.Entered:
                        OnCellPointerEnter(column, row);
                    break;
                }
            };
        }

        private void OnCellPressedDown(int column, int row)
        {
            m_cellDragBegan = true;
            m_cellDragStartPosition = new Vector2Int(column, row);
            m_pendingWord = string.Empty;
        }

        private void OnCellPressedUp(int column, int row)
        {
            m_cellDragBegan = false;
            VerifyAnswer(m_pendingWord);
            SetUpHints();
        }

        private void OnCellPointerEnter(int column, int row)
        {
            if (!m_cellDragBegan)
                return;

            m_cellDragEndPosition = new Vector2Int(column, row);
            List<Vector2Int> affectedCellPositions = GetCellPositions(m_cellDragStartPosition, m_cellDragEndPosition);
            m_pendingWord = GetWordFromAffectedCellPositions(affectedCellPositions);
        }

        private string GetWordFromAffectedCellPositions(List<Vector2Int> affectedCellPositions)
        {
            string word = string.Empty;

            foreach (Vector2Int cellPosition in affectedCellPositions)
            {
                WordCell affectedCell = GetCell(cellPosition);

                if (affectedCell == null)
                    continue;

                word += affectedCell.assignedCharacter;
                affectedCell.SetAsHighlighted(true);
            }

            return word;
        }

        private List<Vector2Int> GetCellPositions(Vector2Int start, Vector2Int end)
        {
            List<Vector2Int> positions = new List<Vector2Int>();
            int points;
            Vector2Int normalizedDirection = NormalizeDirection(end - start, out points);
            end = start + normalizedDirection;

            Vector2Int currentPosition = start;
            positions.Add(currentPosition);

            for (int i = 0; i < points; i++)
            {
                currentPosition += normalizedDirection;
                positions.Add(currentPosition);
            }

            return positions;
        }

        private Vector2Int NormalizeDirection(Vector2Int direction, out int points)
        {
            int absoluteX = Mathf.Abs(direction.x);
            int absoluteY = Mathf.Abs(direction.y);
            points = Mathf.Max(absoluteX, absoluteY);

            int signedX = Mathf.FloorToInt(Mathf.Sign(direction.x));
            int signedY = Mathf.FloorToInt(Mathf.Sign(direction.y));

            if (absoluteX > absoluteY)
                return new Vector2Int(signedX, 0);

            else if (absoluteY > absoluteX)
                return new Vector2Int(0, signedY);

            else
                return new Vector2Int(signedX, signedY);
        }
        #endregion


        #region Static Method
        public static (int column, int row) CalculateCellPosition(Vector2Int anchor, int direction, int length, int currentPosition)
        {
            const int FlippedValueThreshold = 4;
            const int HorizontalDirection = 0, VerticalDirection = 1, DiagonalUpDirection = 2;

            bool isFlipped = (direction >= FlippedValueThreshold);

            if (isFlipped)
                direction -= FlippedValueThreshold;
            
            int horizontalDelta = (direction == VerticalDirection ? 0 : currentPosition);
            int verticalDelta = (direction == HorizontalDirection ? 0 : (direction == DiagonalUpDirection ? -currentPosition : currentPosition));

            if (isFlipped)
            {
                verticalDelta = -verticalDelta;
                horizontalDelta = -horizontalDelta;
            }

            int column = (anchor.x + horizontalDelta);
            int row = (anchor.y + verticalDelta);

            return (column, row);
        }
        #endregion
    }
}
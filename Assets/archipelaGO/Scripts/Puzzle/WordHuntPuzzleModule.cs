using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GridWord = archipelaGO.Puzzle.WordPuzzle.GridWord;
using Word = archipelaGO.WordBank.Word;
using CursorEvent = archipelaGO.Puzzle.WordCell.CursorEvent;
using CursorState = archipelaGO.Puzzle.WordCell.CursorState;
using CellState = archipelaGO.Puzzle.WordCell.State;

namespace archipelaGO.Puzzle
{
    public class WordHuntPuzzleModule : WordPuzzleModule
    {
        #region Fields
        [SerializeField]
        private RectTransform m_lineHintParent = null;

        [SerializeField]
        private GameObject m_lineHintPrefab = null;

        [SerializeField]
        private float m_revealAnimationInterval = 0.12f;

        private const string LettersInTheAlphabet =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private bool m_cellDragBegan = false;

        private Vector2Int m_cellDragStartPosition = Vector2Int.zero,
            m_cellDragEndPosition = Vector2Int.zero;
        
        private const float LineHintAlpha = 0.7f;
        private string m_pendingWord = string.Empty;
        #endregion


        #region Data Structure
        private class WordHuntPuzzlePiece : PuzzlePiece
        {
            public WordHuntPuzzlePiece(WordPuzzleModule puzzleModule, float revealAnimationInterval,
                string word, WordCell[] cells, Color hintColor, float alpha) :
                base(puzzleModule, word, cells, revealAnimationInterval)
            {
                m_hintColor = hintColor;
                m_hintColor.a = alpha;
            }

            private bool m_animationFinished = false;

            public override bool revealAnimationFinished => m_animationFinished;

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

            private Color m_hintColor = Color.clear;
            public Color hintColor => m_hintColor;
        }

        private class WordHuntHint : WordHint
        {
            public WordHuntHint (Word word, WordHintType hintType, Color color, int order, bool guessed) : base(word, hintType)
            {
                m_order = order;
                m_guessed = guessed;
                m_color = color;
            }

            #region Fields
            private Color m_color = Color.white;
            private int m_order = -1;
            private bool m_guessed = false;
            #endregion

            public override string GetHint()
            {
                string baseHint = base.GetHint();

                if (!m_guessed)
                    return baseHint;

                string colorString = ColorUtility.ToHtmlStringRGBA(m_color);
                baseHint = $"<color=#{ colorString }><b>{ baseHint }</b></color>";

                return baseHint;
            }
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

        protected override PuzzlePiece GeneratePuzzlePiece(GridWord gridWord, WordCell[] cells) =>
            new WordHuntPuzzlePiece(this, m_revealAnimationInterval, gridWord.word.title, cells, gridWord.hintColor, LineHintAlpha);

        protected override WordHint GenerateHint(int order, GridWord gridWord)
        {
            bool guessed = AlreadyAnswered(gridWord.word.title);

            return new WordHuntHint(gridWord.word, gridWord.hintType, gridWord.hintColor, order, guessed);
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
                    solvedPiece.PlayRevealAnimation();
            }
        }

        private void CreateLineHintImage(Color color, Vector3 startPosition, Vector3 endPosition, float width)
        {
            if (m_lineHintPrefab == null)
                return;
            
            GameObject lineHintGameObject = Instantiate(m_lineHintPrefab, m_lineHintParent);
            Image lineHintImage = lineHintGameObject.GetComponent<Image>();

            if (lineHintImage == null)
                return;

            lineHintImage.color = color;

            Vector3 direction = (endPosition - startPosition);
            float angle = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
            lineHintImage.rectTransform.rotation = Quaternion.Euler(Vector3.forward * angle);

            Vector3 midPoint = (startPosition + endPosition) / 2f;
            lineHintImage.transform.position = midPoint;
            lineHintImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
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
            WordCell cell = GetCell(column, row);
        }

        private void OnCellPressedUp(int column, int row)
        {
            m_cellDragBegan = false;
            PuzzlePiece result;

            if (VerifyAnswer(m_pendingWord, out result))
                DrawLineHint(m_cellDragStartPosition, m_cellDragEndPosition, result);

            SetUpHints();
        }

        private void DrawLineHint(Vector2Int start, Vector2Int end, PuzzlePiece result)
        {
            WordCell startCell = GetCell(start);
            WordCell endCell = GetCell(end);

            RectTransform startCellRectTransform = startCell.transform as RectTransform;
            RectTransform endCellRectTransform = endCell.transform as RectTransform;

            Color hintColor = (result != null ? (result as WordHuntPuzzlePiece).hintColor : Color.clear);
            Vector3 startCellWorldPosition = startCellRectTransform.transform.position;
            Vector3 endCellWorldPosition = endCellRectTransform.transform.position;
            float width = (endCellRectTransform.anchoredPosition3D - startCellRectTransform.anchoredPosition3D).magnitude;
            float widthPadding = (startCellRectTransform.rect.width / 2f) + (endCellRectTransform.rect.width / 2f) - 8f;
            CreateLineHintImage(hintColor, startCellWorldPosition, endCellWorldPosition, width + widthPadding);
        }

        private void OnCellPointerEnter(int column, int row)
        {
            if (!m_cellDragBegan)
                return;

            m_cellDragEndPosition = new Vector2Int(column, row);

            List<Vector2Int> affectedCellPositions = GetCellPositions(m_cellDragStartPosition, m_cellDragEndPosition);
            m_cellDragEndPosition = affectedCellPositions[affectedCellPositions.Count - 1];
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

            if (DirectionIsWithinDiagonalThreshold(new Vector2Int(absoluteX, absoluteY)))
                return new Vector2Int(signedX, signedY);

            else
            {
                if (absoluteX > absoluteY)
                    return new Vector2Int(signedX, 0);

                else
                    return new Vector2Int(0, signedY);
            }
        }

        private bool DirectionIsWithinDiagonalThreshold(Vector2Int direction)
        {
            const float AngleOfTolerance = 60f;

            return Vector2.Angle(Vector2Int.one, direction) <= (AngleOfTolerance / 2f);
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


        #if ARCHIPELAGO_DEBUG_MODE
        protected override void Debug_RevealAnswer(PuzzlePiece puzzlePiece)
        {
            (Vector2Int start, Vector2Int end) positions = puzzlePiece.Debug_GetGridPositions();
            DrawLineHint(positions.start, positions.end, puzzlePiece);
            base.Debug_RevealAnswer(puzzlePiece);
        }
        #endif
    }
}
using System.Linq;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GridDirection = archipelaGO.Crossword.WordPuzzle.Direction;
using GridWord = archipelaGO.Crossword.WordPuzzle.GridWord;
using Word = archipelaGO.WordBank.Word;

namespace archipelaGO.Crossword
{
    public class CrosswordPuzzle : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private WordPuzzle m_crossword = null;

        [SerializeField]
        private GameObject m_cellPrefab = null;

        [SerializeField]
        private Text m_hintText = null;

        [Space]

        [SerializeField]
        private GridLayoutGroup m_cellContainer = null;

        [SerializeField]
        private float m_padding = 50f;

        [SerializeField]
        private InputField m_answerField = null;

        [SerializeField]
        private CellHiding m_emptyCellsShould = CellHiding.Hide;

        private RectTransform m_rectTransform = null;
        private List<PuzzlePiece> m_puzzlePieces = new List<PuzzlePiece>();
        #endregion


        #region Data Structure
        private delegate void CellFunction(int column, int row, ref CrosswordCell cell);


        private struct WordHint
        {
            public WordHint (int order, GridDirection direction, string description) =>
                (m_order, m_direction, m_description) = (order, direction, description);

            private int m_order;
            private GridDirection m_direction;
            private string m_description;

            public GridDirection direction => m_direction;
            public string text => $"{ m_order }. { m_description }";
        }

        [System.Serializable]
        private enum CellHiding
        {
            UseDefaultColor,
            Hide
        }

        private class PuzzlePiece
        {
            public PuzzlePiece(string word, CrosswordCell[] cells) =>
                (m_word, m_cells) = (word.ToLower(), cells);

            private string m_word = string.Empty;
            private CrosswordCell[] m_cells = null;

            public bool Matches(string word)
            {
                if (string.IsNullOrEmpty(m_word) || string.IsNullOrWhiteSpace(m_word))
                    return false;

                return string.Equals(m_word, word.ToLower());
            }

            public void Reveal()
            {
                if (m_cells == null)
                    return;

                foreach (CrosswordCell cell in m_cells)
                {
                    if (cell != null)
                        cell.SetState(CrosswordCell.State.CharacterShown);
                }
            }
        }
        #endregion


        #region MonoBehaviour Implementation
        private void Awake()
        {
            m_rectTransform = transform as RectTransform;

            if (m_answerField != null)
                m_answerField.onEndEdit.AddListener(OnSubmitAnswer);
        }

        private void Start() =>
            SetUpGrid();

        private void OnDestroy()
        {
            if (m_answerField != null)
                m_answerField.onEndEdit.RemoveListener(OnSubmitAnswer);
        }
        #endregion


        #region Grid Implementation
        private void SetUpGrid()
        {
            if (m_crossword == null || m_cellPrefab == null)
                return;

            Vector2Int size = m_crossword.gridSize;
            CrosswordCell[,] grid = InitializeGrid(size);
            SetUpWords(grid);
        }

        private CrosswordCell[,] InitializeGrid(Vector2Int size)
        {
            CrosswordCell[,] grid = GenerateGrid(size);
            ModifyGridLayoutGroup(size);
            InsertCellsToContainer(grid);

            return grid;
        }

        private void SetUpWords(CrosswordCell[,] grid)
        {
            List<Vector2Int> plottedPoints = new List<Vector2Int>();
            List<WordHint> hints = new List<WordHint>();
            m_puzzlePieces.Clear();

            for (int i = 0; i < m_crossword.wordSize; i++)
            {
                GridWord gridWord = m_crossword.GetWord(i);

                if (gridWord == null)
                    continue;

                Word word = gridWord.word;
                GridDirection direction = gridWord.direction;
                Vector2Int position = gridWord.position;

                if (!plottedPoints.Contains(position))
                    plottedPoints.Add(position);

                int wordIndex = plottedPoints.IndexOf(position) + 1;
                PuzzlePiece puzzlePiece = GeneratePuzzlePiece(wordIndex, word.title, position, direction, grid);
                m_puzzlePieces.Add(puzzlePiece);

                WordHint hint = new WordHint(wordIndex, direction, $"({ word.title }) { word.description }");
                hints.Add(hint);
            }

            if (m_hintText != null)
                m_hintText.text = GenerateHints(hints);
        }

        private void ModifyGridLayoutGroup(Vector2Int gridSize)
        {
            m_cellContainer.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            m_cellContainer.constraintCount = gridSize.x;

            Vector2 padding = Vector2.one * (m_padding * 2f);
            Vector2 backgroundSize = (m_rectTransform.rect.size) - padding;
            m_cellContainer.cellSize = CalculateCellSize(gridSize, backgroundSize);
        }

        private void InsertCellsToContainer(CrosswordCell[,] grid)
        {
            if (m_cellContainer != null && !GridSizeInvalid(grid))
                IterateGrid(grid, InsertCellToContainer);
        }

        private void CreateAndAssignCell(int column, int row, ref CrosswordCell cell)
        {
            cell = CreateCell();
            cell.gameObject.name = $"Cell [{ column }, { row }]";

            bool doNotRevealCell = (m_emptyCellsShould == CellHiding.Hide);
            cell.SetAsEmptyTile(doNotRevealCell);
        }

        private void InsertCellToContainer(int row, int column, ref CrosswordCell cell)
        {
            RectTransform cellRect = cell.transform as RectTransform;
            cellRect.SetParent(m_cellContainer.transform);
            cellRect.localScale = Vector3.one;
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

        private void VerifyAnswer(string answer)
        {
            PuzzlePiece result = GetPuzzlePiece(answer);

            if (result == null)
                return;

            result.Reveal();
            m_puzzlePieces.Remove(result);

            if (m_puzzlePieces.Count <= 0)
                DeclareGameFinished();
        }

        private void DeclareGameFinished()
        {
            Debug.Log("Crossword Puzzle: Puzzle finished! You have won!!");
            
            if (m_answerField == null)
                return;

            m_answerField.interactable = false;
            m_answerField.text = "Puzzle finished!";
        }
        #endregion


        #region Helper Methods
        private CrosswordCell[,] GenerateGrid(Vector2Int size)
        {
            if (size.x <= 0 || size.y <= 0)
                return null;

            CrosswordCell[,] grid = new CrosswordCell[size.x, size.y];
            IterateGrid(grid, CreateAndAssignCell);

            return grid;
        }

        private void IterateGrid(CrosswordCell[,] grid, CellFunction function)
        {
            (int columns, int rows) gridSize = GetGridSize(grid);

            for (int row = 0; row < gridSize.rows; row++)
                for (int column = 0; column < gridSize.columns; column++)
                    function?.Invoke(column, row, ref grid[column, row]);
        }

        private CrosswordCell CreateCell()
        {
            if (m_cellPrefab == null)
                return null;

            GameObject cell = Instantiate(m_cellPrefab);

            return cell.GetComponent<CrosswordCell>();
        }

        private bool GridSizeInvalid(CrosswordCell[,] grid)
        {
            (int columns, int rows) gridSize = GetGridSize(grid);

            return (gridSize.rows <= 0) || (gridSize.columns <= 0);
        }

        private (int columns, int rows) GetGridSize(CrosswordCell[,] grid)
        {
            const int ColumnDimension = 0, RowDimension = 1;
            int columns = grid?.GetLength(ColumnDimension) ?? 0;
            int rows = grid?.GetLength(RowDimension) ?? 0;

            return (columns, rows);
        }

        private Vector2 CalculateCellSize(Vector2Int gridSize, Vector2 backgroundSize)
        {
            float width = (backgroundSize.x / gridSize.x);
            float height = (backgroundSize.y / gridSize.y);

            return Vector2.one * Mathf.Floor(Mathf.Min(width, height));
        }

        private string GenerateHints(List<WordHint> hints)
        {
            StringBuilder across = new StringBuilder();
            StringBuilder down = new StringBuilder();

            foreach (WordHint hint in hints)
            {
                if (hint.direction == GridDirection.Across)
                    across.AppendLine(hint.text);
                else
                    down.AppendLine(hint.text);
            }

            return $"<b>ACROSS</b>\n<i>{ across }</i>\n<b>DOWN</b>\n<i>{ down }</i>";
        }

        private PuzzlePiece GeneratePuzzlePiece(int index, string word, Vector2Int position, GridDirection direction, CrosswordCell[,] grids)
        {
            int wordLength = word.Length;
            (int columns, int rows) gridSize = GetGridSize(grids);
            List<CrosswordCell> cells = new List<CrosswordCell>();

            for (int i = 0; i < wordLength; i++)
            {
                int columnOffset = (direction == GridDirection.Across ? i : 0);
                int rowOffset = (direction == GridDirection.Down ? i : 0);

                int column = (position.x + columnOffset);
                int row = (position.y + rowOffset);

                if (row >= gridSize.rows || column >= gridSize.columns)
                    continue;

                CrosswordCell cell = grids[column, row];
                cells.Add(cell);

                char character = word[i];

                if (cell == null)
                    continue;
                
                if (i == 0)
                    cell.SetAsCharacterTileWithIndex(character, index);
                else
                    cell.SetAsCharacterTile(character);

                cell.SetState(CrosswordCell.State.CharacterHidden);
            }

            return new PuzzlePiece(word, cells.ToArray());
        }

        private PuzzlePiece GetPuzzlePiece(string word) =>
        (
            from puzzlePiece in m_puzzlePieces
            where puzzlePiece.Matches(word)
            select puzzlePiece
        ).FirstOrDefault();
        #endregion
    }
}
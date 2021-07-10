using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GridDirection = archipelaGO.Puzzle.WordPuzzle.Direction;
using GridWord = archipelaGO.Puzzle.WordPuzzle.GridWord;
using Word = archipelaGO.WordBank.Word;

namespace archipelaGO.Puzzle
{
    public abstract class WordPuzzleController : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private WordPuzzle m_wordPuzzle = null;

        [SerializeField]
        private GameObject m_cellPrefab = null;

        [Space]

        [SerializeField]
        private GridLayoutGroup m_cellContainer = null;

        [SerializeField]
        private float m_padding = 50f;

        [SerializeField]
        private Text m_hintText = null;

        private RectTransform m_rectTransform = null;
        private List<PuzzlePiece> m_puzzlePieces = new List<PuzzlePiece>();
        private List<WordHint> m_hints = new List<WordHint>();
        #endregion


        #region Data Structure
        private delegate void CellFunction(int column, int row, ref WordCell cell);

        protected class PuzzlePiece
        {
            public PuzzlePiece(string word, WordCell[] cells) =>
                (m_word, m_cells) = (word.ToLower(), cells);

            private string m_word = string.Empty;
            private WordCell[] m_cells = null;

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

                foreach (WordCell cell in m_cells)
                {
                    if (cell != null)
                        cell.SetState(WordCell.State.CharacterShown);
                }
            }
        }

        protected abstract class WordHint
        {
            public abstract string GetHint();
        }
        #endregion


        #region MonoBehaviour Implementation
        protected virtual void Awake() =>
            m_rectTransform = transform as RectTransform;

        protected void Start()
        {
            SetUpGrid();
            SetUpHints();
        }
        #endregion


        #region Abstract Functions
        protected abstract void OnPuzzleCompleted();
        protected abstract void AssignCharacterToCell(char character, int characterIndex, int puzzleIndex, WordCell cell);
        protected abstract void InitializeEmptyCell(WordCell cell);
        protected abstract WordHint GenerateHint(int order, GridWord gridWord);
        protected abstract string GenerateHintText(List<WordHint> hints);
        #endregion


        #region Internal Methods
        private void SetUpGrid()
        {
            if (m_wordPuzzle == null || m_cellPrefab == null)
                return;

            Vector2Int size = m_wordPuzzle.gridSize;
            WordCell[,] grid = InitializeGrid(size);
            SetUpWords(grid);
        }

        private void SetUpHints()
        {
            if (m_hintText != null)
                m_hintText.text = GenerateHintText(m_hints);
        }

        protected void VerifyAnswer(string answer)
        {
            PuzzlePiece result = GetPuzzlePiece(answer);

            if (result == null)
                return;

            result.Reveal();
            m_puzzlePieces.Remove(result);

            if (m_puzzlePieces.Count <= 0)
                OnPuzzleCompleted();
        }
        #endregion


        #region Grid Implementation
        private WordCell[,] InitializeGrid(Vector2Int size)
        {
            WordCell[,] grid = GenerateGrid(size);
            ModifyGridLayoutGroup(size);
            InsertCellsToContainer(grid);

            return grid;
        }

        private void SetUpWords(WordCell[,] grid)
        {
            List<Vector2Int> plottedPoints = new List<Vector2Int>();
            m_puzzlePieces.Clear();
            m_hints.Clear();

            for (int i = 0; i < m_wordPuzzle.wordSize; i++)
            {
                GridWord gridWord = m_wordPuzzle.GetWord(i);

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

                int orderNumber = (i + 1);
                WordHint hint = GenerateHint(orderNumber, gridWord);
                m_hints.Add(hint);
            }
        }

        private void ModifyGridLayoutGroup(Vector2Int gridSize)
        {
            m_cellContainer.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            m_cellContainer.constraintCount = gridSize.x;

            Vector2 padding = Vector2.one * (m_padding * 2f);
            Vector2 backgroundSize = (m_rectTransform.rect.size) - padding;
            m_cellContainer.cellSize = CalculateCellSize(gridSize, backgroundSize);
        }

        private void InsertCellsToContainer(WordCell[,] grid)
        {
            if (m_cellContainer != null && !GridSizeInvalid(grid))
                IterateGrid(grid, InsertCellToContainer);
        }

        private void CreateAndAssignCell(int column, int row, ref WordCell cell)
        {
            cell = CreateCell();
            cell.gameObject.name = $"Cell [{ column }, { row }]";
            InitializeEmptyCell(cell);
        }

        private void InsertCellToContainer(int row, int column, ref WordCell cell)
        {
            RectTransform cellRect = cell.transform as RectTransform;
            cellRect.SetParent(m_cellContainer.transform);
            cellRect.localScale = Vector3.one;
        }
        #endregion


        #region Helper Methods
        private WordCell[,] GenerateGrid(Vector2Int size)
        {
            if (size.x <= 0 || size.y <= 0)
                return null;

            WordCell[,] grid = new WordCell[size.x, size.y];
            IterateGrid(grid, CreateAndAssignCell);

            return grid;
        }

        private void IterateGrid(WordCell[,] grid, CellFunction function)
        {
            (int columns, int rows) gridSize = GetGridSize(grid);

            for (int row = 0; row < gridSize.rows; row++)
                for (int column = 0; column < gridSize.columns; column++)
                    function?.Invoke(column, row, ref grid[column, row]);
        }

        private WordCell CreateCell()
        {
            if (m_cellPrefab == null)
                return null;

            GameObject cell = Instantiate(m_cellPrefab);

            return cell.GetComponent<WordCell>();
        }

        private PuzzlePiece GeneratePuzzlePiece(int index, string word, Vector2Int position, GridDirection direction, WordCell[,] grids)
        {
            int wordLength = word.Length;
            (int columns, int rows) gridSize = GetGridSize(grids);
            List<WordCell> cells = new List<WordCell>();

            for (int characterIndex = 0; characterIndex < wordLength; characterIndex++)
            {
                int columnOffset = (direction == GridDirection.Across ? characterIndex : 0);
                int rowOffset = (direction == GridDirection.Down ? characterIndex : 0);

                int column = (position.x + columnOffset);
                int row = (position.y + rowOffset);

                if (row >= gridSize.rows || column >= gridSize.columns)
                    continue;

                WordCell cell = grids[column, row];
                cells.Add(cell);

                char character = word[characterIndex];

                if (cell != null)
                    AssignCharacterToCell(character, characterIndex, index, cell);
            }

            return new PuzzlePiece(word, cells.ToArray());
        }

        private bool GridSizeInvalid(WordCell[,] grid)
        {
            (int columns, int rows) gridSize = GetGridSize(grid);

            return (gridSize.rows <= 0) || (gridSize.columns <= 0);
        }

        private (int columns, int rows) GetGridSize(WordCell[,] grid)
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

        private PuzzlePiece GetPuzzlePiece(string word) =>
        (
            from puzzlePiece in m_puzzlePieces
            where puzzlePiece.Matches(word)
            select puzzlePiece
        ).FirstOrDefault();
        #endregion
    }
}
using UnityEngine;
using UnityEngine.UI;
using GridDirection = archipelaGO.Crossword.CrosswordConfig.Direction;
using GridWord = archipelaGO.Crossword.CrosswordConfig.GridWord;
using Word = archipelaGO.WordBank.Word;

namespace archipelaGO.Crossword
{
    public class CrosswordPuzzle : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private CrosswordConfig m_crossword = null;

        [SerializeField]
        private GameObject m_cellPrefab = null;

        [Space]

        [SerializeField]
        private GridLayoutGroup m_cellContainer = null;

        [SerializeField]
        private float m_padding = 50f;

        private RectTransform m_rectTransform = null;
        private CrosswordCell[,] m_grid = null;
        private delegate void CellFunction(int column, int row, ref CrosswordCell cell);
        #endregion


        #region MonoBehaviour Implementation
        private void Awake() => m_rectTransform = transform as RectTransform;
        private void Start() => SetUpGrid();
        #endregion


        #region Internal Methods
        private void SetUpGrid()
        {
            if (m_crossword == null || m_cellPrefab == null)
                return;

            Vector2Int size = m_crossword.gridSize;
            m_grid = GenerateGrid(size);
            ModifyGridLayoutGroup(size);
            InsertCellsToContainer(m_grid);
            SetUpWords();
        }

        private void SetUpWords()
        {
            for (int i = 0; i < m_crossword.wordSize; i++)
            {
                GridWord gridWord = m_crossword.GetWord(i);

                if (gridWord != null)
                    AssignGridWord(gridWord);
            }
        }

        private void AssignGridWord(GridWord grid)
        {
            string word = grid.word.title;
            int wordLength = word.Length;
            Vector2Int position = grid.position;
            GridDirection direction = grid.direction;
            (int columns, int rows) gridSize = GetGridSize(m_grid);

            for (int i = 0; i < wordLength; i++)
            {
                int columnOffset = (direction == GridDirection.Across ? i : 0);
                int rowOffset = (direction == GridDirection.Down ? i : 0);

                int column = (position.x + columnOffset);
                int row = (position.y + rowOffset);

                if (row >= gridSize.rows || column >= gridSize.columns)
                    continue;

                CrosswordCell cell = m_grid[column, row];
                char character = word[i];

                if (cell != null)
                    cell.SetAsCharacterTile(character);
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

        private void InsertCellsToContainer(CrosswordCell[,] grid)
        {
            if (m_cellContainer != null && !GridSizeInvalid(grid))
                IterateGrid(grid, InsertCellToContainer);
        }
        #endregion


        #region Grid Iterators
        private void CreateAndAssignCell(int column, int row, ref CrosswordCell cell)
        {
            cell = CreateCell();
            cell.gameObject.name = $"Cell [{ column }, { row }]";
            cell.SetAsEmptyTile();
        }

        private void InsertCellToContainer(int row, int column, ref CrosswordCell cell)
        {
            RectTransform cellRect = cell.transform as RectTransform;
            cellRect.SetParent(m_cellContainer.transform);
            cellRect.localScale = Vector3.one;
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
        #endregion
    }
}
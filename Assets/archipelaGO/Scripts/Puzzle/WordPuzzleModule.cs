using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ScorableGameModule = archipelaGO.Game.ScorableGameModule<archipelaGO.Puzzle.WordPuzzle>;
using GridWord = archipelaGO.Puzzle.WordPuzzle.GridWord;
using Word = archipelaGO.WordBank.Word;
using TMPro;

namespace archipelaGO.Puzzle
{
    public abstract class WordPuzzleModule : ScorableGameModule
    {
        #region Fields
        [SerializeField]
        private GameObject m_cellPrefab = null;

        [Space]

        [SerializeField]
        private GridLayoutGroup m_cellContainer = null;

        [SerializeField]
        private float m_padding = 50f;

        [SerializeField]
        private TextMeshProUGUI m_hintText = null;

        private RectTransform m_rectTransform = null;
        private List<PuzzlePiece> m_pendingPieces = new List<PuzzlePiece>();
        private List<PuzzlePiece> m_solvedPieces = new List<PuzzlePiece>();
        private List<WordHint> m_hints = new List<WordHint>();
        private WordCell[,] m_wordGrid = null;
        #endregion


        #region Data Structure
        private delegate void CellFunction(int column, int row, ref WordCell cell);

        protected abstract class PuzzlePiece
        {
            public PuzzlePiece(WordPuzzleModule puzzleModule, string word, WordCell[] cells, float revealAnimationInterval) =>
                (m_puzzleModule, m_word, m_cells, m_revealAnimationInterval) = (puzzleModule, word.ToLower(), cells, revealAnimationInterval);

            private WordPuzzleModule m_puzzleModule = null;
            private string m_word = string.Empty;
            protected WordCell[] m_cells = null;
            private Coroutine m_revealAnimation = null;
            private float m_revealAnimationInterval = 0f;
            private bool m_revealed = false;

            protected float revealAnimationInterval => m_revealAnimationInterval;

            #if ARCHIPELAGO_DEBUG_MODE
            public (Vector2Int start, Vector2Int end) Debug_GetGridPositions()
            {
                if (m_cells == null || m_cells.Length <= 0 || m_puzzleModule == null)
                    return (new Vector2Int(-1, -1), new Vector2Int(-1, -1));

                WordCell startCell = m_cells[0];
                Vector2Int startPosition = m_puzzleModule.Debug_GetCellPosition(startCell);

                WordCell endCell = m_cells[m_cells.Length - 1];
                Vector2Int endPosition = m_puzzleModule.Debug_GetCellPosition(endCell);

                return (startPosition, endPosition);
            }
            #endif

            public abstract bool revealAnimationFinished { get; }

            public bool Matches(string word)
            {
                if (string.IsNullOrEmpty(m_word) || string.IsNullOrWhiteSpace(m_word))
                    return false;

                return string.Equals(m_word, word.ToLower());
            }

            public void PlayRevealAnimation()
            {
                if (m_puzzleModule == null || m_revealed)
                    return;

                if (m_revealAnimation != null)
                    m_puzzleModule.StopCoroutine(m_revealAnimation);

                m_revealAnimation =
                    m_puzzleModule.StartCoroutine(OnRevealPuzzle());

                m_revealed = true;
            }

            protected abstract IEnumerator OnRevealPuzzle();
        }

        protected abstract class WordHint
        {
            public WordHint (Word word, WordHintType hintType) =>
                m_hintText = WordPuzzleHintGenerator.GetHint(word, hintType);

            private string m_hintText;
            public virtual string GetHint() => m_hintText;
        }
        #endregion


        #region Properties
        protected int Columns => (m_wordGrid?.GetLength(0) ?? 0);
        protected int Rows => (m_wordGrid?.GetLength(1) ?? 0);
        protected int SolvedPiecesCount => m_solvedPieces.Count;
        #endregion


        #region Scorable Module Implementation
        public override int currentScore =>
            m_solvedPieces?.Count ?? 0;

        public override int totalScore
        {
            get
            {
                int unsolvedPieces = m_pendingPieces?.Count ?? 0;
                int solvedPieces = m_solvedPieces?.Count ?? 0;

                return unsolvedPieces + solvedPieces;
            }
        }
        #endregion


        #region Game Module Implementation
        protected override void OnInitialize()
        {
            m_rectTransform = transform as RectTransform;
            SetUpGrid();
            SetUpHints();
        }
        #endregion


        #region Puzzle Functions
        protected virtual void InitializeCell(int column, int row, WordCell cell) =>
            cell.gameObject.name = $"Cell [{ column }, { row }]";

        protected abstract void OnPuzzleCompleted();
        protected abstract void AssignCharacterToCell(char character, int characterIndex, int puzzleIndex, WordCell cell);
        protected abstract WordHint GenerateHint(int order, GridWord gridWord);
        protected abstract string GenerateHintText(List<WordHint> hints);
        protected abstract PuzzlePiece GeneratePuzzlePiece(GridWord gridWord, WordCell[] cells);
        #endregion


        #region Internal Methods
        private void SetUpGrid()
        {
            if (config == null || m_cellPrefab == null)
                return;

            Vector2Int size = config.gridSize;
            m_wordGrid = InitializeGrid(size);
            SetUpWords(m_wordGrid);
        }

        protected void SetUpHints()
        {
            GenerateHints();

            if (m_hintText != null)
                m_hintText.text = GenerateHintText(m_hints);
        }

        protected bool VerifyAnswer(string answer)
        {
            PuzzlePiece result;

            return VerifyAnswer(answer, out result);
        }

        protected bool VerifyAnswer(string answer, out PuzzlePiece result)
        {
            result = GetPuzzlePiece(answer);

            if (result == null)
                return false;

            SetPuzzleAsSolved(result);

            if (m_pendingPieces.Count <= 0)
                InvokePuzzleCompleted();

            return true;
        }

        private void SetPuzzleAsSolved(PuzzlePiece puzzle)
        {
            puzzle.PlayRevealAnimation();

            if (m_pendingPieces.Contains(puzzle))
                m_pendingPieces.Remove(puzzle);

            if (!m_solvedPieces.Contains(puzzle))
                m_solvedPieces.Add(puzzle);
        }

        private void InvokePuzzleCompleted()
        {
            OnPuzzleCompleted();
            InvokeGameOver();
        }

        protected bool AlreadyAnswered(string answer)
        {
            foreach (PuzzlePiece solvedPiece in m_solvedPieces)
            {
                if (solvedPiece.Matches(answer))
                    return true;
            }

            return false;
        }

        protected PuzzlePiece GetSolvedPuzzlePiece(int index)
        {
            if (index < 0 || index >= SolvedPiecesCount)
                return null;

            return m_solvedPieces[index];
        }
        #endregion


        #region Grid Implementation
        protected WordCell GetCell(Vector2Int position) =>
            GetCell(position.x, position.y);

        protected WordCell GetCell(int column, int row)
        {
            if (m_wordGrid == null || (column < 0) ||
                (row < 0) || (column >= Columns) ||
                (row >= Rows))
                return null;

            return m_wordGrid[column, row];
        }

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
            m_pendingPieces.Clear();

            for (int i = 0; i < config.wordSize; i++)
            {
                GridWord gridWord = config.GetWord(i);

                if (gridWord == null)
                    continue;

                Vector2Int position = gridWord.position;

                if (!plottedPoints.Contains(position))
                    plottedPoints.Add(position);

                int wordIndex = plottedPoints.IndexOf(position) + 1;
                PuzzlePiece puzzlePiece = GeneratePuzzlePiece(wordIndex, gridWord, grid);
                m_pendingPieces.Add(puzzlePiece);
            }
        }

        private void GenerateHints()
        {            
            m_hints.Clear();

            for (int i = 0; i < config.wordSize; i++)
            {
                GridWord gridWord = config.GetWord(i);

                if (gridWord == null)
                    continue;

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
            cell = CreateCell(column, row);

            if (cell != null)
                InitializeCell(column, row, cell);
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

        private WordCell CreateCell(int column, int row)
        {
            if (m_cellPrefab == null)
                return null;

            GameObject cell = Instantiate(m_cellPrefab);
            WordCell wordCellComponent = cell.GetComponent<WordCell>();

            return wordCellComponent;
        }

        private PuzzlePiece GeneratePuzzlePiece(int index, GridWord gridWord, WordCell[,] grids)
        {
            string word = gridWord.word.title;
            int direction = gridWord.direction;
            Vector2Int position = gridWord.position;
            int wordLength = word.Length;
            (int columns, int rows) gridSize = GetGridSize(grids);
            List<WordCell> cells = new List<WordCell>();

            for (int characterIndex = 0; characterIndex < wordLength; characterIndex++)
            {
                (int column, int row) currentCellPosition = GetCellPosition(position, direction, wordLength, characterIndex);

                if (currentCellPosition.row >= gridSize.rows || currentCellPosition.column >= gridSize.columns ||
                    currentCellPosition.row < 0 || currentCellPosition.column < 0)
                    continue;

                WordCell cell = grids[currentCellPosition.column, currentCellPosition.row];
                cells.Add(cell);

                char character = word[characterIndex];

                if (cell != null)
                    AssignCharacterToCell(character, characterIndex, index, cell);
            }

            return GeneratePuzzlePiece(gridWord, cells.ToArray());
        }

        protected abstract (int column, int row) GetCellPosition(Vector2Int anchor, int direction, int length, int currentPosition);

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
            from puzzlePiece in m_pendingPieces
            where puzzlePiece.Matches(word)
            select puzzlePiece
        ).FirstOrDefault();
        #endregion


        #if ARCHIPELAGO_DEBUG_MODE
        public Vector2Int Debug_GetCellPosition(WordCell cell)
        {
            if (cell == null || m_wordGrid == null)
                goto End;
            
            for (int column = 0; column < Columns; column++)
            {
                for (int row = 0; row < Rows; row++)
                {
                    Vector2Int currentPosition = new Vector2Int(column, row);
                    WordCell currentCell = m_wordGrid[column, row];

                    if (cell == currentCell)
                        return currentPosition;
                }
            }

            End:
            return new Vector2Int(-1, -1);
        }

        public override IEnumerator Debug_Autoplay()
        {
            for (int i = m_pendingPieces.Count - 1; i >= 0; i--)
            {
                PuzzlePiece pendingPuzzle = m_pendingPieces[i];

                if (pendingPuzzle == null)
                    continue;

                Debug_RevealAnswer(pendingPuzzle);

                yield return new WaitUntil(() => pendingPuzzle.revealAnimationFinished);
                yield return new WaitForSeconds(0.25f);
            }

            yield return new WaitForSeconds(0.7362901f);
            InvokePuzzleCompleted();
        }

        protected virtual void Debug_RevealAnswer(PuzzlePiece puzzlePiece) =>
            SetPuzzleAsSolved(puzzlePiece);
        #endif
    }
}
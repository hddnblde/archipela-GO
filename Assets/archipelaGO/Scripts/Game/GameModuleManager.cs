using UnityEngine;
using VisualNovelConfig = archipelaGO.VisualNovel.StorySystem.Plot;
using WordPuzzleConfig = archipelaGO.Puzzle.WordPuzzle;
using QuizConfig = archipelaGO.Quiz.QuizConfig;
using VisualNovelModule = archipelaGO.VisualNovel.StorySystem.VisualNovelModule;
using CrosswordPuzzleModule = archipelaGO.Puzzle.CrosswordPuzzleModule;
using WordHuntPuzzleModule = archipelaGO.Puzzle.WordHuntPuzzleModule;
using QuizModule = archipelaGO.Quiz.QuizModule;

namespace archipelaGO.Game
{
    public class GameModuleManager : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private VisualNovelModule m_visualNovelModule = null;

        [SerializeField]
        private CrosswordPuzzleModule m_crosswordModule = null;

        [SerializeField]
        private WordHuntPuzzleModule m_wordHuntModule = null;

        [SerializeField]
        private QuizModule m_quizModule = null;
        #endregion


        #region Public Method
        public void LoadModule(GameConfig gameConfig)
        {
            if (gameConfig == null)
            {
                Debug.LogError("Failed to load module because game config is null.");
                return;
            }

            switch (gameConfig)
            {
                case VisualNovelConfig vnConfig:
                    LoadVisualNovel(vnConfig);
                break;

                case WordPuzzleConfig wordPuzzleConfig:
                    LoadPuzzleModule(wordPuzzleConfig);
                break;

                case QuizConfig quizConfig:
                    LoadQuizModule(quizConfig);
                break;
            }
        }
        #endregion


        #region Internal Methods
        private void LoadVisualNovel(VisualNovelConfig config) =>
            LoadModule<VisualNovelModule, VisualNovelConfig>(m_visualNovelModule, config);

        private void LoadPuzzleModule(WordPuzzleConfig config)
        {
            switch (config.puzzleType)
            {
                case Puzzle.PuzzleType.Crossword:
                    LoadCrosswordModule(config);
                break;

                case Puzzle.PuzzleType.WordHunt:
                    LoadWordHuntModule(config);
                break;
            }
        }

        private void LoadCrosswordModule(WordPuzzleConfig config) =>
            LoadModule<CrosswordPuzzleModule, WordPuzzleConfig>(m_crosswordModule, config);

        private void LoadWordHuntModule(WordPuzzleConfig config) =>
            LoadModule<WordHuntPuzzleModule, WordPuzzleConfig>(m_wordHuntModule, config);

        private void LoadQuizModule(QuizConfig config) =>
            LoadModule<QuizModule, QuizConfig>(m_quizModule, config);
        #endregion


        #region Helper Methods
        private void LoadModule<M, G>(M module, G config) where M : GameModule<G>
            where G : GameConfig
        {
            if (module == null)
                return;

            module.OnGameCompleted += InvokeOnGameCompleted;
            module.gameObject.SetActive(true);
            module.Initialize(config);
        }

        private void InvokeOnGameCompleted(GameConfig config)
        {
            if (config != null)
                ProgressDataHandler.Unlock(config.name);
        }
        #endregion
    }
}
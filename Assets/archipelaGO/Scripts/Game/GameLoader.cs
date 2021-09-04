using System.Collections.Generic;
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
    public class GameLoader : MonoBehaviour
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

        private List<string> m_unlockableModules = new List<string>();
        #endregion


        #region Public Method
        public void LoadModule(GameModuleConfig module, params string[] unlockableModules)
        {
            if (module == null)
            {
                Debug.LogError("Failed to load module because it is null.");
                return;
            }

            m_unlockableModules.AddRange(unlockableModules);

            switch (module)
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
            where G : GameModuleConfig
        {
            if (module == null)
                return;

            module.OnGameCompleted += InvokeOnGameCompleted;
            module.gameObject.SetActive(true);
            module.Initialize(config);
        }

        private void InvokeOnGameCompleted()
        {
            foreach (string unlockedModule in m_unlockableModules)
                ProgressDataHandler.Unlock(unlockedModule);
        }
        #endregion
    }
}
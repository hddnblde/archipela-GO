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
        private void LoadVisualNovel(VisualNovelConfig config)
        {
            if (m_visualNovelModule == null)
                return;

            m_visualNovelModule.gameObject.SetActive(true);
            m_visualNovelModule.Initialize(config);
        }

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

        private void LoadCrosswordModule(WordPuzzleConfig config)
        {
            if (m_crosswordModule == null)
                return;

            m_crosswordModule.gameObject.SetActive(true);
            m_crosswordModule.Initialize(config);
        }

        private void LoadWordHuntModule(WordPuzzleConfig config)
        {
            if (m_wordHuntModule == null)
                return;

            m_wordHuntModule.gameObject.SetActive(true);
            m_wordHuntModule.Initialize(config);
        }

        private void LoadQuizModule(QuizConfig config)
        {
            if (m_quizModule == null)
                return;

            m_quizModule.gameObject.SetActive(true);
            m_quizModule.Initialize(config);
        }
        #endregion
    }
}
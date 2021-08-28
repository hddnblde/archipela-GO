using UnityEngine;
using VisualNovelConfig = archipelaGO.VisualNovel.StorySystem.Plot;
using WordPuzzleConfig = archipelaGO.Puzzle.WordPuzzle;
using QuizConfig = archipelaGO.Quiz.QuizConfig;
using VisualNovelModule = archipelaGO.VisualNovel.StorySystem.NarrativeDirector;
using CrosswordPuzzleModule = archipelaGO.Puzzle.CrosswordPuzzle;
using WordHuntPuzzleModule = archipelaGO.Puzzle.WordHuntPuzzle;
using QuizModule = archipelaGO.Quiz.QuizController;

namespace archipelaGO.Game
{
    public class GameModuleManager : MonoBehaviour
    {
        [SerializeField]
        private VisualNovelModule m_visualNovelModule = null;

        [SerializeField]
        private CrosswordPuzzleModule m_crosswordModule = null;

        [SerializeField]
        private WordHuntPuzzleModule m_wordHuntModule = null;

        [SerializeField]
        private QuizModule m_quizModule = null;

        public void LoadModule(GameConfig gameConfig)
        {
            switch (gameConfig)
            {
                case VisualNovelConfig vnConfig:
                    m_visualNovelModule?.Initialize(vnConfig);
                break;

                case WordPuzzleConfig wordPuzzleConfig:

                    switch (wordPuzzleConfig.puzzleType)
                    {
                        case Puzzle.PuzzleType.Crossword:
                            m_crosswordModule?.Initialize(wordPuzzleConfig);
                        break;

                        case Puzzle.PuzzleType.WordHunt:
                            m_wordHuntModule?.Initialize(wordPuzzleConfig);
                        break;
                    }

                break;

                case QuizConfig quizConfig:
                    m_quizModule?.Initialize(quizConfig);
                break;
            }
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using VisualNovelConfig = archipelaGO.VisualNovel.StorySystem.Plot;
using WordPuzzleConfig = archipelaGO.Puzzle.WordPuzzle;
using QuizConfig = archipelaGO.Quiz.QuizConfig;
using VisualNovelModule = archipelaGO.VisualNovel.StorySystem.VisualNovelModule;
using CrosswordPuzzleModule = archipelaGO.Puzzle.CrosswordPuzzleModule;
using WordHuntPuzzleModule = archipelaGO.Puzzle.WordHuntPuzzleModule;
using QuizModule = archipelaGO.Quiz.QuizModule;
using IWorldMapLinker = archipelaGO.WorldMap.IWorldMapLinker;
using GameHint = archipelaGO.UI.Windows.GameHintWindow;
using EndScreen = archipelaGO.UI.Windows.EndScreenWindow;
using archipelaGO.GameData;
using archipelaGO.UI;

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

        [SerializeField]
        private GameHint m_hintScreen = null;

        [SerializeField]
        private EndScreen m_endScreen = null;

        private List<string> m_unlockableModules = new List<string>();
        #endregion


        #region Public Methods
        public void SetUpWorldMapLinkers(GameLibrary library)
        {
            GameObject[] rootGameObjects = GetRootGameObjects();

            if (rootGameObjects == null || rootGameObjects.Length <= 0)
                return;
            
            foreach (GameObject root in rootGameObjects)
            {
                IWorldMapLinker[] linkers =
                    root.GetComponentsInChildren<IWorldMapLinker>(true);

                if (linkers == null)
                    continue;

                foreach (IWorldMapLinker linker in linkers)
                    linker.SetGameLibrary(library);
            }
        }

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

        private void ShowHintScreen(string hintText, UIWindow.Activity hideCallback)
        {
            if (m_hintScreen != null)
                m_hintScreen.ShowHintText(hintText);
            
            m_hintScreen.OnHide += hideCallback;
        }

        private void HideHintScreen()
        {
            if (m_hintScreen != null)
                m_hintScreen.Hide();
        }

        private void UnlockKeys()
        {
            PlayerData playerData = GameDataHandler.CurrentPlayer();

            if (playerData == null)
                return;

            GameProgressionData gameProgress =
                playerData.Access<GameProgressionData>();

            if (gameProgress == null)
                return;

            if (gameProgress.UnlockTheseKeys(m_unlockableModules.ToArray()))
                GameDataHandler.SaveCurrentPlayer();
        }

        private void ShowEndScreen(string message)
        {
            if (m_endScreen != null)
                m_endScreen.Show(message);
        }
        #endregion


        #region Helper Methods
        private void LoadModule<M, G>(M module, G config) where M : GameModule<G>
            where G : GameModuleConfig
        {
            if (module == null)
                return;

            module.OnFailed += ShowEndScreen;
            module.OnSucceeded += (string message) =>
                {
                    ShowEndScreen(message);
                    UnlockKeys();
                };

            module.gameObject.SetActive(true);
            module.Initialize(config);

            if (m_hintScreen != null)
                m_hintScreen.OnHide += module.Begin;

            if (config.requireTapToContinue)
                ShowHintScreen(config.hint, module.Begin);

            else
            {
                module.Begin();
                HideHintScreen();
            }
        }

        public static GameObject[] GetRootGameObjects() =>
            UnityEngine.SceneManagement.SceneManager.
                GetActiveScene().GetRootGameObjects();
        #endregion
    }
}
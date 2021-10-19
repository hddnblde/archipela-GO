using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using archipelaGO.SceneHandling;
using GameLibrary = archipelaGO.Game.GameLibrary;
using World = archipelaGO.Game.GameWorldManager;
using IWorldMapLinker = archipelaGO.WorldMap.IWorldMapLinker;

namespace archipelaGO.UI.Windows
{
    public sealed class EndScreenWindow : UIWindow, IWorldMapLinker
    {
        #region Fields
        [SerializeField]
        private Text m_messageText = null;

        [Space]

        [SerializeField]
        private Scene m_returnTo = Scene.WorldMap;

        [SerializeField]
        private Button m_returnButton = null;

        private GameLibrary m_library = null;
        #endregion


        #region MonoBehaviour Implementation
        protected override void Awake()
        {
            RegisterReturnButtonEvent();
            base.Awake();
        }

        private void Start() => Hide();

        private void OnDestroy() => DeregisterReturnButtonEvent();
        #endregion


        #region Public Methods
        public void SetGameLibrary(GameLibrary library) =>
            m_library = library;

        public void Show(string message)
        {
            if (m_messageText == null)
                return;

            m_messageText.text = message;
            this.Show();
        }
        #endregion


        #region Internal Methods
        private void RegisterReturnButtonEvent()
        {
            if (m_returnButton != null)
                m_returnButton.onClick.AddListener(OnReturnButtonClicked);
        }

        private void DeregisterReturnButtonEvent()
        {
            if (m_returnButton != null)
                m_returnButton.onClick.RemoveListener(OnReturnButtonClicked);
        }

        private void OnReturnButtonClicked()
        {
            DeregisterReturnButtonEvent();
            StartCoroutine(LoadSceneRoutine());
        }

        private IEnumerator LoadSceneRoutine()
        {
            yield return new WaitUntil(SceneLoader.CanLoadANewScene);
            SceneLoader.LoadScene(m_returnTo);
            Hide();
            yield return new WaitUntil(SceneLoader.FinishedLoadingScene);

            World world = GameObject.FindObjectOfType<World>();

            if (world != null)
                world.LoadWorld(m_library);
        }
        #endregion
    }
}
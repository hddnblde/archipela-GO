using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace archipelaGO.Game
{
    public interface IAutoplayable
    {
        IEnumerator Debug_Autoplay();
    }

    [RequireComponent(typeof(CanvasGroup))]
    public sealed class GameModuleAutoplayer : MonoBehaviour
    {
        [SerializeField]
        private Button m_button = null;

        private CanvasGroup m_canvasGroup = null;
        private Coroutine m_autoplayer = null;

        private void Awake()
        {
            #if !ARCHIPELAGO_DEBUG_MODE
            Destroy(gameObject)
            return;
            #else
            m_canvasGroup = GetComponent<CanvasGroup>();
            RegisterButtonEvent();
            #endif
        }

        #if ARCHIPELAGO_DEBUG_MODE
        private void OnDestroy() =>
            DeregisterButtonEvent();

        private void RegisterButtonEvent()
        {
            if (m_button != null)
                m_button.onClick.AddListener(OnButtonClick);
        }

        private void DeregisterButtonEvent()
        {
            if (m_button != null)
                m_button.onClick.RemoveListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            DeregisterButtonEvent();
            m_canvasGroup.interactable = false;
            m_canvasGroup.alpha = 0f;
            m_canvasGroup.blocksRaycasts = false;

            List<IAutoplayable> playables = GetAllAutoplayablesInScene();
            Stop();
            m_autoplayer = StartCoroutine(AutoplayRoutine(playables));
        }

        public void Stop()
        {
            if (m_autoplayer != null)
                StopCoroutine(m_autoplayer);
        }

        private IEnumerator AutoplayRoutine(List<IAutoplayable> playables)
        {
            if (playables == null || playables.Count <= 0)
                goto End;

            foreach (IAutoplayable playable in playables)
                yield return playable.Debug_Autoplay();

            End:
            Destroy(gameObject);
        }

        private List<IAutoplayable> GetAllAutoplayablesInScene()
        {
            List<IAutoplayable> playables = new List<IAutoplayable>();

            GameObject[] rootGameObjects = GameLoader.GetRootGameObjects();

            if (rootGameObjects == null || rootGameObjects.Length <= 0)
            goto End;

            foreach (GameObject root in rootGameObjects)
            {
                IAutoplayable[] playablesInChildren =
                    root.GetComponentsInChildren<IAutoplayable>(false);

                if (playablesInChildren != null && playablesInChildren.Length > 0)
                    playables.AddRange(playablesInChildren);
            }

            End:
            return playables;
        }
        #endif
    }
}
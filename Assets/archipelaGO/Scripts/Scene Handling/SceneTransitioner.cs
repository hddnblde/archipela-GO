using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace archipelaGO.SceneHandling
{
    public class SceneTransitioner : MonoBehaviour
    {
        [SerializeField]
        private List<TransitionableScreen> m_screens = new List<TransitionableScreen>();

        [SerializeField]
        private Scene m_nextSceneToLoad = Scene.SignIn;

        private void Start() =>
            StartCoroutine(TransitionRoutine());

        private IEnumerator TransitionRoutine()
        {
            foreach (TransitionableScreen screen in m_screens)
            {
                if (screen != null)
                    yield return screen.WaitUntilDone();
            }

            yield return new WaitUntil(SceneLoader.NotTransitioning);

            SceneLoader.LoadScene(m_nextSceneToLoad);
        }
    }
}

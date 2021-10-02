using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using archipelaGO.SceneHandling;

namespace archipelaGO
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField]
        private float m_delayBeforeLoadingFirstScene = 5f;

        [SerializeField]
        private VideoPlayer m_splashScreenPlayer = null;

        [SerializeField]
        private Scene m_firstSceneToLoad = Scene.WorldMap;

        private void Start() => StartCoroutine(BootRoutine());

        private IEnumerator BootRoutine()
        {
            if (m_splashScreenPlayer != null)
            {
                m_splashScreenPlayer.Prepare();
                yield return new WaitUntil(() => m_splashScreenPlayer.isPrepared);
                m_splashScreenPlayer.Play();
                yield return new WaitUntil(() => !m_splashScreenPlayer.isPlaying);
                yield return new WaitForSeconds(m_delayBeforeLoadingFirstScene);
            }

            SceneLoader.LoadScene(m_firstSceneToLoad);
        }
    }
}
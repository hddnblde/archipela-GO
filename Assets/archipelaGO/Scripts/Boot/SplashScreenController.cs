using System.Collections;
using UnityEngine;
using UnityEngine.Video;

namespace archipelaGO
{
    public class SplashScreenController : MonoBehaviour
    {
        #if UNITY_EDITOR
        [SerializeField]
        private bool m_skip = false;
        #endif

        [SerializeField]
        private VideoPlayer m_splashScreenPlayer = null;

        [SerializeField]
        private AudioSource m_splashScreenAudioStinger = null;

        public IEnumerator PlaybackRoutine()
        {
            #if UNITY_EDITOR
            if (m_skip)
                yield break;
            #endif

            if (m_splashScreenPlayer != null)
            {
                m_splashScreenPlayer.Prepare();
                yield return new WaitUntil(() => m_splashScreenPlayer.isPrepared);

                m_splashScreenPlayer.Play();

                if (m_splashScreenAudioStinger != null)
                    m_splashScreenAudioStinger.Play();

                yield return new WaitUntil(() => !m_splashScreenPlayer.isPlaying);
            }
        }
    }
}
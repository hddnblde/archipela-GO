using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using archipelaGO.SceneHandling;

namespace archipelaGO.Boot
{
    public class SplashScreenController : TransitionableScreen
    {
        [SerializeField]
        private float m_delayAfterSplashScreen = 5f;

        [SerializeField]
        private VideoPlayer m_splashScreenPlayer = null;

        [SerializeField]
        private AudioSource m_splashScreenAudioStinger = null;

        public override IEnumerator WaitUntilDone()
        {
            if (m_splashScreenPlayer == null)
                yield break;

            m_splashScreenPlayer.Prepare();
            yield return new WaitUntil(() => m_splashScreenPlayer.isPrepared);

            m_splashScreenPlayer.Play();

            if (m_splashScreenAudioStinger != null)
                m_splashScreenAudioStinger.Play();

            yield return new WaitUntil(() => !m_splashScreenPlayer.isPlaying);
            yield return new WaitForSeconds(m_delayAfterSplashScreen);
        }
    }
}
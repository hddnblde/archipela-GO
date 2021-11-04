using System.Collections;
using UnityEngine;

namespace archipelaGO.Audio
{
    public class BackgroundMusicPlayer : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private AudioSource m_audioSource = null;

        [SerializeField]
        private float m_transitionTime = 3f;

        private Coroutine m_musicTransitionRoutine = null;
        #endregion

    
        #region Internal Methods
        public void Play(AudioClip music)
        {
            if (m_audioSource.isPlaying && m_audioSource.clip == music)
                return;

            if (m_musicTransitionRoutine != null)
                StopCoroutine(m_musicTransitionRoutine);

            m_musicTransitionRoutine = StartCoroutine(MusicTransitionRoutine(music));
        }

        private IEnumerator MusicTransitionRoutine(AudioClip audioClip)
        {
            if (m_audioSource.isPlaying)
            {
                yield return TransitionVolumeRoutine(false, m_transitionTime);
                m_audioSource.Stop();
            }

            m_audioSource.clip = audioClip;
            m_audioSource.Play();
            yield return TransitionVolumeRoutine(true, m_transitionTime);
        }

        private IEnumerator TransitionVolumeRoutine(bool fadeIn, float duration)
        {
            float start = (fadeIn ? 0f : 1f);
            float end = (fadeIn ? 1f : 0f);
            float volumeRatio = Mathf.InverseLerp(start, end, m_audioSource.volume);

            for (float current = Mathf.Lerp(0f, duration, volumeRatio);
                current < duration; current += Time.deltaTime)
            {
                float t = Mathf.InverseLerp(0f, duration, current);
                float volume = Mathf.Lerp(start, end, t);
                SetVolume(volume);
                yield return null;
            }
        }

        private void SetVolume(float volume)
        {
            if (m_audioSource != null)
                m_audioSource.volume = volume;
        }
        #endregion
    }
}
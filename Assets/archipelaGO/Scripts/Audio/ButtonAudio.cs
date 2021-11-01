using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace archipelaGO.Audio
{
    public class ButtonAudio : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private Button m_button = null;

        [SerializeField]
        private List<AudioClip> m_clips = new List<AudioClip>();

        [SerializeField]
        private AudioSource m_source = null;
        #endregion


        #region MonoBehaviour Implementation
        private void Awake() => RegisterButtonEvent();
        private void OnDestroy() => DeregisterButtonEvent();
        #endregion


        #region Internal Methods
        private void RegisterButtonEvent()
        {
            if (m_button != null)
                m_button.onClick.AddListener(PlayButtonSFX);
        }

        private void DeregisterButtonEvent()
        {
            if (m_button != null)
                m_button.onClick.RemoveListener(PlayButtonSFX);
        }

        private void PlayButtonSFX()
        {
            AudioClip clip = GetRandomAudioClip();
            PlayAudioClip(clip);
        }

        private void PlayAudioClip(AudioClip clip)
        {
            if (m_source == null || clip == null)
                return;

            if (m_source.isPlaying)
                m_source.Stop();

            m_source.clip = clip;
            m_source.Play();
        }

        private AudioClip GetRandomAudioClip()
        {
            if (m_clips == null || m_clips.Count <= 0)
                return null;

            int randomIndex = Random.Range(0, m_clips.Count);
            return m_clips[randomIndex];
        }
        #endregion
    }
}
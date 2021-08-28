using UnityEngine;
using UnityEngine.UI;
using Word = archipelaGO.WordBank.Word;
using AnimatedText = archipelaGO.UI.AnimatedText;

namespace archipelaGO.UI.Windows
{
    public class LexemeWindow : UIWindow
    {
        #region Fields
        [SerializeField]
        private Button m_closeButton = null;

        [SerializeField]
        private Button m_pronunciationButton = null;

        [SerializeField]
        private AudioSource m_pronunciationAudioSource = null;

        [SerializeField]
        private LayoutGroup m_titleLayoutGroup = null;

        [SerializeField]
        private Text m_titleText = null;

        [SerializeField]
        private Text m_partOfSpeechText = null;

        [SerializeField]
        private Text m_definitionText = null;

        private AudioClip m_cachedPronunciationClip = null;
        #endregion


        #region MonoBehaviour Implementation
        private void OnEnable() => RegisterEventListeners();
        private void OnDisable() => DeregisterEventListeners();
        #endregion


        #region Internal Methods
        private void RegisterEventListeners()
        {
            AnimatedText.OnWordSelected += OnWordSelected;

            if (m_closeButton != null)
                m_closeButton.onClick.AddListener(Hide);

            if (m_pronunciationButton != null)
                m_pronunciationButton.onClick.AddListener(PlayPronunciationClip);
        }

        private void DeregisterEventListeners()
        {
            AnimatedText.OnWordSelected -= OnWordSelected;

            if (m_closeButton != null)
                m_closeButton.onClick.RemoveListener(Hide);

            if (m_pronunciationButton != null)
                m_pronunciationButton.onClick.RemoveListener(PlayPronunciationClip);
        }

        private void OnWordSelected(Word word)
        {
            SetTitleText(word.title);
            SetPartOfSpeechText(word.partOfSpeech.ToString().ToLower());
            SetDefinitionText(word.definition);
            m_cachedPronunciationClip = word.pronunciationClip;
            Show();
        }

        private void SetText(Text textComponent, string textValue)
        {
            if (textComponent != null)
                textComponent.text = textValue;
        }

        private void SetTitleText(string title)
        {
            SetText(m_titleText, title);

            if (m_titleLayoutGroup != null)
                LayoutRebuilder.ForceRebuildLayoutImmediate(m_titleLayoutGroup.transform as RectTransform);
        }
        
        private void SetPartOfSpeechText(string partOfSpeech) =>
            SetText(m_partOfSpeechText, partOfSpeech);

        private void SetDefinitionText(string definition) =>
            SetText(m_definitionText, definition);
        
        private void PlayPronunciationClip()
        {
            if (m_pronunciationAudioSource == null)
                return;

            if (m_pronunciationAudioSource.isPlaying)
                m_pronunciationAudioSource.Stop();

            if (m_pronunciationAudioSource.clip != m_cachedPronunciationClip)
                m_pronunciationAudioSource.clip = m_cachedPronunciationClip;

            m_pronunciationAudioSource.Play();
        }
        #endregion
    }
}
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
        private Text m_titleText = null;

        [SerializeField]
        private Text m_partOfSpeechText = null;

        [SerializeField]
        private Text m_definitionText = null;
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
        }

        private void DeregisterEventListeners()
        {
            AnimatedText.OnWordSelected -= OnWordSelected;

            if (m_closeButton != null)
                m_closeButton.onClick.RemoveListener(Hide);
        }

        private void OnWordSelected(Word word)
        {
            SetTitleText(word.title);
            SetPartOfSpeechText(word.partOfSpeech.ToString());
            SetDefinitionText(word.definition);
            Show();
        }

        private void SetText(Text textComponent, string textValue)
        {
            if (textComponent != null)
                textComponent.text = textValue;
        }

        private void SetTitleText(string title) =>
            SetText(m_titleText, title);
        
        private void SetPartOfSpeechText(string partOfSpeech) =>
            SetText(m_partOfSpeechText, partOfSpeech);

        private void SetDefinitionText(string definition) =>
            SetText(m_definitionText, definition);
        #endregion
    }
}
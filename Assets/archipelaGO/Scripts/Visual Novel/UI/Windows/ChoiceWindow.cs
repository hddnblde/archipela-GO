using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace archipelaGO.VisualNovel.UI.Windows
{
    public sealed class ChoiceWindow : UIWindow
    {
        #region Fields
        [SerializeField]
        private Text m_titleText = null;

        [SerializeField]
        private List<Button> m_choiceButtons = new List<Button>();

        private List<Text> m_choiceButtonLabels = new List<Text>();

        public delegate void ChoiceSelected(int index);
        public event ChoiceSelected OnChoiceSelected;
        #endregion


        #region MonoBehaviour Implementation
        protected override void Awake()
        {
            base.Awake();
            InitializeChoiceButtons();
        }

        private void OnDestroy() => UninitializeChoiceButtons();
        #endregion


        #region Public Methods
        public void Show(string title, params string[] labels)
        {
            SetHeaderText(title);
            SetChoiceLabels(labels);
            ShowCanvas(true);
        }

        public void Hide() => ShowCanvas(false);
        #endregion


        #region Internal Methods
        private void InitializeChoiceButtons()
        {
            m_choiceButtonLabels.Clear();

            foreach (Button choiceButton in m_choiceButtons)
            {
                Text labelText = choiceButton.GetComponentInChildren<Text>();
                m_choiceButtonLabels.Add(labelText);
                int choiceIndex = m_choiceButtons.IndexOf(choiceButton);

                choiceButton.onClick.AddListener(() =>
                {
                    OnChoiceSelected?.Invoke(choiceIndex);
                    Hide();
                });
            }
        }

        private void UninitializeChoiceButtons()
        {
            foreach (Button choiceButton in m_choiceButtons)
                choiceButton.onClick.RemoveAllListeners();
        }

        private void SetHeaderText(string text)
        {
            if (m_titleText != null)
                m_titleText.text = text;
        }

        private void SetChoiceLabels(params string[] labels)
        {
            int labelCount = Mathf.Max(m_choiceButtonLabels.Count, labels.Length);

            for (int i = 0; i < labelCount; i++)
            {
                if (i >= m_choiceButtonLabels.Count)
                    break;

                if (i >= labels.Length)
                    ShowChoiceButton(i, false);

                else
                {
                    string label = labels[i];
                    SetChoiceLabel(i, label);
                    ShowChoiceButton(i, true);
                }
            }
        }

        private void SetChoiceLabel(int index, string label)
        {
            if (index < 0 || index >= m_choiceButtonLabels.Count)
                return;

            Text textComponent = m_choiceButtonLabels[index];

            if (textComponent != null)
                textComponent.text = label;
        }

        private void ShowChoiceButton(int index, bool shown)
        {
            if (index < 0 || index >= m_choiceButtons.Count)
                return;

            Button choiceButton = m_choiceButtons[index];

            if (choiceButton != null)
                choiceButton.gameObject.SetActive(shown);
        }
        #endregion
    }
}
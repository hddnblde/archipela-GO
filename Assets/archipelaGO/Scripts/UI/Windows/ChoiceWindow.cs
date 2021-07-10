using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace archipelaGO.UI.Windows
{
    public sealed class ChoiceWindow : UIWindow
    {
        #region Data Structure
        public class WaitForChosenOption : CustomYieldInstruction
        {
            public WaitForChosenOption(ChoiceWindow choiceWindow, string title, string[] choices, bool hideAfterChoosing)
            {
                if (choiceWindow == null)
                    return;

                ChoiceSelected callback = (int choice) =>
                {
                    SelectOption(choice);

                    if (hideAfterChoosing && choiceWindow != null)
                        choiceWindow.Hide();
                };

                choiceWindow.ShowInternal(title, choices, callback);
            }

            private void SelectOption(int choice) =>
                m_chosenIndex = choice;

            private int m_chosenIndex = -1;
            public int choiceIndex => m_chosenIndex;
            public override bool keepWaiting => (m_chosenIndex == -1);
        }
        #endregion


        #region Fields
        [SerializeField]
        private Text m_messageText = null;

        [SerializeField]
        private List<Button> m_choiceButtons = new List<Button>();

        private List<Text> m_choiceButtonLabels = new List<Text>();

        public delegate void ChoiceSelected(int index);
        private event ChoiceSelected OnChoiceSelected;
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
        public WaitForChosenOption Show(string message, string[] choiceLabels, bool hideAfterChoosing) =>
            new WaitForChosenOption(this, message, choiceLabels, hideAfterChoosing);

        private void ShowInternal(string message, string[] choiceLabels, ChoiceSelected callback)
        {
            OnChoiceSelected = callback;
            SetMessage(message);
            SetChoiceLabels(choiceLabels);
            Show();
        }
        #endregion


        #region UI Window Implementation
        public override void Hide()
        {
            OnChoiceSelected = null;
            base.Hide();
        }
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
                choiceButton.onClick.AddListener(() => SelectChoice(choiceIndex));
            }
        }

        private void SelectChoice(int choice)
        {
            OnChoiceSelected?.Invoke(choice);
            OnChoiceSelected = null;
        }

        private void UninitializeChoiceButtons()
        {
            foreach (Button choiceButton in m_choiceButtons)
                choiceButton.onClick.RemoveAllListeners();
        }

        private void SetMessage(string text)
        {
            if (m_messageText != null)
                m_messageText.text = text;
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
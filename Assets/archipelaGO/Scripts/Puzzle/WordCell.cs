using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace archipelaGO.Puzzle
{
    public class WordCell : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        #region Fields
        [SerializeField]
        private bool m_interactable = true;

        [SerializeField]
        private Text m_indexContainer = null;

        [SerializeField]
        private Text m_characterContainer = null;

        [SerializeField]
        private Image m_backgroundImage = null;

        [SerializeField]
        private float m_revealAnimationTime = 0.25f;

        [Space]

        [SerializeField]
        private Color m_enabledColor = Color.white;

        [SerializeField]
        private Color m_emptyColor = Color.gray;

        public event PointerEvent OnPressedDown,
            OnPressedUp;
        #endregion


        #region Data Structure
        public delegate void PointerEvent();

        public enum State
        {
            Empty = 0,
            CharacterHidden = 1,
            CharacterShown = 2
        }
        #endregion


        #region Pointer Events Implementation
        public void OnPointerDown(PointerEventData pointerEventData)
        {
            if (m_interactable)
                OnPressedDown?.Invoke();
        }

        public void OnPointerUp(PointerEventData pointerEventData)
        {
            if (!m_interactable)
                OnPressedUp?.Invoke();
        }
        #endregion


        #region Public Methods
        public void SetAsCharacterTileWithIndex(char character, int index)
        {
            AssignIndex(index);
            SetAsCharacterTile(character);
        }

        public void SetAsCharacterTile(char character)
        {
            AssignCharacter(character);
            SetState(State.CharacterShown);
        }

        public void SetAsEmptyTile(bool isTransparent)
        {
            if (isTransparent)
                m_emptyColor = Color.clear;

            AssignCharacter(' ');
            AssignIndex(-1);
            SetState(State.Empty);
        }

        public void SetState(State state)
        {
            (Color backgroundColor, bool showCharacter) cell = GetCellState(state);
            SetBackgroundColor(cell.backgroundColor);
            ShowCharacter(cell.showCharacter);
        }
        #endregion


        #region Internal Methods
        private void AssignIndex(int index)
        {
            if (m_indexContainer != null)
                m_indexContainer.text = (index > 0 ? index.ToString() : string.Empty);
        }
        
        private void AssignCharacter(char character)
        {
            if (m_characterContainer != null)
                m_characterContainer.text = $"{ character }".ToUpper();
        }

        private void SetBackgroundColor(Color color)
        {
            if (m_backgroundImage != null)
                m_backgroundImage.color = color;
        }

        private void ShowCharacter(bool shown)
        {
            if (m_characterContainer != null)
                m_characterContainer.enabled = shown;
        }
        #endregion


        #region Helper Method
        private (Color backgroundColor, bool showCharacter) GetCellState(State state)
        {
            Color backgroundColor = (state == State.Empty ?
                m_emptyColor : m_enabledColor);

            bool showCharacter = (state == State.CharacterShown);
        
            return (backgroundColor, showCharacter);
        }
        #endregion
    }
}
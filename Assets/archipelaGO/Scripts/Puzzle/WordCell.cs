using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace archipelaGO.Puzzle
{
    public class WordCell : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
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
        
        [SerializeField]
        private Color m_highlightedColor = Color.yellow;

        private bool m_highlighted = false;
        public event CursorEvent OnCursorEventInvoked;
        private State m_currentState = State.Empty;
        private char m_assignedCharacter = ' ';
        #endregion


        #region Data Structure
        public delegate void CursorEvent(CursorState state);

        public enum CursorState
        {
            Pressed,
            Entered,
            Released
        }

        public enum State
        {
            Empty = 0,
            CharacterHidden = 1,
            CharacterShown = 2
        }
        #endregion


        #region Property
        public char assignedCharacter => m_assignedCharacter;
        #endregion


        #region Pointer Events Implementation
        public void OnPointerDown(PointerEventData pointerEventData)
        {
            if (m_interactable)
                OnCursorEventInvoked?.Invoke(CursorState.Pressed);
        }

        public void OnPointerUp(PointerEventData pointerEventData)
        {
            if (m_interactable)
                OnCursorEventInvoked?.Invoke(CursorState.Released);
        }

        public void OnPointerEnter(PointerEventData pointerEventData)
        {
            if (m_interactable)
                OnCursorEventInvoked?.Invoke(CursorState.Entered);
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
            m_currentState = state;
            (Color backgroundColor, bool showCharacter) cell = GetCellState(state);
            SetBackgroundColor(cell.backgroundColor);
            ShowCharacter(cell.showCharacter);
        }

        public void SetAsHighlighted(bool highlighted)
        {
            m_highlighted = highlighted;
            SetState(m_currentState);
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
            m_assignedCharacter = character;

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
            Color backgroundColor = GetStateColor(state);
            bool showCharacter = (state == State.CharacterShown);
        
            return (backgroundColor, showCharacter);
        }

        private Color GetStateColor(State state)
        {
            switch (state)
            {
                case State.Empty:
                    return m_emptyColor;

                default:

                    if (m_highlighted)
                        return m_highlightedColor;

                    else
                        return m_enabledColor;
            }
        }
        #endregion
    }
}
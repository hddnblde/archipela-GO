using UnityEngine;
using UnityEngine.UI;

namespace archipelaGO.GameData
{
    public class SaveSlotUI : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private Button m_loadButton = null;

        [SerializeField]
        private Button m_deleteButton = null;

        [SerializeField]
        private Text m_nameText = null;

        private PlayerData m_playerData = null;
        public event Selection OnLoad;
        public event Selection OnDelete;

        public delegate void Selection(PlayerData data);
        #endregion


        #region MonoBehaviour Implementation
        private void Awake() => RegisterButtonEvents();
        private void OnDestroy() => DeregisterButtonEvents();
        #endregion


        #region Public Method
        public void Set(PlayerData data)
        {
            m_playerData = data;

            SetName(m_playerData != null ?
                m_playerData.name : "Empty");
        }
        #endregion


        #region Internal Methods
        private void RegisterButtonEvents()
        {
            if (m_loadButton != null)
                m_loadButton.onClick.AddListener(InvokeLoadEvent);

            if (m_deleteButton != null)
                m_deleteButton.onClick.AddListener(InvokeDeleteEvent);
        }

        private void DeregisterButtonEvents()
        {
            if (m_loadButton != null)
                m_loadButton.onClick.RemoveListener(InvokeLoadEvent);

            if (m_deleteButton != null)
                m_deleteButton.onClick.RemoveListener(InvokeDeleteEvent);
        }

        private void InvokeLoadEvent() =>
            InvokeSelectionEvent(OnLoad);

        private void InvokeDeleteEvent() =>
            InvokeSelectionEvent(OnDelete);

        private void InvokeSelectionEvent(Selection selectionEvent)
        {
            if (m_nameText != null)
                selectionEvent?.Invoke(m_playerData);
        }

        private void SetName(string name)
        {
            if (m_nameText != null)
                m_nameText.text = name;
        }
        #endregion
    }
}
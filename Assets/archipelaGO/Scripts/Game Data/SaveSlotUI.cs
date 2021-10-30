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

        public event Selection OnLoad;
        public event Selection OnDelete;

        public delegate void Selection();
        #endregion


        #region MonoBehaviour Implementation
        private void Awake() => RegisterButtonEvents();
        private void OnDestroy() => DeregisterButtonEvents();
        #endregion


        #region Public Method
        public void Set(PlayerData data, int totalUnlockableKeys) =>
            SetLabel(ConstructLabel(data, totalUnlockableKeys));

        private string ConstructLabel(PlayerData data, int totalUnlockableKeys)
        {
            if (data == null)
                return "\n\t<i>Create new file</i>";

            int unlockedKeys = data.Access<GameProgressionData>()?.CountUnlockedKeys() ?? 0;

            float totalProgress = (totalUnlockableKeys > 0 ? 
                Mathf.FloorToInt(((unlockedKeys * 1f) / (totalUnlockableKeys * 1f)) * 100f) :
                0f);

            return $"{ data.name }\n\t<b>{ totalProgress.ToString("F0") }</b>%";
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
                selectionEvent?.Invoke();
        }

        private void SetLabel(string name)
        {
            if (m_nameText != null)
                m_nameText.text = name;
        }
        #endregion
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using archipelaGO.UI;
using archipelaGO.GameData;
using archipelaGO.Game;

namespace archipelaGO.Boot
{
    public class SignInManager : UIWindow
    {
        #region Fields
        [SerializeField]
        private GameProgressCollection m_progressKeys = null;

        [SerializeField]
        private List<SaveSlotUI> m_saveSlots = new List<SaveSlotUI>();

        [SerializeField]
        private CanvasGroup m_inputGroup = null;

        [SerializeField]
        private InputField m_inputField = null;

        private bool m_inputSubmitted = false;
        private Coroutine m_signInRoutine = null;
        private int m_selectedSaveSlot = -1;
        #endregion

        #region Data Structure
        private delegate void LoadSaveSlot(int index);

        public class WaitForSignIn : CustomYieldInstruction
        {
            public WaitForSignIn(SignInManager signInManager)
            {
                if (signInManager != null)
                    signInManager.Show(OnLoadSaveSlot);
            }

            private int m_saveSlot = -1;

            private void OnLoadSaveSlot(int index) =>
                m_saveSlot = index;

            public int saveSlot => m_saveSlot;
            public override bool keepWaiting => m_saveSlot == -1;
        }
        #endregion


        #region MonoBehaviour Implementation
        protected override void Awake()
        {
            base.Awake();
            InitializeButtonEvents();
        }
        #endregion


        #region Public Method
        public WaitForSignIn SignIn() => new WaitForSignIn(this);
        #endregion


        #region Internal Methods
        private void Show(LoadSaveSlot callback)
        {
            m_selectedSaveSlot = -1;

            if (m_signInRoutine != null)
                StopCoroutine(m_signInRoutine);

            m_signInRoutine =
                StartCoroutine(SignInRoutine(callback));
        }

        private void LoadExistingPlayersData()
        {
            int totalUnlockableKeys = (m_progressKeys != null ? m_progressKeys.keyCount : 0);
            InitializeSaveSlots(GetPlayerDataList(), totalUnlockableKeys);
        }

        private PlayerData[] GetPlayerDataList()
        {
            PlayerData[] list = new PlayerData[m_saveSlots.Count];
            List<(int index, PlayerData data)> availableSaveSlots = GameDataHandler.GetListOfSaveSlots();

            foreach ((int index, PlayerData data) saveSlot in availableSaveSlots)
            {
                if (saveSlot.index >= 0 && saveSlot.index < list.Length && saveSlot.data != null)
                    list[saveSlot.index] = saveSlot.data;
            }

            return list;
        }

        private void InitializeButtonEvents()
        {
            for (int i = 0; i < m_saveSlots.Count; i++)
            {
                int slotIndex = i;
                SaveSlotUI saveSlot = m_saveSlots[i];

                if (saveSlot == null)
                    continue;

                saveSlot.OnLoad += () => OnLoadData(slotIndex);
                saveSlot.OnDelete += () => OnDeleteData(slotIndex);
            }

            if (m_inputField != null)
                m_inputField.onEndEdit.AddListener(OnInputFieldSubmitted);
        }

        private void InitializeSaveSlots(PlayerData[] saveList, int totalUnlockableKeys)
        {
            if (saveList == null)
                saveList = new PlayerData[0];

            for (int i = 0; i < m_saveSlots.Count; i++)
            {
                SaveSlotUI saveSlot = m_saveSlots[i];

                if (saveSlot == null)
                    continue;

                PlayerData data = (i < saveList.Length ? saveList[i] : null);
                saveSlot.Set(data, totalUnlockableKeys);
            }
        }

        private IEnumerator SignInRoutine(LoadSaveSlot signIn)
        {
            LoadExistingPlayersData();
            this.Show();

            yield return new WaitUntil(() => m_selectedSaveSlot != -1);
            signIn?.Invoke(m_selectedSaveSlot);
        }

        private void ShowInputGroup(bool shown)
        {
            if (m_inputGroup == null || m_inputField == null)
                return;
            
            m_inputGroup.interactable = shown;
            m_inputGroup.blocksRaycasts = shown;
            m_inputGroup.alpha = (shown ? 1f : 0f);
        }

        private void CreateNewPlayerData(int slot)
        {
            ClearInputField();

            if (m_createNewPlayerDataRoutine != null)
                StopCoroutine(m_createNewPlayerDataRoutine);

            m_createNewPlayerDataRoutine = StartCoroutine(CreateNewPlayerDataRoutine(slot));
        }

        private Coroutine m_createNewPlayerDataRoutine = null;

        private IEnumerator CreateNewPlayerDataRoutine(int saveSlot)
        {
            ShowInputGroup(true);
            yield return new WaitUntil(() => m_inputSubmitted);

            if (!m_inputField.wasCanceled)
                GameDataHandler.Create(saveSlot, m_inputField.text);

            LoadExistingPlayersData();
        }

        private void OnInputFieldSubmitted(string text)
        {
            m_inputSubmitted = true;
            ShowInputGroup(false);
        }

        private void ClearInputField()
        {
            m_inputField.text = string.Empty;
            m_inputSubmitted = false;
        }
        #endregion


        #region Button Events Implementation
        private void OnLoadData(int index)
        {
            if (!GameDataHandler.SaveSlotExists(index))
                CreateNewPlayerData(index);

            else
            {
                m_selectedSaveSlot = index;
                Hide();
            }
        }

        private void OnDeleteData(int index)
        {
            GameDataHandler.Delete(index);
            LoadExistingPlayersData();
        }
        #endregion
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using archipelaGO.UI;
using archipelaGO.GameData;

namespace archipelaGO.Boot
{
    public class SignInManager : UIWindow
    {
        #region Fields
        #if UNITY_EDITOR
        [Header ("Debug")]

        [SerializeField]
        private bool m_debugForcedSignIn = false;

        [SerializeField]
        private string m_debugPlayerName = "player1";

        [Space]
        #endif
        [SerializeField]
        private List<SaveSlotUI> m_saveSlots = new List<SaveSlotUI>();

        [SerializeField]
        private Text m_inputField = null;

        private Coroutine m_signInRoutine = null;
        private PlayerData m_selectedPlayer = null;
        #endregion

        #region Data Structure
        private delegate void SuccessfullySignedIn(string playerName);

        public class WaitForSignIn : CustomYieldInstruction
        {
            public WaitForSignIn(SignInManager signInManager)
            {
                if (signInManager != null)
                    signInManager.Show(OnPlayerSuccessfullySignedIn);
            }

            private string m_playerName = string.Empty;
            private bool m_successfullySignedIn = false;

            private void OnPlayerSuccessfullySignedIn(string playerName)
            {
                m_playerName = playerName;
                m_successfullySignedIn = true;
            }

            public string playerName => m_playerName;
            public override bool keepWaiting => !m_successfullySignedIn;
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
        private void Show(SuccessfullySignedIn callback)
        {
            if (m_signInRoutine != null)
                StopCoroutine(m_signInRoutine);

            m_signInRoutine =
                StartCoroutine(SignInRoutine(callback));
        }

        private void LoadExistingPlayersData() =>
            InitializeSaveSlots(GetPlayerDataList());

        private PlayerData[] GetPlayerDataList()
        {
            PlayerData[] list = new PlayerData[4];

            for (int i = 0; i < list.Length; i++)
            {

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

                saveSlot.OnLoad += (PlayerData data) => OnLoadData(slotIndex, data);
                saveSlot.OnDelete += (PlayerData data) => OnDeleteData(slotIndex, data);
            }
        }

        private void InitializeSaveSlots(PlayerData[] saveList)
        {
            if (saveList == null)
                saveList = new PlayerData[0];

            for (int i = 0; i < m_saveSlots.Count; i++)
            {
                SaveSlotUI saveSlot = m_saveSlots[i];

                if (saveSlot == null)
                    continue;

                PlayerData data = (i < saveList.Length ? saveList[i] : null);
                saveSlot.Set(data);
            }
        }

        private IEnumerator SignInRoutine(SuccessfullySignedIn signIn)
        {
            LoadExistingPlayersData();
            this.Show();

            yield return null;

            #if UNITY_EDITOR
            if (m_debugForcedSignIn)
                signIn(m_debugPlayerName);
            #else
            yield return new WaitUntil(() => m_selectedPlayer != null);
            signIn?.Invoke(m_selectedPlayer.name);
            #endif
        }
        #endregion


        #region Button Events Implementation
        private void OnLoadData(int index, PlayerData data)
        {
            if (data != null)
                m_selectedPlayer = data;
            else
            {
                // create new save
            }
        }

        private void OnDeleteData(int index, PlayerData data)
        {
            // if player decides to delete -- delete data
        }
        #endregion
    }
}
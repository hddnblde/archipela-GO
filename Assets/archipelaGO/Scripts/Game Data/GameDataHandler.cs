using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace archipelaGO.GameData
{
    public static class GameDataHandler
    {
        #region Fields
        private static PlayerData m_currentPlayer = null;

        private const string FolderRoot = "Debug_GameData",
            FileExtension = "dat";
        #endregion


        #region Public Methods
        public static PlayerData CurrentPlayer() => m_currentPlayer;

        public static PlayerData Load(string playerName)
        {
            if (m_currentPlayer != null && string.Equals(m_currentPlayer.realName, playerName))
                goto End;

            string path = GetDataPathForPlayer(playerName);

            SerializedPlayerData serializedPlayerData =
                DataManager.Load<SerializedPlayerData>(path);

            if (serializedPlayerData != null)
            {
                m_currentPlayer = serializedPlayerData.ToPlayerData(playerName);
                Debug.LogWarning($"Data successfully retrieved from: { path }");
            }

            else
            {
                m_currentPlayer = new PlayerData(playerName);
                Debug.LogWarning($"Creating new data to: { path }");
                Save(m_currentPlayer);
            }

            End:
            return m_currentPlayer;
        }

        public static void SaveCurrentPlayerData() =>
            Save(m_currentPlayer);

        public static void Save(PlayerData playerData)
        {
            if (playerData == null)
            {
                Debug.LogWarning("Cannot save empty data.");
                return;
            }

            string path = GetDataPathForPlayer(playerData.realName);
            DataManager.Save(playerData.AsSerializable(), path);
            Debug.LogWarning($"Data saved to: { path }");
        }
        #endregion


        #region Helper Methods
        private static string GetDataPathForPlayer(string playerName) =>
            $"{ GetRootDataPath() }/{ playerName }.{ FileExtension }";

        private static string GetRootDataPath() =>
            $"{ Application.persistentDataPath }/{ FolderRoot }";

        private static bool StringIsInvalid(string name) =>
            string.IsNullOrWhiteSpace(name) || string.IsNullOrEmpty(name);
        #endregion
    }
}
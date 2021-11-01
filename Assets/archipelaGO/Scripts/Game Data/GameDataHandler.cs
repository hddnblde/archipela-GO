using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace archipelaGO.GameData
{
    public static class GameDataHandler
    {
        #region Fields
        private static int m_currentSaveSlot = -1;
        private static PlayerData m_currentPlayer = null;

        private const string FolderRoot = "Debug_GameData",
            FileName = "sav",
            FileExtension = "dat";
        #endregion


        #region Public Methods
        public static PlayerData CurrentPlayer() => m_currentPlayer;

        public static bool SaveSlotExists(int slot)
        {
            string path;

            return SaveSlotExists(slot, out path);
        }

        public static void Load(int slot)
        {
            string path;

            if (!SaveSlotExists(slot, out path))
            {
                Debug.LogWarning($"[Save Slot : { slot }] does not exist.");
                return;
            }

            SerializedPlayerData serializedPlayerData = LoadPlayerData(path);

            if (serializedPlayerData == null)
            {
                Debug.LogError($"[Save Slot : { slot }] failed to load!");
                return;
            }

            m_currentPlayer = serializedPlayerData.ToPlayerData();
            m_currentSaveSlot = slot;
            Debug.LogWarning($"[Save Slot : { slot }] was retrieved from: { path }");
        }

        public static void SaveCurrentPlayer() =>
            Save(m_currentSaveSlot, m_currentPlayer);

        public static void Create(int slot, string name) =>
            Save(slot, new PlayerData(name));
        
        public static void Delete(int slot)
        {
            string path;

            if (!SaveSlotExists(slot, out path))
                return;

            DataManager.Delete(path);
            Debug.LogWarning($"[Save Slot : { slot }] has been deleted.");
        }

        private static void Save(int slot, PlayerData playerData)
        {
            if (playerData == null)
            {
                Debug.LogWarning($"[Save Slot : { slot }] cannot be saved as empty.");
                return;
            }

            string path = GetDataPathForSlot(slot);
            DataManager.Save(playerData.AsSerializable(), path);
            Debug.LogWarning($"[Save Slot : { slot }] was saved to: { path }");
        }

        public static List<(int slot, PlayerData data)> GetListOfSaveSlots()
        {
            List<(int slot, PlayerData data)> list = new List<(int slot, PlayerData data)>();

            string[] files = Directory.GetFiles(GetRootDataPath());

            foreach (string file in files)
            {
                string fileNiceName = Path.GetFileName(file);
                string[] substrings = fileNiceName.Split(".".ToCharArray());

                if (substrings == null || substrings.Length < 2)
                    continue;
                
                int value = -1;

                if (!int.TryParse(substrings[1], out value))
                {
                    Debug.LogError($"Failed to parse save.... { fileNiceName }");
                    continue;
                }
                else
                    Debug.Log($"File { fileNiceName } detected.");
                
                string path;

                if (!SaveSlotExists(value, out path))
                    continue;

                SerializedPlayerData serializedPlayerData = LoadPlayerData(path);

                if (serializedPlayerData != null)
                    list.Add((value, serializedPlayerData.ToPlayerData()));
            }
            return list;
        }
        #endregion


        #region Helper Methods
        private static bool SaveSlotExists(int slot, out string path)
        {
            path = GetDataPathForSlot(slot);

            return DataManager.DataExists(path);
        }

        private static SerializedPlayerData LoadPlayerData(string path) =>
            DataManager.Load<SerializedPlayerData>(path);

        private static string GetDataPathForSlot(int slot) =>
            $"{ GetRootDataPath() }/{ FileName }.{ slot }.{ FileExtension }";

        private static string GetRootDataPath()
        {
            string path = $"{ Application.persistentDataPath }/{ FolderRoot }";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }
        #endregion
    }
}
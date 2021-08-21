using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace archipelaGO.Progression
{
    public static class ProgressDataHandler
    {
        private static ProgressData m_currentProgress = null;

        private static FileStream GetFileStream(FileMode fileMode) =>
            new FileStream(ProgressFilePath(), fileMode);

        private static string ProgressFilePath()
        {
            const string FileName = "game_progress", Extension = "prg";

            return Application.persistentDataPath + $"/{ FileName }.{ Extension }";
        }

        private static void CreateNewFile()
        {
            m_currentProgress = new ProgressData();
            Save();
        }

        private static void Save()
        {
            using (FileStream stream = GetFileStream(FileMode.OpenOrCreate))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, m_currentProgress);
            }
        }

        private static void Load()
        {
            if (!File.Exists(ProgressFilePath()))
                CreateNewFile();

            using (FileStream stream = GetFileStream(FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                m_currentProgress = formatter.Deserialize(stream) as ProgressData;
                m_currentProgress.LogAllKeys();
            }
        }

        private static bool isLoaded => (m_currentProgress != null);

        public static void ClearProgress() =>
            CreateNewFile();

        public static bool IsUnlocked(string key)
        {
            if (!isLoaded)
                Load();

            return m_currentProgress.IsUnlocked(key);
        }

        public static void Unlock(string key)
        {
            if (!isLoaded)
                Load();

            m_currentProgress.Unlock(key);
            Save();
            m_currentProgress.LogAllKeys();
        }
    }

    [System.Serializable]
    public class ProgressData
    {
        [SerializeField]
        private List<string> m_keys = new List<string>();

        public bool IsUnlocked(string key) => m_keys.Contains(key);

        public void Unlock(string key)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
                return;

            if (!IsUnlocked(key))
                m_keys.Add(key);
        }

        public void LogAllKeys()
        {
            foreach (string key in m_keys)
                Debug.Log(key);
        }
    }
}
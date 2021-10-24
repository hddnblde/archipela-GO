using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace archipelaGO.GameData
{
    public static class GameDataHandler
    {
        #region Data Structure
        [System.Serializable]
        private struct GameDataJSON
        {
            public GameDataJSON(BaseGameData data)
            {
                m_jsonData = data.ToJson();
                m_type = data.GetType().FullName;
            }

            [SerializeField]
            private string m_jsonData;

            [SerializeField]
            private string m_type;

            public string json => m_jsonData;
            public System.Type type => System.Type.GetType(m_type);
        }

        [System.Serializable]
        private class SerializedGameData
        {
            public SerializedGameData (List<GameDataJSON> jsonClusters)
            {
                List<GameDataJSON> newCluster = new List<GameDataJSON>();

                foreach (GameDataJSON data in jsonClusters)
                    newCluster.Add(data);

                m_dataCluster = newCluster;
            }

            [SerializeField]
            private List<GameDataJSON> m_dataCluster = new List<GameDataJSON>();

            public List<BaseGameData> GetCollection()
            {
                List<BaseGameData> collection = new List<BaseGameData>();

                foreach (GameDataJSON data in m_dataCluster)
                {
                    if (StringIsInvalid(data.json) || !(data.type.IsSubclassOf(typeof(BaseGameData))))
                        continue;

                    BaseGameData gameData =
                        JsonUtility.FromJson(data.json, data.type)
                        as BaseGameData;

                    if (gameData != null)
                        collection.Add(gameData);
                }

                return collection;
            }
        }
        #endregion
        
        
        #region Default Data Implementation
        private static List<BaseGameData> GenerateDefaultData()
        {
            List<BaseGameData> defaultCollection = new List<BaseGameData>();
            defaultCollection.Add(new GameProgressionData());

            return defaultCollection;
        }
        #endregion


        #region Fields
        [SerializeField]
        private static List<BaseGameData> m_dataCollection = new List<BaseGameData>();

        private static string m_currentPlayerName = string.Empty;

        private const string FolderRoot = "GAMEDATA",
            FileExtension = "dat";
        #endregion


        #region Public Methods
        public static G GetDataFromCurrentPlayer<G>() where G : BaseGameData =>
            GetData<G>(m_currentPlayerName);

        public static G GetData<G>(string playerName) where G : BaseGameData
        {
            if (StringIsInvalid(playerName))
                return null;

            if (!string.Equals(playerName, m_currentPlayerName))
                Load(playerName);

            foreach (BaseGameData data in m_dataCollection)
            {
                if (data is G)
                    return (data as G);
            }

            return null;
        }

        public static void Load(string playerName)
        {
            if (StringIsInvalid(playerName))
                return;

            string path = GetPlayerDataPath(playerName);

            SerializedGameData data = DataManager.
                Load<SerializedGameData>(path);

            if (data != null)
            {
                m_dataCollection = data.GetCollection();
                ValidateCurrentData();
                Debug.LogWarning($"Data successfully retrieved from: { path }");
            }

            else
            {
                ValidateCurrentData();
                Save(playerName);
            }

            m_currentPlayerName = playerName;
        }

        public static void SaveCurrentPlayerData() =>
            Save(m_currentPlayerName);

        public static void Save(string playerName)
        {
            if (StringIsInvalid(playerName))
                return;

            string path = GetPlayerDataPath(playerName);
            Debug.LogWarning($"Saving data to: { path }");

            SerializedGameData data = new SerializedGameData(GenerateJSONClusters());
            DataManager.Save(data, path);
        }
        #endregion


        #region Helper Methods
        private static string GetPlayerDataPath(string playerName) =>
            $"{ Application.persistentDataPath }/{ FolderRoot }/{ playerName }.{ FileExtension }";

        private static List<GameDataJSON> GenerateJSONClusters()
        {
            List<GameDataJSON> dataClusters = new List<GameDataJSON>();

            foreach (BaseGameData data in m_dataCollection)
            {
                GameDataJSON cluster = new GameDataJSON(data);
                dataClusters.Add(cluster);
            }

            return dataClusters;
        }

        private static void ValidateCurrentData()
        {
            if (m_dataCollection != null && m_dataCollection.Count > 0)
                return;

            Debug.LogWarning("Creating default player data.");
            m_dataCollection = GenerateDefaultData();
        }

        private static bool StringIsInvalid(string name) =>
            string.IsNullOrWhiteSpace(name) || string.IsNullOrEmpty(name);
        #endregion
    }
}
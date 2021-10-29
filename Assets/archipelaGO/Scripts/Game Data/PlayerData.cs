using System.Collections.Generic;
using UnityEngine;

namespace archipelaGO.GameData
{
    public class PlayerData
    {
        public PlayerData (string name)
        {
            m_name = name;
            m_data = new List<BaseGameData>();
        }

        public PlayerData (string name, List<BaseGameData> data)
        {
            m_name = name;
            m_data = data;
        }

        private string m_name = string.Empty;
        private List<BaseGameData> m_data = new List<BaseGameData>();

        public string realName => m_name;

        public G Access<G>() where G : BaseGameData, new()
        {
            foreach (BaseGameData data in m_data)
            {
                if (data is G)
                    return (data as G);
            }

            G defaultData = new G();
            defaultData.Reset();

            Debug.LogWarning($"Creating new instance of { typeof(G).Name }");
            m_data.Add(defaultData);

            return defaultData;
        }

        public SerializedPlayerData AsSerializable() => new SerializedPlayerData(m_name, m_data);
    }

    [System.Serializable]
    public class SerializedPlayerData
    {
        [SerializeField]
        private string m_name = string.Empty;

        [SerializeField]
        private List<SerializableGameData> m_serializedData = new List<SerializableGameData>();

        public SerializedPlayerData (string name) =>
            new SerializedPlayerData(name, new List<BaseGameData>());

        public SerializedPlayerData (string name, List<BaseGameData> data)
        {
            m_name = name;
            m_serializedData = ToSerializableGameData(data);
        }

        private static List<SerializableGameData> ToSerializableGameData(List<BaseGameData> data)
        {
            List<SerializableGameData> serializedData = new List<SerializableGameData>();

            foreach (BaseGameData gameData in data)
                serializedData.Add(new SerializableGameData(gameData));
            
            return serializedData;
        }

        private static List<BaseGameData> ToGameData(List<SerializableGameData> data)
        {
            List<BaseGameData> gameData = new List<BaseGameData>();

            if (data == null)
                goto End;

            foreach (SerializableGameData serializedData in data)
            {
                if (!(serializedData.type.IsSubclassOf(typeof(BaseGameData))))
                    continue;

                BaseGameData target = JsonUtility.
                    FromJson(serializedData.json, serializedData.type)
                    as BaseGameData;

                if (target != null)
                    gameData.Add(target);
            }

            End:
            return gameData;
        }

        [System.Serializable]
        private struct SerializableGameData
        {
            public SerializableGameData(BaseGameData data)
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

        public PlayerData ToPlayerData(string playerName) => new PlayerData(playerName, ToGameData(m_serializedData));
    }
}
using UnityEngine;

namespace archipelaGO.GameData
{
    [System.Serializable]
    public abstract class BaseGameData
    {
        public string ToJson() => JsonUtility.ToJson(this);
    }
}
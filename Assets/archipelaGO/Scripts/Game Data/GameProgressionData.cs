using System.Collections.Generic;
using UnityEngine;

namespace archipelaGO.GameData
{
    public sealed class GameProgressionData : BaseGameData
    {
        [SerializeField]
        private List<string> m_unlockedKeys = new List<string>();

        public bool IsUnlocked(string key) =>
            m_unlockedKeys.Contains(key);

        public void UnlockThisKey(string key)
        {
            if (!IsUnlocked(key))
                m_unlockedKeys.Add(key);
        }

        public bool AreUnlocked(string[] keys)
        {
            foreach (string key in keys)
            {
                if (!IsUnlocked(key))
                    return false;
            }

            return true;
        }

        public void UnlockTheseKeys(string[] keys)
        {
            foreach (string key in keys)
                UnlockThisKey(key);
        }
    }
}
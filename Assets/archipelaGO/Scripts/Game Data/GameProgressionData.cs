using System.Collections.Generic;
using UnityEngine;

namespace archipelaGO.GameData
{
    public sealed class GameProgressionData : BaseGameData
    {
        [SerializeField]
        private List<string> m_unlockedKeys = new List<string>();

        public override void Reset() =>
            m_unlockedKeys = new List<string>();

        public int CountUnlockedKeys() => m_unlockedKeys.Count;

        public bool IsUnlocked(string key) =>
            m_unlockedKeys.Contains(key);

        public void UnlockThisKey(string key)
        {
            if (IsUnlocked(key))
                return;

            m_unlockedKeys.Add(key);
            Debug.LogWarning($"Player earned a new key [{ key }].");
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
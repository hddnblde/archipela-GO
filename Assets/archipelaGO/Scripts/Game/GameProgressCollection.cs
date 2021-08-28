using System.Collections.Generic;
using UnityEngine;

namespace archipelaGO.Game
{
    [CreateAssetMenu(fileName = "Game Progress", menuName = "archipelaGO/Game Progress")]
    public class GameProgressCollection : ScriptableObject
    {
        [SerializeField]
        private List<GameConfig> m_keys = new List<GameConfig>();

        public int keyCount => m_keys.Count;

        public string GetKey(int index)
        {
            if (index < 0 || index >= keyCount)
                return string.Empty;

            return m_keys[index].name;
        }
    }
}
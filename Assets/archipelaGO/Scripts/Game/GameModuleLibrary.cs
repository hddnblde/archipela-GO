using System.Collections.Generic;
using UnityEngine;

namespace archipelaGO.Game
{
    [CreateAssetMenu(fileName = "Game Module Library", menuName = "archipelaGO/Game Module Library")]
    public class GameConfigLibrary : ScriptableObject
    {
        [SerializeField]
        private List<GameModuleConfig> m_modules = new List<GameModuleConfig>();
    }
}
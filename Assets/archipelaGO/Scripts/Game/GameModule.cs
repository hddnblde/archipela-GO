using UnityEngine;

namespace archipelaGO.Game
{
    public abstract class GameModule<T> : MonoBehaviour
        where T : GameConfig
    {
        public abstract void Initialize(T config);
    }
}